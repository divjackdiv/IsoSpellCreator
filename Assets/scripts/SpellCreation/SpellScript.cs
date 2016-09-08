using UnityEngine;
using System.Collections;

public class SpellScript : MonoBehaviour {

	public Spell spell;
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
		foreach(Branch b in spell.branches){
			b.getGameObject().GetComponent<SpellBranch>().updateBranch();
		}
	}
}
