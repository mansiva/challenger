using System.Collections;
using System;

public class CoreTask : IEnumerator
{
	protected IEnumerator _task;
	private bool? _prime;
	protected bool _done;
	
	public bool Done
	{
		get
		{
			return _done;
		}
	}
	
	public static CoreTask Wrap(IEnumerator task)
	{
		return new CoreTask(task);
	}
	
	protected CoreTask(IEnumerator task)
	{
		_task = task;
	}
	
	public bool Prime()
	{
		CoreAssert.Fatal(_task != null);
		_prime = true;
		return _task.MoveNext();
	}
	
	public void Stop()
	{
		_prime = null;
		_task = null;
		_done = true;
	}
	
	#region IEnumerator implementation
	public virtual bool MoveNext ()
	{
		if(_prime.HasValue)
		{
			var retVal = _prime.Value;
			_prime = null;
			return retVal;
		}
		else if(_task != null)
		{
			var moveNext = _task.MoveNext();
			_done = !moveNext;
			return moveNext;
		}
		
		_done = true;
		return false;
	}

	public virtual void Reset ()
	{
		throw new System.NotSupportedException();
	}

	public virtual object Current 
	{
		get 
		{
			CoreAssert.Fatal(_task != null);
			return _task.Current;
		}
	}
	#endregion
}

public class CoreTaskResetable : CoreTask
{
	private Func<IEnumerator> _taskProvider;
	
	public static CoreTaskResetable WrapResetable(Func<IEnumerator> taskProvider)
	{
		return new CoreTaskResetable(taskProvider);
	}
	
	public CoreTaskResetable() : base(null)
	{
	}
	
	private CoreTaskResetable(Func<IEnumerator> taskProvider) : base(null)
	{
		_taskProvider = taskProvider;
		Reset();
	}
	
	public override bool MoveNext ()
	{
		base.MoveNext ();
		return true;
	}
	public override void Reset ()
	{
		Stop ();
		
		_task = _taskProvider();
		_done = false;
	}
	
	public override object Current 
	{
		get 
		{
			if(_task == null)
			{
				return null;
			}
			return base.Current;
		}
	}
}