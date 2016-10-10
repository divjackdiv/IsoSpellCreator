using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;

public class overallManager : MonoBehaviour {

	//Scripts and canvas related to world/combat/spell making
    public GameObject spellCreationCanvas;
    public GameObject combatCanvas;
    public GameObject spellCreationScripts;
    public GameObject combatScripts;
    public GameObject worldScripts;
    public GameObject worldCanvas;
    public GameObject escMenuCanvas;
    public GameObject escMenuArroww;
    public GameObject saves;
    public GameObject load;

    public GameObject gridManager;
    public GameObject inputSpellCreator;
    public GameObject spellCreator;
    public GameObject combatManager;
    public GameObject mobs;

    public GameObject player;
    bool isEscMenuOpened;
    List<bool> menus;
	// Use this for initialization
	void Awake(){
		if(Game.current != null){
			if(player != null) player.GetComponent<playerStats>().load();
   			if(mobs != null) loadMobs(mobs.transform);
		}
		menus = new List<bool>();
		if(escMenuCanvas != null) isEscMenuOpened = escMenuCanvas.active;
		if(combatCanvas != null) menus.Add(combatCanvas.active);
		if(spellCreationCanvas != null) menus.Add(spellCreationCanvas.active);
		if(worldCanvas != null) menus.Add(worldCanvas.active);
	}
	void Start () {
		Time.timeScale = 1;
	}
	
	public void escMenuArrow(){
		if(isEscMenuOpened) closeEscMenu();
		else openEscMenu();
	}
	public void openEscMenu(){
		escMenuCanvas.active = true;
		isEscMenuOpened = true; 
		if(!MainMenu.isSceneMainMenu){
			Time.timeScale = 0;
			menus.Add(combatCanvas.active);
			menus.Add(spellCreationCanvas.active);
			menus.Add(worldCanvas.active);
	        combatScripts.active = false;
	        combatCanvas.active = false;
	        spellCreationCanvas.active = false;
	        spellCreationScripts.active = false;
	        worldScripts.active = false;
	        worldCanvas.active = false;
		}	
	}
	public void showSaves(){
    	escMenuCanvas.active = false;
    	saves.active = true;
    }
    public void showLoad(){
    	escMenuCanvas.active = false;
    	load.active = true;
    }

	public void closeEscMenu(){
		escMenuCanvas.active = false;
		load.active = false;
	    isEscMenuOpened = false;
		if(!MainMenu.isSceneMainMenu){
			Time.timeScale = 1;
			saves.active = false;
			combatScripts.active = menus[0];
	        combatCanvas.active = menus[0];
	        spellCreationCanvas.active = menus[1];
	        spellCreationScripts.active = menus[1];
	        worldScripts.active = menus[2];
	        worldCanvas.active = menus[2];
	   	}
	}

	public void openSpellCreator(){
		Time.timeScale = 0;
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
        Time.timeScale = 1;
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


    public void Save(InputField saveField){	//this is weird, I'm pretty sure a string should do
    	string saveName = saveField.text;
    	if(saveName == null || saveName == "") return;
    //	if(SaveManager.savedGames.ContainsKey(saveName)) return;
	    DateTime time = DateTime.Now; 
		string format = "dd MM yy HH:mm";  
		char[] delimiterChars = {' ', ':',};
		string[] words = time.ToString(format).Split(delimiterChars);

    	Game.current.dayDate = words[0];
    	Game.current.monthDate = words[1];
    	Game.current.yearDate = words[2];
    	Game.current.savedHour = words[3];
    	Game.current.savedMinute = words[4];

		Game.current.playerLevel = player.GetComponent<playerStats>().level;
		Game.current.areaName = EditorApplication.currentScene;
    	Game.current.sceneIndex = SceneManager.GetActiveScene().buildIndex;
    	Game.current.playerData = player.GetComponent<playerStats>().save();
    	saveMobs(mobs.transform);
    	SaveManager.Save(saveName);
    	closeEscMenu();
    }

    void saveMobs(Transform g){
    	foreach(Transform child in g){
    		saveMobs(child);
    	}
    	if(g.gameObject.GetComponent<mobStats>() != null){
    		int index = g.gameObject.GetComponent<mobStats>().getIndex();
    		if(Game.current.mobs.ContainsKey(index) != null){
    			Game.current.mobs[index] = g.gameObject.GetComponent<mobStats>().save();
    		}
    		else {
    			Game.current.mobs.Add(index, g.gameObject.GetComponent<mobStats>().save());
    		}
    	} 
    }
    void loadMobs(Transform g){
    	foreach(Transform child in g){
    		loadMobs(child);
    	}
    	if(g.gameObject.GetComponent<mobStats>() != null) g.gameObject.GetComponent<mobStats>().load();
    }
}

