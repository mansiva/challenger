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

	// a move is a simple 2dvector / point structure.
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
			return string.Format("{0}/{1}{2}",source, dest, capture ? "*":"");
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
				startSnapshot = new BGSnapshot(new int[] {-1, -1,0,0,0,0,5, 0,3,0,0,0,-5, 5,0,0,0,-3,0, -5,0,0,0,0,0, 2});
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

		// can the player bear off ?
		public bool BearingOff(){
			for(int i=7; i<26 ; i++){ // take the 25 bar into account
				if (snapshot[i] > 0){ // not if there is token not in the home board
					return false;
				}
			}
			return true;
		}

		//A die may not be used to bear off checkers from a lower-numbered point unless there are no checkers on any higher points.
		private bool BearingOffRule(int point, int die){ 
			if (!BearingOff()) return false; // you can't bear off
			if (die > point){ // trying to use a higher die on a lower point
				for(int i=point+1; i<7 ; i++){ // check if there is any checkers on any higher points.
					if (snapshot[i] > 1){
						return false;
					}
				}
			}
			else {
				return true;
			}
			return true;
		}

		// returns the new snapshot if move is played, assume validity of move
		public BGSnapshot ProjectMove( Move m){
			BGSnapshot board = new BGSnapshot(this);
			// Check capture
			if (board[m.dest] < 0){ // < 0 should be 1 opponent -2 wouldn't be possible
				board[0] -= 1; // put an opponent in 0
//				Debug.Log(string.Format("capture in Project move {0}", board[m.dest]));
				board[m.dest] = 1;
				m.capture = true;
			}
			else {
				board[m.dest] += 1;
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

		private List<int> PossibleSources(){
			List<int> solutions = new List<int> ();
			if (this.snapshot[25] != 0){ // player is forced to get bar in
				solutions.Add(25);
			}
			else {
				for (int i=1; i<25 ; i++){
					if (this.snapshot[i] > 0) solutions.Add(i);
				}
			}
			return solutions;
		}

		// helper, give a list of move possible for one die
		private List<Move> MoveDie(int die){
			List<Move> solutions = new List<Move> ();
			// TBC
			List<int> sources = this.PossibleSources();
			for (int i=0 ; i < sources.Count ; i++){
				int destinationPoint = sources[i] - die; // update here bear off
				
				if ( destinationPoint >= 1 && snapshot[destinationPoint] >= -1) { // is the destination point in the board ?
					solutions.Add(new Move(sources[i],destinationPoint));
				}
				else { // BearOff possible ?
					if(BearingOffRule(sources[i], die)){
						solutions.Add(new Move(sources[i], -1));// -1 means bearing off ?
					}
				}
			}
			//			Debug.Log ("MoveDie -> "+ListMoveInString(solutions));
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
			foreach(Move m in currentBoard.MoveDie(dice.Pop())) // eventually a bug here when list is empty
			{
				BGSnapshot newBoard = currentBoard.ProjectMove(m);
				//Debug.Log(string.Format("Played move : {0} {1}", m.source,m.dest));
				List<Move> solution = new List<Move>(currentSolution);
				solution.Add(m);
				if (dice.Count == 0){
				//addcurrentcolution To finalSolution
					finalSolution.Add(solution); 
//					Debug.Log ("Adding Current Solution :" + ListMoveInString(solution));
					// currentSolution = new List<Backgammon.Move>(); // new Current Solution
				}
				else{	// Compute dice currentSolution currentBoard
					//Debug.Log(string.Format("Going deeper with {0}", dice));
					Compute(new Stack<int>(dice), solution, finalSolution, newBoard);
				}
					//printNode(child); //<-- recursive
			}
		}
	}
}
