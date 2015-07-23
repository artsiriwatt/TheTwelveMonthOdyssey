using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

	public AmbientController ambientController;
	public SoundEffectController soundEffectController;
	public MusicController musicController;

	// Use this for initialization
	void Start () {
	}

	
	// Update is called once per frame
	void Update () {
	}

	public void StartMeditation () {
		ambientController.DecreaseAmbientVolume();
		musicController.PlayMeditationMusic();
		soundEffectController.PlaySitDown();
	}

	public void StopMeditation () {
		ambientController.IncreaseAmbientVolume();
		musicController.StopMeditationMusic();
		soundEffectController.PlayStandUp();
	}

	public void FinishMeditation () {
		StopMeditation();
		soundEffectController.PlayChime();
	}


	public void Walk() {
		soundEffectController.PlayWalkSound();
	}

	public void StopWalk() {
		soundEffectController.StopWalkSound();
	}
	



	
}
