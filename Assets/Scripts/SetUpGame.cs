using UnityEngine;
using System.Collections;

public class SetUpGame : MonoBehaviour {

	public GameObject gameManagerPrefab;

	public static SetUpGame instance;

	// Use this for initialization
	void Awake () {

		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (this);
		Instantiate (gameManagerPrefab);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
