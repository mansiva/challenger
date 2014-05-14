using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Core assert. Will be removed on release.
/// </summary>
public static class CoreEditorAssert
{
	/// <summary>
	/// Fatal assert when the specified condition fails.
	/// </summary>
	/// <param name='condition'>
	/// Condition.
	/// </param>
	/// <param name='context'>
	/// Context.
	/// </param>
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
	public static void Fatal( bool condition, System.Object message, UnityEngine.Object context )
	{
		if ( !condition ) {
			UnityEngine.Debug.LogError( message, context );
		}
	}
}