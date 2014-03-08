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

		// the Board can display a board
		private Board board;
		private BGSnapshot snapshot;
		private bool side;
		private float _startTime;

		private States currentState;
		// Use this for initialization
		void Start ()
		{
			// Load the prefab Assets/Resources/Board.prefab
			Debug.Log("Start");
			GameObject prefabBoard = Resources.Load<GameObject>("Board");
			GameObject boardObject = NGUITools.AddChild(gameObject, prefabBoard);
			// Get script Board attached to prefab
			board = boardObject.GetComponent<Board>();
			snapshot = new BGSnapshot(BGSnapshot.GetStartSnapshot());
			currentState = States.loaded;
			side = true;
		}
		
		// Update is called once per frame
		void Update ()
		{
			switch (currentState)
			{
			case States.loaded:
				Debug.Log("States.loaded");
				currentState = States.started;
				board.SetSnapshot(snapshot);
				break;

			case States.started:
				// choose a side
				currentState = States.throwDice;
//				side = Board.Side.dark;
				break;

			case States.throwDice:
				_startTime = Time.time;
				currentState = States.waitingForPlayersMove;

				int d1 = Random.Range(1,6);
				int d2 = Random.Range(1,6);
				Debug.Log(snapshot.toString());
				Debug.Log("Dice1: "+ d1 + ", Dice2: "+ d2);
				List<List <Move>> sols = snapshot.AllSolutions(d1, d2, side);
				List<Move> sol = sols[Random.Range(0, sols.Count)];
				snapshot = snapshot.ProjectSolution( sol );
				Debug.Log(snapshot.toString());
				board.PlaySolution(sol,side);
				snapshot.Reverse();
				side = !side;
				break;

			case States.waitingForPlayersMove:
				if (Time.time - _startTime > 1f)
					currentState = States.throwDice;
				break;
			}
		}

	}
}
