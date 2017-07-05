using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CombatManager : MonoBehaviour {

    public List<GameObject> characters;
    public GameObject overallManager;
    public int turn;

    GameObject player;
    int index;
    int charactersGettingReady;

    // Use this for initialization
    void Awake()
    {
        player = overallManager.GetComponent<overallManager>().player;
        charactersGettingReady = 0;
    }
	void Start () {
		index = 0;
	}

    public void nextCharacter()
    {
        if (index >= characters.Count)
        {
            turn++;
            index = 0;
        }
        permissionToPlay(characters[index]);
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
            child.GetComponent<mobOverall>().moveToNearestTile();
			child.GetComponent<mobCombat>().startCombat();
		}
        charactersGettingReady = characters.Count;
    }

    public void characterReady()
    {
        charactersGettingReady -= 1;
        if (charactersGettingReady == 0)
            nextCharacter(); //effectively start the combat
    }

    //permissionToPlay method tells a mob or the player it is his turn to play. This allows it to take an action
    public void permissionToPlay(GameObject character){
		if(character == player){
			player.GetComponent<playerCombat>().play();
		}
		else if(character.GetComponent<mobCombat>() != null){
			character.GetComponent<mobCombat>().play();
		}
	}

    public void finishedPlaying()
    {
        index++;
        if (characters.Count == 1) endCombat(); //or player died
        else nextCharacter();
    }
    
	public void endCombat(){
		player.GetComponent<playerCombat>().endCombat();
		characters.Remove(player);
		foreach(GameObject g in characters){
			g.GetComponent<mobCombat>().enabled = false;
			g.GetComponent<mobWorld>().enabled = true;
		}
		overallManager.GetComponent<overallManager>().endCombat();
	}
}
