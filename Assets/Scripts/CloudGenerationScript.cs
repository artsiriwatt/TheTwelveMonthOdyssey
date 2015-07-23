using UnityEngine;
using System.Collections;

public class CloudGenerationScript : MonoBehaviour {

	public GameObject NWCloud;
	public GameObject NECloud;
	public GameObject SWCloud;
	public GameObject SECloud;

	// Use this for initialization
	void Start () {
		Vector3 nwpos = NWCloud.transform.position,
		swpos = SWCloud.transform.position,
		nepos = NECloud.transform.position;
		float i = nwpos.x, j = nwpos.y;
		while (j > swpos.y) {
			i = nwpos.x;
			while (i < nepos.x) {
				GameObject cloud = Instantiate (NWCloud, 
					new Vector3 (i, j + Random.Range(-.2f, .2f), nwpos.z), 
				    Quaternion.identity) as GameObject;
				cloud.transform.parent = transform;
				cloud.GetComponent<SpriteRenderer>().sortingOrder = Random.Range(0, 2);
				i = i + 1.0f + Random.Range(-.1f, .1f);
			}
			j = j - 1.6f;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
