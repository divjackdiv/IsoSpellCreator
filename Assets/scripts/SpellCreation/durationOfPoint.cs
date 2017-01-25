using UnityEngine;
using System.Collections;

public class durationOfPoint : MonoBehaviour {

	public GameObject inputSpellCreator;
	public int number;

	void OnMouseDown () {
    	inputSpellCreator.GetComponent<inputSpellCreator>().modifyPointDuration(number);
	}
}
