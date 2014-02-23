using UnityEngine;
using System.Collections;

namespace Backgammon
{

	public class Token : MonoBehaviour
	{

		// Use this for initialization
		void Start ()
		{
			transform.Rotate(270,0,0);
			transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}

}
