using UnityEngine;
using System.Collections;

public class SpellScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void nextTurn(){
		updateBranches();
	}

	void updateBranches(){
		foreach(Transform child in transform){
			child.GetComponent<SpellBranch>().updateBranch();
		}
		if(transform.childCount == 0)Destroy(gameObject);
	}
}
