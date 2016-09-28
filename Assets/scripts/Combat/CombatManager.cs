using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CombatManager : MonoBehaviour {

    public GameObject spellCreator;
    public GameObject overallManager;
    public GameObject player;
	public int turn;
	public List<GameObject> characters;
	int index;
	bool characterPlayed;
	bool combatStarted;
	// Use this for initialization
	void Start () {
		index = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(combatStarted && characterPlayed){
			if(characters.Count == 1) endCombat();
			if(index < characters.Count){
				permissionToPlay(characters[index]);
			}
			else{
				nextTurn();
				index = 0;
			}
		}
	
	}

	public void nextTurn(){
		turn++;
	}

	public void startCombat(GameObject enemies){
		characters = new List<GameObject>();
		turn = 0;
		characters.Add(player);
		player.GetComponent<playerWorld>().enabled = false;
		player.GetComponent<playerCombat>().enabled = true;
		player.GetComponent<playerCombat>().startCombat();
		foreach(Transform child in enemies.transform){
			characters.Add(child.gameObject);
			child.GetComponent<mobWorld>().enabled = false;
			child.GetComponent<mobCombat>().enabled  = true;
			child.GetComponent<mobCombat>().target = player;
			child.GetComponent<mobCombat>().startCombat();
		}
		characterPlayed = true;
		combatStarted = true;
	}

	//permissionToPlay method tells a mob or the player it is his turn to play. This allows it to take action
	public void permissionToPlay(GameObject character){
		characterPlayed = false;
		if(character == player){
			player.GetComponent<playerCombat>().play();
		}
		else if(character.GetComponent<mobCombat>() != null){
			character.GetComponent<mobCombat>().play();
		}
	}
	public void finishedPlaying(){
		index++;
		characterPlayed = true;
	}

	public void endCombat(){
		combatStarted = false;
		characterPlayed = true;
		player.GetComponent<playerCombat>().endCombat();
		characters.Remove(player);
		foreach(GameObject g in characters){
			g.GetComponent<mobCombat>().enabled = false;
			g.GetComponent<mobWorld>().enabled = true;
		}
		overallManager.GetComponent<overallManager>().endCombat();
	}
}
