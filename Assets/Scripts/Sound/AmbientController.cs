using UnityEngine;
using System.Collections;

public class AmbientController : MonoBehaviour {
	public float lowerAmbientVolumeLevel;
	public float higherAmbientVolumeLevel;

	private bool increaseVolume;
	private bool decreaseVolume;

	public AudioSource ambientSource;

	// Use this for initialization
	void Start () {

		//Set the initial volume to be background
		ambientSource.volume = higherAmbientVolumeLevel;
		ambientSource.Play();

	}

	// Update is called once per frame
	void Update () {
		//Small hack so that the game plays two sounds in a row
		if (increaseVolume){
			if (ambientSource.volume > higherAmbientVolumeLevel){
				increaseVolume = false;
			}
			else{
				ambientSource.volume = ambientSource.volume + 0.0006f;
			}	
		}

		if (decreaseVolume){
			if (ambientSource.volume < lowerAmbientVolumeLevel){
				decreaseVolume = false;
			}
			else{
				ambientSource.volume = ambientSource.volume - 0.0008f;
			}	
		}
	}

	public void IncreaseAmbientVolume() {
		increaseVolume = true;
		decreaseVolume = false;
	}

	public void DecreaseAmbientVolume() {
		decreaseVolume = true;
		increaseVolume = false;
	}

	
}
