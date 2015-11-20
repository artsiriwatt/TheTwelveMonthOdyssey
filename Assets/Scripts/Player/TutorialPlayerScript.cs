using UnityEngine;
using System.Collections;

public class TutorialPlayerScript : PlayerScript {

	private bool openedBottle;
	private bool canMove;
	private bool canMeditate;

	//Flow is: world slowly loads and becomes apparent -> can move becomes true ->
	// -> hits bottle, pops up tutorial -> can move is false, canmeditate is true

	new void Start(){		
		base.Start();
		openedBottle = false;
		canMove = false;
		MakeEverythingBlack();
		mainCamera.SetCameraSizeTo(2f);


		Invoke("FadeInPlayer", 2);
		Invoke("FadeInMap", 2);
		Invoke("EnableMovement", 2);
		//Invoke("PlayerWakeUp", 16);
		//Invoke("IncreaseSound", ??);
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
			if(Input.GetButton("Fire1")){
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

	protected void MakeEverythingBlack() {
		this.GetComponent<StaticObject>().SetAlphaValue(0f);
		background.GetComponent<StaticObject>().SetAlphaValue(0f);

		/*RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) transform.position, 20f, Vector2.zero);
		foreach (RaycastHit2D hit in hits)
		{
			if (hit.transform.gameObject.GetComponent<StaticObject>() != null){
				hit.transform.gameObject.GetComponent<StaticObject>().SetAlphaValue(0f);
			}
		}*/
	}

	private void FadeInPlayer() {	this.GetComponent<StaticObject>().TransitionToAlphaValue(1f, 0.008f);	}
	private void FadeInMap() {	background.GetComponent<StaticObject>().TransitionToAlphaValue(1f, 0.008f);	}

	void EnableMovement() {
		canMove = true;
	}

	void PopUpTutorial() {
		//Activate the Canvas
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
