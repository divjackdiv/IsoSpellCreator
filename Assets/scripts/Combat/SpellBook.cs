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
			print("size " +  s.transform.GetChild(0).childCount);
			foreach(Transform branch in s.transform){
				setupSpell(branch.transform.GetChild(0).gameObject, true);
			}
			playerCombat.GetComponent<playerCombat>().addSpell(s);
			s.transform.parent = player.transform;
			s.transform.localPosition = new Vector3(0, 0, 0);
			s.transform.parent = playerCombat.transform;
		}
	}
	public GameObject getSpell(int index){
		if (index < 0 || index >= Spells.Count) return null;
		return Spells[index];
	}

	public int getSpellCount(){
		return Spells.Count;
	}

	void setupSpell(GameObject spell, bool isRoot){
		if(isRoot) spell.active = true;
		else {
			spell.active = false;
		}
		foreach(Transform child in spell.transform){
			setupSpell(child.gameObject, false);
		}
	}

	public void addSpell(GameObject spell){
		Spells.Add(spell);
	}
}
