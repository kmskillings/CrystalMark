using UnityEngine;
using System.Collections;
using System;

public class ScrollingBackground : MonoBehaviour {

	public Texture[] backgrounds; //All the backgrounds that can be loaded onto the background of the game.
	public static ScrollingBackground instance;
	public Vector2 offsetVel; //How much the background should scroll per second, in tiles

	public Texture backgroundTexture {
		get { 
			return backgrounds [LevelStats.currentLevel.number % backgrounds.Length];
		}
	}
		

	private int backgroundIndex; //The index of the texture to load. Assigned during level loading

	private Renderer rend;

	void Awake () {

		rend = GetComponent<Renderer> ();
		
	}

	// Use this for initialization
	void Start () {

		loadTexture ();
	
	}
	
	// Update is called once per frame
	void Update () {

		rend.material.SetTextureOffset("_MainTex", rend.material.GetTextureOffset("_MainTex") + offsetVel * Time.deltaTime);
	
	}

	void OnLevelWasLoaded (){
		backgroundIndex++;
		Debug.Log ("Loading up new texture. Current backgroundIndex is " + backgroundIndex.ToString ());
		if (backgroundIndex >= backgrounds.Length) {
			backgroundIndex = 0;
			Debug.Log ("Just set backgroundIndex to 0");
		}
		loadTexture ();
	}

	void loadTexture () {
		
		//Asignes renderer's material to texture
		rend.material.SetTexture ("_MainTex", backgroundTexture);

		//Gets camera size and scales width accordingly
		float width = (Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 10)) - Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 3))).x;
		float height = ((float)backgroundTexture.height / (float)backgroundTexture.width) * width;

		transform.localScale = new Vector3 (width, height, 1);

	}
}