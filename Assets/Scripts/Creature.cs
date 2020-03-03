using UnityEngine;
using System.Collections;

public class Creature : MonoBehaviour, IRecyclable {

	public float maxHealth = 1f; //How much health this object has
	public float fadeOut = 0.5f; //How long the fade-out of the damage effect takes.
	public bool enableEffect;
    public int points;

	//public float fadeOutCountdown; //How long the fade-out has left to go
	public float health; //How much health is left until this object dies

	public delegate void delegateDie ();
	public event delegateDie eventDie;

	public Color damageColor; //The color to use in the damage effect

	private SpriteRenderer render;

	private bool fading  = false; //Whether or not the creature is currently in its color fade
	private float timeDamaged; //The time that the creature was first damaged on. Used to calculate color fade

	void Awake () {
		render = GetComponent<SpriteRenderer> ();
		//finds any IRecyclabes and attaches their shutdowns to eventDie
		foreach (MonoBehaviour script in GetComponents<MonoBehaviour> ()) {
			if (script is IRecyclable)
				eventDie += (script as IRecyclable).shutdown;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (enableEffect && fading) {
			render.color = Color.Lerp (damageColor, Color.white, (Time.time - timeDamaged) / fadeOut); 
		}
	
	}

	public void damage(float amount){
		health -= amount;
		if (health <= 0) {
			if (eventDie != null)
				eventDie ();
			gameObject.SetActive (false); //Shuts down the object
            Player.instance.addPoints(points);
			return; //Exit the function, so the 'staying alive' code isn't run
		}

		//start the damage effect
		if (enableEffect) {
			fading = true;
			timeDamaged = Time.time;
		}
		
	}

	public void activate() {
		health = maxHealth;
	}

	public void shutdown() {
	}
}
