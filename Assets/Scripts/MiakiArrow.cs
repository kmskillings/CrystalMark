using UnityEngine;
using System.Collections;

public class MiakiArrow : MonoBehaviour, IRecyclable {

	public float initialSpeed = 0f; //The hypotenuse of its velocity on launch
	public string targetTag = "Monster";
	public float steeringDegrees = 1f;

	public GameObject targetInstance;

	private Rigidbody2D body2d;

	void Awake () {
		body2d = GetComponent<Rigidbody2D> ();
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (targetInstance == null || !targetInstance.activeSelf) {
			// nothing to steer towards, so don't steer
		} else
			MotionUtils.steerTowards (gameObject, targetInstance.transform, steeringDegrees);

	}

	public void shutdown(){
	}

	public void activate(){
		//looks at current rotation and sets velocity. For this to work, activate() must be called AFTER rotation has been set.
		float facingAngle = Mathf.PI / 180 * transform.rotation.eulerAngles.z;
		body2d.velocity = new Vector2 (initialSpeed * Mathf.Cos (facingAngle), initialSpeed * Mathf.Sin (facingAngle));

		//targetInstance is now set by Miake when she fires the arrow, so don't worry about it here.
	}
}