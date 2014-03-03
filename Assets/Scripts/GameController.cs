using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Backgammon
{
	public class GameController : MonoBehaviour
	{
		public enum States
		{
			loaded,
			started,
		}

		// the Engine knows the rule of BG :)
		private BGEngine bgEngine;
		// the Board can display a board
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
			bgEngine = new BGEngine();
			bgEngine.SetPosition(BGEngine.GetStartPosition()); // this will put the engine in a start position of backgammon
			currentState = States.loaded;
		}
		
		// Update is called once per frame
		void Update () {
			if (currentState == States.loaded){
				currentState = States.started;
				board.SetPosition(BGEngine.GetStartPosition());
				List<List <Move>> sols = bgEngine.AllSolutions(6,1,Board.Side.light);
				for (int i=0; i<sols.Count ; i++)
				{
					List<Move> movs = sols[i];
					for(int j=0; j<movs.Count; j++)
					{
						Debug.Log(sols[i][j].source);
						Debug.Log(sols[i][j].dest);
					}
				}
			}
			else if (currentState == States.started)
			{}
		}
	}
}
