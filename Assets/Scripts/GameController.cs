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
			playerMove,
			waitingForPlayersChoice,
			waitingForPlayersValidation
		}

		// Temp dice
		public UILabel dice1;
		public UILabel dice2;
		// Temp colors b & w
		private Color black;
		private Color white;


		// the Board is the view that can display a board
		private Board board;
		private BGSnapshot snapshot;
		private int die1;
		private int die2;
		private List<List <Move>> sols;
		private bool side; // true is black, false is white.
		private State currentState;

		// ------------------------------------------------------------
		// Use this for initialization
		// ------------------------------------------------------------
		void Awake ()
		{
			if (board != null)
				Destroy (board.gameObject);
			board = NGUITools.AddChild(gameObject, Resources.Load<GameObject>("Board")).GetComponent<Board>();
			board.InitBoard (); // creates the token
			white = new Color(1,1,1);
			black = new Color(0,0,0);
		}


		void Start()
		{
			OnReset ();
		}

		// ------------------------------------------------------------
		// Reset board to starting position, also equal to give up
		// ------------------------------------------------------------
		void OnReset()
		{
			dice1.text = "";
			dice2.text = "";

			snapshot = new BGSnapshot(BGSnapshot.GetStartSnapshot());
			board.SetSnapshot(snapshot);
			//board.HomeBoard ();
			//currentState = State.loaded;
			OnDecideSide();
		}


		// ------------------------------------------------------------
		// Decide wich side should start the game, we decide you start with black :)
		// ------------------------------------------------------------
		void OnDecideSide(){
			side = true;
			currentState = State.waitingForPlayersChoice;
		}

		// ------------------------------------------------------------
		// Throw Dice, calculate possible solutions
		// ------------------------------------------------------------		
		void OnThrowDice()
		{
			die1 = Random.Range(1,6);
			die2 = Random.Range(1,6);
			dice1.text = die1.ToString ();
			dice2.text = die2.ToString ();
			dice1.color = side? black:white;
			dice2.color = side? black:white;
			dice1.gameObject.SetActive(true);
			dice2.gameObject.SetActive(true);

			Debug.Log(snapshot.toString());
			sols = snapshot.AllSolutions(die1, die2);
			if(sols.Count == 0){ // forced pass
				Debug.Log("No move possible");
				OnPlayerValidate();
			}
			else if (sols.Count == 1){ // forced move
				Debug.Log("Forced Move");
				OnSimulate();
			}
			else {
				currentState = State.playerMove;
			}
		}

		// ------------------------------------------------------------
		// Simulate next move
		// ------------------------------------------------------------
		void OnSimulate()
		{
			List<Move> rsol = sols[Random.Range(0, sols.Count)];
			snapshot = snapshot.ProjectSolution( rsol ); 
			if (!side){
				Move.ListMoveReverse(rsol);
			} 
			Debug.Log(snapshot.toString());
			board.PlaySolution(rsol);
			OnPlayerValidate();
		}

		// ------------------------------------------------------------
		// Player just took his dice, to say his move was over
		// ------------------------------------------------------------
		void OnPlayerValidate()
		{
			Debug.Log("Should check if player won");
			Debug.Log("Switch Player");
			dice1.gameObject.SetActive(false);
			dice2.gameObject.SetActive(false);
			snapshot.Reverse();
			side = !side;
			currentState = State.waitingForPlayersChoice;
		}

		void OnForcedMove(){

		}

		void OnValidateMove(){
			//player has finished its move, we check if it's valid
			//we also check if this is a win
			//it's now the opponent side
			//opponent can chose to double, throw the dice or restart;
		}

		void OnDouble()
		{

		}

		void OnWin(){
			Debug.Log ("WIN");
		}


		void OnRightButtonClick(){
			switch(currentState)
			{
			case State.waitingForPlayersChoice :
				OnThrowDice();
				break;
			case State.playerMove:
				OnSimulate();
				break;
			}
		}

	}

}
