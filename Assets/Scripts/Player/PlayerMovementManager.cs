using UnityEngine;
using System.Collections;

public class PlayerMovementManager : MonoBehaviour {

	public float playerSpeed;

	//Variables for reference. Do not change. 
	private Vector3 targetPosition;
	[HideInInspector]public bool moving;

	private GameObject movementGuide;


	// Use this for initialization
	void Start () {
		movementGuide = GameObject.FindGameObjectsWithTag("MovementIndicator")[0];

		moving = false;
	}

	void FixedUpdate () {
		if (moving){
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * playerSpeed);

			movementGuide.SetActive(true);
			movementGuide.transform.position = targetPosition;
		}
		else{
			movementGuide.SetActive(false);
		}
    }		

    public void MoveToLocation (Vector2 p){
    	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Vector3 point = ray.origin + (ray.direction*10);
		point.z = 0;
		targetPosition = point;

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
