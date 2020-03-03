using UnityEngine;
using System.Collections;

public class DeactivateOffscreen : MonoBehaviour {

	public delegate void delegateDeactivate ();
	public event delegateDeactivate eventDeactivate; //Event containing all the functions to run when this shuts down. Currently only IRecyclables are supported.

	private Rigidbody2D body2d;

	void Awake () {
		body2d = GetComponent<Rigidbody2D> ();
	}

	// Use this for initialization
	void Start () {
		//Gets all the IRecyclables on this object and attaches them to eventDeactivate
		foreach (MonoBehaviour script in GetComponents<MonoBehaviour> ()) {
			if (script is IRecyclable)
				eventDeactivate += (script as IRecyclable).shutdown;
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (getOffscreen()) {
			gameObject.SetActive (false);
			if (eventDeactivate != null)
				eventDeactivate ();
		}

	}

	private bool getOffscreen () {
		//checks if the object is off the screen
		//gets the viewport point of the object, centered on the center of the screen instead of the corner
		Vector3 viewportPoint = (Camera.main.WorldToViewportPoint (transform.position) - new Vector3 (0.5f, 0.5f, 0.5f)); 

		if (Mathf.Abs (viewportPoint.x) > 0.5) { //If the viewport's x value is more than 0.5, meaning it's offscreen in the x direction.
			//if the x component of the velocity is the same sign as its x value
			if (viewportPoint.x * body2d.velocity.x > 0)
				return(true);
		
		}

		//Same, except for the y component
		if (Mathf.Abs (viewportPoint.y) > 0.5) { //If the viewport's x value is more than 0.5, meaning it's offscreen in the x direction.
			//if the x component of the velocity is the same sign as its x value
			if (viewportPoint.y * body2d.velocity.y > 0)
				return(true);

		}

		return false;
	}
}