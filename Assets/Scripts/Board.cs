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

		// right now player is light, and opponent is dark
		public enum Side
		{
			light,
			dark,
			empty
		}
		//maybe useful later
		public enum TokenState
		{
			onBoard,
			captured,
			home
		}

		// stack zones : slots, homes and bars are now slots[0] for dark and slots[25]
		private TokenStack[] slots = new TokenStack[26];
		private TokenStack[] homes = new TokenStack[2];

		//private Token[] tokens = new Token[30];
		private GameObject prefabPeon;
		// I don't know how to create tupples, so I create tables, 
		// used for start position
//		public static readonly int[] initPos = new int[] { 23, 12, 7, 5};
//		public static readonly int[] initQty = new int[] { 2,  5,  3, 5};


		// ---------------------------------------------------------------------------
		// Create board components
		// ---------------------------------------------------------------------------
		void Start ()
		{
			// Create the 24 slots. 0 is actually slot 1 lower right corner
			Vector3 orientation = new Vector3 (270,0,0);
			Vector3 offset = new Vector3 (0, 0, this.slotWidth);
			Vector3 slotPos = new Vector3();
			for (int id = 1; id < 25; id++)
			{
				// TokenStack(int id, Vector3 startposition, Vector3 orientation, Vector3 offset)
				if (id < 7){
					slotPos = new Vector3 (this.spineWidth/2.0f + (6.5f - id) * this.slotWidth, 0.1f, -this.slotHeight + this.slotWidth/2.0f);
					slots[id] = new TokenStack(slotPos, orientation,offset);
				}
				else if (id < 13) {
					slotPos = new Vector3 (-this.spineWidth/2.0f + (6.5f - id) * this.slotWidth,  0.1f, -this.slotHeight + this.slotWidth/2.0f);
					slots[id] = new TokenStack(slotPos, orientation,offset);
				}
				else if (id < 19){
					slotPos = new Vector3 (-this.spineWidth/2.0f + (id - 18.5f) * this.slotWidth,  0.1f, this.slotHeight - this.slotWidth/2.0f);
					slots[id] = new TokenStack(slotPos, orientation,-offset);
				}
				else if (id < 25){
					slotPos = new Vector3 (this.spineWidth/2.0f + (id - 18.5f) * this.slotWidth,  0.1f, this.slotHeight - this.slotWidth/2.0f);
					slots[id] = new TokenStack(slotPos, orientation,-offset);
				}
			}

			// Create Home for dark
			orientation = new Vector3 (0,0,0);
			offset = new Vector3 (0, 0, this.tokenWidth);
			slotPos = new Vector3(this.spineWidth/2.0f + 6.5f * this.slotWidth + this.borderWidth, 0.1f + this.slotWidth/2.0f, -this.slotHeight);
			homes[(int)Board.Side.dark] = new TokenStack(slotPos, orientation,offset);

			// and light
			orientation = new Vector3 (180,0,0);
			offset = new Vector3 (0, 0, -this.tokenWidth);
			slotPos = new Vector3(this.spineWidth/2.0f + 6.5f * this.slotWidth + this.borderWidth, 0.1f + this.slotWidth/2.0f, this.slotHeight);
			homes[(int)Board.Side.light] = new TokenStack(slotPos, orientation,offset);

			// Create Capture Zones for dark
			orientation = new Vector3 (270,0,0);
			offset = new Vector3 (0, 0, -this.slotWidth);
			slotPos = new Vector3(0, 0.1f, -this.slotWidth/2.0f);
			slots[0] = new TokenStack(slotPos, orientation,offset);

			// and light
			orientation = new Vector3 (270,0,0);
			offset = new Vector3 (0, 0, this.slotWidth);
			slotPos = new Vector3(0, 0.1f, this.slotWidth/2.0f);
			slots[25] = new TokenStack(slotPos, orientation,offset);

			// ref to Token
			prefabPeon = Resources.Load<GameObject>("Token");
		}

		public void SetPosition(BGPosition position){
			// right now I only do one light out of this !!
			for (int i = 0; i < position.Length; i++)
			{
				{
					if (position[i].qty > 0 ){
						for (int j=0; j<position[i].qty; j++){
							Token t = NGUITools.AddChild(gameObject, prefabPeon).GetComponent<Token>();
							t.SetSide(position[i].side);
							slots[i].AddToken(t);
						}
					}
				}
			}
		}

		public void playMove(Move m){

		}

		public void TESTS(){
			// TEST THINGS
			//homes[(int)Board.Side.dark].AddToken (slots [0].RemoveToken ());
			//captured[(int)Board.Side.dark].AddToken (slots [0].RemoveToken ());
		}
	}
}
