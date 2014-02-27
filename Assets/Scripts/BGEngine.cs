using UnityEngine;
using System.Collections.Generic;

namespace Backgammon
{
	// this is used to compute possible moves without interfering with the View

	//A player may not move any other checkers until all checkers on the bar belonging to that player have re-entered the board
	//If moves can be made according to either one die or the other, but not both, the higher number must be used.
	//If one die is unable to be moved, but such a move is made possible by the moving of the other die, that move is compulsory.
	//A die may not be used to bear off checkers from a lower-numbered point unless there are no checkers on any higher points.
	//The same checker may be moved twice as long as the two moves are distinct

	// I use BGPoint[0] for light capture, and BGPoint[25] for dark capture

	public struct BGPoint
	{
		public int qty;
		public Board.Side side;
	}

	public struct Move
	{
		public Move(int s, int d){
			source = s;
			dest = d;
		}
		public int  source;
		public int dest;
	}

	public class BGPosition{
		public int[] light; //25 ints start1 ends at the bar for light
		public int[] dark; //25 ints start24 ends at the bar for dark
		public BGPosition(int[] l, int[] d){
			light = l;
			dark = d;
		}
	}

	public class BGEngine
	{
		private BGPoint[] points = new BGPoint[26];
		private static BGPosition startPosition; 

		List<List <Move>> finalSolution; // this shouldn't be here, i'll have to see what's wrong

		public BGEngine(){

		}

		public static BGPosition getStartPosition(){
			if (startPosition == null){
				BGEngine.startPosition = new BGPosition(
					// TBD put the real solution here
					new int[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
					new int[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0});
			}
			return startPosition;

		}
		// Copy slots and captures into points table
		public void setPosition(BGPosition position){
			// TBD
//			for(int i=1; i<25 ; i++){
//				points[i].qty = (int)slots[i].Count();
//				points[i].side = slots[i].side;
//			}
//			points[0].qty = (int)captured[(int)Board.Side.light].Count();
//			points[25].qty = (int)captured[(int)Board.Side.dark].Count();
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
			finalSolution = new List<List <Move>>();
			this.Compute(dice, new List<Backgammon.Move>(), points, side);
			return finalSolution;
			}
		
		public  void Compute(Stack<int> dice, List<Move> currentSolution, BGPoint[] currentBoard, Board.Side side){
			//pop die and get possibleMoves
			foreach(Move m in MoveDie(dice.Pop(),side))
			{
				PlayMove(m);
				if (dice.Count == 1){
				//addcurrentcolution To finalSolution
					currentSolution.Add(m);
					finalSolution.Add(new List<Move>(currentSolution));
				}
						//Compute dice currentSolution currentBoard
				Compute(dice, currentSolution, currentBoard, side);
					//printNode(child); //<-- recursive
			}
		}

		// play a move that has to be possible
		// TBD take care of the capture
		private void PlayMove(Move m){
			points[m.source].qty -= 1;
			points[m.dest].qty += 1;
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
				if (points[i].side == side) solutions.Add(i);
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
				if (points[i].side == side){
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
				if (points[i].side != Board.Side.empty){
					return false;
				}
			}
			return true;
		}
		
		// helper, is this point accessible ?
		private bool PossiblePoint(int point, Board.Side side){
			if (points[point].qty < 2) return true;
			if (points[point].side == side) return true;
			return false;
		}
		
		// helper, check if that side has a capture
		private bool HasCaptured(Board.Side side){
			return side==Board.Side.light ? points[0].qty>0 : points[25].qty>0;
		}

	}
}
