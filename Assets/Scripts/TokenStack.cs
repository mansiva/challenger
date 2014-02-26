using UnityEngine;
using System.Collections.Generic;

namespace Backgammon
{
	// a Slot is a Zone within the board that contains a Stack of tokens
	// Homes and Captures are subclasses of Slots.
	public class TokenStack
	{
		public Vector3 startPosition { get; private set; }
		public Vector3 orientation;
		public Vector3 offset;
		public Stack<Token> tokens;
		public Board.Side side;
			
		public TokenStack(Vector3 position, Vector3 orientation, Vector3 offset)
		{
			this.startPosition = position;
			this.orientation = orientation;
			this.offset = offset;
			this.tokens = new Stack<Token> ();
		}

		public  void AddToken(Token token)
		{
			//token.setState(Board.TokenState.onBoard);
			this.side = token.side;
			// position the token using the position, offset, and tokens.Count
			token.transform.localPosition = this.startPosition + this.tokens.Count * this.offset;
			token.transform.localEulerAngles = this.orientation;
			tokens.Push (token);
		}

		public Token RemoveToken()
		{
			// remove the last element of the Slot
			if (tokens.Count == 1) side=Board.Side.empty;
			return tokens.Pop();
		}
		public int Count() {
			return this.tokens.Count;
		}

	}

}
