using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IBossAI { //The interface for boss AIs

	float attackFrequency { set; }
	float attackStrength { set; }

}

public class BossNight : MonoBehaviour, IBossAI {

	public float teleportCooldown; //The time between the starts of teleport cycles
	public int bulletsPerTeleport; //The number of bullets spawned per teleport cycle
	public float bulletsArcWidth; //The number of degrees that the bullets spread over
	public float bulletSpeed; //The speed each bullet travels at
	public Vector3 bulletOffset;

	public Vector2 teleportMin; //the minimum corner for teleportation
	public Vector2 teleportMax; //Same, but maximum;

	public float fadeInTime; //The number of seconds that Night spends fading in
	public float fadeOutTime; //The number of seconds that Night spends fading out

	private float teleportHeat; //The time since the last teleport has started. Check if it's greater than the cooldown for starting a new one.
	private List<GameObject> bulletsThisTeleport = new List<GameObject> ();

	private ObjectPool bulletPool; //The attached pool that manages bullets.
	private Creature creature; //The attached creature script
	private SpriteRenderer render;
	private KilledBy[] killedBys;
	private string normalTag;

	public float attackFrequency {
		set {
			teleportCooldown = value;
		} 
	}

	public float attackStrength {
		set {
			bulletsPerTeleport = (int)value;
		} 
	}

	void Awake () {
	
		bulletPool = GetComponent<ObjectPool> ();
		creature = GetComponent<Creature> ();
		render = GetComponent<SpriteRenderer> ();
		killedBys = GetComponents<KilledBy> ();
		normalTag = gameObject.tag;
	
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		teleportHeat += Time.deltaTime;

		//ticks wave
		float fadeInAlpha = Mathf.Lerp(0, 1, teleportHeat/fadeInTime); //The alpha, according to the fade-in time, of the sprite (and bullets)
		float fadeOutAlpha = Mathf.Lerp(0, 1, (teleportCooldown-teleportHeat) / fadeInTime); //the alpha according to fadeout


		//Applies fadeInAlpha to bullets
		foreach (GameObject bullet in bulletsThisTeleport) {
			bullet.GetComponent<SpriteRenderer> ().color = new Color (255, 255, 255, fadeInAlpha);
		}
		//Applies alpha to self
		float appliedAlpha = fadeInAlpha;
		if (fadeInAlpha > fadeOutAlpha)
			appliedAlpha = fadeOutAlpha;
		render.color = new Color (render.color.r, render.color.g, render.color.b, appliedAlpha);

		//starts new cycle, if applicable
		if (teleportHeat > teleportCooldown) {
			startNewWave ();
		}

		//If Night is transparent, she can't be harmed by bullets.
		foreach (KilledBy killedBy in killedBys) {
			if (appliedAlpha < 1) {
				killedBy.isEnabled = false;
				gameObject.tag = "Untagged"; //Nothing can collide at all with Night
			} else {
				killedBy.isEnabled = true;
				gameObject.tag = normalTag;
			}
		}
		
	
	}

	void startNewWave () {

		teleportHeat = 0; //Resets counter

		bulletsThisTeleport = new List<GameObject> ();

		//chooses new position to teleport to
		System.Random rand = new System.Random ();
		float newX = (float)rand.NextDouble () * (teleportMax.x - teleportMin.x) + teleportMin.x;
		float newY = (float)rand.NextDouble () * (teleportMax.y - teleportMin.y) + teleportMin.y;
		transform.position = new Vector3 (newX, newY, transform.position.z);

		//spawns bullets
		float degreesPerBullet = bulletsArcWidth / bulletsPerTeleport;
		float startingAngle = -90f - (bulletsArcWidth / 2f);
		for (int i = 0; i < bulletsPerTeleport; i++) {
			float angle = (startingAngle + degreesPerBullet * i) * Mathf.PI / 180; //The angle that this bullet will the traveling, in radians
			GameObject bullet = bulletPool.nextObject (transform.position + bulletOffset);
			bulletsThisTeleport.Add (bullet);
			bullet.GetComponent<Rigidbody2D> ().velocity = new Vector2 (Mathf.Cos (angle) * bulletSpeed, Mathf.Sin (angle) * bulletSpeed); //Sets bullet's velocity
		}

	}
}
