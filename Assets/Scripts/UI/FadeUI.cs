using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{
	public Image image;
	public float startingAlpha;

	void Start() {
		SetAlphaValue(startingAlpha);
	}

	public void SetAlphaValue(float a){
		Color c = image.color;
		c.a = a;
		image.color = c;
		//image.GetComponent<Image>().CrossFadeAlpha(a, 0f, false);
	}

	public void TransitionToAlphaValue(float a) {
		image.GetComponent<Image>().CrossFadeAlpha(a, 5.0f, false);
	}

	public void TransitionToAlphaValue(float a, float dur) {
		image.GetComponent<Image>().CrossFadeAlpha(a, dur, false);
	}	
 
}