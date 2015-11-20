using UnityEngine;
using System.Collections;

public class PlayerSpriteManager : MonoBehaviour {
	public float fadeInRate;
	public float fadeOutRate;

	public bool disableBalenceIndicator;

	//public Sprite standingSprite;
	//public Sprite sittingSprite;

	private GameObject balenceIndicator;
	private PlayerScript player;
	private PlayerMovementManager animationManager;

	// Set the rate of the tick(how often the sprite changes). Change only if necessary. 
	private static int tick = 5;
	private static int balenceTick = 30;
	private static float balenceIndicatorDelay = 1f;
	private static float yBalenceThreshold = 0.012f;
	private static float xBalenceThreshold = 0.005f;

	//Variables for reference. Do not change.
	private bool hasNotTransitionedYet;
	[HideInInspector]public bool isSitting;
	private int framecount = 0;
	private int balenceFrameCount = 0;
	private bool changeToSitting = false;
	private bool changeToStanding = false;
	private float initialYAccel;
	private float initialXAccel;
	private Vector3 balenceIndicatorLocation;
	private Vector2 screenPosition;


	void Start () {
		if (!disableBalenceIndicator) {
			balenceIndicator = GameObject.FindGameObjectsWithTag("BalenceIndicator")[0];
		}

		player = this.gameObject.GetComponent<PlayerScript>();
		animationManager = this.gameObject.GetComponent<PlayerMovementManager>();
		balenceIndicatorLocation = new Vector3(0,0,0);
	}
	
	void FixedUpdate () {
		//Handler for Balence Indicator
		if (!disableBalenceIndicator && player.isMeditating){
			balenceIndicator.transform.position = balenceIndicatorLocation;
			screenPosition = Camera.main.WorldToScreenPoint(balenceIndicator.transform.position);

			if (screenPosition.y > Screen.height || screenPosition.y < 0 || screenPosition.x > Screen.width || screenPosition.x < 0){
				//Handheld.Vibrate();
				player.InterruptMeditation();
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
		if (isTransparent() && hasNotTransitionedYet){
			if (changeToSitting){
				animationManager.SetSitting();
				//this.GetComponent<SpriteRenderer>().sprite = sittingSprite;
				changeToSitting = false;

				initialXAccel = Input.acceleration.x;
				initialYAccel = Input.acceleration.y;
			}
			else if (changeToStanding){
				animationManager.SetStanding();
				//this.GetComponent<SpriteRenderer>().sprite = standingSprite;
				changeToStanding = false;
			}
			Invoke("ReturnToOriginalState", 0.25f);
			hasNotTransitionedYet = false;
		}


	}

	void MakeTransparent () {
		TransitionToAlphaValue(0f, fadeOutRate);
		hasNotTransitionedYet = true;
	}

	void ReturnToOriginalState () {
		TransitionToAlphaValue(1f, fadeInRate);
		hasNotTransitionedYet = true;
	}

	void TransitionToAlphaValue(float a, float r){
		StopAllCoroutines();
		StartCoroutine(TransitionToAlpha(a, r));
	}

	IEnumerator TransitionToAlpha(float alpha, float rate) {
		Color c = this.gameObject.GetComponent<Renderer>().material.color;
		framecount = 0;

		if (c.a > alpha){
			while (c.a > alpha) {
				framecount++;
				if (framecount >= tick){
					framecount = 0;
					c.a -= rate;
					this.gameObject.GetComponent<Renderer>().material.color = c;
				}
				yield return null;
			}
		}
		else if (c.a < alpha) {
			
			while (c.a < alpha) {
				framecount++;
				if (framecount >= tick){
					framecount = 0;
					c.a += rate;
					this.gameObject.GetComponent<Renderer>().material.color = c;
				}
				yield return null;
			}
		}	
	}

	bool isTransparent() {
		Color update = this.gameObject.GetComponent<Renderer>().material.color;
		return update.a <= 0;
	}

	public void SetAlphaValue (float a) {
		Color update = this.gameObject.GetComponent<Renderer>().material.color;
		update.a = a;
		this.gameObject.GetComponent<Renderer>().material.color = update;
	}

	public void changeSpriteToSitting() {		
		//Set internal boolean true
		isSitting = true;

		MakeTransparent();
		changeToSitting = true;
		changeToStanding = false;

		if (!disableBalenceIndicator){
			Invoke("activateBalenceIndicator", balenceIndicatorDelay);
		}

	}

	public void changeSpriteToStanding() {
		//Set internal boolean false
		isSitting = false;

		MakeTransparent();
		changeToStanding = true;
		changeToSitting = false;

		//Cancel Balence Indicator function call if 
		if (!disableBalenceIndicator){
			CancelInvoke("activateBalenceIndicator");
		}
		
		//Make inactive the Balence Indicator Object
		if (!disableBalenceIndicator){
			balenceIndicator.SetActive(false);
		}
	}

	public void activateBalenceIndicator() {
		//Make active the Balence Indicator Object
		if (!disableBalenceIndicator){
			balenceIndicator.SetActive(true);
			balenceIndicatorLocation = new Vector3(transform.position.x, transform.position.y + 1, 0);
			balenceIndicator.transform.position = balenceIndicatorLocation;
			
			calibrateAcceleration();
		}
	}

	void calibrateAcceleration() {
		//Without Fancy-Schmancy Smoothing
		initialXAccel = (initialXAccel + Input.acceleration.x)/2;
		initialYAccel = (initialYAccel + Input.acceleration.y)/2;
	}
}
