using UnityEngine;
using System.Collections;

public class SaveManager : MonoBehaviour {

	public bool clearData;
	public bool enableTutorial;
	private static SaveManager instance;
	private PlayerScript player;

	public DateTimeTracker dateTimeTracker;
	[HideInInspector]public int NumAreasCleared;
	[HideInInspector]public string LastMeditated;

	/* Player Prefs Keys:
	LastMeditated: String
	NumAreasCleared: Int
	*/

	/* Player PrefsX Keys:
	PlayerLocation: Vector3
	MeditationLocations: Vector3[]
	DoneTutorial: Bool
	*/

	public static SaveManager GetInstance() {
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
			ClearSaveData();
		}
	}

	void Start () {
		if (enableTutorial) {
			player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<TutorialPlayerScript>();
			player.enabled = true;
		}
		else {
			player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerScript>();
			player.enabled = true;
		}
		/*
		if (enableTutorial && !PlayerPrefsX.GetBool("DoneTutorial")) {
			player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<TutorialPlayerScript>();
		}
		else {
			player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerScript>();
		}*/
		
		Load();
	}

	public void Load() {
		if (PlayerPrefsX.GetBool("DoneTutorial")){
			NumAreasCleared = PlayerPrefs.GetInt("NumAreasCleared");
			player.SetLocation(PlayerPrefsX.GetVector3("PlayerLocation"));
			player.ClearClouds(PlayerPrefsX.GetVector3Array("MeditationLocations"));
		}
		else{
			Vector3[] meditationLocations = new Vector3[100];
			NumAreasCleared = 0;
			PlayerPrefs.SetInt("NumAreasCleared", NumAreasCleared);
			meditationLocations[PlayerPrefs.GetInt("NumAreasCleared")] = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
			NumAreasCleared++;
			PlayerPrefs.SetInt("NumAreasCleared", NumAreasCleared);
			PlayerPrefsX.SetVector3Array("MeditationLocations", meditationLocations);
			PlayerPrefsX.SetVector3("PlayerLocation", transform.position);
			PlayerPrefsX.SetBool("DoneTutorial", true);

			player.ClearClouds(PlayerPrefsX.GetVector3Array("MeditationLocations"));

			PlayerPrefsX.SetVector3("PlayerLocation", player.transform.position);
		}
	}

	public bool IsDailyMeditation() {
		return dateTimeTracker.IsFirstDailyMeditation();
	}

	public void Save() {
		PlayerPrefsX.SetVector3("PlayerLocation", player.transform.position);
		Vector3[] meditationLocations = PlayerPrefsX.GetVector3Array("MeditationLocations");
		meditationLocations[PlayerPrefs.GetInt("NumAreasCleared")] = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
		PlayerPrefsX.SetVector3Array("MeditationLocations", meditationLocations);
		NumAreasCleared++;
		PlayerPrefs.SetInt("NumAreasCleared", NumAreasCleared);

		dateTimeTracker.MeditatedToday();
	}

	public void ClearSaveData() {
		PlayerPrefs.DeleteAll();
	}


}
