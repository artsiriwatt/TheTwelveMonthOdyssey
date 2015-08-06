﻿using UnityEngine;
using System.Collections;

public class PlayerMeditationManager : MonoBehaviour {
	public float fadeInRate;
	public float fadeOutRate;

	public Sprite standingSprite;
	public Sprite sittingSprite;

	// Set the rate of the tick(how often the sprite changes). Change only if necessary. 
	private int tick = 5;

	//Variables for reference. Do not change.
	private bool isBecomingTransparent = false;
	private bool isBecomingNormal = false;
	private int framecount = 0;
	private Color originalColor;
	private float originalAlpha;

	private bool changeToSitting = false;
	private bool changeToStanding = false;


	void Start () {
		originalColor = this.gameObject.GetComponent<Renderer>().material.color;
		originalAlpha = originalColor.a;
	}
	
	void FixedUpdate () {
		if (changeToSitting && isTransparent()){
			this.GetComponent<SpriteRenderer>().sprite = sittingSprite;
			changeToSitting = false;
			ReturnToOriginalState();
		}

		if (changeToStanding && isTransparent()){
			this.GetComponent<SpriteRenderer>().sprite = standingSprite;
			changeToStanding = false;
			ReturnToOriginalState();
		}


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
		}
		framecount++;
	}

	public void MakeTransparent () {
		isBecomingTransparent = true;
	}

	public void ReturnToOriginalState () {
		isBecomingTransparent = false;

		if (this.gameObject.GetComponent<CloudRemovalScript>() != null && this.gameObject.GetComponent<CloudRemovalScript>().isDying()){
			// If object is a cloud, don't let it return to normal, let the Cloud Removal Script take it's course to kill the object.
			isBecomingNormal = false;
		}
		else{
			isBecomingNormal = true;
		}
	}


	public bool isTransparent() {
		Color update = this.gameObject.GetComponent<Renderer>().material.color;
		return update.a <= 0;
	}

	public void meditationStart() {
		MakeTransparent();
		changeToSitting = true;
	}

	public void meditationStop() {
		MakeTransparent();
		changeToStanding = true;
	}
}