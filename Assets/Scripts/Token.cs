using UnityEngine;
using System.Collections;

namespace Backgammon
{

	public class Token : MonoBehaviour
	{
		public static Board board;
		public Color tokenColorDark;
		public Color tokenColorLight;
		private Board.Side side;
		public Board.TokenState state;
		// Use this for initialization
		void Start ()
		{
//			transform.localRotation = new Vector3(0,0,0);
			//			transform.Rotate(270,0,0);
			transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
		}

		public void setSide(Board.Side side)
		{
			this.side = side;
			if (side == Board.Side.light)
				renderer.material.color = tokenColorLight;
			else
				renderer.material.color = tokenColorDark;
		}

		public void setState(Board.TokenState tokenState)
		{
			state = tokenState;
			if (tokenState == Board.TokenState.home){
				transform.localPosition = new Vector3(0,5f,0);
				if (side == Board.Side.light){
					transform.localRotation = Quaternion.identity;
				}
				else{
					transform.localRotation = Quaternion.Euler(180, 0, 0);
				}
			}
			else if (tokenState == Board.TokenState.onBoard){
				transform.localPosition = new Vector3(0,0,0);
				transform.localRotation = Quaternion.Euler(270, 0, 0);
			}
			else if (tokenState == Board.TokenState.captured)
				transform.localEulerAngles = Vector3.zero;

		}

		// Update is called once per frame
		void Update () {
		
		}
	}

}
