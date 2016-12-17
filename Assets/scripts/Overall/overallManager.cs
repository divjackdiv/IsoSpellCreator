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
    public GameObject spellBook;
    public GameObject saves;
    public GameObject load;

    public GameObject gridManager;
    public GameObject spellCreator;
    public GameObject combatManager;
    public GameObject mobs;

    public GameObject player;
    private GameObject defaultSpellGameObject;
    private GameObject defaultBranchGameObject;
    bool isEscMenuOpened;
    List<bool> menus;

    public GameObject spellCanvasObject;
    // Use this for initialization
    void Awake()
    {
        if(spellCreator != null) defaultBranchGameObject = spellCreator.GetComponent<SpellCreator>().defaultBranch;
        if(spellCreator != null) defaultSpellGameObject = spellCreator.GetComponent<SpellCreator>().spellGameObject;
        if (Game.current != null)
        {
            if (player != null && Game.current.playerData != null) player.GetComponent<playerStats>().load();
            if (Game.current.mobs != null && Game.current.mobs.Count > 0) loadMobs(mobs.transform);
            if (Game.current.spells != null)
            {
                loadSpells(spellBook.transform);
            }
        }
        menus = new List<bool>();
        if (escMenuCanvas != null) isEscMenuOpened = escMenuCanvas.activeSelf;
        if (combatCanvas != null) menus.Add(combatCanvas.activeSelf);
        if (spellCreationCanvas != null) menus.Add(spellCreationCanvas.activeSelf);
        if (worldCanvas != null) menus.Add(worldCanvas.activeSelf);
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
        saveScene(Game.current);
        StaticFunctions.loadScene(2);
    }
    public void loadScene(int i)
    {
        StaticFunctions.loadScene(i);
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
            if (game.mobs.ContainsKey(id))
            {
                game.mobs[id] = g.gameObject.GetComponent<mobStats>().save();
            }
            else
            {
                game.mobs.Add(id, g.gameObject.GetComponent<mobStats>().save());
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
            Sprite sprite = spellCreator.GetComponent<SpellCreator>().spellSprites[s.getUiSpriteIndex()];
            GameObject spell = (GameObject) Instantiate(defaultSpellGameObject, s.getPos(), Quaternion.identity);
            spell.GetComponent<SpellScript>().player = player;
            spell.GetComponent<SpellScript>().cost = cost;
            spell.GetComponent<SpellScript>().setSpellData(s);
            foreach (BranchData b in s.getBranches())
            {
                GameObject branch = (GameObject)Instantiate(defaultBranchGameObject);
                List<PointData> points = b.getPoints();
                List<Transform> pointsGO = new List<Transform>();
                foreach (PointData p in points)
                {
                    //first Create the right default spell point
                    GameObject point = (GameObject)Instantiate(spellCreator.GetComponent<SpellCreator>().spellsGameObjects[p.getGameObjectIndex()], p.getPosition(), Quaternion.identity);
                    //then update the value
                    point.GetComponent<SpellPoint>().duration = p.getDuration();
                    point.GetComponent<SpellPoint>().damage = p.getDamage();
                    point.GetComponent<SpellPoint>().cost = p.getCost();
                    point.GetComponent<SpellPoint>().movementSpeed = p.getMovementSpeed();
                    point.GetComponent<SpellPoint>().spriteIndex = p.getSpriteIndex();
                    //actually change the sprite
                    point.GetComponent<SpriteRenderer>().sprite = spellCreator.GetComponent<SpellCreator>().spellSprites[p.getSpriteIndex()];
                    if (p.getParentIndex() == -1)
                    {
                        point.transform.parent = branch.transform;
                    }
                    else
                    {
                        point.transform.parent = pointsGO[p.getParentIndex()];
                    }
                    pointsGO.Add(point.transform);
                }
                branch.transform.parent = spell.transform;
            }
            spell.transform.parent = g;
            spell.SetActive(false);
            //Now that the spell has been reloaded, we should "update" the Ui
            if (spellCanvasObject.transform.GetChild(counter))
            {// This is what will be used to set the ui buttons' image and function
                spellCanvasObject.transform.GetChild(counter).GetChild(0).GetComponent<Text>().text = cost + "";
                spellCanvasObject.transform.GetChild(counter).GetComponent<Image>().sprite = sprite;
                counter++;
            }
            spellBook.GetComponent<SpellBook>().addSpell(spell);
        }
    }
}

