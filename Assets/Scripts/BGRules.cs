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
		public int  source;
		public int dest;
	}

	public class BGRules
	{
		private BGPoint[] points = new BGPoint[26];

		public BGRules(){

		}

		// Copy slots and captures into points table
		public void InitBoard(TokenStack[] slots, TokenStack[] captured){
			for(int i=1; i<25 ; i++){
				points[i].qty = (int)slots[i].Count();
				points[i].side = slots[i].side;
			}
			points[0].qty = (int)captured[(int)Board.Side.light].Count();
			points[25].qty = (int)captured[(int)Board.Side.dark].Count();
		}

		// returns a list of solutions (= list of moves)
		public List<List <Move>> AllSolutions(int die1, int die2, Board.Side side){
			List<List <Move>> result = new List<List <Move>> ();
			// doubles or singles
			List<int> dice = new List<int> ();
			if (die1 == die2) {
				dice.Add(die1);
				dice.Add(die1);
				dice.Add(die1);
				dice.Add(die1);
			}
			else {
				dice.Add(die1);
				dice.Add(die2);
			}
			return result;
		}
		
		public List<List <Move>>  Compute(List<int> dice, List<Move> currentSolution, BGPoint[] points){
			if dice.Count == 1 {
				return AllSolutions.
			}
			// somme des solutions pour chaque dés
			//die1 solutions x die2 solutions
			//	pop die
			//	for all moves
			//	if no more dice add move to solution
			//		else Compute remaining dice + current path and matrix
			//		
		}

		// helper, give a list of move possible for one die
		private List<Move> MoveDie(int die, Board.Side side){
			List<Move> solutions = new List<Move> ();
			// TBC
			List<int> availables = this.PossibleTokens(side);
			for (int i=0 ; i < availables.Count ; i++){
				if (this.Aim(availables[i], die, side) >= 0) {
					solutions.Add(availables[i]);
				}
				else { // BearOff possible ?
					if(BearingOffRule(availables[i], die, side)){
						solutions.Add(availables[i]);
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
		private List<int> PossibleTokens(Board.Side side){
			List<int> solutions = new List<int> ();
			for (int i=0; i<24 ; i++){
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
				if (slots[i].side == side){
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
				if (slots[i].side != Board.Side.empty){
					return false;
				}
			}
			return true;
		}
		
		// helper, is this point accessible ?
		private bool PossiblePoint(int point, Board.Side side){
			if (slots[point].Count() < 2) return true;
			if (slots[point].side == side) return true;
			return false;
		}
		
		// helper, check if that side has a capture
		private bool HasCaptured(Board.Side side){
			return captured[(int)side].Count() > 0;
		}

	}
}
