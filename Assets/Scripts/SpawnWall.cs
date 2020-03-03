using UnityEngine;
using System.Collections;

public class SpawnWall : MonoBehaviour, IRecyclable {

	public bool useMovementCenter; //Whether or not to spawn the wall according to the monster's movement center or its actual position
	public bool spawnWhileRushing; //Whether or not to spawn walls while the monster is rushing
	public Vector3 offset;
	public GameObject wallPrefab;
	public float destructionCooldown; //How long the monster has to wait before spawning another wall after the last one has been destroyed
	public float initialDelay; //How long to wait between the spawning of the monster and the spawning of the first wall

	public bool effectMonster; //Wether to do the color-effect on the spawning monster
	public bool effectWall; //Wether to do the color-effect on the spawned wall
	public float fadeOutTime; //The time that the color lasts
	public Color effectColor;

	private GameObject wall; //The wall that has been spawned
	private ObjectPool wallPool;
	public float destructionHeat = 0;
	private float fadeOutCountdown = 0;
	private bool wallDestructionAcknowledged; //Wether or not the monster has noticed that its wall is destroyed

	private SpriteRenderer render;

	void Awake () {

		render = GetComponent<SpriteRenderer> ();
		wallPool = GetComponent<ObjectPool> ();
		destructionHeat = initialDelay;

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (fadeOutCountdown > 0 && effectMonster) {
			render.color = effectColor + (Color.white * (fadeOutTime - fadeOutCountdown) / fadeOutCountdown); //makes the sprite's color scale as fadeOutCountdown wears down

			fadeOutCountdown -= Time.deltaTime; //Ticks at the end of the function. This is important.
		}

		if (fadeOutCountdown > 0 && effectWall) {
			wall.GetComponent<SpriteRenderer> ().color = effectColor + (Color.white * (fadeOutTime - fadeOutCountdown) / fadeOutCountdown);

		}


		bool rushing = false;
		if (!spawnWhileRushing) {
			rushing = GetComponent<MonsterMovement> ().rushing;
		}
		if ((wall == null || !wall.activeSelf) && !rushing) { //If the wall is destroyed...
			destructionHeat -= Time.deltaTime;
			if (!wallDestructionAcknowledged) { //If the monster hasn't noticed...
				destructionHeat = destructionCooldown; //Que up a new wall
				wallDestructionAcknowledged = true;
			} else {
				if (destructionHeat <= 0) { //If it has noticed and the destructionheat has ran out...
					wallDestructionAcknowledged = false;
					fadeOutCountdown = fadeOutTime;
					if (useMovementCenter)
						wall = wallPool.nextObject (MotionUtils.unflattenVector(GetComponent<MonsterMovement> ().movementCenter) + offset);
					else
						wall = wallPool.nextObject (transform.position + offset);
				}
			}
			
		}
	}

	public void activate(){
		wall = null;
		destructionHeat = 0;
		fadeOutCountdown = 0;
	}

	public void shutdown(){
	}
}
