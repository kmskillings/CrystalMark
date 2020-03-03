using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPlayButton : MonoBehaviour {

	private Text buttonText;
	private Button button;

	// Use this for initialization
	void Awake () {
		buttonText = GetComponentInChildren<Text> ();
		button = GetComponent<Button> ();
	}

	void Start () {
	
		button.onClick.AddListener (() => {
			coinButtonClick ();
		});
	
	}

	public void coinButtonClick () {

		//Changes text of button, clears listeners, and adds playButtonClick
		buttonText.text = "Play";
		button.onClick.RemoveAllListeners ();
		button.onClick.AddListener (() => {
			playButtonClick ();
		});

	}

	public void playButtonClick () {
	
		//Loads new scene
		SceneManager.LoadScene("LevelCharSelect");
	
	}
}