using UnityEngine;
using System.Collections;

public class DateTimeTracker : MonoBehaviour {

	public bool clearData;

	private bool HasMeditatedToday;
	private static DateTimeTracker instance;
	private string LastMeditated;

	/* Player Prefs Keys:
	LastMeditated: String
	NumAreasCleared: Int
	*/

	/* Player PrefsX Keys:
	PlayerLocation: Vector3
	MeditationLocations: Vector3[]
	DoneTutorial: Bool
	*/

	public static DateTimeTracker GetInstance() {
		return instance;
	}

	void Awake () {
		if (instance != null && instance != this) {
			Destroy( this.gameObject );
			return;
		}
		else {
			instance = this;
		}

		if (clearData){
			PlayerPrefs.DeleteAll();
		}

		LoadMeditationState();
	}


	public bool IsFirstDailyMeditation() {
		return !HasMeditatedToday;
	}

	public void MeditatedToday() {
		//LastMeditated = System.DateTime.Now.ToString("HH:mm:ss");
		LastMeditated = System.DateTime.Now.ToString("MM/dd/yyyy");
		PlayerPrefs.SetString("LastMeditated", LastMeditated);
		HasMeditatedToday = true;

		ChangeMapToNight();
	}

	private void LoadMeditationState() {
		if (PlayerPrefs.HasKey("LastMeditated")){
			LastMeditated = PlayerPrefs.GetString("LastMeditated");
			//if (LastMeditated == System.DateTime.Now.ToString("HH:mm:ss")){
			if (LastMeditated == System.DateTime.Now.ToString("MM/dd/yyyy")){
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
	}

	private void ChangeMapToDay () {
		GameObject.FindGameObjectsWithTag("background")[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Maps/map_1");
	}
	private void ChangeMapToNight () {
		GameObject.FindGameObjectsWithTag("background")[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Maps/map_1_night");

	}
}
