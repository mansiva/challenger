using UnityEngine;
using System.Collections;
namespace Backgammon
{

	public class Home : Slot {

		public Home(int id)  : base(id)
		{

		}

		public override Vector2 getPosfromId(int id)
		{
			Vector2 slotPos = new Vector2();
			if (id == (int)Board.Side.light)
				slotPos = new Vector2 (board.spineWidth/2.0f + 7 * board.slotWidth + board.borderWidth, - board.slotHeight - board.slotWidth / 2.0f);
			else
				slotPos = new Vector2 (board.spineWidth/2.0f + 7 * board.slotWidth + board.borderWidth,   board.slotHeight + board.slotWidth / 2.0f);
			return slotPos;
		}

		public override void AddToken(Token token)
		{
			token.setState(Board.TokenState.home);
			tokens.Push (token);
			// position the token using the offset()
			token.transform.localPosition = new Vector3(position.x, 0.1f, position.y + offset(this.id, tokens.Count));
		}


		public override float offset(int id, int count)
		{
			// offset is the position on that slot based on the provided count
			float offset = board.slotWidth * (count - 0.5f);
			// this offset is different if we're on the upper or lower side of the board
			if (id == (int)Board.Side.dark)
				offset *= -1;
			return offset;
		}
	}
}