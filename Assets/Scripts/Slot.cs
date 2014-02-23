using UnityEngine;
using System.Collections.Generic;

namespace Backgammon
{
	public class Slot
	{
		public int id { get; private set; }
		public Vector2 position { get; private set; }
		public List<Token> tokens;
			
		public Slot(int id, Vector2 position)
		{
			this.id = id;
			this.position = position;
			this.tokens = new List<Token> ();
		}
	}

}
