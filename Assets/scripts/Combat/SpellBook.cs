using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SpellBook : MonoBehaviour {

	public GameObject combatManager;
	public List<GameObject> Spells;
	// Use this for initialization
	void Start () {
		Spells = new List<GameObject>();
	}

	public void instantiateSpell(int index){
		if (index < 0) return;
		if (index < Spells.Count){
			GameObject s = Spells[index];
			s.active = true;
			foreach(Branch b in s.GetComponent<SpellScript>().spell.branches){
				bool durOverOne = false;
				Point point = b.root;
				setupPoints(point, durOverOne);
			}
			combatManager.GetComponent<CombatManager>().spells.Add(s);
		}
	}

	void setupPoints(Point point, bool b){
		bool durOverOne = b;
		if(!durOverOne && point.duration <= 1) point.getGameObject().active = true;
		else {
			point.getGameObject().active = false;
			durOverOne = true;
		}
		foreach(Point p in point.children){
			setupPoints(p, durOverOne);
		}
	}
}
