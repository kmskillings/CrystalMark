using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChaosBomb : MonoBehaviour {

    public float fuseMin; //How long the bomb takes to explode at a minimum
    public float fuseMax; //How long the bomb takes to explode at a maximum
    public float fuseLeft; //How much time is left
    public GameObject explosion; //The splash-damage explosion that is created when this explodes

    // Use this for initialization
	void Awake () {
        fuseLeft = Random.Range(fuseMin, fuseMax);
	}
	
	// Update is called once per frame
	void Update () {

        fuseLeft -= Time.deltaTime;
        if(fuseLeft <= 0)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);

            //resets fuse
            fuseLeft = Random.Range(fuseMin, fuseMax) ;
            gameObject.SetActive(false);
        }
		
	}
}
