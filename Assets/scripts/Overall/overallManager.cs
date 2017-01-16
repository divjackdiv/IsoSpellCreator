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
    public GameObject combatCanvas;
    public GameObject combatScripts;
    public GameObject worldScripts;
    public GameObject worldCanvas;
    public GameObject escMenuCanvas;
    public GameObject escMenuArroww;
    public GameObject spellBook;
    public GameObject spellBookCanvas;
    public GameObject spellTemplateUI;
    public GameObject warningTooManySpells;
    public GameObject saves;
    public GameObject load;

    public GameObject gridManager;
    public GameObject combatManager;
    public GameObject mobs;

    public GameObject player;
    public int groundLayer;
    public static int groundLayerS;
    bool isEscMenuOpened;
    List<bool> menus;

    public GameObject spellCanvasObject;
    // Use this for initialization
    void Awake()
    {
        if (Game.current != null)
        {
            if (player != null && Game.current.playerData != null) player.GetComponent<playerStats>().load();
            if (Game.current.sceneSpecificMobs.ContainsKey(EditorApplication.currentScene) && Game.current.sceneSpecificMobs[EditorApplication.currentScene].Count > 0) loadMobs(mobs.transform);
            if (Game.current.spells != null && spellBook != null)
            {
                loadSpells(spellBook.transform);
            }
        }
        menus = new List<bool>();
        if (escMenuCanvas != null) isEscMenuOpened = escMenuCanvas.activeSelf;
        if (combatCanvas != null) menus.Add(combatCanvas.activeSelf);
        if (worldCanvas != null) menus.Add(worldCanvas.activeSelf);
        groundLayerS = groundLayer;
        Time.timeScale = 1;
    }
    void Start()
    {
    }

    public void escMenuArrow()
    {
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
            menus.Add(worldCanvas.activeSelf);
            combatScripts.SetActive(false);
            combatCanvas.SetActive(false);
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
            worldScripts.SetActive(menus[2]);
            worldCanvas.SetActive(menus[2]);
        }
    }
    public void openSpellCreator()
    {
        spellBookCanvas.SetActive(true);
    }
    public void loadSpellCreationScene(int spellIndex)
    {
        if (Game.current.spells.Count < 4) //4 will have to be changed to the character's spell ability
        {
            Game.current.editingSpell = spellIndex;
            saveScene(Game.current);
            StaticFunctions.loadScene(2);
        }
        else
        {
            warningTooManySpells.SetActive(true);
        }
    }
    public void dismissWarning()
    {
        print("dismissing");
        warningTooManySpells.SetActive(false);
    }
    public void loadScene(int i)
    {
        StaticFunctions.loadScene(i);
    }

    public void startCombat(GameObject enemies)
    {
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
        worldScripts.SetActive(true);
        worldCanvas.SetActive(true);
        //gridManager.GetComponent<gridManager>().clearGrid(); // will find another way
    }


    public void Save(InputField saveField)
    {   // this is weird, I'm pretty sure a string should do
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
        saveScene(g);
        SaveManager.Save(g); //this save func writes all the info to a file
        StartCoroutine(GetComponent<MainMenu>().updateButtons(g));
        closeEscMenu();
    }
    //saves all info about the scene into the game instance
    void saveScene(Game g)
    {
        g.playerLevel = player.GetComponent<playerStats>().level;
        g.areaName = EditorApplication.currentScene;
        g.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        g.playerData = player.GetComponent<playerStats>().save();
        if(g.sceneSpecificMobs != null && g.sceneSpecificMobs.ContainsKey(g.areaName))
        {
            g.sceneSpecificMobs[g.areaName] = new Dictionary<int, mobData>();
        }
        else g.sceneSpecificMobs.Add(g.areaName, new Dictionary<int, mobData>());
        saveMobs(mobs.transform, g);
        saveSpells(spellBook.transform, g);
    }
    void saveMobs(Transform g, Game game)
    {
        foreach (Transform child in g)
        {
            saveMobs(child,game);
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
    void deleteSpell(int i, Transform spellBook)
    {
        spellCanvasObject.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = "";
        spellCanvasObject.transform.GetChild(i).GetComponent<Image>().sprite = null;
        Destroy(spellBookCanvas.transform.GetChild(0).GetChild(0).GetChild(i+1).gameObject);
        Game.current.spells.RemoveAt(i);
        Destroy(spellBook.GetChild(i).gameObject);
    }
    void loadSpells(Transform g)
    {
        int counter = 0;
        foreach (SpellData s in Game.current.spells)
        {
            int cost = s.getCost();
            string spellName = s.getSpellName();

            GameObject spell = SpellBook.loadSpell(s, false, spellBook.GetComponent<SpellBook>().spellsSprites, spellBook.GetComponent<SpellBook>().spellsGameObjects, spellBook.GetComponent<SpellBook>().defaultSpell, spellBook.GetComponent<SpellBook>().defaultBranch, player);
            spell.transform.parent = g;
            spell.SetActive(false);
            //Now that the spell has been reloaded, we should "update" the Ui
            if (spellCanvasObject.transform.GetChild(counter))
            {// This is what will be used to set the ui buttons' image and function
                GameObject spellTemplate  = (GameObject) Instantiate(spellTemplateUI);
                spellTemplate.transform.parent = spellBookCanvas.transform.GetChild(0).GetChild(0);
                spellTemplate.transform.GetChild(0).GetComponent<Image>().sprite = spell.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite;
                spellTemplate.transform.GetChild(1).GetComponent<Text>().text = spellName + "";
                spellTemplate.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = cost + "";
                int i = counter;
                spellTemplate.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => loadSpellCreationScene(i));
                spellTemplate.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => deleteSpell(i, spellBook.transform));

                spellCanvasObject.transform.GetChild(counter).GetChild(0).GetComponent<Text>().text = cost + "";
                spellCanvasObject.transform.GetChild(counter).GetComponent<Image>().sprite = spell.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite;
                counter++;
            }
            spellBook.GetComponent<SpellBook>().addSpell(spell);
        }
    }
}

