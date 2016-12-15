using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellScript : MonoBehaviour {
    //children
	public int cost;
	public List<Transform> currentBranches;
	public GameObject player;
    public int spriteIndex;
    public bool shouldDelete = true;
    SpellData spell; //Saved parameters
	bool updating;

	// Update is called once per frame
	void Update () {
		if(updating && currentBranches.Count <= 0){
			updating = false;
			player.GetComponent<playerCombat>().spellFinished();
			player.GetComponent<playerCombat>().removeSpell(gameObject);
			if(shouldDelete) Destroy(gameObject);
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
    public SpellData getSpellData()
    {
        return spell;
    }
    public void setSpellData(SpellData s)
    {
        spell = s;
    }
}
