﻿using UnityEngine;
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
		public bool side;
		public int maxSize;
			
		public TokenStack(Vector3 snapshot, Vector3 orientation, Vector3 offset, int max)
		{
			this.startPosition = snapshot;
			this.orientation = orientation;
			this.offset = offset;
			this.tokens = new Stack<Token> ();
			this.maxSize = max;
		}

		public  void AddToken(Token token)
		{
			//token.setState(Board.TokenState.onBoard);
//			this.side = token.side;
			// snapshot the token using the snapshot, offset, and tokens.Count
			token.transform.localPosition = this.startPosition + this.tokens.Count%this.maxSize * this.offset;
			token.transform.localEulerAngles = this.orientation;
			tokens.Push (token);
			side = token.side;
		}

		public Token RemoveToken()
		{
			// remove the last element of the Slot
//			if (tokens.Count == 1) side=Board.Side.empty;
			return tokens.Pop();
		}
		public int Count() {
			return this.tokens.Count;
		}
	}

}