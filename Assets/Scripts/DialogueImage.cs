using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogueImage : MonoBehaviour {

	public Color imageColor;
	public Color textColor;

	public float imageFadeIn; //How long, in seconds the image should take to fade in after the boss has died
	public float textFadeIn; //How long, in seconds, the text should take to fade in after the image is fully displayed
	public float buttonAppears; //How long after the text is faded in should the button appear

	public Image backgroundImage;
	public Text dialogueText;
	public Button continueButton;

	public float timeActivated; //The time that this object became active, and thusly began fading in

	void Awake () {


		backgroundImage.color = Color.clear;
		dialogueText.color = Color.clear;
		continueButton.gameObject.SetActive(false);

		dialogueText.text = LevelStats.currentLevel.dialogue;
	}

	// Use this for initialization
	void Start () {

		timeActivated = Time.time;

        //subscribes Gamemanager's advance function to the advance button. This is a weird place to do it, though.
        CanvasPointers.instance.continueButton.onClick.AddListener(() => { GameManager.instance.advaceLevel(); });
        Debug.Log("subscribing");

    }
	
	// Update is called once per frame
	void Update () {

        Debug.Log("attached to" + gameObject.name);
        backgroundImage.color = Color.Lerp (Color.clear, imageColor, (Time.time - timeActivated) / imageFadeIn);
		dialogueText.color = Color.Lerp (Color.clear, textColor, (Time.time - (timeActivated + textFadeIn)) / textFadeIn);

		if (Time.time > timeActivated + imageFadeIn + textFadeIn + buttonAppears) {
			continueButton.gameObject.SetActive(true);
		}
	
	}
}
