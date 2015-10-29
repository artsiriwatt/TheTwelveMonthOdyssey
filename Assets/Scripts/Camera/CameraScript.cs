using UnityEngine;
using System.Collections;
public class CameraScript : MonoBehaviour {


	public float zoomedOutOrthographicSize = 4f; 
	public float zoomedInOrthographicSize = 2f;
	public float zoomInSpeedOrthographic = 0.015f;
	public float zoomOutSpeedOrthographic = 0.085f;
	public float deadzoneX = 3f;
	public float deadzoneY = 0.8f;

	//public float zoomedOutFOV;
	//public float zoomedInFOV;
	//public float zoomSpeedPerspective;
	//public float cameraPerspectiveShift;

	private GameObject player;
	private Vector3 offset;
	//private bool zoomedIn;

	void Start () {
		player = GameObject.FindGameObjectsWithTag("Player")[0];
		offset = new Vector3(0, 0, -10);
		transform.position = player.transform.position + offset;
	}
	
	// Update is called once per frame
	void Update () {

		//For Deadzone camera movement, I handles 2 cases: movement outside the X boundary and movement outside the Y boundard
		//X Deadzone Camera movement
		if(player.transform.position.x < this.transform.position.x - deadzoneX){
			transform.position = new Vector3(player.transform.position.x + deadzoneX, transform.position.y, transform.position.z);
		}
		else if(player.transform.position.x > transform.position.x + deadzoneX){
			transform.position = new Vector3(player.transform.position.x - deadzoneX, transform.position.y, transform.position.z);
		}
		//Y Deadzone Camera Movement
		if(player.transform.position.y > this.transform.position.y + deadzoneY){
			transform.position = new Vector3(transform.position.x, player.transform.position.y - deadzoneY, transform.position.z);
		}
		else if(player.transform.position.y < transform.position.y - deadzoneY){
			transform.position = new Vector3(transform.position.x, player.transform.position.y + deadzoneY, transform.position.z);
		}

		/*
		//Functions for zooming in the camera at a linear rate for orthographic
		if(zoomedIn && Camera.main.orthographicSize > zoomedInOrthographicSize) {
			Camera.main.orthographicSize = (float)(Camera.main.orthographicSize - zoomInSpeedOrthographic);
		}
		else if(!zoomedIn && Camera.main.orthographicSize < zoomedOutOrthographicSize) {
			Camera.main.orthographicSize = (float)(Camera.main.orthographicSize + zoomOutSpeedOrthographic);
		}*/
		
		//Functions for zooming in camera for perspective
		/*
		if(zoomedIn && Camera.main.fov > zoomedInFOV) {
			Camera.main.fov = (float)(Camera.main.fov - zoomSpeedOrthographic);
		}
		else if(!zoomedIn && Camera.main.fov < zoomedOutFOV) {
			Camera.main.fov = (float)(Camera.main.fov + zoomSpeedOrthographic);
		}*/
		
		//Function for linear X & Y movement for the camera
		/*if(zoomedIn && transform.position != player.transform.position){
			Vector3 targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, -10);
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 3);
		}*/
	}

	public void SetCameraSizeTo(float size){
		Camera.main.orthographicSize = size;
	}

	public void TransitionCameraSizeTo(float size, float rate){
		StopAllCoroutines();
		StartCoroutine(TransitionCameraSize(size, rate));
	}

	IEnumerator TransitionCameraSize(float size, float rate) {
		if (Camera.main.orthographicSize > size){
			StartCoroutine(CenterCameraToPlayer());
			while (Camera.main.orthographicSize > size) {
				Camera.main.orthographicSize = (float)(Camera.main.orthographicSize - rate);
				yield return null;
			}
		}
		else if (Camera.main.orthographicSize < size) {
			while (Camera.main.orthographicSize < size) {
				Camera.main.orthographicSize = (float)(Camera.main.orthographicSize + rate);
				yield return null;
			}
		}	
	}


	IEnumerator CenterCameraToPlayer() {
		Vector3 targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, -10);
		while(transform.position != player.transform.position){
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 3);
			yield return null;
		}
	}


	public void zoomIn () {
		//zoomedIn = true;
		TransitionCameraSizeTo(zoomedInOrthographicSize, zoomInSpeedOrthographic);
	}

	public void zoomOut () {
		//zoomedIn = false;
		TransitionCameraSizeTo(zoomedOutOrthographicSize, zoomOutSpeedOrthographic);
	}

	public void CenterCamera() {
		//transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
		StartCoroutine(CenterCameraToPlayer());
	}

}
