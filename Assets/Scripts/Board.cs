using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Backgammon
{
	public class Board : MonoBehaviour
	{
		public float spineWidth;
		public float slotWidth;
		public float slotHeight;

		public Color tokenColorDark;
		public Color tokenColorLight;

		private Slot[] slots = new Slot[24];
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
			// Create the 24 slots
			Vector2 slotPos = new Vector2 (spineWidth + 6 * slotWidth, slotHeight);
			for (int i = 0; i < 6; i++)
			{
				slotPos.x -= slotWidth;
				slots[i] = new Slot(i, slotPos);
			}
			slotPos.x -= spineWidth;
			for (int i = 6; i < 12; i++)
			{
				slotPos.x -= slotWidth;
				slots[i] = new Slot(i, slotPos);
			}
			slotPos.x -= slotWidth;
			slotPos.y -= 2 * slotHeight;
			for (int i = 12; i < 18; i++)
			{
				slotPos.x += slotWidth;
				slots[i] = new Slot(i, slotPos);
			}
			slotPos.x += spineWidth;
			for (int i = 18; i < 24; i++)
			{
				slotPos.x += slotWidth;
				slots[i] = new Slot(i, slotPos);
			}

			// Create the 30 Tokens
			GameObject prefabPeon = Resources.Load<GameObject>("Token");

			for (int i = 0; i < 30; i++)
			{
				tokens[i] = NGUITools.AddChild(gameObject, prefabPeon).GetComponent<Token>();
				tokens[i].renderer.material.color = i < 15 ? tokenColorLight : tokenColorDark;
			}

			ResetTokens ();
		}


		// ---------------------------------------------------------------------------
		// place all tokens in a startPosition
		// ---------------------------------------------------------------------------
		void ResetTokens()
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
					Slot slot = slots[ initPos[i] ];
					slot.AddToken(tokens[k]);
					k++;
				}
			}

			for (int i = 0; i < initPos.Length; i++)
			{
				for (int j = 0; j < initQty[i]; j++)
				{
					// dark
					Slot slot = slots[ 23 - initPos[i] ];
					slot.tokens.Add(tokens[k]);
					tokens[k].transform.localPosition = new Vector3(slot.position.x, 0, slot.position.y);
					k++;
				}
			}
		}

	}
}
