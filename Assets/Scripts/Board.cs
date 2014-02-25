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
		public float tokenWidth;

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

		private TokenStack[] slots = new TokenStack[24];
		private TokenStack homeDark;
		private TokenStack homeLight;

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
			// Create the 24 slots. 0 is actually slot 1 lower right corner
			Vector3 orientation = new Vector3 (270,0,0);
			Vector3 offset = new Vector3 (0, 0, this.slotWidth);
			Vector3 slotPos = new Vector3();
			for (int id = 0; id < 24; id++)
			{
				// TokenStack(int id, Vector3 startposition, Vector3 orientation, Vector3 offset)
				if (id < 6){
					slotPos = new Vector3 (this.spineWidth/2.0f + (5.5f - id) * this.slotWidth, 0.1f, -this.slotHeight + this.slotWidth/2.0f);
					slots[id] = new TokenStack(id, slotPos, orientation,offset);
				}
				else if (id < 12) {
					slotPos = new Vector3 (-this.spineWidth/2.0f + (5.5f - id) * this.slotWidth,  0.1f, -this.slotHeight + this.slotWidth/2.0f);
					slots[id] = new TokenStack(id, slotPos, orientation,offset);
				}
				else if (id < 18){
					slotPos = new Vector3 (-this.spineWidth/2.0f + (id - 17.5f) * this.slotWidth,  0.1f, this.slotHeight - this.slotWidth/2.0f);
					slots[id] = new TokenStack(id, slotPos, orientation,-offset);
				}
				else if (id < 24){
					slotPos = new Vector3 (this.spineWidth/2.0f + (id - 17.5f) * this.slotWidth,  0.1f, this.slotHeight - this.slotWidth/2.0f);
					slots[id] = new TokenStack(id, slotPos, orientation,-offset);
				}
			}

			// Create Home for dark
			orientation = new Vector3 (0,0,0);
			offset = new Vector3 (0, 0, this.tokenWidth);
			slotPos = new Vector3(this.spineWidth/2.0f + 6.5f * this.slotWidth + this.borderWidth, 0.1f + this.slotWidth/2.0f, -this.slotHeight);
			homeDark = new TokenStack((int)Side.dark, slotPos, orientation,offset);
			// and light
			orientation = new Vector3 (180,0,0);
			offset = new Vector3 (0, 0, -this.tokenWidth);
			slotPos = new Vector3(this.spineWidth/2.0f + 6.5f * this.slotWidth + this.borderWidth, 0.1f + this.slotWidth/2.0f, this.slotHeight);
			homeLight = new TokenStack((int)Side.light, slotPos, orientation,offset);

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
			TEST_HOME ();
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
		}

		public void TEST_HOME(){
			// TEST THINGS
			homeDark.AddToken (slots [0].RemoveToken ());
			homeLight.AddToken (slots [5].RemoveToken ());
			homeLight.AddToken (slots [5].RemoveToken ());

		}
	}
}
