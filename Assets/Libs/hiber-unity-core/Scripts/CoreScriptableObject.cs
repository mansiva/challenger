using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Diagnostics;
using System.IO;

/// <summary>
/// Core scriptable object.
/// </summary>
public abstract class CoreScriptableObject : ScriptableObject
{
#if UNITY_EDITOR
    private string AssetPath { get; set; }
#endif

	[HideInInspector,SerializeField]
	private string _name;
	
	/// <summary>
	/// Gets or sets the name.
	/// </summary>
	/// <value>
	/// The name.
	/// </value>
	public string Name
	{
		get
		{
			if(string.IsNullOrEmpty(_name))
			{
				_name = name;
			}
			return _name;
		}
		set
		{
			if(_name != value)
			{
				_name = value;
				name = _name;
			}
		}
	}

    public virtual bool OnInspectorGUI()
    {
        var dirty = false;
#if UNITY_EDITOR
        if(string.IsNullOrEmpty(AssetPath))
        {
            AssetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
        }
        if(string.IsNullOrEmpty(AssetPath))
        {
            AssetPath = "Assets/SO/";
        }

        GUILayout.BeginHorizontal();
        var newPath = GUILayout.TextField(AssetPath);

        if(string.IsNullOrEmpty(UnityEditor.AssetDatabase.GetAssetPath(this)))
        {
            if(System.IO.Path.HasExtension(newPath) && GUILayout.Button("Create"))
            {
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                UnityEditor.AssetDatabase.CreateAsset(this, newPath);
                _name = System.IO.Path.GetFileNameWithoutExtension(newPath);
                dirty = true;
            }
        }
        else
        {
            if(System.IO.Path.HasExtension(newPath) && GUILayout.Button("Move"))
            {
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                UnityEditor.AssetDatabase.MoveAsset(UnityEditor.AssetDatabase.GetAssetPath(this), newPath);
                _name = System.IO.Path.GetFileNameWithoutExtension(newPath);
                dirty = true;
            }
        }

        AssetPath = newPath;
        GUILayout.EndHorizontal();
     
#endif
        return dirty;
    }
}