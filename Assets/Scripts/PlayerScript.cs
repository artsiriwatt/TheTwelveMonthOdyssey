using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public bool onMobile;
	public bool PointAndClickMovement;
	public float StartingViewCircleRadius;
	public float MeditationClearingRadius;
	public int meditationTimeInSeconds;

	private GameObject background;
	private GameObject playerSpawnPoint;
	private AudioManager audioManager;

	//Variables for reference. Do not change. 
	private PlayerMeditationManager mediationManager;
	private PlayerMovementManager movementManager;
	private GameObject mainCamera;
	private Collider2D playerColl;
	private bool isMeditating;
	private int interruptMeditationFrames;
	private float previousAccel;
	private static double InterruptionTolerance = 0.35;
	//removedBothFingers is used to ensure meditation can't immediately occur after the phone is shaken.
	private bool removedBothFingers;




	// Use this for initialization
	void Start () {
		mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
		playerSpawnPoint = GameObject.FindGameObjectsWithTag("PlayerSpawnPoint")[0];
		background = GameObject.FindGameObjectsWithTag("background")[0];

		playerColl = this.gameObject.GetComponent<Collider2D>();
		mediationManager = this.gameObject.GetComponent<PlayerMeditationManager>();
		movementManager = this.gameObject.GetComponent<PlayerMovementManager>();

		audioManager = AudioManager.GetInstance();

		transform.position = playerSpawnPoint.transform.position;

		interruptMeditationFrames = 0;
		
		RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) playerSpawnPoint.transform.position, StartingViewCircleRadius, Vector2.zero); //, 0f, LayerMask.NameToLayer("Clouds"));
		foreach (RaycastHit2D hit in hits)
		{	
			if (hit.transform.tag == "cloud"){
				GameObject.Destroy(hit.transform.gameObject);
			}
		}

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
	

	void FixedUpdate () {
		//Debug.Log(Mathf.Abs(Input.acceleration.x) + Mathf.Abs(Input.acceleration.y) + Mathf.Abs(Input.acceleration.z));
		//Debug.Log ("X accel is" + Input.acceleration.x);
		//Debug.Log ("Y accel is" + Input.acceleration.y);
		//Debug.Log ("Z accel is" + Input.acceleration.z);

		if (onMobile) {
			if (isMeditating){
				if (PointAndClickMovement && phoneMoved())  {
					InterruptMeditation();
				}

				if (Input.touchCount < 2){
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
	        	else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) {
	        		movementManager.MoveToLocation(Input.mousePosition);
	        	}
				else if (Input.touchCount == 2 && removedBothFingers){
					StartMeditation();
				}
	        }
		}

		if (!onMobile) {
			if (isMeditating) {
				if (Input.GetKeyDown(KeyCode.Mouse0)){
					InterruptMeditation();	
				}
			}
			else {
				if(Input.GetButton("Fire1") && interruptMeditationFrames == 0){
					movementManager.MoveToLocation(Input.mousePosition);
				}				
				if(Input.GetKeyUp(KeyCode.Mouse0)) {
					movementManager.Stop();
				}
			}
        }

      	// Walking sound effect handler. If Player is moving, play the walk sound effect
      	if(movementManager.isMoving()){
      		audioManager.Walk();
      	}
      	else{
      		audioManager.StopWalk();
      	}

   		//Interruption frames handler. if player recent comes out of meditation, don't allow them
   		//to do anything for half a second. 
   		if (interruptMeditationFrames > 0){
        	interruptMeditationFrames--;
        }
    }		


	bool phoneMoved () {
		float sumMovement = Mathf.Abs(Input.acceleration.x) + Mathf.Abs(Input.acceleration.y) + Mathf.Abs(Input.acceleration.z);
		float diff = Mathf.Abs(sumMovement - previousAccel);

		if (diff >= InterruptionTolerance){
			return true;
		}
		previousAccel = sumMovement;
		return false;
	}

    void StartMeditation() {
    	mediationManager.meditationStart();
    	//Zoom the Camera In
		mainCamera.SendMessage("zoomIn");
		//Set isMeditating to be True
    	isMeditating = true;
		//Set Volume Levels to Meditation Levels
		audioManager.StartMeditation();
		//Center the Character
		movementManager.Stop();
		//Make the Background transparent
		MakeBackgroundTransparent();
		//Start the Finish Meditation Method to be done in X amount of time
		Invoke("FinishMeditation", meditationTimeInSeconds);

		if (onMobile){
			removedBothFingers = false;
			previousAccel = Mathf.Abs(Input.acceleration.x) + Mathf.Abs(Input.acceleration.y) + Mathf.Abs(Input.acceleration.z);
		}
    }

    void InterruptMeditation() {
    	mediationManager.meditationStop();
    	//Zoom the Camera Out
    	mainCamera.SendMessage("zoomOut");
    	//Set isMeditating to be False
		isMeditating = false;
		//Set Volume to Background Levels
		audioManager.StopMeditation();
		//Make the Transparency the normal color for everything around the player
		ResetBackgroundTransparency();
		//Cancel The Finish Meditation Method if they interrupt the process
		CancelInvoke("FinishMeditation");


		interruptMeditationFrames = 30;
    }

    void FinishMeditation() {
    	mediationManager.meditationStop();
    	//Zoom the Camera Out
    	mainCamera.SendMessage("zoomOut");
    	//Set isMeditating to False
    	isMeditating = false;
    	//Play Finished Meditating Sounds
    	audioManager.FinishMeditation();
    	//Clear Clouds
    	ClearClouds();
    	//Set Sound Level Back to Normal
    	ResetBackgroundTransparency();

    }

    void MakeBackgroundTransparent() {
    	RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) transform.position, 10f, Vector2.zero); //, 0f, LayerMask.NameToLayer("Clouds"));
		
		foreach (RaycastHit2D hit in hits)
		{
			if (hit.transform.tag != "Player" && hit.transform.gameObject.GetComponent<ObjectTransparencyScript>() != null){
				hit.transform.gameObject.GetComponent<ObjectTransparencyScript>().MakeTransparent();
				//Test Case - Just delete the object
				//GameObject.Destroy(hit.transform.gameObject);
			}
		}
		background.GetComponent<ObjectTransparencyScript>().MakeTransparent();
    }

    void ResetBackgroundTransparency () {
    	RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) transform.position, 10f, Vector2.zero); //, 0f, LayerMask.NameToLayer("Clouds"));
		foreach (RaycastHit2D hit in hits)
		{
			if (hit.transform.tag != "Player" && hit.transform.gameObject.GetComponent<ObjectTransparencyScript>() != null){
				hit.transform.gameObject.GetComponent<ObjectTransparencyScript>().ReturnToOriginalState();
				//Test Case - Just delete the object
				//GameObject.Destroy(hit.transform.gameObject);
			}
		}
		background.GetComponent<ObjectTransparencyScript>().ReturnToOriginalState();
    }


    void ClearClouds() {
		playerColl.enabled = false;
    	RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) transform.position, MeditationClearingRadius, Vector2.zero); //, 0f, LayerMask.NameToLayer("Clouds"));
		
		foreach (RaycastHit2D hit in hits)
		{
			if (hit.transform.tag == "cloud"){
				hit.transform.gameObject.GetComponent<CloudRemovalScript>().Disappear();
			}
		}
		playerColl.enabled = true;
    }

	//Occurs when the player clicks on the character. Works for FixedUpdate but not for Updated
	void OnMouseDown() {
		if(onMobile){
			return;
		}

		if(!isMeditating && interruptMeditationFrames == 0){
			StartMeditation();
		}
		else {
			InterruptMeditation();
		}
    }



}
