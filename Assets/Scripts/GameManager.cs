using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public Dictionary<string, ObjectPool> monsterPools = new Dictionary<string, ObjectPool>();

	public string[] poolNameArray = new string[] {
		"initiatePool",
		"acolytePool",
		"priestPool",
		"bossPool"
	};

    public Dictionary<string, int> pointsDict = new Dictionary<string, int>
    {
        {"initiatePool", 100 },
        {"acolytePool", 200 },
        {"priestPool", 150 },
        {"bossPool", 1000 }
    };

	public GameObject[] bossArray; //A list of prefabs that represent the bosses for each level, in order.

	void Awake() {
		//sets up monsterPools
		ObjectPool[] pools = GetComponents<ObjectPool> ();
		for (int i = 0; i < pools.Length && i < poolNameArray.Length; i++) {
			monsterPools [poolNameArray [i]] = pools [i];
		}
		Debug.Log (monsterPools.Count);


		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}


		DontDestroyOnLoad (gameObject);
	}

	// Use this for initialization
	void Start () {
		loadLevelStats ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void loadLevelStats () {

		loadBoss ();

		//configures ending screen
		CanvasPointers.instance.dialogueText.text = LevelStats.currentLevel.dialogue;

		//Configures monsters
		foreach (KeyValuePair<string, ObjectPool> pair in monsterPools) {
			Creature creature = pair.Value.prefab.GetComponent<Creature> ();
			if (creature != null) {
				creature.maxHealth = LevelStats.currentLevel.difficultyDictionary [pair.Key] ["health"];
                creature.points = pointsDict[pair.Key] * (LevelStats.currentLevel.number + 1);
			}

			MonsterUtilSweep sweep = pair.Value.prefab.GetComponent<MonsterUtilSweep> ();
			if (sweep != null) {
				sweep.sweepCooldown = LevelStats.currentLevel.difficultyDictionary [pair.Key] ["sweepCooldown"];
				sweep.sweepBullets = (int)LevelStats.currentLevel.difficultyDictionary [pair.Key] ["sweepBullets"];
				pair.Value.prefab.GetComponent<ObjectPool> ().prefab.GetComponent<SpriteRenderer> ().color = LevelStats.currentLevel.bulletColor;
			}
		}
			
		//configures boss
		foreach (MonoBehaviour script in monsterPools ["bossPool"].prefab.GetComponents <MonoBehaviour> ()) {
			if (script is IBossAI) {
				(script as IBossAI).attackFrequency = LevelStats.currentLevel.difficultyDictionary ["bossPool"] ["attackFrequency"];
				(script as IBossAI).attackStrength = LevelStats.currentLevel.difficultyDictionary ["bossPool"] ["attackStrength"];
				break; //Break, because each boss should only have one IBossAI on it at a time. This is a check against 'broken' bosses.
			}
		}
			
	}

	public void OnLevelWasLoaded () {
		loadLevelStats ();

		//Clears bossPool's instances. Avoids repeating bosses.
		monsterPools["bossPool"].instances = new List <GameObject> ();
	}

	public void advaceLevel () { //I need to do it here because Unity can't see LevelStats.cs
		LevelStats.advance();
	}

	void loadBoss() { //Loads the boss for this level into bossArray's ObjectPool
		monsterPools["bossPool"].prefab = LevelStats.currentLevel.boss;
	}	
}