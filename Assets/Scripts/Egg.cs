using UnityEngine;
using System.Collections;

public class Egg : MonoBehaviour {

	public static float eggHatchThreshold = 25f; //The y value at which all eggs should hatch below. This needs to be the same for all eggs, so it's static.
	public string poolName = "initiatePool";

	public Vector2 movementCenter; //THIS IS A PLACEHOLDER. In the future, the ranking system will be used instead.

	private ObjectPool monsterPool; //the pool that stores spawned monsters

	// Use this for initialization
	void Start () {

		if (GameManager.instance == null)
			Debug.Log ("GameManager instance is null");
		Debug.Log ("Spawning egg with pool: " + poolName);
		monsterPool = GameManager.instance.monsterPools [poolName];
	
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y <= eggHatchThreshold) {
			GameObject monster = monsterPool.nextObject (transform.position);
			//monster.GetComponent<MonsterMovement> ().movementCenter = movementCenter;
			Destroy (gameObject);
		}
	}
}