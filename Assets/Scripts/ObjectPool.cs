using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IRecyclable{
	void shutdown ();
	void activate ();
}

public class ObjectPool : MonoBehaviour {

	public GameObject prefab; //The prefab that this object manages

	public List<GameObject> instances = new List<GameObject> (); //A list of objects to draw from

	public GameObject nextObject (Vector3 pos, Quaternion rotation) {
		GameObject instance = null; //Assumes that it can't find one, until proved wrong.

		foreach (GameObject go in instances) {
			if (!go.activeSelf) { //If the object is inactive
				go.SetActive (true);

				instance = go; //Found object to use. Set instance to it.
				break; //Stop looking. You already found one
			}
		}

		if (instance == null) //If it couldn't find an already existant object, creates a new one
			instance = createNewObject ();
		
		//instance is definetly something now, so set it up to be used
		instance.transform.position = pos;
		instance.transform.rotation = rotation;
		//activates any IRecyclables on the object
		foreach (MonoBehaviour script in instance.GetComponents<MonoBehaviour> ()) {
			if (script is IRecyclable)
				(script as IRecyclable).activate ();
		}

		return instance;
	}

	public GameObject nextObject(Vector3 pos){
		return nextObject (pos, Quaternion.identity);
	}

	private GameObject createNewObject () {
		GameObject go = Instantiate (prefab);
		instances.Add (go);
		return go;
	}

	void OnLevelWasLoaded () {
		instances = new List<GameObject> (); //Clear the list so it 'starts fresh'
	}

}