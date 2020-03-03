using UnityEngine;
using System.Collections;

public class MonsterMovement : MonoBehaviour, IRecyclable {

	public Vector2 movementCenter; //The point that the monster moves towards/around
	public float movementRadius; //The radius of the circle
	public float speed; //The speed with which the monster moves
	public float rushFactor; //By how much it should rush

	public static float antiBugRadius = 0.5f; //How far away from the the current positoin the new destination should be to avoid the monster flying into the void

	public string rankTag; //What rank the monster should request a spot from

	public float age;

	private Rigidbody2D body2d;
	public bool firstTime = true;
	private bool ignoreOutsideCircle = true; //Whether the fact that the monster is outside their movement circle should be ignored

	public bool rushing {
		get { return body2d.velocity.magnitude > speed; }
	}

	void Awake () {
		body2d = GetComponent<Rigidbody2D> ();

	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//checks to see if the monster is inside the movement circle
		bool insideCircle = getInsideCircle();
		//acts accordingly
		if (insideCircle && ignoreOutsideCircle) {
			ignoreOutsideCircle = false;
		} else if (!insideCircle && !ignoreOutsideCircle) {
			//resets destination
			setDestination();
		}
	}

	void setDestination(){ //Chooses a destination and sets velocity accordingly.
		//Checks to see if it's the monster's first time setting a destination
		Vector2 destination;
		if (firstTime) {
			//sets the destination to movementCenter.
			destination = movementCenter;
		} else { //If it's not the first time...
			//This do-while loop generates new destinations until a 'good' one is found.
			do {
				destination = Random.insideUnitCircle.normalized * movementRadius + movementCenter; //Destination is now a point on the unit circle surrounding movementCenter.
			} while ((destination - MotionUtils.flattenVector(transform.position)).magnitude < antiBugRadius);
		}

		//Destination is now set, so it's time to set velocity towards the destination
		//gets normalized vector towards destination
		Vector2 destinationDirection = (MotionUtils.flattenVector(transform.position) - destination).normalized;
		//velocity is similar to destinationDirection.
		body2d.velocity = -destinationDirection * speed;

		if (firstTime) {
			body2d.velocity *= rushFactor;
			firstTime = false;
		}

		ignoreOutsideCircle = true;

	}

	bool getInsideCircle() { //returns of the monster is inside their movement circle
		return (MotionUtils.flattenVector(transform.position) - movementCenter).magnitude < movementRadius;
	}

	public void exitScreen() { //Takes the monster off the screen by setting its movementCenter offscreen.
		//Removes it from its rank
		EggManager.instance.throwOutOfRank (gameObject);
		//Gets the sign of its x-position
		int sign = (transform.position.x < 0 ? -1 : 1);
		movementCenter = new Vector2 (Camera.main.ViewportToWorldPoint(new Vector3 (sign, 0, 0)).x, transform.position.y);
		firstTime = true;
		setDestination ();
		
	}

	public void activate(){
		firstTime = true;
		ignoreOutsideCircle = true;

		movementCenter = EggManager.instance.requestSpot (gameObject);
		setDestination ();

		age = Time.time;
	}

	public void shutdown(){
		bool gotThrownOut;
		gotThrownOut = EggManager.instance.throwOutOfRank (gameObject);
	}
}