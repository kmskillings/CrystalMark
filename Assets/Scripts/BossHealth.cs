using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealth : MonoBehaviour, IRecyclable {

	private Slider healthBar;
	private Image deathImage;

	private Creature bossCreature;

	void Awake () {
		
	}

	void Update () {
		healthBar.value = bossCreature.health;
	}

	public void activate (){

		bossCreature = GetComponent<Creature> ();
		//deathImage.gameObject.SetActive (false);
		healthBar = CanvasPointers.instance.bossHealthSlider;
		deathImage = CanvasPointers.instance.dialogueImage;

		//subscribes Gamemanager's advance function to the advance button. This is a weird place to do it, though.
		CanvasPointers.instance.continueButton.onClick.AddListener(() => { GameManager.instance.advaceLevel(); } );

		deathImage.gameObject.SetActive (false);
		healthBar.gameObject.SetActive (true);
		healthBar.maxValue = bossCreature.maxHealth;

	}

	public void shutdown () {
		deathImage.gameObject.SetActive (true);
		healthBar.gameObject.SetActive (false);

		Player.instance.GetComponent<KilledBy> ().isEnabled = false;
	}
}
