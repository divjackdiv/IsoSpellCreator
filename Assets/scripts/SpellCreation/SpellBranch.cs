using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellBranch : MonoBehaviour {


	public void Start(){
	}

	public void updateBranch(){
		List<Transform> currentChildren = new List<Transform>();
		foreach(Transform child in transform){
			currentChildren.Add(child);
		}
		foreach(Transform child in currentChildren){
			bool pointDestroyed = child.gameObject.GetComponent<SpellPoint>().updatePoint();
			if(pointDestroyed){
				foreach(Transform grandChild in child.transform){
					grandChild.parent = transform;
				}
				Destroy(child.gameObject);
			}
		}
		if(transform.childCount == 0)Destroy(gameObject);
	}
}