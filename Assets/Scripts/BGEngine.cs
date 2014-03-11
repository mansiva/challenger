using UnityEngine;
using System.Collections.Generic;
using System;

namespace Backgammon
{
	//A player may not move any other checkers until all checkers on the bar belonging to that player have re-entered the board
	//If moves can be made according to either one die or the other, but not both, the higher number must be used.
	//If one die is unable to be moved, but such a move is made possible by the moving of the other die, that move is compulsory.
	//A die may not be used to bear off checkers from a lower-numbered point unless there are no checkers on any higher points.
	//The same checker may be moved twice as long as the two moves are distinct

	public class Move
	{
		public int source;
		public int dest;
		public bool capture;
		public bool bearoff;

		public Move(int s, int d){
			source = s;
			dest = d;
			capture=false;
			bearoff=false;
		}

		public Move Reverse(){
			dest = 25 - dest;
			source = 25 - source;
			return this;
		}
		public string toString(){
			return string.Format("{0}/{1}{2}",source, bearoff? "off" : ""+dest, capture ? "*":"");
		}
		
		public static string ListMoveToString (List<Move> moves){
			string result = "";
			for (int i=0 ; i<moves.Count ; i++){
				result = result + " " + moves[i].toString();
			}
			return result;
		}
		public static void ListMoveReverse (List<Move> moves){
			for (int i=0 ; i<moves.Count ; i++){
				moves[i].Reverse();
			}
		}
	}
	


	// A BG Snapshot contains a bg position always from the player's turn point of view. 
	public class BGSnapshot
	{
		// + means player, - means opponent, 0 means empty
		// index 0 is bar for opponent, index 25 is bar for player. player is trying to bring everything to 1.
		private int[] snapshot = new int[26];

		// private start position, use the static method to get it.
		private static BGSnapshot startSnapshot; 

		// creates a BGSnapshot
		public BGSnapshot(int[] v){
			this.snapshot = v;
		}
		public BGSnapshot(BGSnapshot s){
			Array.Copy(s.snapshot,this.snapshot,26);
		}

		// return the start Position
		public static BGSnapshot GetStartSnapshot(){
			// create the start snapshot, or simply returns it if already created
			if (startSnapshot == null){
				// test with one capture to start with
				startSnapshot = new BGSnapshot(new int[] {0, -2,0,0,0,0,5, 0,3,0,0,0,-5, 5,0,0,0,-3,0, -5,0,0,0,0,2, 0});
//				startSnapshot = new BGSnapshot(new int[] {0, 3,0,0,0,0,0, 0,0,0,0,0,-5, -5,-5,0,0,0,0, 0,0,0,0,0,0, 0});
			}
			
			return startSnapshot;
		}

		public int this[int index] {
			get {
				return snapshot[index];
			}
			set {
				snapshot[index] = value;
			}
		}

		public int Length {
			get {
				return snapshot.Length;
			}
		}

		public string toString(){
			string s = "";
			for (int i=0; i<snapshot.Length ; i++){
				s+= string.Format("{0,3}",snapshot[i]);
			}
			return s;
		}


		// returns the new snapshot if move is played, assume validity of move
		public BGSnapshot ProjectMove( Move m){
			BGSnapshot board = new BGSnapshot(this);
			// Check nothing to be done on the dest point if bearoff.
			if (!m.bearoff){
				if (board[m.dest] < 0){ // Test if capture
					board[0] -= 1; // put an opponent in 0
	//				Debug.Log(string.Format("capture in Project move {0}", board[m.dest]));
					board[m.dest] = 1;
					m.capture = true;
				}
				else {
					board[m.dest] += 1;
				}
			}
			board[m.source] -= 1;
			return board;
		}

		// returns a Snapshot if list of moves are played
		public BGSnapshot ProjectSolution( List<Move> solution){
			BGSnapshot snapshot = new BGSnapshot(this);
//			Debug.Log(Move.ListMoveToString(solution));
			for (int i=0 ; i<solution.Count ; i++){
				snapshot = snapshot.ProjectMove(solution[i]);
			}
			return snapshot;
		}
		// Updated, streamlined version, with only one scan of the Board
		private List<Move> MoveDie2(int die){
			List<Move> solutions = new List<Move> ();
			Move m;
			int bearOffIndex = 0; // the further tocken
			// Normal Moves
			for (int i=25; i>die; i--) { // if die = 6, will stop at 7
				if  (snapshot[i] > 0) { // is there a Tocken to consider ?
					bearOffIndex = Math.Max(i,bearOffIndex); // the further tocken
					if (snapshot[i-die] < -1){ // Point is occupied
						if (i == 25) return solutions; 
					}
					else if (snapshot[i-die] == -1){ // Capture
						m = new Move(i, i-die);
						m.capture = true;
						solutions.Add(m);
						if (i == 25) return solutions; 
					} else { 						// simple enter
						m = new Move(i, i-die);
						solutions.Add(m);
						if (i == 25) return solutions; 
					}
					// simple move
				}
			}
			// BearOff Moves
			if (bearOffIndex < 7){
				for (int i=die; i>0; i--) { // last tokens
					if  (snapshot[i] > 0) { // is there a Tocken to consider ?
						bearOffIndex = Math.Max(i,bearOffIndex); // the further tocken
						// the rule here is that i can t use a die=5 on a token in i=4 if there is a token is index=6
						//A die may not be used to bear off checkers from a lower-numbered point unless there are no checkers on any higher points.
						// index=6  die=5 i=4
						if (! (i < bearOffIndex && die > i)){
							m = new Move(i,0 );
							m.bearoff = true;
							solutions.Add(m);
						}
						else {
//							Debug.Log(string.Format("bearOff impossible i={0} bearOffIndex={1} die={2}", i,bearOffIndex,die));
						}
					}
				}
			}
			return solutions;
		}

		// returns a list of solutions (= list of moves)
		public List<List <Move>> AllSolutions(int die1, int die2){
			//List<List <Move>> result = new List<List <Move>> ();
			// doubles or singles
			List <Stack<int>> diceConfig = new List<Stack<int>> ();
			if (die1 == die2) {
				Stack<int> dice = new Stack<int>();
				dice.Push(die1);
				dice.Push(die1);
				dice.Push(die1);
				dice.Push(die1);
				diceConfig.Add(dice);
			}
			else {
				Stack<int> diceStraight = new Stack<int>();
				diceStraight.Push(die1);
				diceStraight.Push(die2);
				diceConfig.Add(diceStraight);
				Stack<int> dice = new Stack<int>();
				dice.Push(die2); // reversed
				dice.Push(die1);
				diceConfig.Add(dice);
			}

			List<List <Move>> finalSolution = new List<List <Move>>();
			foreach(Stack<int> d in diceConfig){
				this.Compute(d, new List<Move>(), finalSolution ,this);
			}
			// should also compute reversed dice when not double.

			return finalSolution;
		}

		public BGSnapshot Reverse(){
			int[] newBoard = new int[26];
			for (int i=0; i<26 ; i++){
				newBoard[25-i] = -snapshot[i];
			}
			snapshot = newBoard;
			return this;
		}
		
		public  void Compute(Stack<int> dice, List<Move> currentSolution, List<List <Move>> finalSolution, BGSnapshot currentBoard){
			//pop die and get possibleMoves
			//currentBoard.PrintSnapshot();
//			Debug.Log(string.Format("Compute with a dice count of {0}",dice.Count));

			// when first die has no move go deeper
			// when second die has no move doesn't add the solution
			BGSnapshot newBoard;
			List<Move> solution = new List<Move>();
			List<Move> moves = currentBoard.MoveDie2(dice.Pop());
			// this isn't the good fix it seems.
//			Debug.Log(Move.ListMoveToString(moves));
			if (moves.Count == 0 && dice.Count>0){ // use second die
				solution = new List<Move>(currentSolution);
				Compute(new Stack<int>(dice), solution, finalSolution, currentBoard);
//				Debug.Log("moves.Count == 0 && dice.Count>0");
			}
			else if (moves.Count == 0 && dice.Count == 0 && currentSolution.Count > 0){
				finalSolution.Add(currentSolution); 
				// no move no dice ?
			}

			foreach(Move m in moves)
			{
				newBoard = currentBoard.ProjectMove(m);
//				Debug.Log(string.Format("Played move : {0} {1}", m.source,m.dest));
				solution = new List<Move>(currentSolution); // BUG somewhere the solution get erased
				solution.Add(m);
//				Debug.Log(string.Format("Dice.count:{0}",dice.Count));
				if (dice.Count == 0){ // No more Die to Compute
				//addcurrentcolution To finalSolution
					finalSolution.Add(solution); 
//					Debug.Log ("Adding Current Solution :" + Move.ListMoveToString(solution));
				}
				else{	// There is still Dice to Compute
					//Debug.Log(string.Format("Going deeper with {0}", dice));
					Compute(new Stack<int>(dice), solution, finalSolution, newBoard);
				}
			}
		}
	}
}
