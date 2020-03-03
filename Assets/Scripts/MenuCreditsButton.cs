using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuCreditsButton : MonoBehaviour {

	public Text creditText;
	private string[] creditSlides = new string[] {
        "But wait... \n\n What will I find in this darkness? \n\n Is this what I really want?",
        "What if... \n\n what if I'm afraid of what I might find? \n\n What if the monsters aren't outside of me, \n\n but inside?",
        "What if... \n\nthey can't really be defeated, \n\nwill the beauty of light forever \n\nbe tainted with the hatred of darkness?",
        "No...",
        "These battles, they have led up to this moment. \n\nEven as the shadows of our sins crawl around us, \n\nlet it be known there is still light \n\neven in the darkest of nights!",
        "Created by: \n Michael Skillings \n Chris Fang \n Karisa Holmberg \n \n Special Thanks to: \n Steven Brown \n Richard Collette \n Unity \n Microsoft",
        "You earned "+Player.instance.points.ToString()+" Points \n\n in \n\n " + Player.instance.coins.ToString() + " coins."
	};

	private int slidesIndex = 0;
	private Text buttonText;

	void Awake(){
		buttonText = GetComponentInChildren<Text> ();

        creditText.text = creditSlides[0];

		//Cleans up lingering gameObjects from gameplay stage
		if (GameManager.instance != null)
			GameManager.instance.gameObject.SetActive (false);
		if (Player.instance != null)
			Player.instance.gameObject.SetActive (false);
		if (EggManager.instance != null)
			EggManager.instance.gameObject.SetActive (false);

        if (GameManager.instance != null)
            Destroy(GameManager.instance.gameObject);
        if (Player.instance != null)
            Destroy(Player.instance.gameObject);
        if (EggManager.instance != null)
            Destroy(EggManager.instance.gameObject);
    }

	void Start(){
	

		
	}

	public void advance(){
	
		creditText.gameObject.GetComponent<Animator> ().Play ("credits", -1, 0f);
		slidesIndex++;
		if (slidesIndex == creditSlides.Length - 1) //If the last slide is being loaded
			buttonText.text = "Main Menu";
		if (slidesIndex >= creditSlides.Length) {
			SceneManager.LoadScene ("LevelMenu");
			return;
		}

		creditText.text = creditSlides [slidesIndex];
	}
}
