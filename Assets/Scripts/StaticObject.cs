using UnityEngine;
using System.Collections;

public class StaticObject : MonoBehaviour {

	enum RenderState{
		BecomingTransparent, 
		BecomingNormal,
		Transparent,
		Normal
	};

	/* The ratio of the rates for 2 minutes of meditation where the world 
	will be completely transparent after the 2 minutes:
	Tick = 30, Fade Out = 0.005f, Fade In = 0.04f */
	public float fadeInRate = 0.012f;
	public float fadeOutRate = 0.00083333333334f;

	// Set the rate of the tick(how often the sprite changes). Change only if necessary. 
	private int tick = 5;

	//Variables for reference. Do not change.
	private int framecount = 0;
	private RenderState state;
	Color originalColor;
	float originalAlpha;


	protected void Start () {
		state = RenderState.Normal;
		originalColor = this.gameObject.GetComponent<Renderer>().material.color;
		originalAlpha = originalColor.a;
	}
	
	void FixedUpdate () {
		switch (state) {
			case RenderState.BecomingTransparent:
				if(framecount >= tick) {
					framecount = 0;
					Color update = this.gameObject.GetComponent<Renderer>().material.color;
					update.a -= fadeOutRate;
					this.gameObject.GetComponent<Renderer>().material.color = update;
					if (isTransparent()) {
						state = RenderState.Transparent;
					}
				}	
				framecount++;
				break;

			case RenderState.BecomingNormal:
				if(framecount >= tick) {
					framecount = 0;
					Color update = this.gameObject.GetComponent<Renderer>().material.color;
					update.a += fadeInRate;
					this.gameObject.GetComponent<Renderer>().material.color = update;
					if (update.a >= originalAlpha) {
						this.gameObject.GetComponent<Renderer>().material.color = originalColor;
						state = RenderState.Normal;
					}
				}
				framecount++;
				break;
		}
	}

	public void MakeTransparent () {
		state = RenderState.BecomingTransparent;
	}

	public void ReturnToOriginalState () {
		state = RenderState.BecomingNormal;
	}

	public bool isTransparent() {
		Color update = this.gameObject.GetComponent<Renderer>().material.color;
		return update.a <= 0;
	}
}
