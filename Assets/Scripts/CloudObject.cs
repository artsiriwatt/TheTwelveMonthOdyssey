using UnityEngine;
using System.Collections;

public class CloudObject : StaticObject {

	public bool moveAround;

	private Vector3 targetPosition;
	private float originalXPos;
	private float originalYPos;

	new void Start(){
		base.Start();
		originalXPos = transform.position.x;
		originalYPos = transform.position.y;

		if (moveAround){
			RandomlyMove();
		}
	}

	public void KillCloud () {
		Destroy(this.gameObject);
	}

	public void RandomlyMove() {
		float ranX = Random.Range(-0.5f, 0.5f);
		float ranY = Random.Range(-0.5f, 0.5f);
		targetPosition = new Vector3(originalXPos + ranX, originalYPos + ranY, 0);

		StartCoroutine("MoveToTarget");
	}

	IEnumerator MoveToTarget() {
		while (transform.position != targetPosition){
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 0.5f);
			yield return new WaitForSeconds(1f + Random.Range(0f, 3.0f));
		}

		RandomlyMove();
	}
}
