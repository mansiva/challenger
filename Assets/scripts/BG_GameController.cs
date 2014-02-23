using UnityEngine;
using System.Collections;

public class BG_GameController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// Load the prefab Assets/Resources/Board.prefab
		GameObject prefabBoard = Resources.Load<GameObject>("Board");
		GameObject boardObject = NGUITools.AddChild(gameObject, prefabBoard);
		// Get script Board attached to prefab
		BG_Board boardScript = boardObject.GetComponent<BG_Board>();	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
