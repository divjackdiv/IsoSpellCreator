using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SpellBook : MonoBehaviour {

	public GameObject playerCombat;
	public GameObject player;
	List<GameObject> Spells;
	// Use this for initialization
	void Start () {
		Spells = new List<GameObject>();
	}

	public void instantiateSpell(int index){
		if (index < 0) return;
		if (index < Spells.Count){
			GameObject s = Spells[index];
			s = (GameObject) Instantiate(s);
			s.active = true;
			setupSpell(s.transform.GetChild(0).transform.GetChild(0).gameObject, false);
			playerCombat.GetComponent<playerCombat>().addSpell(s);
			s.transform.parent = player.transform;
			s.transform.localPosition = new Vector3(0, 0, 0);
			s.transform.parent = playerCombat.transform;
		}
	}

	void setupSpell(GameObject spell, bool b){
		bool durOverOne = b;
		if(!durOverOne && spell.GetComponent<SpellPoint>().duration <= 1) spell.active = true;
		else {
			spell.active = false;
			durOverOne = true;
		}
		foreach(Transform child in spell.transform){
			setupSpell(child.gameObject, durOverOne);
		}
	}
	public void addSpell(GameObject spell){
		Spells.Add(spell);
	}
}
