using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;

#region Editor YieldInstruction Types
public class EditorWaitForSeconds : YieldInstruction
{
	private float _duration;
	
	public float duration {
		get { return _duration; }
	}
	
	public EditorWaitForSeconds(float duration) {
		_duration = duration;
	}
}

public class EditorCoroutine : YieldInstruction
{
	private CoreTask _child;

	public CoreTask child {
		get { return _child; }
	}

	public EditorCoroutine(CoreTask child) {
		_child = child;
	}
}
#endregion

/// <summary>
/// EditorCoroutineManager
/// 
/// An attempt to replicate the unity coroutine behaviour for use in the editor
/// 
/// NOTE: This class does not work with the built in YieldInstruction types such as WaitForSeconds, WaitForEndOfFrame, Coroutine
/// 	  make sure to use the Editor version of these functions i.e EditorWaitForSeconds, EditorCoroutine etc.
/// 
/// If you need to add more YieldInstruction behaviours
///  -create a class for the instruction
///  -implement the behaviour in a strategy  
///  -map the strategy to the type (see sStrategies dictionary)
/// 
/// </summary>
public static class EditorCoroutineManager
{
	/// <summary>
	/// Editor coroutine strategy, extend this class to add new instruction logic to the system
	/// </summary>
	private abstract class EditorCoroutineStrategy
	{
		protected List<CoreTask> _completed = new List<CoreTask>();

		public List<CoreTask> completed { 
			get { 
				return _completed;
			} 
		}

		public void ClearCompleted() 
		{
			_completed.Clear();
		}

		/// <summary>
		/// Update tick, called 100 times per second
		/// 
		/// Perform any processing or checks for completed coroutines here
		/// </summary>
		public virtual void Update() {}

		/// <summary>
		/// Should return true if the instance holds no coroutines
		/// </summary>
		public abstract bool IsEmpty();

		/// <summary>
		/// Used to delegate a coroutine to the instance
		/// </summary>
		public abstract void AddCoroutine(CoreTask coroutine);
	}

	/// <summary>
	/// Convenience class for strategys that pause until a condition is met on the object
	/// </summary>
	private abstract class EditorPausedCoroutineStrategy : EditorCoroutineStrategy
	{
		protected List<CoreTask> _paused = new List<CoreTask>();
		
		public override bool IsEmpty() 
		{
			return _paused.Count == 0 && _completed.Count == 0;
		}
		
		public override void AddCoroutine(CoreTask coroutine) 
		{
			_paused.Add(coroutine);
		}
	}

	/// <summary>
	/// Coroutine is paused until the child is Done
	/// </summary>
	private class EditorNestedCoroutineStrategy : EditorPausedCoroutineStrategy
	{
		public override void Update() 
		{
			for (int i=0; i<_paused.Count; i++)
			{	
				if (((EditorCoroutine)_paused[i].Current).child.Done) {
					_completed.Add(_paused[i]);
					_paused.RemoveAt(i);
					i--;
				}
			}
		}
	}

	/// <summary>
	/// Coroutine is paused until the WWW call is done
	/// </summary>
	private class EditorWWWCoroutineStrategy : EditorPausedCoroutineStrategy 
	{
		public override void Update ()
		{
			for (int i=0; i<_paused.Count; i++)
			{	
				if (((WWW)_paused[i].Current).isDone) {
					_completed.Add(_paused[i]);
					_paused.RemoveAt(i);
					i--;
				}
			}
		}
	}

	/// <summary>
	/// Coroutine is paused until x seconds have passed
	/// </summary>
	private class EditorWaitForSecondsStrategy : EditorCoroutineStrategy
	{
		private SortedList<DateTime, CoreTask> _delayed = new SortedList<DateTime, CoreTask>();
		
		public override bool IsEmpty() 
		{
			return _delayed.Count == 0 && _completed.Count == 0;
		}
		
		public override void AddCoroutine(CoreTask coroutine) 
		{
			_delayed.Add(DateTime.Now.AddSeconds(((EditorWaitForSeconds)coroutine.Current).duration), coroutine);
		}
		
		public override void Update() 
		{
			int count = 0;
			foreach (var pair in _delayed)
			{	
				if (DateTime.Compare(pair.Key, DateTime.Now) <= 0) {
					_completed.Add(pair.Value);
					count++;
				}
				else {
					break;
				}
			}
			
			for (int i=0; i<count; i++) {
				_delayed.RemoveAt(0);
			}
		}
	}

	/// <summary>
	/// Coroutine is immediately added to the completed list and run next frame
	/// </summary>
	private class EditorRunNextTickStrategy : EditorCoroutineStrategy
	{
		public override bool IsEmpty ()
		{
			return true;
		}

		public override void AddCoroutine (CoreTask coroutine)
		{
			_completed.Add(coroutine);
		}
	}

	/*
	 * EditorCoroutineManager
	 */ 
	private static EditorApplication.CallbackFunction sEditorUpdate = new EditorApplication.CallbackFunction(EditorUpdate);
	private static bool sUpdating = false;

	private static List<CoreTask> sCoroutines = new List<CoreTask>();

	/// <summary>
	/// Mapping of how the manager should handle instruction types
	/// </summary>
	private static Dictionary<Type, EditorCoroutineStrategy> sStrategies = new Dictionary<Type, EditorCoroutineStrategy>() {
		{ typeof(EditorWaitForSeconds), 	new EditorWaitForSecondsStrategy() 	},
		{ typeof(EditorCoroutine), 			new EditorNestedCoroutineStrategy()	},
		{ typeof(WWW), 						new EditorWWWCoroutineStrategy()	},
		{ typeof(int), 						new EditorRunNextTickStrategy()		}, // don't output an error on int, instead just run it next frame
	};

	public static EditorCoroutine StartCoroutine(IEnumerator routine) 
	{
		return StartCoroutine(CoreTask.Wrap(routine));
	}

	public static EditorCoroutine StartCoroutine(CoreTask routine) 
	{
		if (!sUpdating) {
			// Make sure our update loop is running
			EditorApplication.update += sEditorUpdate;
			sUpdating = true;
		}

		sCoroutines.Add(routine);
		
		return new EditorCoroutine(routine);
	}

	/// <summary>
	/// Editor update tick, called approx 100 times per second
	/// </summary>
	private static void EditorUpdate()
	{
		List<CoreTask> nextFrameCoroutines = new List<CoreTask>();
		List<CoreTask> coroutines = new List<CoreTask>(sCoroutines);

		sCoroutines.Clear();

		foreach (CoreTask coroutine in coroutines)
		{
			if (!coroutine.MoveNext()) {
				// This coroutine has finished
				continue;
			}

			if (null == coroutine.Current) {
				// This coroutine yielded null, run it next frame
				nextFrameCoroutines.Add(coroutine);
				continue;
			}

			// Try to delegate this instruction to the strategy obejcts
			Type yieldType = coroutine.Current.GetType();

			if (sStrategies.ContainsKey(yieldType)) {
				sStrategies[yieldType].AddCoroutine(coroutine);
				continue;
			}

			Debug.LogError ("EditorCoroutineManager:: Unsupported yield type '" + yieldType.ToString() + "'");

			// We were unable to delegate this instruction run it again next frame
			nextFrameCoroutines.Add(coroutine);
		}

		sCoroutines.AddRange(nextFrameCoroutines);

		bool allStrategiesEmpty = true;

		foreach (EditorCoroutineStrategy strategy in sStrategies.Values) {
			// tick the strategy object and fetch all completed coroutines
			strategy.Update();
			sCoroutines.AddRange(strategy.completed);
			strategy.ClearCompleted();
			allStrategiesEmpty &= strategy.IsEmpty();
		}

		if (sCoroutines.Count == 0 && allStrategiesEmpty) {
			// We are done processing all coroutines, unregister from update loop
			sUpdating = false;
			EditorApplication.update -= sEditorUpdate;
		}
	}
}


