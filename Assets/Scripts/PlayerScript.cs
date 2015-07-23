using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public bool onMobile;

	public float playerSpeed;
	public float StartingViewCircleRadius;

	public int meditationTimeinSeconds;

	private GameObject background;
	public GameObject playerSpawnPoint;

	public AudioController audioManager;



	//Variables for reference. Do not change. 
	private PlayerMeditationController mediationManager;
	private Vector3 targetPosition;
	private GameObject mainCamera;
	private Collider2D playerColl;
	private Vector3 lastPosition;
	private bool isMeditating;
	private int interruptMeditationFrames;
	private float previousAccel;
	//removedBothFingers is used to ensure meditation can't immediately occur after the phone is shaken.
	private bool removedBothFingers;




	// Use this for initialization
	void Start () {
		mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0];

		RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) playerSpawnPoint.transform.position, StartingViewCircleRadius, Vector2.zero); //, 0f, LayerMask.NameToLayer("Clouds"));
		foreach (RaycastHit2D hit in hits)
		{	
			if (hit.transform.tag == "cloud"){
				GameObject.Destroy(hit.transform.gameObject);
			}
		}

		transform.position = playerSpawnPoint.transform.position;
		targetPosition = transform.position;

		playerColl = this.gameObject.GetComponent<Collider2D>();

		background = GameObject.FindGameObjectsWithTag("background")[0];
		mediationManager = this.gameObject.GetComponent<PlayerMeditationController>();

		interruptMeditationFrames = 0;

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
				if (phoneMoved())  {
					InterruptMeditation();
					interruptMeditationFrames = 30;
				}

				if (Input.touchCount < 2){
					removedBothFingers = true;
        			InterruptMeditation();
					interruptMeditationFrames = 30;
        		}
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
					StartMeditation();
				}
	        }
		}

		if (!onMobile) {
			if(Input.GetKeyDown(KeyCode.Mouse0)) {
				if(isMeditating){
					InterruptMeditation();
					interruptMeditationFrames = 30;
				}
			}

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


	   

      	// Walking sound effect handler. If Player is moving, play the walk sound effect
        if (lastPosition != this.gameObject.transform.position){
      		audioManager.Walk();
      	}
      	else{
      		audioManager.StopWalk();
      	}
   		lastPosition = this.gameObject.transform.position;

   		//Interruption frames handler. if player recent comes out of meditation, don't allow them
   		//to do anything for half a second. 
   		if (interruptMeditationFrames > 0){
        	interruptMeditationFrames--;
        }
   		if (interruptMeditationFrames == 0){
        	transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * playerSpeed);	
   		}

    }		


	bool phoneMoved () {
		float sumMovement = Mathf.Abs(Input.acceleration.x) + Mathf.Abs(Input.acceleration.y) + Mathf.Abs(Input.acceleration.z);
		float diff = Mathf.Abs(sumMovement - previousAccel);

		if (diff >= 0.2){
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
		targetPosition = transform.position;
		//Make the Background transparent
		MakeBackgroundTransparent();
		//Start the Finish Meditation Method to be done in X amount of time
		Invoke("FinishMeditation", meditationTimeinSeconds);

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
    	RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) transform.position, 2.5f, Vector2.zero); //, 0f, LayerMask.NameToLayer("Clouds"));
		
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

    void OnCollisionEnter2D(Collision2D collision) {
    	targetPosition = transform.position;
	}

}
