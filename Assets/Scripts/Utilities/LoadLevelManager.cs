using UnityEngine;
using System.Collections;

public class LoadLevelManager : MonoBehaviour {

	public bool StartAtWorld1;
	public bool ClearData;

	/* Player Prefs Keys:
	LastMeditated: String
	NumAreasCleared: Int
	*/

	/* Player PrefsX Keys:
	PlayerLocation: Vector3
	MeditationLocations: Vector3[]
	DoneTutorial: Bool
	*/

	void Awake () {
		if (ClearData) {
			ClearSaveData();
		}
		else if (StartAtWorld1){
			if (PlayerPrefsX.GetBool("DoneTutorial")) {
				Application.LoadLevel("Alpha_World1");
			}
			else{
				ClearSaveData();
				PlayerPrefsX.SetBool("DoneTutorial", true);
				PlayerPrefsX.SetBool("InitializeWorld1", true);
				Application.LoadLevel("Alpha_World1");			}
		}
		else{
			if (PlayerPrefsX.GetBool("DoneTutorial")) {
				Application.LoadLevel("Alpha_World1");
			}
			else {
				Application.LoadLevel("Alpha_Tutorial");
			}
		}

	}

	public void ClearSaveData() {
		PlayerPrefs.DeleteAll();
	}
}
