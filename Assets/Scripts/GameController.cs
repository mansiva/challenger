using UnityEngine;
using System.Collections;

namespace Backgammon
{
	public class GameController : MonoBehaviour
	{
		private Board board;

		// Use this for initialization
		void Start ()
		{
			// Load the prefab Assets/Resources/Board.prefab
			GameObject prefabBoard = Resources.Load<GameObject>("Board");
			GameObject boardObject = NGUITools.AddChild(gameObject, prefabBoard);
			// Get script Board attached to prefab
			board = boardObject.GetComponent<Board>();	

//			board.ResetTokens ();
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}
