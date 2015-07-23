using UnityEngine;
using System.Collections;

public class CloudRemovalScript : MonoBehaviour {

	public float fadeOutRate = 0.00138f;

	// Set the rate of the tick(how often the sprite changes). Change only if necessary. 
	private int tick = 5;

	//Variables for reference. Do not change.
	private bool isDisappearing = false;
	private int framecount = 0;

	void Start () {
	}
	
	void FixedUpdate () {
		if (isDisappearing) {
			if(framecount >= tick) {
				framecount = 0;
				Color update = this.gameObject.GetComponent<Renderer>().material.color;
				update.a -= fadeOutRate;
				this.gameObject.GetComponent<Renderer>().material.color = update;
				if (update.a <= 0) {
					Destroy(this.gameObject);
				}
			}
			framecount++;
		}
	}

	public void Disappear () {
		isDisappearing = true;
	}

	public bool isDying () {
		return isDisappearing;
	}
}
