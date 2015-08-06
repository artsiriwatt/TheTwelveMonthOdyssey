using UnityEngine;
using System.Collections;

public class PlayerMovementManager : MonoBehaviour {

	public bool onMobile;
	public float playerSpeed;

	//Variables for reference. Do not change. 
	private Vector3 targetPosition;
	private Collider2D playerColl;
	private Vector3 lastPosition;


	// Use this for initialization
	void Start () {
		playerColl = this.gameObject.GetComponent<Collider2D>();

		targetPosition = transform.position;

		/* Detect which playform the user is playing on
		string[] desktopPlatforms = {"OSXEditor", "OSXPlayer", "WindowsPlayer", "OSXWebPlayer", 
		"OSXDashboardPlayer", "WindowsWebPlayer", "WindowsEditor"};
		if (System.Array.IndexOf(desktopPlatforms, Application.platform) == -1) {
			onMobile = false;
		}
		else{
			onMobile = true;
		}*/
	}


	void moveTowardsPoint() {

	}

	void FixedUpdate () {
		if (onMobile) {
			if (Input.touchCount == 1){

			}
	     
	        if (!isMeditating){
	        	if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended) {
			    	Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			   		Vector3 point = ray.origin + (ray.direction*10);
			   		point.z = 0;
			   		if (interruptMeditationFrames == 0){
		       			targetPosition = point;			
					}
	        	}

	        	
	        	if (Input.touchCount < 2){
	        		removedBothFingers = true;
	       	 	}
				if (Input.touchCount == 2 && removedBothFingers){
					transform.position = Vector3.MoveTowards(transform.position, transform.position, Time.deltaTime * playerSpeed * 10);
				}
	        }
		}

		else if (!onMobile) {
			if(Input.GetKeyDown(KeyCode.Mouse0)) {
				//For Orthographic Camera
				//Vector3 click = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		    	//Vector3 adjustedClick = new Vector3(click.x, click.y, 0);
		    	//Debug.Log(click);
		    	//targetPosition = adjustedClick;

		    	//For Perspective Camera
		    	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		   		Vector3 point = ray.origin + (ray.direction*10);
		   		point.z = 0;
		   		if (interruptMeditationFrames == 0){
	       			targetPosition = point;			
				}
        	}
        }

    }		

    void Stop (){
    	transform.position = Vector3.MoveTowards(transform.position, transform.position, Time.deltaTime * playerSpeed * 10);
    }


    void enablePhoneMovement(){
    	onMobile = true;
    }

    void disablePhoneMovement(){
    	onMobile = false;
    }

    void OnCollisionEnter2D(Collision2D collision) {
    	targetPosition = transform.position;
	}

}
