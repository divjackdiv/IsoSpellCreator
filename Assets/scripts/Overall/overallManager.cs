using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;

public class overallManager : MonoBehaviour
{

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
    void Awake()
    {
        print(Application.persistentDataPath);
        if (Game.current != null)
        {
            if (player != null) player.GetComponent<playerStats>().load();
            if (mobs != null) loadMobs(mobs.transform);
        }
        menus = new List<bool>();
        if (escMenuCanvas != null) isEscMenuOpened = escMenuCanvas.activeSelf;
        if (combatCanvas != null) menus.Add(combatCanvas.activeSelf);
        if (spellCreationCanvas != null) menus.Add(spellCreationCanvas.activeSelf);
        if (worldCanvas != null) menus.Add(worldCanvas.activeSelf);
    }
    void Start()
    {
        Time.timeScale = 1;
    }

    public void escMenuArrow()
    {
        foreach (Game g in SaveManager.savedGames)
        {
            print("    HEyoo " + g.fileName);
        }
        if (isEscMenuOpened) closeEscMenu();
        else openEscMenu();
    }
    public void openEscMenu()
    {
        escMenuCanvas.SetActive(true);
        isEscMenuOpened = true;
        if (!MainMenu.isSceneMainMenu)
        {
            Time.timeScale = 0;
            menus.Add(combatCanvas.activeSelf);
            menus.Add(spellCreationCanvas.activeSelf);
            menus.Add(worldCanvas.activeSelf);
            combatScripts.SetActive(false);
            combatCanvas.SetActive(false);
            spellCreationCanvas.SetActive(false);
            spellCreationScripts.SetActive(false);
            worldScripts.SetActive(false);
            worldCanvas.SetActive(false);
        }
    }
    public void showSaves()
    {
        escMenuCanvas.SetActive(false);
        saves.SetActive(true);
    }
    public void showLoad()
    {
        escMenuCanvas.SetActive(false);
        load.SetActive(true);
    }

    public void closeEscMenu()
    {
        escMenuCanvas.SetActive(false);
        load.SetActive(false);
        isEscMenuOpened = false;
        if (!MainMenu.isSceneMainMenu)
        {
            Time.timeScale = 1;
            saves.SetActive(false);
            combatScripts.SetActive(menus[0]);
            combatCanvas.SetActive(menus[0]);
            spellCreationCanvas.SetActive(menus[1]);
            spellCreationScripts.SetActive(menus[1]);
            worldScripts.SetActive(menus[2]);
            worldCanvas.SetActive(menus[2]);
        }
    }

    public void openSpellCreator()
    {
        Time.timeScale = 0;
        combatScripts.SetActive(false);
        combatCanvas.SetActive(false);
        spellCreationCanvas.SetActive(true);
        spellCreationScripts.SetActive(true);
        worldScripts.SetActive(false);
        worldCanvas.SetActive(false);
        player.transform.GetComponent<playerWorld>().enabled = false;
        player.transform.GetComponent<playerOverall>().moveToNearestTile();
        spellCreator.GetComponent<SpellCreator>().open();
    }

    public void closeSpellCreator(bool saved)
    {
        inputSpellCreator.GetComponent<inputSpellCreator>().hideOptions();
        combatScripts.SetActive(false);
        combatCanvas.SetActive(false);
        spellCreationCanvas.SetActive(false);
        spellCreationScripts.SetActive(false);
        worldScripts.SetActive(true);
        worldCanvas.SetActive(true);
        gridManager.GetComponent<gridManager>().clearGrid();
        if (!saved) spellCreator.GetComponent<SpellCreator>().close();
        Time.timeScale = 1;
        player.transform.GetComponent<playerWorld>().enabled = true;
    }

    public void startCombat(GameObject enemies)
    {
        spellCreationCanvas.SetActive(false);
        spellCreationScripts.SetActive(false);
        worldScripts.SetActive(false);
        worldCanvas.SetActive(false);
        combatScripts.SetActive(true);
        combatCanvas.SetActive(true);
        combatManager.GetComponent<CombatManager>().startCombat(enemies);
    }

    public void endCombat()
    {
        combatScripts.SetActive(false);
        combatCanvas.SetActive(false);
        spellCreationCanvas.SetActive(false);
        spellCreationScripts.SetActive(false);
        worldScripts.SetActive(true);
        worldCanvas.SetActive(true);
        gridManager.GetComponent<gridManager>().clearGrid();
    }


    public void Save(InputField saveField)
    {   //this is weird, I'm pretty sure a string should do
        string saveName = saveField.text;
        if (saveName == null || saveName == "") return;
        //	if(SaveManager.savedGames.ContainsKey(saveName)) return;
        DateTime time = DateTime.Now;
        string format = "dd MM yy HH:mm";
        char[] delimiterChars = { ' ', ':', };
        string[] words = time.ToString(format).Split(delimiterChars);
        Game g = new Game();
        g.fileName = saveName;
        g.dayDate = words[0];
        g.monthDate = words[1];
        g.yearDate = words[2];
        g.savedHour = words[3];
        g.savedMinute = words[4];

        g.playerLevel = player.GetComponent<playerStats>().level;
        g.areaName = EditorApplication.currentScene;
        g.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        g.playerData = player.GetComponent<playerStats>().save();
        saveMobs(mobs.transform, g);
        SaveManager.Save(g);
        StartCoroutine(GetComponent<MainMenu>().updateButtons(g));
        closeEscMenu();
    }

    void saveMobs(Transform g, Game game)
    {
        foreach (Transform child in g)
        {
            saveMobs(child,game);
        }
        if (g.gameObject.GetComponent<mobStats>() != null)
        {
            int index = g.gameObject.GetComponent<mobStats>().getIndex();
            if (game.mobs.ContainsKey(index))
            {
                game.mobs[index] = g.gameObject.GetComponent<mobStats>().save();
            }
            else
            {
                game.mobs.Add(index, g.gameObject.GetComponent<mobStats>().save());
            }
        }
    }
    void loadMobs(Transform g)
    {
        foreach (Transform child in g)
        {
            loadMobs(child);
        }
        if (g.gameObject.GetComponent<mobStats>() != null) g.gameObject.GetComponent<mobStats>().load();
    }
}

