using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;  
using System.Collections;

public class TutorialUIScript : MonoBehaviour {

	private int pageCounter = 1;

	public GameObject page1;
	public GameObject page2;
	public GameObject page3;

	public GameObject buttonLeft;
	public GameObject buttonRight;

	public GameObject player;


	// Use this for initialization
	void Start () {
		pageCounter = 1;
		Redraw();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Redraw(){
		if(pageCounter == 1) {
			buttonLeft.GetComponent<Button>().interactable = false;
			page1.SetActive(true);
			page2.SetActive(false);
		}
		if(pageCounter == 2) {
			buttonLeft.GetComponent<Button>().interactable = true;
			page1.SetActive(false);
			page2.SetActive(true);
			page3.SetActive(false);
		}
		if(pageCounter == 3) {
			page2.SetActive(false);
			page3.SetActive(true);
		}
	}

	public void PreviousPage() {
		if (pageCounter != 1) {
			pageCounter--;
			Redraw();
		}


	}

	public void NextPage() {
		if (pageCounter != 3) {
			pageCounter++;
			Redraw();
		}
		else{
			player.GetComponent<TutorialPlayerScript>().CloseTutorialPopUp();
		}
	}
}
