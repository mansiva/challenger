using UnityEngine;
using System.Collections.Generic;

namespace Backgammon
{
	// a Slot is a Zone within the board that contains a Stack of tokens
	// Homes and Captures are subclasses of Slots.
	public class Slot
	{
		static public Board board;

		public int id { get; private set; }
		public Vector2 position { get; private set; }
		public Stack<Token> tokens;
			
		public Slot(int id)
		{
			this.id = id;
			this.position = this.getPosfromId(id);
			this.tokens = new Stack<Token> ();
		}

		// A slot knows where it is from it's id
		public virtual Vector2 getPosfromId(int id)
		{
			Vector2 slotPos = new Vector2();
			if (id < 6)
				slotPos = new Vector2 (board.spineWidth/2.0f + (5.5f - id) * board.slotWidth, -board.slotHeight);
			else if (id < 12)
				slotPos = new Vector2 (-board.spineWidth/2.0f + (5.5f - id) * board.slotWidth, -board.slotHeight);
			else if (id < 18)
				slotPos = new Vector2 (-board.spineWidth/2.0f + (id - 17.5f) * board.slotWidth, board.slotHeight);
			else if (id < 24)
				slotPos = new Vector2 (board.spineWidth/2.0f + (id - 17.5f) * board.slotWidth, board.slotHeight);
			return slotPos;
		}

		public virtual void AddToken(Token token)
		{
			token.setState(Board.TokenState.onBoard);
			tokens.Push (token);
			// position the token using the offset()
			token.transform.localPosition = new Vector3(position.x, 0.1f, position.y + offset(this.id, tokens.Count));
		}

		public Token RemoveToken()
		{
			// remove the last element of the Slot
			return tokens.Pop();
		}

		// calculate position offset for that slot with a specific Count
		public virtual float offset(int id, int count)
		{
			// offset is the position on that slot based on the provided count
			float offset = board.slotWidth * (count - 0.5f);
			// this offset is different if we're on the upper or lower side of the board
			if (id > 11)
				offset *= -1;
			return offset;
		}
	}

}
