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

		// return the start Position
		public static BGSnapshot GetStartSnapshot(){
			// create the start snapshot, or simply returns it if already created
			if (startSnapshot == null){
				startSnapshot = new BGSnapshot(new int[] {0, -2,0,0,0,0,5, 0,3,0,0,0,-5, 5,0,0,0,-3,0, -5,0,0,0,0,2, 0});
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
				s+= " " + snapshot[i];
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
			if (board[m.dest] < 0){ // < 0 means -1
				board[25] -= 1; // put an opponent in 25
			}
			else {
				board[m.dest] += 1;
			}
			// empty check and side
			board[m.source] -= 1;
			return board;
		}

		// returns a Snapshot if list of moves are played
		public BGSnapshot ProjectSolution( List<Move> solution){
			BGSnapshot snapshot = new BGSnapshot(this);
			Debug.Log(BGEngine.ListMoveInString(solution));

			for (int i=0 ; i<solution.Count ; i++){
				snapshot = snapshot.ProjectMove(solution[i]);
			}
			return snapshot;
		}

		private List<int> PossibleSources(){
			List<int> solutions = new List<int> ();
			if (currentSnapshot[25] > 0){ // player is forced to get bar in
				solutions.Add(25);
			}
			else {
				for (int i=1; i<25 ; i++){
					if (currentSnapshot[i] > 0) solutions.Add(i);
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
				
				if ( destinationPoint >= 1 && snapshot[point] >= -1) { // is the destination point in the board ?
					solutions.Add(new Move(sources[i],destinationPoint));
				}
				else { // BearOff possible ?
					if(BearingOffRule(sources[i], die)){
						solutions.Add(new Move(sources[i], -1));
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
				dice.Push(die1);
				dice.Push(die2);
				diceConfig.Add(diceStraight);
				Stack<int> dice = new Stack<int>();
				dice.Push(die2); // reversed
				dice.Push(die1);
				diceConfig.Add(dice);
			}

			List<List <Move>> finalSolution = new List<List <Move>>();
			foreach(Stack<int> dice in diceConfig){
				this.Compute(dice, new List<Backgammon.Move>(), finalSolution ,snapshot);
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
			foreach(Move m in MoveDie(currentBoard, dice.Pop()))
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
