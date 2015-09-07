using UnityEngine;
using System.Collections;

public class TutorialPlayerScript : PlayerScript {

	public bool onMobile;

	public float playerSpeed;
	public float StartingViewCircleRadius;

	public int meditationTimeinSeconds;

	private GameObject background;
	public GameObject playerSpawnPoint;

	public AudioManager audio;

	public GameObject leftLamp;
	public GameObject rightLamp;


	//Variables for reference. Do not change. 
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

		interruptMeditationFrames = 0;

		StartMeditation();

		/* Detect which playform the user is playing on
		string[] desktopPlatforms = {"OSXEditor", "OSXPlayer", "WindowsPlayer", "OSXWebPlayer", 
		"OSXDashboardPlayer", "WindowsWebPlayer", "WindowsEditor"};
		if (System.Array.IndexOf(desktopPlatforms, Application.platform) == -1) {
			onMobile = false;
		}
		else{
			onMobile = true;
		}*/
		removedBothFingers = false;
	}
	

	void FixedUpdate () {

		for (int i = 0; i < Input.touchCount; i++) {
			Touch touch = Input.GetTouch(i);

			Ray ray = Camera.main.ScreenPointToRay(touch.position);
			Vector3 point = ray.origin + (ray.direction*10);

			if (touch.position.x < Screen.width/2){
				leftLamp.transform.position = point;
			}
			else if (touch.position.x >= Screen.width/2){
				rightLamp.transform.position = point;
			}
		}



		if (isMeditating){
			if (phoneMoved())  {
				InterruptMeditation();
				interruptMeditationFrames = 30;
			}

			if (Input.touchCount < 2){
				removedBothFingers = true;
    			InterruptMeditation();
				interruptMeditationFrames = 60;
    		}
        }
		
        if (!isMeditating){
        	if (Input.touchCount < 2){
        		removedBothFingers = true;
       	 	}
			else if (Input.touchCount == 2 && removedBothFingers && interruptMeditationFrames == 0){
				StartMeditation();
			}
        }

      	// Walking sound effect handler. If Player is moving, play the walk sound effect
        if (lastPosition != this.gameObject.transform.position){
      		audio.Walk();
      	}
      	else{
      		audio.StopWalk();
      	}
   		lastPosition = this.gameObject.transform.position;

   		//Interruption frames handler. if player recent comes out of meditation, don't allow them
   		//to move for half a second. 
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
    	//Zoom the Camera In
		mainCamera.SendMessage("zoomIn");
		//Set isMeditating to be True
    	isMeditating = true;
		//Set Volume Levels to Meditation Levels
		audio.StartMeditation();
		//Center the Character
		targetPosition = transform.position;
		//Make the Background transparent
		MakeBackgroundTransparent();
		//Start the Finish Meditation Method to be done in X amount of time
		Invoke("FinishMeditation", meditationTimeinSeconds);

		if (onMobile){
			previousAccel = Mathf.Abs(Input.acceleration.x) + Mathf.Abs(Input.acceleration.y) + Mathf.Abs(Input.acceleration.z);
			removedBothFingers = false;
		}
    }

    void InterruptMeditation() {
    	//Zoom the Camera Out
    	mainCamera.SendMessage("zoomOut");
    	//Set isMeditating to be False
		isMeditating = false;
		//Set Volume to Background Levels
		audio.StopMeditation();
		//Make the Transparency the normal color for everything around the player
		ResetBackgroundTransparency();
		//Cancel The Finish Meditation Method if they interrupt the process
		CancelInvoke("FinishMeditation");
    }

    void FinishMeditation() {
    	//Zoom the Camera Out
    	mainCamera.SendMessage("zoomOut");
    	//Set isMeditating to False
    	isMeditating = false;
    	//Play Finished Meditating Sounds
    	audio.FinishMeditation();
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
				//GameObject.Destroy(hit.transform.gameObject);
			}
		}
		playerColl.enabled = true;
    }

    void OnCollisionEnter2D(Collision2D collision) {
    	targetPosition = transform.position;
	}
}
