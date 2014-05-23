using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Adding some useful behaviour to monobehaviour
/// </summary>
public abstract class CoreMonoBehaviour : MonoBehaviour
{
	/// <summary>
	/// The _blocking coroutine.
	/// </summary>
	private readonly List<Coroutine> _blocking = new List<Coroutine>();
	
	/// <summary>
	/// The _nonblocking.
	/// </summary>
	private readonly List<CoreTask> _nonblocking = new List<CoreTask>();
	
	/// <summary>
	/// Adds the blocking.
	/// </summary>
	/// <param name='task'>
	/// Task.
	/// </param>
	public void AddBlocking(IEnumerator task)
	{
		AddBlocking(CoreTask.Wrap(task));
	}
	
	/// <summary>
	/// Adds the blocking.
	/// </summary>
	/// <param name='task'>
	/// Task.
	/// </param>
	public void AddBlocking(CoreTask task)
	{
		if(task.Prime())
		{
			_blocking.Add(StartCoroutine(task));
		}
	}
	
    /// <summary>
    /// Adds the blocking.
    /// </summary>
    /// <param name='coroutine'>
    /// Coroutine.
    /// </param>
    public void AddBlocking(Coroutine coroutine)
    {
        _blocking.Add(coroutine);
    }
    
	/// <summary>
	/// Adds the blocking.
	/// </summary>
	/// <param name='task'>
	/// Task.
	/// </param>
    public Coroutine AddNonBlocking(IEnumerator task)
	{
		return AddNonBlocking(CoreTask.Wrap(task));
	}
	
	/// <summary>
	/// Adds the blocking.
	/// </summary>
	/// <param name='task'>
	/// Task.
	/// </param>
	public Coroutine AddNonBlocking(CoreTask task)
	{
		if(task.Prime())
		{
			_nonblocking.Add(task);
			return StartCoroutine(task);
		}
        return null;
	}
	
	/// <summary>
	/// Shoulds we wait on any blocking coroutine.
	/// </summary>
	/// <returns>
	/// The wait.
	/// </returns>
	public bool ShouldWait()
	{
		return _blocking.Count > 0;
	}
	
	/// <summary>
	/// Wait this instance.
	/// </summary>
	public IEnumerator Wait(bool waitNonBlocking = false)
	{
		while(_blocking.Count > 0)
		{
			yield return _blocking[_blocking.Count - 1];
			_blocking.RemoveAt(_blocking.Count - 1);
		}
		
		if(waitNonBlocking)
		{
			while(_nonblocking.Count > 0)
			{
				PurgeNonBlocking();
				yield return null;
			}
		}
		else
		{
			PurgeNonBlocking();
		}
	}
	
	public void PurgeNonBlocking()
	{
		for(var i = 0; i < _nonblocking.Count;)
		{
			if(_nonblocking[i].Done)
			{
				_nonblocking.RemoveAt(i);
			}
			else
			{
				i++;
			}
		}
	}
}