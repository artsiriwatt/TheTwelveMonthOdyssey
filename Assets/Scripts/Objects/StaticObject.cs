using UnityEngine;
using System.Collections;

public class StaticObject : MonoBehaviour {


	/* The ratio of the rates for 2 minutes of meditation where the world 
	will be completely transparent after the 2 minutes:
	Tick = 30, Fade Out = 0.005f, Fade In = 0.04f */
	public float fadeInRate = 0.012f;
	public float fadeOutRate = 0.00083333333334f;

	// Set the rate of the tick(how often the sprite changes). Change only if necessary. 
	private int tick = 5;

	//Variables for reference. Do not change.
	private int framecount = 0;
	Color originalColor;
	float originalAlpha;


	protected void Start () {
		originalColor = this.gameObject.GetComponent<Renderer>().material.color;
		originalAlpha = originalColor.a;
	}
	
	void FixedUpdate () {
	}

	public void TransitionToAlphaValue(float a, float r){
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

	public void SetAlphaValue (float a) {
		Color update = this.gameObject.GetComponent<Renderer>().material.color;
		update.a = a;
		this.gameObject.GetComponent<Renderer>().material.color = update;
	}

	public void MakeTransparent () {
		TransitionToAlphaValue(0f, fadeOutRate);
	}

	public void ReturnToOriginalState () {
		TransitionToAlphaValue(originalAlpha, fadeInRate);
	}

	public bool isTransparent() {
		Color update = this.gameObject.GetComponent<Renderer>().material.color;
		return update.a <= 0;
	}
}
