﻿using UnityEngine;
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
		// used for start snapshot
//		public static readonly int[] initPos = new int[] { 23, 12, 7, 5};
//		public static readonly int[] initQty = new int[] { 2,  5,  3, 5};


		// ---------------------------------------------------------------------------
		// Create board components
		// ---------------------------------------------------------------------------
		void Awake ()
		{
			// Create the 24 slots. 0 is actually slot 1 lower right corner
			Vector3 orientation = new Vector3 (270,0,0);
			Vector3 offset = new Vector3 (0, 0, this.slotWidth);
			Vector3 slotPos = new Vector3();
			for (int id = 1; id < 25; id++)
			{
				// TokenStack(int id, Vector3 startsnapshot, Vector3 orientation, Vector3 offset)
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
			homes[1] = new TokenStack(slotPos, orientation,offset);

			// and light
			orientation = new Vector3 (180,0,0);
			offset = new Vector3 (0, 0, -this.tokenWidth);
			slotPos = new Vector3(this.spineWidth/2.0f + 6.5f * this.slotWidth + this.borderWidth, 0.1f + this.slotWidth/2.0f, this.slotHeight);
			homes[0] = new TokenStack(slotPos, orientation,offset);

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

		public void SetSnapshot(BGSnapshot snapshot){
			// not tested for captures, and no homes check
			for (int i = 0; i < snapshot.Length; i++){
				for (int j=0; j< Mathf.Abs(snapshot[i]); j++){
					Token t = NGUITools.AddChild(gameObject, prefabPeon).GetComponent<Token>();
					t.SetSide(snapshot[i]>0);
					slots[i].AddToken(t);
				}
			}
		}

		public void PlayMove(Move m, bool side){
			// If Capture 
//				slots[BGSnapshot.GetBarIndex(BGEngine.OppositeSide(side))].AddToken(slots[m.dest].RemoveToken());
//			}
			int dest = side ? m.dest : 25 - m.dest;
			int source = side ? m.source : 25 - m.source;
			Token s = slots[source].RemoveToken ();
			if (slots[dest].Count() > 0 && slots[dest].side != s.side){
				Debug.Log("Capture !!!!");
				slots[side? 0:25].AddToken(slots[dest].RemoveToken());
			}
			slots[dest].AddToken(s);
		}

		public void PlaySolution(List<Move> solution, bool side){
			for (int i =0 ; i< solution.Count ; i++){
				PlayMove(solution[i], side);
			}
		}

		public void TESTS(){
			// TEST THINGS
			//homes[(int)Board.Side.dark].AddToken (slots [0].RemoveToken ());
			//captured[(int)Board.Side.dark].AddToken (slots [0].RemoveToken ());
		}
	}
}
