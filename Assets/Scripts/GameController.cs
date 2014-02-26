using UnityEngine;
using System.Collections;

namespace Backgammon
{
	public class GameController : MonoBehaviour
	{
		public enum States
		{
			loaded,
			started,
		}

		private Board board;
		private States currentState;
		// Use this for initialization
		void Start ()
		{
			// Load the prefab Assets/Resources/Board.prefab
			GameObject prefabBoard = Resources.Load<GameObject>("Board");
			GameObject boardObject = NGUITools.AddChild(gameObject, prefabBoard);
			// Get script Board attached to prefab
			board = boardObject.GetComponent<Board>();
			currentState = States.loaded;

		}
		
		// Update is called once per frame
		void Update () {
			if (currentState == States.loaded){
				board.ResetTokens ();
				currentState = States.started;
			}
			else if (currentState == States.started)
			{}
		}
	}
}
