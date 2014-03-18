using UnityEngine;
using System.Collections;

public class Playtest : MonoBehaviour {
	private Animator animator;

	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator>();
		//animator.SetTrigger("throw_dice");
	}
	
	// Update is called once per frame
	void Update () {
	}
}
