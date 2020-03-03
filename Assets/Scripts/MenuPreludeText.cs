using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPreludeText : MonoBehaviour {


    public static string[] texts =
    {
        "Level 1 \n\n WASD keys to move \n Space key to fire \n Shift key to focus \n\n Good luck",
        "Level 2 \n Prejudice \n\n Every weakling has a place \n beneath the superiority of the gods.",
        "Level 3 \n Void \n\n Emptiness is the purest absolute\nAll things are subject to its power.",
        "Level 4 \n Night \n\n Sins to ugly to be commited by day \n become all the more beautiful by night."
    };

    public Text text;
    public Image image;

    public float fadeInTime;
    public float fadeOutTime;
    public float totalTime;

    public Color fullColor;

    private float timeSinceLevelLoad = 0; //How long ago the level was loaded
    
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        timeSinceLevelLoad += Time.deltaTime;
        Color appliedImageColor = Color.Lerp(Color.clear, fullColor, timeSinceLevelLoad / fadeInTime);
        if (timeSinceLevelLoad > totalTime - fadeOutTime)
            appliedImageColor = Color.Lerp(Color.clear, fullColor, (totalTime - timeSinceLevelLoad / fadeOutTime));
        Color appliedTextColor = Color.Lerp(Color.clear, Color.black, timeSinceLevelLoad / fadeInTime);
        if (timeSinceLevelLoad > totalTime - fadeOutTime)
            appliedTextColor = Color.Lerp(Color.clear, Color.black, (totalTime - timeSinceLevelLoad / fadeOutTime));
        text.color = appliedTextColor;
        image.color = appliedImageColor;
		
	}

    void OnLevelWasLoaded()
    {

        text.text = texts[LevelStats.currentLevel.number];
        timeSinceLevelLoad = 0;

    }
}
