using UnityEngine;
using System.Collections;

public abstract class IndestructibleSingletonBehaviour<T> : SingletonBehaviour<T> where T : UnityEngine.Component
{
	protected override void Awake() {
		// Make sure we don't keep our parents alive with DontDestroyOnLoad
		transform.parent = null;
		DontDestroyOnLoad(gameObject);		
		base.Awake();	
	}
}
