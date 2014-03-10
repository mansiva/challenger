﻿using UnityEngine;
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
		// Temp colors b & w
		private Color black;
		private Color white;


		// the Board can display a board
		private Board board;
		private BGSnapshot snapshot;
		private bool side; // true is black, false is white.
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
			white = new Color(1,1,1);
			black = new Color(0,0,0);

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
			//			int d1 = Random.Range(1,6);
			//			int d2 = Random.Range(1,6);
			int d1 = 1;
			int d2 = 6;
			dice1.text = d1.ToString ();
			dice2.text = d2.ToString ();
			dice1.color = side? black:white;
			dice2.color = side? black:white;

			Debug.Log(snapshot.toString());
			List<List <Move>> sols = snapshot.AllSolutions(d1, d2);
			Debug.Log(string.Format("Solutions :{0}",sols.Count));
			List<Move> rsol = sols[Random.Range(0, sols.Count)];
			snapshot = snapshot.ProjectSolution( rsol ); 
			if (!side){
//				Debug.Log("Reversing for the board");
				Move.ListMoveReverse(rsol);
			} 
			Debug.Log(snapshot.toString());
			board.PlaySolution(rsol);
			snapshot.Reverse();
			side = !side;
		}

	}

}
