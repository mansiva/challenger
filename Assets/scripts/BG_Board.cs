using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BG_Board : MonoBehaviour {

	public BG_Token[] tokens = new BG_Token[30];

	// create the array for 26 lists, positions for the tokens
	// 0 is home for light 25 is home for dark
	// captured is a special list for captured tokens
	// note that captured tokens can be of both colors
	public List<BG_Token>[] triangles = new List<BG_Token>[26]; 
	//public List<BG_Token> captured = new List<BG_Token>;

	// I don't know how to create tupples, so I create tables, 
	// used for start position
	public static readonly int[] initPos = new int[] { 24, 13, 8, 6};
	public static readonly int[] initQty = new int[] { 2,  5,  3, 5};

	// used for position in the game board itself
	// TBD : this should be using spot form the board itself
	public static readonly int[] boardX = new int[] { 12 , 10, 8, 6, 4, 2, -2, -4, -6, -8, -10, -12};
	public static readonly int[] boardZ = new int[] { -10, +10};


	void Start () {
		// create the 30 Tokens
		GameObject prefabPeon = Resources.Load<GameObject>("Token");
		for (int i = 0; i < 30; i++)
		{
			//tokens[i] = Instantiate(prefabPeon, new Vector3(i * 2.0F, 0, 0), Quaternion.identity) as BG_Token;
			tokens[i] = NGUITools.AddChild(gameObject, prefabPeon).GetComponent<BG_Token>();
			tokens[i].transform.localPosition = new Vector3(0,0,0);
			// coudn t find a clean way to make it flat before.
			tokens[i].transform.Rotate(270,0,0);
			// Set the color (Peon would be the class attached to the prefab peon that contains a public field of type Color)
			//tokens[i].renderer.material.color = i < 15 ? Color.black : Color.white;
		}

	}

	// place all tokens in a startPosition
	void StartPosition(){
		// light player vs dark player light 1 - 24, dark 24 - 1
		// light : 24:2, 13:5, 8:3, 6:5
		// light to dark : (-x + 25)
		int k = 0;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < initQty[i]; j++)
			{
				// light
				triangles[ initPos[i] ].Add(tokens[k]);
				tokens[k].transform.localPosition = getVectorFromTriangle( initPos[i] );
				k++;
			}
		}
	}
	
	static Vector3 getVectorFromTriangle(int i)
	{
		//1st is 12 -10
		//12th is -12 -10
		//13th is -12 10
		//24th is 12 10
		int x = i < 13 ? boardX[i] : -boardX[i];
		int z = i < 13 ? boardZ[0] : boardZ[1];
		return new Vector3(x, 0, z);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
