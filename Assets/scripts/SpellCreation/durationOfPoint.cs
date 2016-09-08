using UnityEngine;
using System.Collections;

public class durationOfPoint : MonoBehaviour {

	public GameObject inputManager;
	public int number;
	void OnMouseDown () {
    	inputManager.GetComponent<inputManager>().modifyPointDuration(number);
	}
}
