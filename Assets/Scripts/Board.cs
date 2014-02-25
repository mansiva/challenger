using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Backgammon
{
	public class Board : MonoBehaviour
	{
		public float spineWidth;
		public float borderWidth;
		public float slotWidth;
		public float slotHeight;


		public enum Side
		{
			light,
			dark
		}

		public enum TokenState
		{
			onBoard,
			captured,
			home
		}

		private Slot[] slots = new Slot[24];
		private Home homeDark;
		private Home homeLight;

		private Token[] tokens = new Token[30];

		// I don't know how to create tupples, so I create tables, 
		// used for start position
		public static readonly int[] initPos = new int[] { 23, 12, 7, 5};
		public static readonly int[] initQty = new int[] { 2,  5,  3, 5};


		// ---------------------------------------------------------------------------
		// Create board components
		// ---------------------------------------------------------------------------
		void Start ()
		{
			Slot.board = this;

			// Create the 24 slots. 0 is actually slot 1 lower right corner
			for (int i = 0; i < 24; i++)
			{
				slots[i] = new Slot(i);
			}

			// Create Homes for dark and light
			homeDark = new Home((int)Side.dark);
			homeLight = new Home((int)Side.light);

			// Create Capture Zones for dark and light
//			capturedDark = new Capture(Side.dark);
//			capturedLight = new Capture(Side.light);

			// Create the 30 Tokens
			GameObject prefabPeon = Resources.Load<GameObject>("Token");

			for (int i = 0; i < 30; i++)
			{
				tokens[i] = NGUITools.AddChild(gameObject, prefabPeon).GetComponent<Token>();
				if (i < 15)
					tokens[i].setSide(Side.light);
				else
					tokens[i].setSide(Side.dark);

			}

			ResetTokens();

		}


		// ---------------------------------------------------------------------------
		// place all tokens in a startPosition
		// ---------------------------------------------------------------------------
		public void ResetTokens()
		{
			// light player vs dark player light 1 - 24, dark 24 - 1
			// light : 24:2, 13:5, 8:3, 6:5
			// light to dark : (-x + 25)
			int k = 0;
			for (int i = 0; i < initPos.Length; i++)
			{
				for (int j = 0; j < initQty[i]; j++)
				{
					// light
					slots[ initPos[i] ].AddToken(tokens[k]);
					k++;
				}
			}

			for (int i = 0; i < initPos.Length; i++)
			{
				for (int j = 0; j < initQty[i]; j++)
				{
					// dark
					slots[ 23 - initPos[i] ].AddToken(tokens[k]);
					k++;
				}
			}
			homeDark.AddToken(slots[0].RemoveToken());
			homeLight.AddToken(slots[5].RemoveToken());
		}

	}
}
