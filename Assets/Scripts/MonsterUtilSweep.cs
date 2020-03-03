using UnityEngine;
using System.Collections;

public class MonsterUtilSweep : MonoBehaviour {
	// A MonsterUtilSweep represents a 'sweeping' behavior exhibited by most small monsters. 
	//A Monster may have any number of these attached, but these must be attached to a monster.
	//Sweeps are handled entirely from the 'inside.' The Monster script never has to worry about a sweep.

	public float sweepAngle; //The total angle, in degrees, of the sweep
	public int sweepBullets; //The number of bullets in the sweep
	public float sweepTime; //The time, in seconds, that the sweep takes to complete
	public float sweepCooldown; //The time between the beginning of one sweep and the beginning of the next.
	public float bulletSpeed;
	public ObjectPool bulletPool; //The pool that manages bullets
	public float targetingOffset; //An offset, in degrees, towards which this sweep will fire
	public bool sweepWhileRushing;
    public Vector3 offset;

	private bool sweeping; //Whether or not a sweep is in progress
	private float sweepHeat;
	private float bulletHeat;
	private int bulletsSoFar;
	private float maxAngle; //Counterintuitively, the is the angle the sweep BEGINS at.

	void Awake () {
		bulletPool = GetComponent<ObjectPool> ();
		bulletHeat = 0;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		bool rushing = false;
        if (!sweepWhileRushing)
        {
            if (GetComponent<MonsterMovement>() != null)
                rushing = GetComponent<MonsterMovement>().rushing;
            else
                rushing = false;
        }
        
            

		if(Player.instance.gameObject.activeSelf) //Only advance sweep if player is still alive
			sweepHeat -= Time.deltaTime;

		if (sweeping) { 
			if (bulletHeat > 0) { //If the bullet heat is still cooling down
				bulletHeat -= Time.deltaTime;
			} else { //Bullet done cooling, so fire again
				bulletHeat = sweepTime / sweepBullets; //resets heat
				bulletsSoFar++;
				if (bulletsSoFar >= sweepBullets)
					sweeping = false; //Stops sweep, if necessary
				//fires bullet
				float bulletAngle = (maxAngle - (bulletsSoFar * sweepAngle / sweepBullets)) * Mathf.PI / 180;
                GameObject bullet = bulletPool.nextObject(transform.position + offset);
				bullet.GetComponent<Rigidbody2D> ().velocity = new Vector2 (bulletSpeed * Mathf.Cos (bulletAngle), bulletSpeed * Mathf.Sin (bulletAngle));
			}
			
		} else {
			if (sweepHeat <= 0 && !rushing) {
				//starts sweep
				startSweep(Player.instance.transform.position);
				//sets up for next sweep
				sweepHeat = sweepCooldown;
			}
		}
	}

	void startSweep (Vector3 targetCoords){
		sweeping = true;
		bulletsSoFar = 0;
		bulletHeat = 0f;

		//finds angle to target in degrees
		float angleTarget = MotionUtils.getAngleTo(transform.position, targetCoords);
		//Uses that to find the angle the sweep starts at in degrees
		maxAngle = angleTarget + (sweepBullets/2) * (sweepAngle / sweepBullets) + targetingOffset;
	}
}
