using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class KilledBy : MonoBehaviour {
	//Make this object 'die' on contact with a specific tag. Note that the other party does not die inherently.

	public string tagKilledBy; //The tag that this object is killed by.
	public bool useCreature; //Whether or not to try to find a Creature script to apply damage to. If none can be found, proceed normally.
	public float damage; //How much damage this takes from a tagged object

	public bool isEnabled = true; //If false, disables the target being deactivated on contact with tagKilledBy
	public bool enableDeath = true;

	public bool explode; //if true, spawns an explosion at time of death.
	public GameObject explosion;
	public Vector3 explosionScale;

	public delegate void delegateDie ();
	public event delegateDie eventDie;

    public int points; //How many points the player earns for this dying. can be negative, I guess.

	private Creature creature;

	void Awake () {
		if (useCreature) {
			creature = GetComponent<Creature> (); //Tries to find a Creature object
			if (creature == null)
				useCreature = false; //If none can be found, doesn't try to use creature
			else {
				if (explode) {
					creature.eventDie += (() => {
						GameObject go = (GameObject)Instantiate (explosion, transform.position, Quaternion.identity);
						go.transform.localScale = explosionScale;
					});
				}
			}
		}

		if (!useCreature) { //Gets any IRecyclables attached to this object and attaches them to eventDie (if no Creature can be found)
			//If a creature can be found, it will assemble an event of death functions and use that in its private die() function.
			foreach (MonoBehaviour script in GetComponents<MonoBehaviour> ()) { 
				if (script is IRecyclable)
					eventDie += (script as IRecyclable).shutdown;
			}
		}
	}

	void OnTriggerEnter2D (Collider2D collider) {

		if (!isEnabled)
			return;

		GameObject otherObject = collider.gameObject; //The other party in this collision
		if (otherObject.tag == tagKilledBy) {

			//kills  or damages this object
			if (useCreature) {
				creature.damage (damage);
			} else {
				if (explode) {
					GameObject go = (GameObject)Instantiate (explosion, transform.position, Quaternion.identity);
					go.transform.localScale = explosionScale;
				}
				if (enableDeath) {
					gameObject.SetActive (false);
                    //adds points
                    Player.instance.addPoints(points);
				}
				if (eventDie != null)
					eventDie ();
			}
		}
	}
}