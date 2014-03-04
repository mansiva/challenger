using UnityEngine;
using System.Collections.Generic;
using System;

namespace Backgammon
{
	// this is used to compute possible moves without interfering with the View

	//A player may not move any other checkers until all checkers on the bar belonging to that player have re-entered the board
	//If moves can be made according to either one die or the other, but not both, the higher number must be used.
	//If one die is unable to be moved, but such a move is made possible by the moving of the other die, that move is compulsory.
	//A die may not be used to bear off checkers from a lower-numbered point unless there are no checkers on any higher points.
	//The same checker may be moved twice as long as the two moves are distinct

// TBD : BGPosition out : simpler from [0 to 25] with 0 is bar for light?, 25 bar for dark?, the rest are BGPoint with qty and side.
// light goes from 24 to 1, 25 should be bar for light, that will be more natural
// and 0 is bar for dark

	public struct Move
	{
		public Move(int s, int d){
			source = s;
			dest = d;
		}
		public int  source;
		public int dest;
	}

	public struct BGPoint
	{
		public BGPoint(int q, Board.Side s){
			qty = q;
			side = s;
		}
		public int qty;
		public Board.Side side;
	}

	public class BGPosition
	{
		private BGPoint[] position = new BGPoint[26];
		public BGPosition(){
			position = new BGPoint[26];
		}
		public BGPoint this[int index]   // long is a 64-bit integer
		{
			// Read one byte at offset index and return it.
			get 
			{
				return position[index];
			}
			// Write one byte at offset index and return it.
			set 
			{
				position[index] = value;
			}
		}
	}
	
	public class BGEngine
	{
		// Save the current Position under position.
		private BGPoint[] position = new BGPoint[26];

		// a static position holding the startPosition
		private static BGPoint[] startPosition; 
		public BGEngine(){

		}

		public static BGPoint[] GetStartPosition(){
			// create the start position, or simply returns it if already created
			if (startPosition == null){
				startPosition = new BGPoint[26];
				int[] light = new int[] {0, 0,0,0,0,0,5, 0,3,0,0,0,0, 5,0,0,0,0,0, 0,0,0,0,0,2, 0};
				int[] dark = new int[]  {0, 2,0,0,0,0,0, 0,0,0,0,0,5, 0,0,0,0,3,0, 5,0,0,0,0,0, 0};
				for(int i=1 ; i<25 ; i++){
					Board.Side s;
					if (light[i] > 0) s = Board.Side.light;
					else if (dark[i] > 0) s = Board.Side.dark;
					else s = Board.Side.empty;
					startPosition[i] = new BGPoint(light[i] + dark[i],s);
				}
			}

			return startPosition;
		}

		// Copy slots and captures into points table
		public void SetPosition(BGPoint[] position){
			this.position = position;
		}

		// returns a list of solutions (= list of moves)
		public List<List <Move>> AllSolutions(int die1, int die2, Board.Side side){
			//List<List <Move>> result = new List<List <Move>> ();
			// doubles or singles
			Stack<int> dice = new Stack<int> ();
			if (die1 == die2) {
				dice.Push(die1);
				dice.Push(die1);
				dice.Push(die1);
				dice.Push(die1);
			}
			else {
				dice.Push(die1);
				dice.Push(die2);
			}
			List<List <Move>> finalSolution = new List<List <Move>>();
			this.Compute(dice, new List<Backgammon.Move>(), finalSolution ,position, side);
			return finalSolution;
			}
		
		public  void Compute(Stack<int> dice, List<Move> currentSolution, List<List <Move>> finalSolution, BGPoint[] currentBoard, Board.Side side){
			//pop die and get possibleMoves
			PrintPosition(currentBoard);
			foreach(Move m in MoveDie(dice.Pop(),side))
			{
				BGPoint[] newBoard = ProjectMove(m, side);
				Debug.Log(string.Format("Played move : {0} {1}", m.source,m.dest));
				if (dice.Count == 0){
				//addcurrentcolution To finalSolution
					List<Move> solution = new List<Move>(currentSolution);
					solution.Add(m);
					finalSolution.Add(solution); 
					Debug.Log ("Adding Current Solution :" + ListMoveInString(solution));
					// currentSolution = new List<Backgammon.Move>(); // new Current Solution
				}
				else{	// Compute dice currentSolution currentBoard
					Debug.Log(string.Format("Going deeper with {0}", dice));
					currentSolution.Add(m);
					Compute(new Stack<int>(dice), currentSolution, finalSolution, newBoard, side);
				}
					//printNode(child); //<-- recursive
			}
		}

		// Play, assume validity of move, position[m.source].side should be the same as side
		private BGPoint[] ProjectMove( Move m, Board.Side side){
			BGPoint[] board = new BGPoint[26];
			Array.Copy(position, board, 26);
			Board.Side destSide = board[m.dest].side; // pratique
			// Check capture
			if (destSide == OppositeSide(side)){
				board[GetHomeIndex(destSide)].qty += 1; //capture this checker
				board[m.dest].side = side;
			}
			else {
				board[m.dest].qty += 1;
				board[m.dest].side = side;
			}
			// empty check and side
			board[m.source].qty -= 1;
			if (board[m.source].qty == 0) board[m.source].side = Board.Side.empty;
			return board;
		}

		private int GetHomeIndex(Board.Side side){
			return side == Board.Side.dark ? 0 : 25;
		}

		// helper, give a list of move possible for one die
		private List<Move> MoveDie(int die, Board.Side side){
			List<Move> solutions = new List<Move> ();
			// TBC
			List<int> availables = this.PossibleSourceTokens(side);
			for (int i=0 ; i < availables.Count ; i++){
				int destinationPoint = this.Aim(availables[i], die, side);

				if ( destinationPoint >= 1) { // is the destination point in the board ?
					// make sure the spot is possible ?
					if (PossiblePoint(destinationPoint,side)){
						solutions.Add(new Move(availables[i],destinationPoint));
					}
				}
				else { // BearOff possible ?
					if(BearingOffRule(availables[i], die, side)){
						solutions.Add(new Move(availables[i], -1));
					}
				}
			}
			Debug.Log ("MoveDie -> "+ListMoveInString(solutions));
			return solutions;
		}
		
		// tell where that point would aim with this die, -1 for bear off
		private int Aim(int point, int die, Board.Side side){
			int aimPoint;
			if (side == Board.Side.light){
				aimPoint = point + die;
			}
			else {
				aimPoint = point - die;
			}
			if (aimPoint < 1 || aimPoint > 24) return -1;
			return aimPoint;
		}
		
		// helper, all tokens that are availables on board, doesn't count captured
		private List<int> PossibleSourceTokens(Board.Side side){
			List<int> solutions = new List<int> ();
			for (int i=1; i<25 ; i++){
				if (position[i].side == side) solutions.Add(i);
			}
			return solutions;
		}
		
		// helper, are all tokens in your home board ?
		private bool BearingOff(Board.Side side){
			if (HasCaptured(side)) return false;
			int start;
			int finish;
			if (side == Board.Side.light){
				start = 6;
				finish = 24;
			}
			else{
				start = 0;
				finish = 18;
			}
			for(int i=start; i<finish ; i++){
				if (position[i].side == side){
					return false;
				}
			}
			return true;
		}
		
		// tells if you can use that die on that token to Bear off
		private bool BearingOffRule(int point, int die, Board.Side side){
			if (!BearingOff(side)) return false;
			if (die == 6) return true;
			int start;
			int finish;
			if (side == Board.Side.light){
				start = die;
				finish = 6;
			}
			else{
				start = 18;
				finish = 24 - die;
			}
			for(int i=start; i<finish ; i++){
				if (position[i].side != Board.Side.empty){
					return false;
				}
			}
			return true;
		}
		
		// helper, is this point accessible ?
		private bool PossiblePoint(int point, Board.Side side){
			if (position[point].qty < 2) return true;
			if (position[point].side == side) return true;
			return false;
		}
		
		// helper, check if that side has a capture
		private bool HasCaptured(Board.Side side){
			return side==Board.Side.light ? position[0].qty>0 : position[25].qty>0;
		}

		private Board.Side OppositeSide(Board.Side side){
			if (side == Board.Side.dark) return Board.Side.light;
			if (side == Board.Side.light) return Board.Side.dark;
			return Board.Side.empty;
		}

		public void PrintPosition(BGPoint[] pos){
			string s = "";
			for (int i=0; i<pos.Length ; i++){
				s+= " " + pos[i].qty + " " + pos[i].side;
			}
			Debug.Log(s);
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
	}
}
