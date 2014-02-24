using UnityEngine;
using System.Collections.Generic;

namespace Backgammon
{
	public class Slot
	{
		static public Board board;

		public int id { get; private set; }
		public Vector2 position { get; private set; }
		public List<Token> tokens;
			
		public Slot(int id, Vector2 position)
		{
			this.id = id;
			this.position = position;
			this.tokens = new List<Token> ();
		}

		public void AddToken(Token token)
		{
			float offset = board.slotWidth / 2 + tokens.Count * board.slotWidth;
			if (position.y > 0)
				offset *= -1;
			token.transform.localPosition = new Vector3(position.x, 0.1f, position.y + offset);
			tokens.Add (token);
		}
	}

}
