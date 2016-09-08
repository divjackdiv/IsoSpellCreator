using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CombatManager : MonoBehaviour {

    public GameObject spellCreator;
	public int turn;
	public List<GameObject> spells;
	public List<GameObject> characters;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void nextTurn(){
		turn++;
		foreach (GameObject s in spells){
            s.GetComponent<SpellScript>().nextTurn();
        }
	}
}
