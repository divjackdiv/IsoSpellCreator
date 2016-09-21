using UnityEngine;
using System.Collections;

public class overallManager : MonoBehaviour {

	//Scripts and canvas related to world/combat/spell making
    public GameObject spellCreationCanvas;
    public GameObject combatCanvas;
    public GameObject spellCreationScripts;
    public GameObject combatScripts;
    public GameObject worldScripts;
    public GameObject worldCanvas;

    public GameObject gridManager;
    public GameObject inputSpellCreator;
    public GameObject spellCreator;
    public GameObject combatManager;

    public GameObject player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void openSpellCreator(){
        combatScripts.active = false;
        combatCanvas.active = false;
        spellCreationCanvas.active = true;
        spellCreationScripts.active = true;
        worldScripts.active = false;
        worldCanvas.active = false;
        spellCreator.GetComponent<SpellCreator>().open();
	}

	public void closeSpellCreator(bool saved){
        inputSpellCreator.GetComponent<inputSpellCreator>().hideOptions();
        combatScripts.active = false;
        combatCanvas.active = false;
        spellCreationCanvas.active = false;
        spellCreationScripts.active = false;
        worldScripts.active = true;
        worldCanvas.active = true;
        gridManager.GetComponent<gridManager>().clearGrid();
        if (!saved) spellCreator.GetComponent<SpellCreator>().close();
    }

    public void startCombat(GameObject enemies){
        spellCreationCanvas.active = false;
        spellCreationScripts.active = false;
        worldScripts.active = false;
        worldCanvas.active = false;
    	combatScripts.active = true;
        combatCanvas.active = true;
        combatManager.GetComponent<CombatManager>().startCombat(enemies);
    }

    public void endCombat(){
    	combatScripts.active = false;
        combatCanvas.active = false;
        spellCreationCanvas.active = false;
        spellCreationScripts.active = false;
        worldScripts.active = true;
        worldCanvas.active = true;
        gridManager.GetComponent<gridManager>().clearGrid();
    }
}
