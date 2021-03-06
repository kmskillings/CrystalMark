using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LevelStats {
	//A level is a collection of stuff that changes from level to level, including bosses, difficulty, and script.

	public static float levelLength = 180; //The number of seconds each level lasts

	public static LevelStats currentLevel {
		get{return(levels[currentLevelNumber]);}
	}

	private static Dictionary<string, Dictionary<string, float>> level1Difficulty;
    private static Dictionary<string, Dictionary<string, float>> level2Difficulty;
    private static Dictionary<string, Dictionary<string, float>> level3Difficulty;
    private static Dictionary<string, Dictionary<string, float>> level4Difficulty;

	protected static int lastGeneratedLevel = 0; //The number that the next generated level will be
	static int currentLevelNumber = 0; //The number of the level that is currently being played

	private static LevelStats[] levels;

	public int number; //the number of the level, starting at 1 and going to 4
	public GameObject boss; //the boss prefab that should spawn at the end of the level
	public Dictionary<string, Dictionary<string, float>> difficultyDictionary; //A dictionary containing a string (ex. acolytePool) and a dict of ints representing stats for the specified monster.
	public string dialogue; //The one or two lines of dialogue to display at the end of the level.
	public int monsters; //The number of monsters to spawn during the level
	public Color bulletColor; //the color of monster bullets

	static LevelStats() {

        DifficultySettings settings = JsonUtility.FromJson<DifficultySettings>(File.ReadAllText("config.txt"));
        Debug.Log(settings.level1monsters);

       level1Difficulty = new Dictionary<string, Dictionary<string, float>>{
            {"initiatePool", new Dictionary<string, float> {
                    { "health", settings.level1initiateHealth },
                    { "sweepCooldown", settings.level1initiateHealth },
                    { "sweepBullets", settings.level1initiateBullets }
                }
            }, {"acolytePool", new Dictionary<string, float>{
                    { "health", settings.level1acolyteHealth },
                    { "sweepCooldown", settings.level1acolyteCooldown },
                    { "sweepBullets", settings.level1acolyteHealth }
                }
            }, {"priestPool", new Dictionary<string, float>{
                    { "health", settings.level1priestHealth }
                }
            }, {"bossPool", new Dictionary<string, float>{
                    { "health", settings.level1bossHealth },
                    { "attackFrequency", settings.level1bossFrequency },
                    { "attackStrength", settings.level1bossStrength }
                }
            }
        };

        level2Difficulty = new Dictionary<string, Dictionary<string, float>>{
            {"initiatePool", new Dictionary<string, float> {
                    { "health", settings.level2initiateHealth },
                    { "sweepCooldown", settings.level2initiateHealth },
                    { "sweepBullets", settings.level2initiateBullets }
                }
            }, {"acolytePool", new Dictionary<string, float>{
                    { "health", settings.level2acolyteHealth },
                    { "sweepCooldown", settings.level2acolyteCooldown },
                    { "sweepBullets", settings.level2acolyteHealth }
                }
            }, {"priestPool", new Dictionary<string, float>{
                    { "health", settings.level2priestHealth }
                }
            }, {"bossPool", new Dictionary<string, float>{
                    { "health", settings.level2bossHealth },
                    { "attackFrequency", settings.level2bossFrequency },
                    { "attackStrength", settings.level2bossStrength }
                }
            }
        };

        level3Difficulty = new Dictionary<string, Dictionary<string, float>>{
            {"initiatePool", new Dictionary<string, float> {
                    { "health", settings.level3initiateHealth },
                    { "sweepCooldown", settings.level3initiateHealth },
                    { "sweepBullets", settings.level3initiateBullets }
                }
            }, {"acolytePool", new Dictionary<string, float>{
                    { "health", settings.level3acolyteHealth },
                    { "sweepCooldown", settings.level3acolyteCooldown },
                    { "sweepBullets", settings.level3acolyteHealth }
                }
            }, {"priestPool", new Dictionary<string, float>{
                    { "health", settings.level3priestHealth }
                }
            }, {"bossPool", new Dictionary<string, float>{
                    { "health", settings.level3bossHealth },
                    { "attackFrequency", settings.level3bossFrequency },
                    { "attackStrength", settings.level3bossStrength }
                }
            }
        };

        level4Difficulty = new Dictionary<string, Dictionary<string, float>>{
            {"initiatePool", new Dictionary<string, float> {
                    { "health", settings.level4initiateHealth },
                    { "sweepCooldown", settings.level4initiateHealth },
                    { "sweepBullets", settings.level4initiateBullets }
                }
            }, {"acolytePool", new Dictionary<string, float>{
                    { "health", settings.level4acolyteHealth },
                    { "sweepCooldown", settings.level4acolyteCooldown },
                    { "sweepBullets", settings.level4acolyteHealth }
                }
            }, {"priestPool", new Dictionary<string, float>{
                    { "health", settings.level4priestHealth }
                }
            }, {"bossPool", new Dictionary<string, float>{
                    { "health", settings.level4bossHealth },
                    { "attackFrequency", settings.level4bossFrequency },
                    { "attackStrength", settings.level4bossStrength }
                }
            }
        };

        levels = new LevelStats[] {
		
			new LevelStats(level1Difficulty, "Stage 1 - Chaos, Level of Disorder and Ruination \n\n\n Around me lie the cries of the fallen, \n\n a cacophony of voices, lost in this chaotic hell. \n\n Yes, there is discord, but the whispers that fill this earth  \n\n seem only all to familiar, somehow...", settings.level1monsters, Color.red),
			new LevelStats(level2Difficulty, "Stage 2 - Prejudice, Level of Ignorance and Enmity \n\n\n The suffering of others,\n\n the pain of existence,\n\n is that what brought you here?\n\n Is that what brought me?\n\n This needless pain and hatred sows and reaps guilt in the heart, \n\nyet, we continue this cycle of pain anyways...", settings.level2monsters, Color.green),
			new LevelStats(level3Difficulty, "Stage 3 - Void, Level of Silence and Nothingness \n\n\n The lonely emptiness is surreal... \n\nAnd you, are you the one who is empty? \n\nI will show you that we are never alone... \n\nFor better or worse, there is always someone with us...", settings.level3monsters, Color.magenta),
			new LevelStats(level4Difficulty, "Stage 4 - Night, Level of Darkness and Fear \n\n\n The night grows strong... \n\n But I will grow stronger \n\nand shine my light into the darkness!", settings.level4monsters, Color.blue)

		};
	}

	public static void advance() {

		currentLevelNumber++;
		if (currentLevelNumber >= levels.Length) {
			SceneManager.LoadScene ("LevelCredits");
			currentLevelNumber = 0;
			return;
		}
		SceneManager.LoadScene ("Level0");
		Debug.Log ("Current level is " + currentLevelNumber.ToString ());

	}

	LevelStats(Dictionary<string, Dictionary<string, float>> difficultyDictionary, string dialogue, int monsters, Color bulletColor){

		number = lastGeneratedLevel;

		this.boss = GameManager.instance.bossArray [lastGeneratedLevel]; //Assigns the boss for this level to the respective boss in Gamemanager's BossArray
		this.difficultyDictionary = difficultyDictionary;
		this.dialogue = dialogue;
		this.monsters = monsters;
		this.bulletColor = bulletColor;

		lastGeneratedLevel++;
	
	}

}