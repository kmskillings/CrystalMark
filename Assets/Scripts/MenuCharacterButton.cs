using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuCharacterButton : MonoBehaviour {

	public Image bioImage; //The image object that bioSprite will be placed on
	public Sprite bioSprite; //The sprite that will be placed on bioImage

	public Text bioText; //The text object that bioString will be written to.
	public string bioString; //The string that will be written to bioText

	public Text nameText;
	public string nameString;

	private Button button;

	void Awake () {
	
		button = GetComponent<Button> ();
	
	}

	// Use this for initialization
	void Start () {

		button.onClick.AddListener (() => {
			SceneManager.LoadScene ("Level0");
		});
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void loadBio () {
	
		bioImage.sprite = bioSprite;
		bioImage.color = Color.white;
		nameText.text = nameString;
		bioText.text = bioString;
	
	}
}
