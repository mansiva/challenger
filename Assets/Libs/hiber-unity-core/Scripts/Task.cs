using UnityEngine;
using System.Collections;

namespace Hibernum
{
	/// <summary>
	/// A Task object represents a coroutine.  Tasks can be started, paused, and stopped.
	/// It is an error to attempt to start a task that has been stopped or which has
	/// naturally terminated.
	/// </summary>
	public class Task
	{
		/// Delegate for termination subscribers.  manual is true if and only if
		/// the coroutine was stopped with an explicit call to Stop().
		public delegate void FinishedHandler(bool manual);
		
		/// Termination event. Triggered when the coroutine completes execution.
		public event FinishedHandler Finished;
		
		private TaskManager.TaskState _task;
		
		/// <summary>
		/// Returns true if and only if the coroutine is running.  Paused tasks
		/// are considered to be running.
		/// </summary>
		/// <value><c>true</c> if running; otherwise, <c>false</c>.</value>
		public bool Running
		{
			get { return _task.Running; }
		}
		
		/// <summary>
		/// Returns true if and only if the coroutine is currently paused.
		/// </summary>
		/// <value><c>true</c> if paused; otherwise, <c>false</c>.</value>
		public bool Paused
		{
			get { return _task.Paused; }
		}
		
		/// <summary>
		/// Creates a new Task object for the given coroutine.
		///
		/// If autoStart is true (default) the task is automatically started
		/// upon construction.
		/// </summary>
		/// <param name="c">C.</param>
		/// <param name="autoStart">If set to <c>true</c> auto start.</param>
		public Task(IEnumerator coroutine, bool autoStart = true)
		{
			_task = TaskManager.CreateTask(coroutine);

			_task.Finished += TaskFinished;

			if (autoStart)
			{
				Start();
			}
		}
		
		/// <summary>
		/// Begins execution of the coroutine
		/// </summary>
		public void Start()
		{
			_task.Start();
		}
		
		/// <summary>
		/// Discontinues execution of the coroutine at its next yield.
		/// </summary>
		public void Stop()
		{
			_task.Stop();
		}

		/// <summary>
		/// Pause this instance.
		/// </summary>
		public void Pause()
		{
			_task.Pause();
		}

		/// <summary>
		/// Unpause this instance.
		/// </summary>
		public void Unpause()
		{
			_task.Unpause();
		}

		private void TaskFinished(bool manual)
		{
			FinishedHandler handler = Finished;

			if (handler != null)
			{
				handler(manual);
			}
		}
	}
}