using UnityEngine;
using System.Collections;

public class PlayerAnimationScript : MonoBehaviour {

	private Animator animator;
	private PlayerMovementManager movementManager;

	private GameObject player;
	private GameObject movementIndicator;
	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator>();
		movementManager = this.GetComponent<PlayerMovementManager>();

		//player = GameObject.FindGameObjectsWithTag("Player")[0];
		movementIndicator = GameObject.FindGameObjectsWithTag("MovementIndicator")[0];
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (movementManager.moving){
			float xdif = movementIndicator.transform.position.x - transform.position.x;
			float ydif = movementIndicator.transform.position.y - transform.position.y;
			//Debug.Log("xdif is " + xdif);
			//Debug.Log("ydif is " + ydif);

			if (Mathf.Abs(xdif) >= Mathf.Abs(ydif)){
				if (xdif > 0){
					animator.SetInteger("Direction", 2);
				}
				else {
					animator.SetInteger("Direction", 4);
				}
			} 
			else {
				if (ydif > 0){
					animator.SetInteger("Direction", 1);
				}
				else {
					animator.SetInteger("Direction", 3);
				}
			}
		}

		else{
			animator.SetInteger("Direction", 0);
		}
	}
}
