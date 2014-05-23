using UnityEngine;
using System.Collections;

public static class ResourceExtensions {

	public static T[] LoadAll<T>(string path) where T : Object {
		Object[] objs = Resources.LoadAll(path,  typeof(T));	
		T[] r = new T[objs.Length];
		
		for (int i=0; i<r.Length; i++) {
			r[i] = (T)objs[i];
		}	
		
		return r;
	}
}
