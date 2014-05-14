using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

[CustomEditor(typeof(UnityEngine.ScriptableObject), true)]
public class CoreScriptableObjectInspectable : Editor
{
	private bool _folded;
	public override void OnInspectorGUI ()
	{
		if(target is ICoreInspectableWithDefaultView)
		{
			(target as ICoreInspectableWithDefaultView).OnInspectorGUI();
			DrawDefaultInspector();
		}
		else if(target is ICoreInspectableWithFoldedDefaultView)
		{
			(target as ICoreInspectableWithFoldedDefaultView).OnInspectorGUI();

			_folded = EditorGUILayout.Foldout(_folded, "Default View");
			if(_folded)
			{
				EditorGUI.indentLevel++;
				DrawDefaultInspector();
				EditorGUI.indentLevel--;
			}
		}
		else if(target is ICoreInspectableWithoutDefaultView)
		{
			(target as ICoreInspectableWithoutDefaultView).OnInspectorGUI();
		}
		else
		{
			DrawDefaultInspector();
		}
	}
	
	public void OnSceneGUI(){
		if(target is ICoreSceneDrawable){
			(target as ICoreSceneDrawable).OnSceneGUI();
		}
	}
}