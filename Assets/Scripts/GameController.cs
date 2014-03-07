using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Backgammon
{
	
	// a move is a simple 2dvector / point structure.
	public struct Move
	{
		public Move(int s, int d){
			source = s;
			dest = d;
		}
		public int  source;
		public int dest;
	}
	
	
	public string MoveInString(Move m){
		return string.Format("{0}/{1}",m.source, m.dest);
	}
	
	public string ListMoveInString (List<Move> moves){
		string result = "";
		for (int i=0 ; i<moves.Count ; i++){
			result = result + " " + MoveInString(moves[i]);
		}
		return result;
	}

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
		private BGSnapshot snapshot;
		private Board.Side side;

		private float _startTime;

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
			snapshot = BGEngine.GetStartSnapshot(); // this will put the engine in a start snapshot of backgammon
			currentState = States.loaded;
		}
		
		// Update is called once per frame
		void Update ()
		{
			switch (currentState)
			{
			case States.loaded:
				currentState = States.started;
				board.SetSnapshot(snapshot);
				break;

			case States.started:
				// choose a side
				currentState = States.throwDice;
				side = Board.Side.dark;
				break;

			case States.throwDice:
				_startTime = Time.time;
				currentState = States.waitingForPlayersMove;
				side = BGEngine.OppositeSide(side);

				int d1 = Random.Range(1,6);
				int d2 = Random.Range(1,6);
				Debug.Log("Dice1: "+ d1 + ", Dice2: "+ d2);
				List<List <Move>> sols = bgEngine.AllSolutions(d1, d2, snapshot, side);
				List<Move> sol = sols[Random.Range(0, sols.Count)];
				snapshot = snapshot.ProjectSolution( sol ,side );
				board.PlaySolution(sol, side);
				break;

			case States.waitingForPlayersMove:
				if (Time.time - _startTime > 2f)
					currentState = States.throwDice;
				break;
			}
		}

	}
}
