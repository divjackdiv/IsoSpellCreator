using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SpellBook : MonoBehaviour {

	public GameObject player;
	public GameObject combatManager;
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
			s.SetActive(true);
			foreach(Transform branch in s.transform){
				setupSpell(branch.transform.GetChild(0).gameObject, true);
			}
			player.GetComponent<playerCombat>().addSpell(s);
			s.transform.parent = player.transform;
			s.transform.localPosition = new Vector3(0, 0, 0);
			s.transform.parent = combatManager.transform;
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
        if (isRoot)
        {
            spell.SetActive(true);
            spell.GetComponent<SpellPoint>().initPoint();
        }
        else
        {
            spell.SetActive(false);
        }
		foreach(Transform child in spell.transform){
			setupSpell(child.gameObject, false);
		}
	}

	public void addSpell(GameObject spell){
		Spells.Add(spell);
	}
}
