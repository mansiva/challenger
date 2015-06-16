﻿using UnityEngine;
using System.Collections;

namespace Backgammon
{

	public class Token : MonoBehaviour
	{
		//public static Board board;
		public Color tokenColorDark;
		public Color tokenColorLight;
		public bool side; // white 0 dark 1

		// Use this for initialization
		void Start ()
		{
//			transform.localRotation = new Vector3(0,0,0);
			//			transform.Rotate(270,0,0);
			//transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
		}

		public void SetSide(bool side)
		{
			this.side = side;
			if (!side)
				GetComponent<Renderer>().material.color = tokenColorLight;
			else
				GetComponent<Renderer>().material.color = tokenColorDark;
		}

//		public void SetState(Board.TokenState tokenState)
//		{
//			state = tokenState;
//		}

		// Update is called once per frame
		void Update () {
		
		}
	}

}