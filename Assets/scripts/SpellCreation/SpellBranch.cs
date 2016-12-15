using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellBranch : MonoBehaviour {
    //children
	public List<Transform> currentPoints;
	public bool shouldDelete = true;
	bool updating = false;

	void Update(){
		if(updating && currentPoints.Count == 0){
			updating = false;
			transform.parent.GetComponent<SpellScript>().currentBranches.Remove(transform);
			if(shouldDelete) Destroy(gameObject);
			else transform.parent.GetComponent<SpellScript>().shouldDelete = false;
		}
	}

	public void updateBranch(){
		transform.parent.GetComponent<SpellScript>().currentBranches.Add(transform);
		updating = true;
		shouldDelete = true;
		currentPoints = new List<Transform>();

		List<Transform> currentChildren = new List<Transform>();
		foreach(Transform child in transform){
			currentChildren.Add(child);
		}
		foreach(Transform child in currentChildren){
			child.gameObject.GetComponent<SpellPoint>().updatePoint(gameObject);
		}
	}

}