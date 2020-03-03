using UnityEngine;
using System.Collections;
using System;

public class EggManager : MonoBehaviour {
	//The class that distributes and moves monsterEggs. it also talks to the monsters to manage posts, deaths, and so on. The current version is highly simplified.

	public static EggManager instance;

	public Vector2 spawnAreaMin; //the minimum corner of the spawning area to spawn eggs in at the start of the game.
	public Vector2 spawnAreaMax; //the maximum corner ^^
	public Vector2 postAreaMin; //The area that monsters will move to, where their 'post' will be
	public Vector2 postAreaMax;
	public float downSpeed; //the speed with which the eggs will scroll downwards
	public int eggsToSpawn; //How many eggs to spawn at the at the start of the game.
	private GameObject[] eggs;

	private MonsterRank[] monsterRanks;

	private bool onlyBossLeft = false; //Whether there's only one egg left to spawn (the boss egg)

	public GameObject eggPrefab; //The egg prefab to spawn. Must have an Egg.cs script attached to it.

	// Use this for initialization

	void Awake () {
		monsterRanks = GetComponents<MonsterRank> ();
		eggs = new GameObject[eggsToSpawn + 1];

		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
	}

	void Start () {
		eggsToSpawn = LevelStats.currentLevel.monsters;
		spawnEggs ();
		GetComponent<Rigidbody2D> ().velocity = new Vector2 (0f, downSpeed); //All eggs are parented to the eggManager, so by moving itself downwards, all the eggs follow.
	}
	
	// Update is called once per frame
	void Update () {

		if (onlyBossLeft){
			pause ();

			//Determines if no monsters are left
			bool ranksAreEmpty = true;
			foreach(MonsterRank rank in monsterRanks){
				foreach (GameObject monster in rank.monsterArray) {
					if(monster != null){
						ranksAreEmpty = false;
						break;
					}
				}
			}

			if (ranksAreEmpty) {
				unPause ();
			}
		}		

	}

	void spawnEggs() {
		//Spawns eggsToSpawn eggs at random places throughout the level
		System.Random rand = new System.Random ();
		for (int i = 0; i < eggsToSpawn; i++) {

			//Chooses the positions and post positions of the egg
			float b = spawnAreaMin.y; //The offset from 0 to the start of the difficult curve
			double upperBound = Math.Pow (spawnAreaMax.y - spawnAreaMin.y, 2); //The square of the distance between b and the top of the spawn area
			float yPos = (float)Math.Pow(rand.NextDouble () * upperBound, 0.5) + b; //xPos is the position of the new egg, with the difficulty curve accounted for.

			float xPos = (float)(rand.NextDouble () * (postAreaMax.x - postAreaMin.x) + postAreaMin.x);

			Vector2 eggPos = new Vector2 (xPos, yPos);


			//Vector2 postPos = new Vector2 ((float)rand.NextDouble () * (postAreaMax.x - postAreaMin.x) + postAreaMin.x, (float)rand.NextDouble () * (postAreaMax.y - postAreaMin.y) + postAreaMin.y);

			//Chooses the monster type
			string poolname;
			do {
				int poolIndex = rand.Next(GameManager.instance.poolNameArray.Length); //Some random index from poolNameArray
				poolname = GameManager.instance.poolNameArray [poolIndex]; //Finds the string name of that index
			} while (poolname == "bossPool"); //prevents bosses from being spawned initially. TODO: Make this not hacky

			//spawns the egg
			GameObject egg = (GameObject) Instantiate (eggPrefab, eggPos, Quaternion.identity);
			egg.transform.parent = transform;
			//egg.GetComponent<Egg> ().movementCenter = postPos;
			egg.GetComponent<Egg> ().poolName = poolname;
			eggs [i] = egg;
		}

		//Spawns a single egg at the end of the level that spawns the boss monster
		GameObject bossEgg = (GameObject) Instantiate (eggPrefab, new Vector2 (0, spawnAreaMax.y), Quaternion.identity);
		bossEgg.transform.parent = transform;
		bossEgg.GetComponent<Egg> ().poolName = "bossPool";
		eggs [eggsToSpawn] = bossEgg;
	}

	public Vector2 requestSpot (GameObject requester){

		string rankTag = requester.GetComponent<MonsterMovement> ().rankTag;
		Vector2 post = new Vector2 ();
		foreach (MonsterRank rank in monsterRanks) {
			if (rank.rankTag == rankTag) {
				post = rank.requestSpot (requester);
				break;
			}
		}

		/*if (!assignedYet)
			Destroy (requester);*/

		//Determins if there is only one egg left
		int eggCount = 0;
		foreach (GameObject egg in eggs) {
			if (egg != null)
				eggCount++;
		}
		Debug.Log ("Number of eggs detected: " + eggCount.ToString ());
		if (eggCount == 2)
			onlyBossLeft = true;


		return(post);

	}

	public bool throwOutOfRank(GameObject deadMonster){
		foreach (MonsterRank rank in GetComponents<MonsterRank> ()) {
			if(rank.throwOut(deadMonster))
				return(true);
		}

		return(false);
	
	}

	public static void pause () { //A function that stop the eggManager's scrolling, to avoid monsters spawning
		instance.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
	}

	public static void unPause () {
		instance.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, instance.downSpeed);
	}
}
