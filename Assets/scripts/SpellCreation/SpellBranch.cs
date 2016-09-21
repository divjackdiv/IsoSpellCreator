using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellBranch : MonoBehaviour {


	public void Start(){
	}

	public bool updateBranch(){
		List<Transform> currentChildren = new List<Transform>();
		foreach(Transform child in transform){
			currentChildren.Add(child);
		}
		bool shouldDelete = true;
		foreach(Transform child in currentChildren){
			if(!updatePoint(child)) shouldDelete = false;
		}

		if(shouldDelete)Destroy(gameObject);
		return shouldDelete;
	}
	
	public bool updatePoint(Transform child){
		bool pointDestroyed = child.gameObject.GetComponent<SpellPoint>().updatePoint();
		if(pointDestroyed){
			List<Transform> temp = new List<Transform>();
			foreach(Transform grandChild in child){
				temp.Add(grandChild);
			}
			foreach(Transform grandChild in temp){
				grandChild.parent = child.parent;
				grandChild.gameObject.active = true;
				pointDestroyed = updatePoint(grandChild);
			}
			Destroy(child.gameObject);
		}
		return pointDestroyed;
	}
}