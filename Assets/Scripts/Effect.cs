using UnityEngine;
using System.Collections;

public class Effect : MonoBehaviour { //This class is designed to be used by particle effects (like explosions) that terminate when their animations are over.

	public AudioClip effectNoise;

	// Use this for initialization
	void Start () {

		if (effectNoise != null)
			SoundManager.instance.playClip (effectNoise, Mathf.Clamp(transform.localScale.x / 2f, 0, 1));
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void suicide(){
		Destroy (gameObject);
	}
}
