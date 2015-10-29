using UnityEngine;
using System.Collections;

public class TutorialPlayerScript : PlayerScript {

	private bool openedBottle;
	private bool canMove;
	private bool canMeditate;

	//Flow is: world slowly loads and becomes apparent -> can move becomes true ->
	// -> hits bottle, pops up tutorial -> can move is false, canmeditate is true

	void Start(){
		openedBottle = false;
		mainCamera.SetCameraSizeTo(1.5f);
		mainCamera.zoomIn();
		Invoke("EnableMovement", 1);
		canMove = false;

		Invoke("PopUpTutorial", 3);
		Invoke("FinishTutorial", 3);

		//Slowly Draw Character
		//Slowly Draw All Objects in the World
		//Slowly Increase Music 
	}



	new protected void FixedUpdate () {

		if (!canMove){
			return;
		}

		if (onMobile) {
			if (!openedBottle){
				if (Input.touchCount == 0){
					movementManager.Stop();
				}
				else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) {
					movementManager.MoveToLocation(Input.mousePosition);
				}
			}
			else {

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

		Invoke("FinishTutorial", 20);
	}

	void EnableMovement() {
		canMove = true;
	}

	void PopUpTutorial() {
		//Activate the Canvas

		//Pause the action of the game
		Pause();
	}

	void ReadTutorial() {
		canMove = false;
		canMeditate = true;
	}

	void Pause() {
		Time.timeScale = 0.0f;
	}

	void Unpause() {
		Time.timeScale = 1.0f;
	}

	void FinishTutorial() {
		PlayerPrefsX.SetBool("DoneTutorial", true);
		PlayerPrefsX.SetBool("InitializeWorld1", true);
		Application.LoadLevel("Alpha_World1");
	}


}
