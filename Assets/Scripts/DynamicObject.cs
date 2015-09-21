using UnityEngine;
using System.Collections;

public class DynamicObject : MonoBehaviour {

	private LocalState state;

	enum LocalState{
		MeditationState, 
		RegularState
	};

	void Start () {
		state = LocalState.RegularState;
	}
	
	void FixedUpdate () {
		switch (state){
			case LocalState.MeditationState:
				break;

			case LocalState.RegularState:
				break;

		}
	}

	public void StartMeditation (){
		state = LocalState.MeditationState;
	}

	public void StopMeditation (){
		state = LocalState.RegularState;
	}
}
