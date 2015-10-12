using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	private static AudioManager instance;
	public AmbientController ambientController;
	public SoundEffectController soundEffectController;
	public MusicController musicController;

 	void Awake () {
    	if (instance != null && instance != this) {
    		Destroy( this.gameObject );
      		return;
      	}
    	else {
    		instance = this;
    	}
    }

   	public static AudioManager GetInstance() 
    {
       return instance;
    }

	public void StartMeditation () {
		ambientController.DecreaseAmbientVolume();
		musicController.PlayMeditationMusic();
		soundEffectController.PlaySitDown();
	}

	public void StartMeditation (bool decreaseAmbient, bool playMusic, bool playSoundEffects) {
		if (decreaseAmbient){
			ambientController.DecreaseAmbientVolume();
		}
		if (playMusic){
			musicController.PlayMeditationMusic();
		}
		if (playSoundEffects){
			soundEffectController.PlaySitDown();
		}
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
