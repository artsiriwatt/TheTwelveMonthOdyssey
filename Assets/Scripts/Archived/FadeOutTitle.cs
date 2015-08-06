using UnityEngine;
using System.Collections;

public class FadeOutTitle : MonoBehaviour {

	// Use this for initialization
	SpriteRenderer spr;
	double time = 0f;
	void Start () {
		spr = this.gameObject.GetComponent<SpriteRenderer>();
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		time += Time.fixedDeltaTime;
		if(time > 5) {
		
		Color alph = spr.material.color;
		alph.a -= 0.01f;
		spr.material.color = alph;
		//Debug.Log(alph.a);
		if( alph.a <= 0f) {
			GameObject.Destroy(this.gameObject);
		}
		}
	}
}
