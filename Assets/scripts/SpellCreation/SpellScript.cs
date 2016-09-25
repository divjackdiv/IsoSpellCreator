using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellScript : MonoBehaviour {

	public int cost;
	public List<Transform> currentBranches;
	public GameObject playerCombat;
	public bool shouldDelete = true;
	bool updating;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(updating && currentBranches.Count <= 0){
			print("spell has no more points");
			updating = false;
			playerCombat.GetComponent<playerCombat>().spellFinished();
			if(shouldDelete){
				playerCombat.GetComponent<playerCombat>().removeSpell(gameObject);
				Destroy(gameObject);
			}
		}
	}

	public bool nextTurn(){
		updateBranches();
		return false;
	}

	void updateBranches(){
		updating = true;
		foreach(Transform child in transform){
			child.GetComponent<SpellBranch>().updateBranch();
		}
	}
}
