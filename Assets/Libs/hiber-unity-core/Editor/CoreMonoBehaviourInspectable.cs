using UnityEngine;
using UnityEditor;

using System.Collections.Generic;


[CustomEditor(typeof(UnityEngine.MonoBehaviour), true)]
public class CoreMonoBehaviourInspectable : Editor
{
	public override void OnInspectorGUI ()
	{
		var dirty = false;
		if(target is ICoreInspectableWithDefaultView)
		{
			dirty = (target as ICoreInspectableWithDefaultView).OnInspectorGUI();
			DrawDefaultInspector();
		}
		else if(target is ICoreInspectableWithoutDefaultView)
		{
			dirty = (target as ICoreInspectableWithoutDefaultView).OnInspectorGUI();
		}
		else
		{
			DrawDefaultInspector();
		}
		if(dirty)
		{
			Repaint();
		}
	}

	public void OnSceneGUI(){
		if(target is ICoreSceneDrawable){
			(target as ICoreSceneDrawable).OnSceneGUI();
		}
	}
}