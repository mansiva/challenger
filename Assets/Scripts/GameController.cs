using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Backgammon
{
	public class GameController : MonoBehaviour
	{
		public enum State
		{
			loaded,
			started,
			throwDice,
			waitingForPlayersMove,
		}

		// Temp dice
		public UILabel dice1;
		public UILabel dice2;

		// the Board can display a board
		private Board board;
		private BGSnapshot snapshot;
		private bool side;
		private State currentState;

		// ------------------------------------------------------------
		// Use this for initialization
		// ------------------------------------------------------------
		void Awake ()
		{
			// TODO: No need for this now since we create it in reset (temp)
			//board = NGUITools.AddChild(gameObject, Resources.Load<GameObject>("Board")).GetComponent<Board>();
		}


		void Start()
		{
			OnReset ();
		}

		// ------------------------------------------------------------
		// Reset board to starting position
		// ------------------------------------------------------------
		void OnReset()
		{
			// TODO: Board should have a function to set a specific snapshot
			if (board != null)
				Destroy (board.gameObject);
			board = NGUITools.AddChild(gameObject, Resources.Load<GameObject>("Board")).GetComponent<Board>();
			dice1.text = "";
			dice2.text = "";

			snapshot = new BGSnapshot(BGSnapshot.GetStartSnapshot());
			board.SetSnapshot(snapshot);
			currentState = State.loaded;
			side = true;
		}


		// ------------------------------------------------------------
		// Simulate next move
		// ------------------------------------------------------------
		void OnSimulate()
		{
			int d1 = Random.Range(1,6);
			int d2 = Random.Range(1,6);
			dice1.text = d1.ToString ();
			dice2.text = d2.ToString ();

			Debug.Log(snapshot.toString());
			List<List <Move>> sols = snapshot.AllSolutions(d1, d2, side);
			List<Move> sol = sols[Random.Range(0, sols.Count)];
			snapshot = snapshot.ProjectSolution( sol );
			Debug.Log(snapshot.toString());
			board.PlaySolution(sol,side);
			snapshot.Reverse();
			side = !side;
		}

	}

}
