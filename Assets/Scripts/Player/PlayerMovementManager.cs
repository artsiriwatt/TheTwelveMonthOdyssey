using UnityEngine;
using System.Collections;

public class PlayerMovementManager : MonoBehaviour {

	public float playerSpeed;

	//Variables for reference. Do not change. 
	private Vector3 targetPosition;
	[HideInInspector]public bool moving;

	private Animator animator;
	private GameObject movementIndicator;


	// Use this for initialization
	void Start () {
		movementIndicator = GameObject.FindGameObjectsWithTag("MovementIndicator")[0];
		animator = this.GetComponent<Animator>();
		moving = false;
	}

	void FixedUpdate () {
		if (moving){
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

			transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * playerSpeed);

			movementIndicator.SetActive(true);
			movementIndicator.transform.position = targetPosition;
		}
		else if (animator.GetInteger("Direction") != 5){
			//animator.enabled = false;
			movementIndicator.SetActive(false);
			animator.SetInteger("Direction", 0);
		}
    }		


    public void SetStanding() {
    	animator.SetInteger("Direction", 0);
    }

    public void SetSitting() {
    	animator.SetInteger("Direction", 5);
    }

    public void MoveToLocation (Vector2 p){
		targetPosition = new Vector3 (p.x, p.y, 0);
		moving = true;
    }

    public void Stop (){
    	moving = false;
    	//targetPosition = transform.position;
    }

    public bool isMoving() {
    	return moving;
    }

    public void OnCollisionEnter2D(Collision2D collision) {
    	Stop();
	}

}
