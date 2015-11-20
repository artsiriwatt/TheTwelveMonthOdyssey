using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public bool onMobile;
	public float StartingViewCircleRadius;
	public float MeditationClearingRadius;
	public int meditationTimeInSeconds;

	//Variables for reference. Do not change. 
	protected GameObject background;
	protected CameraScript mainCamera;
	protected AudioManager audioManager;
	protected SaveManager saveManager;

	protected PlayerSpriteManager spriteManager;
	protected PlayerMovementManager movementManager;

	[HideInInspector]public bool isMeditating;
	protected Collider2D playerColl;
	protected int interruptMeditationFrames;
	protected float previousAccel;
	protected static double InterruptionTolerance = 0.35;
	//removedBothFingers is used to ensure meditation can't immediately occur after the phone is shaken.
	protected bool removedBothFingers;

    public Vector3 GetLocation() {	return transform.position; }
    public void SetLocation(Vector3 location) {	 transform.position = location; }

	// Use this for initialization
	protected void Awake () {		

		mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<CameraScript>();
		background = GameObject.FindGameObjectsWithTag("background")[0];

		playerColl = this.gameObject.GetComponent<Collider2D>();
		spriteManager = this.gameObject.GetComponent<PlayerSpriteManager>();
		movementManager = this.gameObject.GetComponent<PlayerMovementManager>();

		audioManager = AudioManager.GetInstance();
		saveManager = SaveManager.GetInstance();

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

	protected void Start () {
		movementManager.SetSitting();
		spriteManager.isSitting = true;
	}

	protected void FixedUpdate () {
		if (onMobile) {
			if (isMeditating){
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
	        		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					Vector3 point = ray.origin + (ray.direction*10);
					Vector2 p = new Vector2(point.x, point.y);
	        		movementManager.MoveToLocation(p);
	        	}
				else if (Input.touchCount == 2 && removedBothFingers){
					StartMeditation();
				}
	        }
		}

		if (!onMobile) {
			if (isMeditating || spriteManager.isSitting) {
				if (Input.GetKeyDown(KeyCode.Mouse0)){
					InterruptMeditation();	
				}
			}
			else {
		        if (Input.GetButton("Fire1"))
		        {
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					Vector3 point = ray.origin + (ray.direction*10);
					Vector2 p = new Vector2(point.x, point.y);

					//if point hits character, meditate
					if (playerColl.OverlapPoint(p) && interruptMeditationFrames == 0) {
						StartMeditation();
					}
					else if (interruptMeditationFrames == 0){
						movementManager.MoveToLocation(p);
					}
				}
		
				if (Input.GetKeyUp(KeyCode.Mouse0)) {
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

    protected void StartMeditation() {	
    	//Tell the Meditation Manager to Spawn Balence Indicator and Change Sprite
    	spriteManager.changeSpriteToSitting();
		//Set isMeditating to be True
    	isMeditating = true;
		//Center the Character
		movementManager.Stop();	
		if (saveManager.IsDailyMeditation()){
			//Set Volume Levels to Meditation Levels
			audioManager.StartMeditation();
    		//Zoom the Camera In
			mainCamera.zoomIn();
			//Make the Background transparent
			MakeBackgroundTransparent();
			//Start the Finish Meditation Method to be done in X amount of time
			Invoke("FinishMeditation", meditationTimeInSeconds);
		}
		else{
			//Center the Camera on the player
			mainCamera.CenterCamera();
			//Set Volume Levels to Meditation Levels
			audioManager.StartMeditation(false, false, true);
		}


		if (onMobile){
			removedBothFingers = false;
			previousAccel = Mathf.Abs(Input.acceleration.x) + Mathf.Abs(Input.acceleration.y) + Mathf.Abs(Input.acceleration.z);
		}
    }

    public void InterruptMeditation() {
    	spriteManager.changeSpriteToStanding();
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


		interruptMeditationFrames = 30;
    }

    protected void FinishMeditation() {
    	spriteManager.changeSpriteToStanding();
    	//Zoom the Camera Out
    	mainCamera.zoomOut();
    	//Set isMeditating to False
    	isMeditating = false;
    	//Play Finished Meditating Sounds
    	audioManager.FinishMeditation();
    	//Clear Clouds
    	ClearClouds(MeditationClearingRadius);
    	//Set Sound Level Back to Normal
    	ResetBackgroundTransparency();

    	saveManager.Save();
    }


    protected bool phoneMoved() {
		float sumMovement = Mathf.Abs(Input.acceleration.x) + Mathf.Abs(Input.acceleration.y) + Mathf.Abs(Input.acceleration.z);
		float diff = Mathf.Abs(sumMovement - previousAccel);

		if (diff >= InterruptionTolerance){
			return true;
		}
		previousAccel = sumMovement;
		return false;
	}

    protected void MakeBackgroundTransparent() {
    	RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) transform.position, 10f, Vector2.zero); //, 0f, LayerMask.NameToLayer("Clouds"));
		
		foreach (RaycastHit2D hit in hits)
		{
			if (hit.transform.tag != "Player" && hit.transform.gameObject.GetComponent<StaticObject>() != null){
				hit.transform.gameObject.GetComponent<StaticObject>().MakeTransparent();
				//Test Case - Just delete the object
				//GameObject.Destroy(hit.transform.gameObject);
			}
		}
		background.GetComponent<StaticObject>().MakeTransparent();
    }

    protected void ResetBackgroundTransparency() {
    	RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) transform.position, 10f, Vector2.zero); //, 0f, LayerMask.NameToLayer("Clouds"));
		foreach (RaycastHit2D hit in hits)
		{
			if (hit.transform.tag != "Player" && hit.transform.gameObject.GetComponent<StaticObject>() != null){
				hit.transform.gameObject.GetComponent<StaticObject>().ReturnToOriginalState();
				//Test Case - Just delete the object
				//GameObject.Destroy(hit.transform.gameObject);
			}
		}
		background.GetComponent<StaticObject>().ReturnToOriginalState();
    }

    public void ClearClouds(Vector3[] locations) {
    	for (int x = 0; x < saveManager.NumAreasCleared; x++){
			ClearClouds(locations[x], MeditationClearingRadius);
		}
    }

    protected void ClearClouds(float radius) {
		playerColl.enabled = false;
    	RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) transform.position, radius, Vector2.zero); //, 0f, LayerMask.NameToLayer("Clouds"));
		
		foreach (RaycastHit2D hit in hits)
		{
			if (hit.transform.tag == "cloud" && hit.transform.gameObject.GetComponent<CloudObject>()){
			//if (hit.transform.tag == "cloud"){
				hit.transform.gameObject.GetComponent<CloudObject>().KillCloud();
			}
		}
		playerColl.enabled = true;
    }

    protected void ClearClouds(Vector3 location, float radius) {
		playerColl.enabled = false;
    	RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) location, radius, Vector2.zero); //, 0f, LayerMask.NameToLayer("Clouds"));
		
		foreach (RaycastHit2D hit in hits)
		{
			if (hit.transform.tag == "cloud" && hit.transform.gameObject.GetComponent<CloudObject>()){
			//if (hit.transform.tag == "cloud"){
				hit.transform.gameObject.GetComponent<CloudObject>().KillCloud();
			}
		}
		playerColl.enabled = true;
    }

}
