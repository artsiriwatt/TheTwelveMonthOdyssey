using UnityEngine;
using System.Collections;

public class DateTimeTracker : MonoBehaviour {

	private bool HasMeditatedToday;
	private static DateTimeTracker instance;
	private string lastMeditated;

	public static DateTimeTracker GetInstance() {
		return instance;
	}

	void Awake () {
		//PlayerPrefs.DeleteAll();
		if (instance != null && instance != this) {
			Destroy( this.gameObject );
			return;
		}
		else {
			instance = this;
		}

		LoadMeditationState();
	}


	public bool IsFirstDailyMeditation() {
		return !HasMeditatedToday;
	}

	public void MeditatedToday() {
		//lastMeditated = System.DateTime.Now.ToString("HH:mm:ss");
		lastMeditated = System.DateTime.Now.ToString("MM/dd/yyyy");
		PlayerPrefs.SetString("lastMeditated", lastMeditated);
		HasMeditatedToday = true;

		ChangeMapToNight();
	}

	private void LoadMeditationState() {
		if (PlayerPrefs.HasKey("lastMeditated")){
			lastMeditated = PlayerPrefs.GetString("lastMeditated");
			//if (lastMeditated == System.DateTime.Now.ToString("HH:mm:ss")){
			if (lastMeditated == System.DateTime.Now.ToString("MM/dd/yyyy")){
				HasMeditatedToday = true;
				ChangeMapToNight();
			}
			else{
				HasMeditatedToday = false;
				ChangeMapToDay();
			}
		}
		else{
			HasMeditatedToday = false;
			ChangeMapToDay();
		}

		Debug.Log(HasMeditatedToday);
	}

	private void ChangeMapToDay () {
		GameObject.FindGameObjectsWithTag("background")[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Maps/map_1");
	}
	private void ChangeMapToNight () {
		GameObject.FindGameObjectsWithTag("background")[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Maps/map_1_night");

	}
}
