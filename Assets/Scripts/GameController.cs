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
			throwDice,
			waitingForPlayersMove,
		}

		// the Engine knows the rule of BG :)
		private BGEngine bgEngine;
		// the Board can display a board
		private Board board;
		private BGPosition position;
		private Board.Side side;

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
			position = BGEngine.GetStartPosition(); // this will put the engine in a start position of backgammon
			currentState = States.loaded;
		}
		
		// Update is called once per frame
		void Update () {
			if (currentState == States.loaded)
			{
				currentState = States.started;
				board.SetPosition(position);
			}

			else if (currentState == States.started)
			{
				// choose a side
				currentState = States.throwDice;
				side = Board.Side.dark;
			}
			else if (currentState == States.throwDice)
			{
				currentState = States.waitingForPlayersMove;
				side = BGEngine.OppositeSide(side);
				Debug.Log("Side is now " + side);
				StartCoroutine(RandomPlayer(Random.Range(1, 7), Random.Range(1, 7), position, Board.Side.light));
//				for (int i=0; i<sols.Count ; i++)
//				{
//					Debug.Log(BGEngine.ListMoveInString(sols[i]));
//				}
			}
		}

		IEnumerator RandomPlayer(int d1, int d2, BGPosition position, Board.Side side){
			List<List <Move>> sols = bgEngine.AllSolutions(d1,d2,position, side);
			List<Move> sol = sols[Random.Range(0, sols.Count)];
			position = position.ProjectSolution( sol ,side );
			board.PlaySolution(sol, side);
			currentState = States.throwDice;
			yield return new WaitForSeconds(2);
		}
	}
}
