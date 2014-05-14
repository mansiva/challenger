using UnityEngine;
using System.Collections;

namespace Hibernum
{
	/// <summary>
	/// Task manager.
	/// </summary>
	public class TaskManager : MonoBehaviour
	{
		public class TaskState
		{
			public delegate void FinishedHandler(bool manual);		
			public event FinishedHandler Finished;
			
			private IEnumerator _coroutine;
			private bool _running;
			private bool _paused;
			private bool _stopped;

			/// <summary>
			/// Gets a value indicating whether this <see cref="TaskManager+TaskState"/> is running.
			/// </summary>
			/// <value><c>true</c> if running; otherwise, <c>false</c>.</value>
			public bool Running
			{
				get { return _running; }
			}

			/// <summary>
			/// Gets a value indicating whether this <see cref="TaskManager+TaskState"/> is paused.
			/// </summary>
			/// <value><c>true</c> if paused; otherwise, <c>false</c>.</value>
			public bool Paused
			{
				get	{ return _paused; }
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="TaskManager+TaskState"/> class.
			/// </summary>
			/// <param name="c">C.</param>
			public TaskState(IEnumerator coroutine)
			{
				_coroutine = coroutine;
			}

			/// <summary>
			/// Pause this instance.
			/// </summary>
			public void Pause()
			{
				_paused = true;
			}

			/// <summary>
			/// Unpause this instance.
			/// </summary>
			public void Unpause()
			{
				_paused = false;
			}

			/// <summary>
			/// Start this instance.
			/// </summary>
			public void Start()
			{
				_running = true;
				_instance.StartCoroutine(CallWrapper());
			}

			/// <summary>
			/// Stop this instance.
			/// </summary>
			public void Stop()
			{
				_stopped = true;
				_running = false;
			}
			
			private IEnumerator CallWrapper()
			{
				yield return null;

				IEnumerator enumerator = _coroutine;

				while (_running)
				{
					if (_paused)
					{
						yield return null;
					}
					else
					{
						if (enumerator != null && enumerator.MoveNext())
						{
							yield return enumerator.Current;
						}
						else
						{
							_running = false;
						}
					}
				}

				FinishedHandler handler = Finished;

				if (handler != null)
				{
					handler(_stopped);
				}
			}
		}

		private static TaskManager _instance;

		/// <summary>
		/// Creates the task.
		/// </summary>
		/// <returns>The task.</returns>
		/// <param name="coroutine">Coroutine.</param>
		public static TaskState CreateTask(IEnumerator coroutine)
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject("TaskManager");
				_instance = gameObject.AddComponent<TaskManager>();
			}
			
			return new TaskState(coroutine);
		}
	}
}