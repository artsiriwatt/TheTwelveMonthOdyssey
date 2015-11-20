using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
	public bool turnOffMusic;

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

    void Start () {
    	if (turnOffMusic){
    		ambientController.SetMinVolumeLevel();
    	}
    }

   	public static AudioManager GetInstance() 
    {
       return instance;
    }

	public void StartMeditation () {
		if (turnOffMusic){
			ambientController.IncreaseAmbientVolume();
		}
		else {
			ambientController.DecreaseAmbientVolume();
			musicController.PlayMeditationMusic();
		}

		soundEffectController.PlaySitDown();
	}

	public void StartMeditation (bool decreaseAmbient, bool playMusic, bool playSoundEffects) {
		if (decreaseAmbient){
			ambientController.DecreaseAmbientVolume();
		}
		if (playMusic && !turnOffMusic){
			musicController.PlayMeditationMusic();
		}
		if (playSoundEffects){
			soundEffectController.PlaySitDown();
		}
	}

	public void StopMeditation () {
		if (turnOffMusic){
			ambientController.DecreaseAmbientVolume();
		}
		else {
			ambientController.IncreaseAmbientVolume();
			musicController.StopMeditationMusic();
		}
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
