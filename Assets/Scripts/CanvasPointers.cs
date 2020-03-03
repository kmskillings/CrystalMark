using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CanvasPointers : MonoBehaviour { //Holds all the pointers for the canvas elements, so that other objects can look at them.

	public static CanvasPointers instance;

	public Slider bossHealthSlider;
	public Image dialogueImage;
	public Button continueButton;
	public Text dialogueText;

	public Image[] lives;

	public Image deathImage;

    public Text pointsText;

	// Use this for initialization
	void Awake () {

		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(this);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
