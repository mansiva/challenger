using UnityEngine;
using System.Reflection;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR && !UNITY_WEBPLAYER
using UnityEditor;
public class CoreCustomEditorGUILayout : AssetPostprocessor
{
	/// <summary>
	/// The _runtime assembly.
	/// </summary>
	private static Assembly _runtimeAssembly = System.Reflection.Assembly.GetAssembly(typeof(CoreAssert));
	
	/// <summary>
	/// The _runtime types.
	/// </summary>
	private static List<Type> _runtimeTypes = _runtimeAssembly.GetTypes().ToList();
	
    private static List<KeyValuePair<string, Type>> _soPath;
    public static List<KeyValuePair<string, Type>> SOPath
    {
        get
        {
            if(_soPath == null)
            {
				if(System.IO.Directory.Exists("Assets/SO/"))
				{
	                var assetsPath = System.IO.Directory.GetFiles("Assets/SO/", "*.asset", System.IO.SearchOption.AllDirectories).ToList();
                    _soPath = GetScriptableObjects(assetsPath);
				}
				else
				{
                    _soPath = new List<KeyValuePair<string, Type>>();
				}
            }
            return _soPath;
        }
        set
        {
            _soPath = value;
        }
    }

    private static List<KeyValuePair<string, Type>> GetScriptableObjects(IEnumerable<string> input)
    {
        return input.Where(x=>Path.GetExtension(x) == ".asset").Select(x=>
        {
            var asset = AssetDatabase.LoadMainAssetAtPath(x);
            return new KeyValuePair<string, Type>(x, asset == null ? null : asset.GetType());
        }).Where(x=>x.Value != null && x.Value.IsSubclassOf(typeof(ScriptableObject))).ToList();
    }
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        GetScriptableObjects(importedAssets.Concat(movedAssets)).ForEach(x=>SOPath.Add(x));
        GetScriptableObjects(deletedAssets.Concat(movedFromAssetPaths)).ForEach(x=>SOPath.Remove(x));

        if(_soPath != null)
        {
            SOPath = _soPath.Distinct().ToList();
        }
    }

	/// <summary>
	/// Use to display object field with option to save scriptable objects and edit list more easily.
	/// </summary>
	/// <returns>
	/// The field.
	/// </returns>
	/// <param name='obj'>
	/// Object.
	/// </param>
	public static IEnumerator<bool> ObjectField(System.Object obj)
	{
		var iter = ObjectField(null, null, obj);
		while(iter.MoveNext())
		{
			if((iter.Current || GUI.changed) && obj is UnityEngine.Object)
			{
				EditorUtility.SetDirty(obj as UnityEngine.Object);
			}
			yield return false;
		}
	}
	
	/// <summary>
	/// Use to display object field with option to save scriptable objects and edit list more easily.
	/// </summary>
	/// <returns>
	/// The field.
	/// </returns>
	/// <param name='parent'>
	/// Parent.
	/// </param>
	/// <param name='fieldName'>
	/// Field name.
	/// </param>
	/// <param name='obj'>
	/// Object.
	/// </param>
	private static IEnumerator<bool> ObjectField(System.Object parent, string fieldName, System.Object obj)
	{
		FieldInfo currentField = null;
		// If there is the field value is null create it so we can edit it.
		if(parent != null && obj == null && !string.IsNullOrEmpty(fieldName))
		{
			currentField = parent.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
		
			var type = currentField.FieldType;
			if(type.IsSubclassOf(typeof(ScriptableObject)))
			{
				obj = ScriptableObject.CreateInstance(type);
				currentField.SetValue(parent, obj);
			}
			else if(!type.IsSubclassOf(typeof(UnityEngine.Object)) && !type.IsInterface && !type.IsAbstract)
			{
				obj = Activator.CreateInstance(type);
				currentField.SetValue(parent, obj);
			}
		}
		
		if(obj == null)
		{
			yield break;
		}
		
		// Generate the list of iterators based on field type.
		var iters = new List<IEnumerator>();
		var fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		foreach(FieldInfo field in fields)
		{
			if((field.IsPublic || field.GetCustomAttributes(typeof(SerializeField), true).Length > 0) && field.GetCustomAttributes(typeof(HideInInspector), true).Length == 0)
			{
				if(field.FieldType == typeof(Single))
				{
					iters.Add(FloatField(obj, field));
				}
				else if(field.FieldType.IsEnum)
				{
					iters.Add(EnumField(obj, field));
				}
				else if(field.FieldType == typeof(Int32))
				{
					iters.Add(IntField(obj, field));
				}
				else if(field.FieldType == typeof(Boolean))
				{
					iters.Add(BoolField(obj, field));
				}
				else if(field.FieldType == typeof(string))
				{
					iters.Add(StringField(obj, field));
				}
				else if(field.FieldType.IsSubclassOf(typeof(UnityEngine.ScriptableObject)))
				{
					iters.Add(UnityObjectField(obj, field));
					
					if(field.GetValue(obj) != null)
					{
						iters.Add(ObjectField(obj, field.Name, field.GetValue(obj)));
					}
					else
					{
						iters.Add(UnityObjectField(obj, field));
					}
				}
				else if(field.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
				{
					iters.Add(UnityObjectField(obj, field));
				}
				else if(Array.IndexOf(field.FieldType.GetInterfaces(), typeof(IList)) != -1)
				{
					iters.Add(ListField(obj, field));
				}
				else
				{
					iters.Add(ObjectField(obj, field.Name, field.GetValue(obj)));
				}
			}
		}
		
		// Foldout and scriptable object option menu.
		var show = false;

		while(true)
		{
			var dirty = false;
			
			// The foldout.
			if(!string.IsNullOrEmpty(fieldName))
			{
                show = EditorGUILayout.Foldout(show, CoreCustomEditorGUILayout.FormatToDisplayName(fieldName));
			}
			else
			{
				show = true;
			}
			if(show)
			{
				if(!string.IsNullOrEmpty(fieldName))
				{
					EditorGUI.indentLevel++;
				}
				EditorGUILayout.BeginVertical();

				// If there is a custom gui provided.
				var inspectable = obj as ICoreInspectable;
				if(parent != null && inspectable != null)
				{
					dirty |= inspectable.OnInspectorGUI();
				}
				
				// Display all fields.
				foreach(IEnumerator<bool> iter in iters)
				{
					iter.MoveNext();
					dirty |= iter.Current;
					
					if(iter.Current)
					{
						if(obj is ScriptableObject)
						{
							EditorUtility.SetDirty(obj as ScriptableObject);
						}
					}
				}
				
				EditorGUILayout.EndVertical();
				if(!string.IsNullOrEmpty(fieldName)) 
				{
					EditorGUI.indentLevel--;
				}
			}
			yield return dirty;
		}
	}
	
	/// <summary>
	/// A float field.
	/// </summary>
	/// <returns>
	/// The field.
	/// </returns>
	/// <param name='obj'>
	/// Object.
	/// </param>
	/// <param name='field'>
	/// Field.
	/// </param>
	private static IEnumerator<bool> FloatField(System.Object obj, FieldInfo field)
	{
		while(true)
		{
			field.SetValue(obj, EditorGUILayout.FloatField(FormatToDisplayName(field.Name), float.Parse(field.GetValue(obj).ToString())));
			yield return GUI.changed;
		}
	}
	
	/// <summary>
	/// A int field.
	/// </summary>
	/// <returns>
	/// The field.
	/// </returns>
	/// <param name='obj'>
	/// Object.
	/// </param>
	/// <param name='field'>
	/// Field.
	/// </param>
	private static IEnumerator<bool> IntField(System.Object obj, FieldInfo field)
	{
		while(true)
		{
			field.SetValue(obj, EditorGUILayout.IntField(FormatToDisplayName(field.Name), int.Parse(field.GetValue(obj).ToString())));
			yield return GUI.changed;
		}
	}
	
	/// <summary>
	/// Scriptables the object field.
	/// </summary>
	/// <returns>
	/// The object field.
	/// </returns>
	/// <param name='obj'>
	/// Object.
	/// </param>
	/// <param name='field'>
	/// Field.
	/// </param>
	private static IEnumerator<bool> UnityObjectField(System.Object obj, FieldInfo field)
	{
		while(true)
		{
			field.SetValue(obj, EditorGUILayout.ObjectField(field.GetValue(obj) as UnityEngine.Object, field.FieldType, false));
			yield return GUI.changed;
		}
	}
	
	/// <summary>
	/// A int field.
	/// </summary>
	/// <returns>
	/// The field.
	/// </returns>
	/// <param name='obj'>
	/// Object.
	/// </param>
	/// <param name='field'>
	/// Field.
	/// </param>
	private static IEnumerator<bool> EnumField(System.Object obj, FieldInfo field)
	{
		while(true)
		{
			field.SetValue(obj, EditorGUILayout.EnumPopup(FormatToDisplayName(field.Name), field.GetValue(obj) as Enum));
			yield return GUI.changed;
		}
	}
	
	/// <summary>
	/// A bool field.
	/// </summary>
	/// <returns>
	/// The field.
	/// </returns>
	/// <param name='obj'>
	/// Object.
	/// </param>
	/// <param name='field'>
	/// Field.
	/// </param>
	private static IEnumerator<bool> BoolField(System.Object obj, FieldInfo field)
	{
		while(true)
		{
			field.SetValue(obj, EditorGUILayout.Toggle(FormatToDisplayName(field.Name), Boolean.Parse(field.GetValue(obj).ToString())));
			yield return GUI.changed;
		}
	}
	
	/// <summary>
	/// A string field.
	/// </summary>
	/// <returns>
	/// The field.
	/// </returns>
	/// <param name='obj'>
	/// Object.
	/// </param>
	/// <param name='field'>
	/// Field.
	/// </param> 
	private static IEnumerator<bool> StringField(System.Object obj, FieldInfo field)
	{
		while(true)
		{
			var val = field.GetValue(obj).ToString();
			var newVal = EditorGUILayout.TextField(FormatToDisplayName(field.Name), val != null ? val.ToString() : string.Empty);
			if(val != newVal)
			{
				field.SetValue(obj, newVal);
				yield return GUI.changed;
			}
			else
			{
				yield return false;
			}
		}
	}
	
	private static IEnumerator<bool> ListField(System.Object obj, FieldInfo field)
    {
        if(field.FieldType.GetGenericArguments().Length > 0)
        {
            return ListField(field.GetValue(obj) as IList, field.FieldType.GetGenericArguments()[0], field.Name);
        }
        else
        {
            return ListField(field.GetValue(obj) as IList, field.FieldType, field.Name);
        }
    }

    public static IEnumerator<bool> ListField(IList list, Type type, string name)
	{
		var show = false;
		while(true)
		{
			var iters = new List<IEnumerator<bool>>();
			
			if(list == null)
			{
				yield break;
			}
			
			// Copy the list. Used to determine if the list as been modified.
			var copy = new ArrayList(list);
			
			// Handle empty list.
			if(list.Count == 0)
			{
                iters.Add(ListCreateField(list, type));
				
				while(list.Count == 0)
				{
					show = EditorGUILayout.Foldout(show, name);
					EditorGUI.indentLevel++;
					EditorGUILayout.BeginVertical();
					if(show)
					{
						foreach(IEnumerator<bool> iter in iters)
						{
							iter.MoveNext();
						}
					}
					EditorGUILayout.EndVertical();
					EditorGUI.indentLevel--;
					yield return false;
				}
			}
			else
			{
				// Add list fields.
				for(var i = 0; i < list.Count; i++)
				{
                    iters.Add(ListItemField(list, i, type));
				}
				
				var modified = false;
				while(!modified)
				{
					var dirty = false;
					show = EditorGUILayout.Foldout(show, name);
					EditorGUI.indentLevel++;
					EditorGUILayout.BeginVertical();
					if(show)
					{
						foreach(IEnumerator<bool> iter in iters)
						{
							iter.MoveNext();
							dirty |= iter.Current;
						}
					}
					EditorGUILayout.EndVertical();
					EditorGUI.indentLevel--;
					
					if(list.Count != copy.Count)
					{
						modified = true;
					}
					else
					{
						for(var i = 0; i < list.Count; i++)
						{
							if(list[i] != copy[i])
							{
								modified = true;
								break;
							}
						}
					}
					
					yield return dirty;
				}
			}
		}
	}
	
	/// <summary>
	/// Use to provide the option to create the initial element of the list.
	/// </summary>
	/// <returns>
	/// The create field.
	/// </returns>
	/// <param name='list'>
	/// List.
	/// </param>
	/// <param name='type'>
	/// Type.
	/// </param>
	private static IEnumerator<bool> ListCreateField(IList list, Type type)
	{
		var subTypes = _runtimeTypes.FindAll((x)=>(x == type || x.IsSubclassOf(type)) && !x.IsAbstract);
		var actions = (new string[]{"List Actions"}).ToList().Concat(subTypes.ConvertAll<string>((x)=>"Add: " + x.Name)).ToArray();
		var so = new string[]{};
		var soc = new string[]{};

		var selected = 0;
		var selected2 = 0;
		var selected3 = 0;
		
		while(true)
		{
            if(type.IsSubclassOf(typeof(ScriptableObject)))
            {
                so = (new string[]{"Add ScriptableObject"}).Concat(SOPath.Where(x=>x.Value.IsSubclassOf(type)).Select(x=>x.Key.Replace("Assets/SO/", ""))).ToArray();  
                soc = (new string[]{"Clone ScriptableObject"}).Concat(SOPath.Where(x=>x.Value.IsSubclassOf(type)).Select(x=>x.Key.Replace("Assets/SO/", ""))).ToArray(); 
            }

			EditorGUILayout.BeginHorizontal();
			selected = EditorGUILayout.Popup(selected, actions);
			if(selected != 0)
			{
				var newType = subTypes[selected - 1];
				
				System.Object newInstance = null;
				if(newType.IsSubclassOf(typeof(ScriptableObject)))
				{
					newInstance = ScriptableObject.CreateInstance(newType);
				}
				else
				{
					newInstance = Activator.CreateInstance(newType);
				}

				list.Add(newInstance);
				
				var name = newType.GetProperty("Name");
				if(name != null)
				{
					name.SetValue(newInstance, newType.Name, null);
				}
			}
			
			if(so.Length > 1)
			{
				selected2 = EditorGUILayout.Popup(selected2, so);
			}
			if(selected2 != 0)
			{
                var asset = SOPath.Find((x)=>x.Key.Contains(so[selected2]));
				var original = AssetDatabase.LoadMainAssetAtPath(asset.Key);
				list.Add(original);
				selected2 = 0;
			}
			if(soc.Length > 1)
			{
				selected3 = EditorGUILayout.Popup(selected3, soc);
			}
			if(selected3 != 0)
			{
                var asset = SOPath.Find((x)=>x.Key.Contains(soc[selected3]));
				var original = AssetDatabase.LoadMainAssetAtPath(asset.Key);
				var clone = ScriptableObject.Instantiate(original);
				clone.name = original.name;
				list.Add(clone);
				selected3 = 0;
			}
			EditorGUILayout.EndHorizontal();
			yield return false;
		}
	}
	
	/// <summary>
	/// A list item.
	/// </summary>
	/// <returns>
	/// The item field.
	/// </returns>
	/// <param name='list'>
	/// List.
	/// </param>
	/// <param name='index'>
	/// Index.
	/// </param>
	/// <param name='type'>
	/// Type.
	/// </param>
	private static IEnumerator<bool> ListItemField(IList list, int index, Type type)
	{
		var obj = list[index];
		var iter = ObjectField(obj, string.Empty, obj);
		
		var subTypes = _runtimeTypes.FindAll((x)=>(x == type || x.IsSubclassOf(type)) && !x.IsAbstract);
		var baseActions = new string[]{"List Actions", "Clone", "Remove", "MoveUp", "MoveDown"}.ToList();
		var actions = baseActions.Concat(subTypes.ConvertAll<string>((x)=>"Insert: " + x.Name)).ToArray();
		
		var so = new string[]{};
		var soc = new string[]{};

		var show = false;
		var selected = 0;
		var selected2 = 0;
		var selected3 = 0;
		while(true)
		{
            if(type.IsSubclassOf(typeof(ScriptableObject)))
            {
                so = (new string[]{"Insert ScriptableObject"}).Concat(SOPath.Where(x=>x.Value.IsSubclassOf(type)).Select(x=>x.Key.Replace("Assets/SO/", ""))).ToArray();  
                soc = (new string[]{"Clone ScriptableObject"}).Concat(SOPath.Where(x=>x.Value.IsSubclassOf(type)).Select(x=>x.Key.Replace("Assets/SO/", ""))).ToArray(); 
            }

			EditorGUILayout.BeginHorizontal();
			
			var name = type.GetProperty("Name");
			
			try
			{
				show = EditorGUILayout.Foldout(show, name != null ? FormatToDisplayName(name.GetValue(obj, null).ToString()) + " - " + obj.ToString() : obj.ToString());
			}
			catch
			{
			}
			
			selected = EditorGUILayout.Popup(selected, actions, GUILayout.ExpandWidth(false));
			
			if(selected != 0)
			{
				if(selected == 1)
				{
					if(list[index] is ScriptableObject)
					{
						list[index] = ScriptableObject.Instantiate(list[index] as ScriptableObject);
					}
				}
				else if(selected == 2)
				{
					list.RemoveAt(index);
				}
				else if(selected == 3)
				{
					if(index > 0)
					{
						list.RemoveAt(index);
						list.Insert(index-1, obj);
					}
				}
				else if(selected == 4)
				{
					if(index != list.Count - 1)
					{
						list.RemoveAt(index);
						list.Insert(index+1, obj);
					}
				}
				else
				{
					selected -= baseActions.Count;
					var newType = subTypes[selected];
					
					System.Object newInstance = null;
					if(newType.IsSubclassOf(typeof(ScriptableObject)))
					{
						newInstance = ScriptableObject.CreateInstance(newType);
					}
					else
					{
						newInstance = Activator.CreateInstance(newType);
					}

					list.Insert(index, newInstance);
					
					name = newType.GetProperty("Name");
					if(name != null)
					{
						name.SetValue(newInstance, newType.Name, null);
					}
				}
			}
			
			if(so.Length > 1)
			{
				selected2 = EditorGUILayout.Popup(selected2, so, GUILayout.ExpandWidth(false));
			}
			
			if(selected2 != 0)
			{
                var original = AssetDatabase.LoadMainAssetAtPath(SOPath.Find((x)=>x.Key.Contains(so[selected2])).Key);
				list.Insert(index, original);
				selected2 = 0;
			}
			
			if(soc.Length > 1)
			{
				selected3 = EditorGUILayout.Popup(selected3, soc, GUILayout.ExpandWidth(false));
			}
			
			if(selected3 != 0)
			{
                var original = AssetDatabase.LoadMainAssetAtPath(SOPath.Find((x)=>x.Key.Contains(soc[selected3])).Key);
				var clone = ScriptableObject.Instantiate(original);
				list.Insert(index, clone);
				selected3 = 0;
			}
			
			EditorGUILayout.EndHorizontal();
			
			EditorGUI.indentLevel++;
			EditorGUILayout.BeginVertical();
			
			var dirty = false;
			if(show)
			{
				iter.MoveNext();
				dirty = iter.Current;
			}
			
			EditorGUILayout.EndVertical();
			EditorGUI.indentLevel--;

			yield return dirty;
		}
	}
	
	/// <summary>
	/// Formats the name to a display name.
	/// </summary>
	/// <returns>
	/// The to display name.
	/// </returns>
	/// <param name='name'>
	/// Name.
	/// </param>
	public static string FormatToDisplayName(string name)
	{
		if(name.Length <= 0)
			return string.Empty;
		
		while(name.Length > 0)
		{
			if(char.IsLetter(name[0]))
			{
				break;
			}
			else
			{
				name = name.Substring(1);	
			}
		}
		
		name = char.ToUpper(name[0]) + name.Substring(1);
		var displayName = name[0].ToString();
		for(var i = 1; i < name.Length; i++)
		{
			if(char.IsLetterOrDigit(name[i]))
			{
				if(char.IsLower(name[i - 1]) && char.IsUpper(name[i]))
				{
					displayName += ' ';
				}
				displayName += name[i];
			}
		}
		return displayName;
	}
	
	/// <summary>
	/// Formats for serialization.
	/// </summary>
	/// <returns>
	/// The for serialization.
	/// </returns>
	/// <param name='name'>
	/// Name.
	/// </param>
	public static string FormatForSerialization(string name)
	{
		return name.Replace(" ", string.Empty);
	}
}
#endif