using UnityEngine;
using System.Collections;

public class SoundEffectController : MonoBehaviour {

	public AudioClip Chime1;
	public AudioClip Chime2;
	public AudioClip Chime3;

	public AudioClip sitDownClip;
	public AudioClip standUpClip;

	public AudioSource walking;
	public AudioSource chime;
	public AudioSource sitRustle;

	// Use this for initialization
	void Start () {
		//Set the initial volume to be background
		walking.volume = 0.9f;
		chime.volume = 0.6f;
		sitRustle.volume = 1f;
	}

	// Update is called once per frame
	void Update () {
	}


	public void PlayChime () {
		int r = (int)Random.Range(0, 3);
		if (r == 0){
			chime.PlayOneShot(Chime1);
		}
		else if (r == 1){
			chime.PlayOneShot(Chime2);
		}
		else if (r == 2){
			chime.PlayOneShot(Chime3);
		}
	}
	
	public void PlayWalkSound() {
		if (!walking.isPlaying){
			walking.Play();
		}
	}

	public void StopWalkSound() {
		walking.Stop();
	}

	public void PlaySitDown() {
		sitRustle.PlayOneShot(sitDownClip);
	}

	public void PlayStandUp() {
		sitRustle.PlayOneShot(standUpClip);
	}
}
