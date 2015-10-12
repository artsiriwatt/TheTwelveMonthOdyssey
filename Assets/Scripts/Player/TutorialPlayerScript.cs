using UnityEngine;
using System.Collections;

public class TutorialPlayerScript : PlayerScript {


	new protected void FixedUpdate () {
		if (onMobile) {
       	 	if (Input.touchCount == 0){
       	 		movementManager.Stop();
       	 	}
        	else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) {
        		movementManager.MoveToLocation(Input.mousePosition);
        	}
		}

		if (!onMobile) {
			if(Input.GetButton("Fire1")){
				movementManager.MoveToLocation(Input.mousePosition);
			}				
			if(Input.GetKeyUp(KeyCode.Mouse0)) {
				movementManager.Stop();
			}
        }

      	// Walking sound effect handler. If Player is moving, play the walk sound effect
      	if(movementManager.isMoving()){
      		audioManager.Walk();
      	}
      	else{
      		audioManager.StopWalk();
      	}
    }

}
