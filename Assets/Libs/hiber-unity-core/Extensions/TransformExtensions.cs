using UnityEngine;
using System.Collections.Generic;

public static class TransformExtensions
{
	public static bool TryGetComponent<T>(this Transform node, ref T t) where T : Component
	{
		t = node.GetComponent<T>();
		return t != null;
	}
	
	public static T GetComponentInParent<T>(this Transform node) where T : Component
	{	
		if (node.parent)
		{
			return node.parent.GetComponent<T>();
		}
		
		return null;
	}
	
	public static T GetComponentInParents<T>(this Transform node) where T : Component
	{	
		while (node.parent != null)
		{
			node = node.parent;
			
			T t = node.GetComponent<T>();
			
			if (t != null)
				return t;
		}
		
		return null;
	}
	
	public static List<T> GetListComponentsInChildren<T>(this Transform node) where T : Component
	{
		T[] components = node.GetComponentsInChildren<T>();
		
		List<T> list = new List<T>();
		foreach (T c in components) {
			list.Add(c);
		}
		
		return list;
	}

	public static void UnparentAllChildren(this Transform node) 
	{
		ReparentAllChildren(node, null);
	}

	public static void ReparentAllChildren(this Transform node, Transform parent) 
	{
		foreach (Transform child in node) {
			child.parent = parent;
		}
	}
	
	public static void DestroyAllChildren(this Transform node) 
	{
		for ( int i=node.childCount-1; i>=0; --i ) {
		    GameObject.Destroy(node.GetChild(i).gameObject);
		}
	}
	
	public static void DestroyAllChildrenImmediate(this Transform node)
	{
		for ( int i=node.childCount-1; i>=0; --i ) {
		    GameObject.DestroyImmediate(node.GetChild(i).gameObject);
		}
	}
}