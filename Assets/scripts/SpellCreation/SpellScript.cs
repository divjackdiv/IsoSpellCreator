using UnityEngine;
using System.Collections;

public class SpellScript : MonoBehaviour {

	public int cost;
	public GameObject playerCombat;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool nextTurn(){
		 return updateBranches();
	}

	bool updateBranches(){
		bool shouldDelete = true;
		foreach(Transform child in transform){
			if(!child.GetComponent<SpellBranch>().updateBranch()) shouldDelete = false;
		}
		if(shouldDelete){
			Destroy(gameObject);
		}
		return shouldDelete;
	}
}
