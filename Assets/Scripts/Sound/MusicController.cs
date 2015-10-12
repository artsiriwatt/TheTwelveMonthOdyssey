using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {
	public float musicVolume;
	public AudioSource musicSource;


	// Use this for initialization
	void Start () {
		musicSource.volume = musicVolume;
	}

	// Update is called once per frame
	void Update () {

	}

	public void PlayMeditationMusic() {
		musicSource.Play();
	}

	public void StopMeditationMusic() {
		musicSource.Stop();
	}


}
