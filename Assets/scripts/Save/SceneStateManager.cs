using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Not to be confused with the SceneManager already defined by unity.SceneManagement
// Takes care of keeping the state of the scene, and loading the scene to a saved state.
public class SceneStateManager : MonoBehaviour {


    public GameObject overallManager;
    public GameObject mobs;

    GameObject player;
    GameObject uiManager;
    GameObject spellBook;

    //Load any known data for this scene, such as ennemies position, spells ect
    void Awake () {
        player = overallManager.GetComponent<overallManager>().player;
        uiManager = overallManager.GetComponent<overallManager>().uiManager;
        spellBook = overallManager.GetComponent<overallManager>().spellBook;
        if (Game.current != null)
        {
            Scene scene = SceneManager.GetActiveScene();
            if (player != null && Game.current.playerData != null) player.GetComponent<playerStats>().load();
            if (Game.current.sceneSpecificMobs.ContainsKey(scene.name) && Game.current.sceneSpecificMobs[scene.name].Count > 0) loadMobs(mobs.transform);
            if (scene.name != "spellCreation" && Game.current.spells != null && spellBook != null) //TODO change this check to be based on something else than the scene name
            { 
                loadSpells(spellBook.transform);
            }
        }
    }

    public void saveScene(Game g)
    {
        DateTime time = DateTime.Now;
        string format = "dd MM yy HH:mm";
        char[] delimiterChars = { ' ', ':', };
        string[] words = time.ToString(format).Split(delimiterChars);
        g.dayDate = words[0];
        g.monthDate = words[1];
        g.yearDate = words[2];
        g.savedHour = words[3];
        g.savedMinute = words[4];
        g.playerLevel = player.GetComponent<playerStats>().level;
        g.areaName = SceneManager.GetActiveScene().name;
        g.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        g.playerData = player.GetComponent<playerStats>().save();
        saveMobs(mobs.transform, g);
        saveSpells(spellBook.transform, g);
    }

    public void SaveToFile(InputField saveField)
    {
        string saveName = saveField.text;
        if (saveName == null || saveName == "") return;
        //	if(SaveManager.savedGames.ContainsKey(saveName)) return;

        Game g = new Game();
        g.fileName = saveName;
        saveScene(g);
        SaveManager.Save(g); //this save func writes all the info to a file
        StartCoroutine(overallManager.GetComponent<MainMenu>().updateButtons(g));
    }

    void saveMobs(Transform mobs, Game g)
    {
        if (g.sceneSpecificMobs != null && g.sceneSpecificMobs.ContainsKey(g.areaName))
        {
            g.sceneSpecificMobs[g.areaName] = new Dictionary<int, mobData>();
        }
        else g.sceneSpecificMobs.Add(g.areaName, new Dictionary<int, mobData>());
        saveMobsRecurs(mobs, g);
    }
    void saveMobsRecurs(Transform g, Game game)
    {
        foreach (Transform child in g)
        {
            saveMobs(child, game);
        }
        if (g.gameObject.GetComponent<mobStats>() != null)
        {
            int id = g.gameObject.GetComponent<mobStats>().getUniqueId();
            if (game.sceneSpecificMobs[game.areaName].ContainsKey(id))
            {
                game.sceneSpecificMobs[game.areaName][id] = g.gameObject.GetComponent<mobStats>().save();
            }
            else
            {
                game.sceneSpecificMobs[game.areaName].Add(id, g.gameObject.GetComponent<mobStats>().save());
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

    public static void saveSpells(Transform g, Game game)
    {
        game.spells = new List<SpellData>();
        foreach (Transform child in g)
        {
            if (child.gameObject.GetComponent<SpellScript>() != null)
            {
                SpellData s = child.GetComponent<SpellScript>().getSpellData();
                game.spells.Add(s);
            }
        }
    }



    void loadSpells(Transform g)
    {
        int counter = 0;
        foreach (SpellData s in Game.current.spells)
        {
            int cost = s.getCost();
            string spellName = s.getSpellName();

            GameObject spell = SpellBook.loadSpell(s, false, spellBook.GetComponent<SpellBook>().spellsSprites, spellBook.GetComponent<SpellBook>().spellGameObjects, spellBook.GetComponent<SpellBook>().defaultSpell, spellBook.GetComponent<SpellBook>().defaultBranch, player);
            spell.transform.parent = g;
            spell.SetActive(false);
            Sprite spellSprite = spell.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite;
            //Now that the spell has been reloaded, we should "update" the Ui
            bool uiLoaded = uiManager.GetComponent<UiManager>().loadSpell(counter, spellName, spellSprite, cost);
            if (uiLoaded) counter++; 
            spellBook.GetComponent<SpellBook>().addSpell(spell);
        }
    }

    public void loadScene(int i)
    {
        SceneManager.LoadScene(i);
    }

    public void loadSpellCreationScene(int spellIndex)
    {
        if (Game.current.spells.Count < 4) //4 will have to be changed to the character's spell ability
        {
            Game.current.editingSpell = spellIndex;
            saveScene(Game.current);
            loadScene(2);
        }
        else
        {
            uiManager.GetComponent<UiManager>().warningPopUp();
        }
    }
}
