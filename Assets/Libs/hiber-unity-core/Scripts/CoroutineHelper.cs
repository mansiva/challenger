using System;
using UnityEngine;
using System.Collections;

public class CoroutineHelper : SingletonBehaviour<CoroutineHelper>
{
	public void ExecuteActionNextFrame(Action action) 
	{
		StartCoroutine("DoActionNextFrame", action);
	}

	private IEnumerator DoActionNextFrame(Action action) 
	{
		yield return new WaitForEndOfFrame();
		action();
	}
}
