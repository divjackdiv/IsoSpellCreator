using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

//The overallManager acts above most scripts and sends them commands, for example, if a combat starts, it'll tell UiManager and the CombatManager that combat is starting
//What actually happens after that is decided inside those classes.


public class overallManager : MonoBehaviour
{

    public GameObject gridManager;
    public GameObject combatManager;
    public GameObject uiManager;
    public GameObject sceneStateManager;
    public GameObject player;
    public GameObject spellBook;

    void Awake()
    {
        Time.timeScale = 1;
    }
   
    public void startCombat(GameObject enemies)
    {
        uiManager.GetComponent<UiManager>().startCombat();
        combatManager.GetComponent<CombatManager>().startCombat(enemies);
    }

    public void endCombat()
    {
        uiManager.GetComponent<UiManager>().endCombat();
        //gridManager.GetComponent<gridManager>().clearGrid(); // will find another way
    }
}

