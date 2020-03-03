using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MonsterRank : MonoBehaviour {

	public Vector2 size;
	public string rankTag; //used for rejecting/accepting monsters of certain types

	public Vector2 postSize; //The size of each cell, or post
	public Vector2 position; //the position of the top left corner of the rank

	public GameObject[,] monsterArray; //An array full of the objects that the rank contains.

	void Awake () {
		monsterArray = new GameObject[(int)size.x, (int)size.y];
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	public bool throwOut(GameObject deadMonster){ //Throws out a dead (or otherwise) monster from the rank system. Called by EggManager who is called by a monster's shutdown function.
		//enters loop to check/remove deadMonster
		for (int i = 0; i < monsterArray.GetLength (0); i++) {
			for (int j = 0; j < monsterArray.GetLength (1); j++) {
				if (monsterArray [i, j] == deadMonster) {
					monsterArray [i, j] = null;
					return(true);
				}
			}
		}

		return(false);

	}	

	public Vector2 requestSpot(GameObject requester){ //The requester is guarenteed to be 'compatible' with the rank, so don't worry about it. Returns false if there's no room, a pos if there is.

		//get a list of all available monsterArray spots
		List<int[]> availableSpots = new List<int[]> ();
		for (int i = 0; i < monsterArray.GetLength (0); i++) {
			for (int j = 0; j < monsterArray.GetLength (1); j++) {
				if (monsterArray [i, j] == null || !monsterArray [i, j].activeSelf) {
					
					availableSpots.Add (new int[] { i, j });
				}
			}
		}
			
		//selects a random spot
		System.Random rand = new System.Random ();
		int randIndex = rand.Next (0, availableSpots.Count);
		int[] chosenPost;
		if (availableSpots.Count == 0){
			//Fire the oldest member of the rank
			float highestFoundAge = Time.time; //Counterintuitively, the lower the age of the monster, the older it is. this is important.
			MonsterMovement oldestFoundMonster = monsterArray[0, 0].GetComponent<MonsterMovement> ();
			int[] oldestMonsterPost = {0, 0};
			for (int i = 0; i < monsterArray.GetLength (0); i++) {
				for (int j = 0; j < monsterArray.GetLength (1); j++) {
					MonsterMovement monsterMovement = monsterArray [i, j].GetComponent<MonsterMovement> ();
					if (monsterMovement.age < highestFoundAge) {
						oldestFoundMonster = monsterMovement;
						highestFoundAge = monsterMovement.age;
						oldestMonsterPost = new int[] {i, j};
					}
				}
			}
			//oldestFoundMonster is now the oldest monster in the rank, so fire it.
			oldestFoundMonster.exitScreen();
			chosenPost = oldestMonsterPost; //The new post is that of the oldest found monster.

		} else 
			chosenPost = availableSpots[randIndex];

		//calculates world coords position of post
		Vector2 post = new Vector2(position.x + chosenPost[0] * postSize.x, position.y + chosenPost[1] * postSize.y);

		//finishes up
		monsterArray[chosenPost[0], chosenPost[1]] = requester;
		return(post);


	}
}
