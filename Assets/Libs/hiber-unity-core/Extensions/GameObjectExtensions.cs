using UnityEngine;
using System.Collections.Generic;

public static class GameObjectExtensions
{
	public static void DisableAllBehaviours(this GameObject node)
	{
		var comps = node.GetComponents<Behaviour>();
		for(var i = 0; i < comps.Length; i++)
		{
			comps[i].enabled = false;
		}
	}
	public static void EnableAllBehaviours(this GameObject node)
	{
		var comps = node.GetComponents<Behaviour>();
		for(var i = 0; i < comps.Length; i++)
		{
			comps[i].enabled = true;
		}
	}
	public static bool TryGetComponent<T>(this GameObject node, ref T t) where T : Component
	{
		return node.transform.TryGetComponent<T>(ref t);
	}
	
	public static T GetComponentInParent<T>(this GameObject node) where T : Component
	{	
		return node.transform.GetComponentInParent<T>();
	}
	
	public static T GetComponentInParents<T>(this GameObject node) where T : Component
	{	
		return node.transform.GetComponentInParents<T>();
	}
	
	public static List<T> GetListComponentsInChildren<T>(this GameObject node) where T : Component
	{
		return node.transform.GetListComponentsInChildren<T>();	
	}

	public static void DestroyAllChildren(this GameObject node) 
	{
		node.transform.DestroyAllChildren();	
	}
	
	public static void DestroyAllChildrenImmediate(this GameObject node)
	{
		node.transform.DestroyAllChildrenImmediate();
	}

	public static void SetHideFlagsRecursively(this GameObject node, HideFlags flags) 
	{
		node.hideFlags = flags;
		foreach (Transform child in node.transform) {
			child.gameObject.SetHideFlagsRecursively(flags);
		}
	}
	
	public static void SetLayerRecursively(this GameObject node, int layer) 
	{
		node.layer = layer;
		foreach (Transform child in node.transform) {
			child.gameObject.SetLayerRecursively(layer);
		}
	}

	public static void SetChildrenActive(this GameObject node, bool flag) 
	{
		foreach (Transform child in node.transform) {
			child.gameObject.SetActive(flag);	
		}
	}

	public static void SendMessageDelayed(this GameObject node, string message, SendMessageOptions options = SendMessageOptions.RequireReceiver)
	{
		CoroutineHelper.instance.ExecuteActionNextFrame(() => node.SendMessage(message, options));
	}

	public static void SendMessageDelayed<T>(this GameObject node, string message, T param, SendMessageOptions options = SendMessageOptions.RequireReceiver)
	{
		CoroutineHelper.instance.ExecuteActionNextFrame(() => node.SendMessage(message, param, options));
	}

#if UNITY_EDITOR
	public static void SetStaticRecursively(this GameObject node, bool flag) 
	{
		node.isStatic = flag;
		foreach (Transform child in node.transform) {
			child.gameObject.SetStaticRecursively(flag);
		}
	}
#endif
}