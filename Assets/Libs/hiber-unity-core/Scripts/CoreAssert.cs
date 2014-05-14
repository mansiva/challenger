using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Core assert. Will be removed on release.
/// </summary>
public static class CoreAssert
{
	/// <summary>
	/// Warning assert when the specified condition fails.
	/// </summary>
	/// <param name="condition">If set to <c>true</c> condition.</param>
	/// <param name="context">Context.</param>
	[Conditional("ASSERT_ENABLED")]
	public static void Warning( bool condition, UnityEngine.Object context )
	{
		Warning( condition, context.ToString(), context );
	}

	/// <summary>
	/// Warning assert when the specified condition fails.
	/// </summary>
	/// <param name="condition">If set to <c>true</c> condition.</param>
	[Conditional("ASSERT_ENABLED")]
	public static void Warning( bool condition )
	{
		Warning( condition, string.Empty, null );
	}
	
	/// <summary>
	/// Warning assert when the specified condition fails.
	/// </summary>
	/// <param name="condition">If set to <c>true</c> condition.</param>
	/// <param name="message">Message.</param>
	[Conditional("ASSERT_ENABLED")]
	public static void Warning( bool condition, System.Object message )
	{
		Warning( condition, message, null );
	}
	
	/// <summary>
	/// Warning assert when the specified condition fails.
	/// </summary>
	/// <param name="condition">If set to <c>true</c> condition.</param>
	/// <param name="message">Message.</param>
	/// <param name="context">Context.</param>
	[Conditional("ASSERT_ENABLED")]
	public static void Warning( bool condition, System.Object message, UnityEngine.Object context )
	{
		if ( !condition ) {
			UnityEngine.Debug.LogWarning( message, context );
		}
	}

	/// <summary>
	/// Fatal assert when the specified condition fails.
	/// </summary>
	/// <param name='condition'>
	/// Condition.
	/// </param>
	/// <param name='context'>
	/// Context.
	/// </param>
	[Conditional("ASSERT_ENABLED")]
	public static void Fatal( bool condition, UnityEngine.Object context )
	{
		Fatal( condition, context.ToString(), context );
	}

	/// <summary>
	/// Fatal assert when the specified condition fails.
	/// </summary>
	/// <param name='condition'>
	/// Condition.
	/// </param>
	[Conditional("ASSERT_ENABLED")]
	public static void Fatal( bool condition )
	{
		Fatal( condition, string.Empty, null );
	}

	/// <summary>
	/// Fatal assert when the specified condition fails.
	/// </summary>
	/// <param name='condition'>
	/// Condition.
	/// </param>
	/// <param name='message'>
	/// Message.
	/// </param>
	[Conditional("ASSERT_ENABLED")]
	public static void Fatal( bool condition, System.Object message )
	{
		Fatal( condition, message, null );
	}

	/// <summary>
	/// Fatal assert when the specified condition fails.
	/// </summary>
	/// <param name='condition'>
	/// Condition.
	/// </param>
	/// <param name='message'>
	/// Message.
	/// </param>
	/// <param name='context'>
	/// Context.
	/// </param>
	[Conditional("ASSERT_ENABLED")]
	public static void Fatal( bool condition, System.Object message, UnityEngine.Object context )
	{
		if ( !condition ) {
			UnityEngine.Debug.LogError( message, context );
			UnityEngine.Debug.Break();
		}
	}
}