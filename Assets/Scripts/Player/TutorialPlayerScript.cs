using UnityEngine;
using System.Collections;

public class TutorialPlayerScript : PlayerScript {

	public GameObject canvas;
	private GameObject[] clouds;
	public GameObject titleScreen;
	public GameObject blackBackground;


	private bool openedBottle;
	private bool canMoveTutorial;
	private bool canMeditate;
	private bool transitionToProperGame;

	//Flow is: world slowly loads and becomes apparent -> can move becomes true ->
	// -> hits bottle, pops up tutorial -> can move is false, canmeditate is true

	new protected void Start(){		
		movementManager.SetSitting();
		spriteManager.isSitting = true;

		clouds = GameObject.FindGameObjectsWithTag("cloud");
		openedBottle = false;
		canMoveTutorial = false;
		transitionToProperGame = false;
		MakeEverythingBlack();
		mainCamera.CenterCameraInstantly();



		Invoke("FadeInPlayer", 5);
		Invoke("FadeInClouds", 7);
		Invoke("FadeInMap", 15);
		Invoke("StandUp", 27);
		Invoke("EnableMovement", 28);


		//Invoke("IncreaseSound", ??);
	}

	public void InteractWithBottle() {
		DisableMovement();
		movementManager.Stop();
		PopUpTutorial();
	}



	new protected void FixedUpdate () {
		if (!canMoveTutorial){
			if (onMobile) {
				if (isMeditating){
					if (Input.touchCount < 2 && !transitionToProperGame){
						removedBothFingers = true;
	        			InterruptMeditation();
	        		}
		        }
		        else if (!isMeditating){
		        	if (Input.touchCount < 2){
		        		removedBothFingers = true;
		       	 	}

		       	 	if (Input.touchCount == 0){
		       	 		movementManager.Stop();
		       	 	}
					else if (Input.touchCount == 2 && removedBothFingers && canMeditate){
						StartMeditation();
					}
		        }
			}
		}

		if (canMoveTutorial && onMobile) {
			if (!openedBottle){
				if (Input.touchCount == 0){
					movementManager.Stop();
				}
				else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) {
	        		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					Vector3 point = ray.origin + (ray.direction*10);
					Vector2 p = new Vector2(point.x, point.y);
	        		movementManager.MoveToLocation(p);
				}
			}
			else {

			}
		}

		if (!onMobile) {
			if(Input.GetButton("Fire1") && canMoveTutorial){
        		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				Vector3 point = ray.origin + (ray.direction*10);
				Vector2 p = new Vector2(point.x, point.y);
        		movementManager.MoveToLocation(p);
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


    new protected void StartMeditation() {	
		//Set isMeditating to be True
    	isMeditating = true;
		//Set Volume Levels to Meditation Levels
		audioManager.StartMeditation();
		//Zoom the Camera In
		mainCamera.zoomIn();
		//Make the Background transparent
		MakeBackgroundTransparent();
		//Start the Finish Meditation Method to be done in X amount of time
		Invoke("FinishMeditation", meditationTimeInSeconds);

		if (onMobile){
			removedBothFingers = false;
			previousAccel = Mathf.Abs(Input.acceleration.x) + Mathf.Abs(Input.acceleration.y) + Mathf.Abs(Input.acceleration.z);
		}
    }


    new protected void InterruptMeditation() {
    	//Zoom the Camera Out
    	mainCamera.zoomOut();
    	//Set isMeditating to be False
		isMeditating = false;
		//Set Volume to Background Levels
		audioManager.StopMeditation();
		//Make the Transparency the normal color for everything around the player
		ResetBackgroundTransparency();
		//Cancel The Finish Meditation Method if they interrupt the process
		CancelInvoke("FinishMeditation");


    }


	new protected void FinishMeditation() {
		//audioManager.StopMeditation();
		transitionToProperGame = true;
		this.gameObject.GetComponent<StaticObject>().MakeTransparent();

		Invoke("TransitionToTitleScreen", 5);

		Invoke("FinishTutorial", 21);
	}

	void TransitionToTitleScreen() {
		titleScreen.GetComponent<FadeUI>().TransitionToAlphaValue(220, 15f);
	}

	protected void MakeEverythingBlack() {
		this.GetComponent<StaticObject>().SetAlphaValue(0f);
		background.GetComponent<StaticObject>().SetAlphaValue(0f);


		foreach (GameObject cloud in clouds) {
			if (cloud.GetComponent<StaticObject>() != null){
				cloud.GetComponent<StaticObject>().SetAlphaValue(0f);
			}
        }

		/*RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) transform.position, 20f, Vector2.zero);
		foreach (RaycastHit2D hit in hits)
		{
			if (hit.transform.gameObject.GetComponent<StaticObject>() != null){
				hit.transform.gameObject.GetComponent<StaticObject>().SetAlphaValue(0f);
			}
		}*/
	}

	private void FadeInPlayer() {	this.GetComponent<StaticObject>().TransitionToAlphaValue(1f, 0.012f);	}
	private void FadeInMap() {	background.GetComponent<StaticObject>().TransitionToAlphaValue(1f, 0.01f);	}
	private void FadeInClouds() {
		foreach (GameObject cloud in clouds) {
			if (cloud.GetComponent<StaticObject>() != null){
				cloud.GetComponent<StaticObject>().TransitionToAlphaValue(1f, 0.007f);
			}
        }
	}
	private void StandUp() {
		spriteManager.changeSpriteToStanding();
	}

	private void EnableMovement() {		canMoveTutorial = true;		audioManager.StopMeditation();}
	private void DisableMovement() {	canMoveTutorial = false;	}

	private void Pause() {	Time.timeScale = 0.0f;	}
	private void Unpause() {	Time.timeScale = 1.0f;	}

	private void PopUpTutorial() {
		canvas.SetActive(true);
		mainCamera.CenterCameraInstantly();
	}

	public void CloseTutorialPopUp() {
		canvas.SetActive(false);
		canMoveTutorial = false;
		canMeditate = true;
		movementManager.SetSitting();

		blackBackground.SetActive(false);
	}


	void FinishTutorial() {
		PlayerPrefsX.SetBool("DoneTutorial", true);
		PlayerPrefsX.SetBool("InitializeWorld1", true);
		Application.LoadLevel("Alpha_World1");
	}


}
