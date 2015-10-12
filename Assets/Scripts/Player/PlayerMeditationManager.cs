using UnityEngine;
using System.Collections;

public class PlayerMeditationManager : MonoBehaviour {
	public float fadeInRate;
	public float fadeOutRate;

	public bool disableBalenceIndicator;

	public Sprite standingSprite;
	public Sprite sittingSprite;

	private GameObject balenceIndicator;
	private GameObject player;

	// Set the rate of the tick(how often the sprite changes). Change only if necessary. 
	private static int tick = 5;
	private static int balenceTick = 30;
	private static float balenceIndicatorDelay = 1f;
	private static float yBalenceThreshold = 0.012f;
	private static float xBalenceThreshold = 0.005f;

	//Variables for reference. Do not change.
	private bool isMeditating;
	private bool isBecomingTransparent = false;
	private bool isBecomingNormal = false;
	private int framecount = 0;
	private int balenceFrameCount = 0;
	private Color originalColor;
	private float originalAlpha;
	private bool changeToSitting = false;
	private bool changeToStanding = false;
	private float initialYAccel;
	private float initialXAccel;
	private Vector3 balenceIndicatorLocation;
	private Vector2 screenPosition;


	void Start () {
		originalColor = this.gameObject.GetComponent<Renderer>().material.color;
		originalAlpha = originalColor.a;

		if (!disableBalenceIndicator) {
			balenceIndicator = GameObject.FindGameObjectsWithTag("BalenceIndicator")[0];
		}

		player = GameObject.FindGameObjectsWithTag("Player")[0];

		balenceIndicatorLocation = new Vector3(0,0,0);

		isMeditating = false;
	}
	
	void FixedUpdate () {
		//Handler for Balence Indicator
		if (!disableBalenceIndicator && isMeditating && !this.gameObject.GetComponent<PlayerScript>().PointAndClickMovement){
			balenceIndicator.transform.position = balenceIndicatorLocation;
			screenPosition = Camera.main.WorldToScreenPoint(balenceIndicator.transform.position);

			if (screenPosition.y > Screen.height || screenPosition.y < 0 || screenPosition.x > Screen.width || screenPosition.x < 0){
				Handheld.Vibrate();
				player.SendMessage ("InterruptMeditation");
			}

			if (Mathf.Abs((Input.acceleration.y - initialYAccel)) > yBalenceThreshold){
				if ((Input.acceleration.y - initialYAccel) > 0){
					balenceIndicatorLocation.y += (Input.acceleration.y - initialYAccel) - yBalenceThreshold;
				}
				else{
					balenceIndicatorLocation.y += (Input.acceleration.y - initialYAccel) + yBalenceThreshold;
				}
			}

			if (Mathf.Abs(Input.acceleration.x - initialXAccel) > xBalenceThreshold){
				if ((Input.acceleration.x - initialXAccel) > 0){
					balenceIndicatorLocation.x += (Input.acceleration.x - initialXAccel) - xBalenceThreshold;
				}
				else{
					balenceIndicatorLocation.x += (Input.acceleration.x - initialXAccel) + xBalenceThreshold;
				}
			}

			if(balenceFrameCount > balenceTick){
				balenceFrameCount = 0;
				calibrateAcceleration();
			}
			balenceFrameCount++;
		}


		//This is the handler when the character sprite is transparent. 
		if (isTransparent()){
			if (changeToSitting){
				this.GetComponent<SpriteRenderer>().sprite = sittingSprite;
				changeToSitting = false;

				initialXAccel = Input.acceleration.x;
				initialYAccel = Input.acceleration.y;
			}
			else if (changeToStanding){
				this.GetComponent<SpriteRenderer>().sprite = standingSprite;
				changeToStanding = false;
			}
			ReturnToOriginalState();
		}

		//Handler for Fading in and out during sprite change. 
		if (isBecomingTransparent) {
			if(framecount >= tick) {
				framecount = 0;
				Color update = this.gameObject.GetComponent<Renderer>().material.color;
				update.a -= fadeOutRate;
				this.gameObject.GetComponent<Renderer>().material.color = update;
				if (update.a <= 0) {
					isBecomingTransparent = false;
				}
			}	
			framecount++;
		}
		if (isBecomingNormal) {
			if(framecount >= tick) {
				framecount = 0;
				Color update = this.gameObject.GetComponent<Renderer>().material.color;
				update.a += fadeInRate;
				this.gameObject.GetComponent<Renderer>().material.color = update;
				if (update.a >= originalAlpha) {
					this.gameObject.GetComponent<Renderer>().material.color = originalColor;
					isBecomingNormal = false;
				}
			}
			framecount++;
		}
	}

	public void MakeTransparent () {
		isBecomingTransparent = true;
		isBecomingNormal = false;
	}

	public void ReturnToOriginalState () {
		isBecomingNormal = true;
		isBecomingTransparent = false;
	}

	public bool isTransparent() {
		Color update = this.gameObject.GetComponent<Renderer>().material.color;
		return update.a <= 0;
	}

	public void meditationStart() {
		MakeTransparent();
		changeToSitting = true;
		changeToStanding = false;

		if (!disableBalenceIndicator){
			Invoke("activateBalenceIndicator", balenceIndicatorDelay);
		}
	}

	public void meditationStop() {
		//Set internal boolean true
		isMeditating = false;

		MakeTransparent();
		changeToStanding = true;
		changeToSitting = false;

		CancelInvoke("activateBalenceIndicator");

		//Make inactive the Balence Indicator Object
		if (!this.gameObject.GetComponent<PlayerScript>().PointAndClickMovement){
			balenceIndicator.SetActive(false);
		}
	}

	public void activateBalenceIndicator() {
		//Make active the Balence Indicator Object
		if (!this.gameObject.GetComponent<PlayerScript>().PointAndClickMovement){
			balenceIndicator.SetActive(true);
			balenceIndicatorLocation = new Vector3(player.transform.position.x, player.transform.position.y + 1, 0);
			balenceIndicator.transform.position = balenceIndicatorLocation;
			
			calibrateAcceleration();
		}

		//Set internal boolean true
		isMeditating = true;

	}

	void calibrateAcceleration() {
		//Without Fancy-Schmancy Smoothing
		initialXAccel = (initialXAccel + Input.acceleration.x)/2;
		initialYAccel = (initialYAccel + Input.acceleration.y)/2;
	}
}
