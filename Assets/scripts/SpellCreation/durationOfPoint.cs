using UnityEngine;
using System.Collections;

public class durationOfPoint : MonoBehaviour {

	public GameObject spellCreator;
	public int number;

	void OnMouseDown () {
        spellCreator.GetComponent<SpellCreator>().modifyPointDuration(number);
	}
}
