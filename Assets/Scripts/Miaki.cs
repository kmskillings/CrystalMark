using UnityEngine;
using System.Collections;

public class Miaki : MonoBehaviour, ICharacter {

	public float arrowCooldown = 0.5f; //seconds between an arrow and the next arrow
	public GameObject arrowPrefab; //The prefab of the arrow to be fired
	public Vector3 arrowOffset; //Where the arrow will spawn in relation to Miaki
	public Vector3 bowOffset; //Where the bow will be placed in relation to Miaki
	public bool autoAlignBow; //Whether or not the bow's alignment should be checked each tick
	public Vector3 bowOffsetTolerance; //The margin of error for bow alignment
	public float bowRotationOffset = -90; //How far from 0 the bow should be rotated when it's 'facing' 0
	public float bowDefaultAngle = 90;

	public Color heartColor {
		get {
			return _heartColor;
		}
	}

	private Color _heartColor = new Color (1, 1, 0, 1);

	private GameObject target; //The target that the bow aims at and that arrows home after

	public bool parentArrows = true; //Wheter or not arrows should be created as a child of Miaki's gameobject

	private float arrowHeat = 0; //Number of seconds until the next arrow can be fired

	private GameObject bow;
	public float bowRotation;

	private ObjectPool arrowPool;

	void Awake () {
		arrowPool = GetComponent<ObjectPool> ();
		if (transform.childCount > 0)
			bow = transform.GetChild (0).gameObject;
	}
	
	void Start () {
		Transform bow = transform.GetChild (0);
		bow.localPosition = bowOffset;
	}

	// Update is called once per frame
	void Update () {

		Transform bow = transform.GetChild (0);
		if (bow.localPosition != bowOffset)
			bow.localPosition = bowOffset;

		//ticks heat counters
		float delta = Time.deltaTime;
		arrowHeat -= delta;

		//acquires nearest target, gets angle to it, and rotates bow accordingly. This angle is also used to fire arrows
		GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
		foreach (GameObject monster in monsters) {
			if ((target == null || target.transform.position.y < transform.position.y) || //If there is no valid target currently (target is null OR behind player) OR
                ((monster.transform.position - transform.position).magnitude < (target.transform.position - transform.position).magnitude && 
                monster.transform.position.y > transform.position.y)) { //A closer AND in front of target is found
				target = monster;
			}
		}
		if ((target != null && !target.activeSelf) || (target != null && target.transform.position.y < transform.position.y))
			target = null;
		//By now, target is either a monster or null.
		float angleToMonster = bowDefaultAngle;
		if(target != null)
			angleToMonster = MotionUtils.getAngleTo(transform.position, target.transform.position);

		bowRotation = angleToMonster;
		bowRotation = Mathf.Clamp (bowRotation, 0, 180);
		bow.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, bowRotation + bowRotationOffset));

	}

	public void onFireKey () {

		bool canFire = true; //Assumes Miaki can fire. Checks if she can't.
		//if the arrow heat is too high, she can't fire.
		if (arrowHeat > 0) {
			canFire = false;
		}

		//fires arrow, if possible
		if (canFire) {
			GameObject arrow;
			if (arrowPool == null) { //An object pool doesn't have to be connected to Miaki, but she supports it. Plus, it's highly reccommended.
				arrow = Instantiate (arrowPrefab);
				arrow.transform.position = transform.position + arrowOffset;
				arrow.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, bowRotation));
			} else {
				arrow = arrowPool.nextObject (transform.position + arrowOffset, Quaternion.Euler( new Vector3 (0, 0, bowRotation)));
			}

			if(parentArrows)
				arrow.transform.parent = transform;
			arrow.GetComponent<MiakiArrow>().targetInstance = target;

			arrowHeat = arrowCooldown;
		}
	}
}