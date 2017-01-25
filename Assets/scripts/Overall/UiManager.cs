using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//Takes care of most of the User interface code, in some cases this manager activates/deactivates scripts but only if this behaviour is due to a change in UI 
public class UiManager : MonoBehaviour {

    public GameObject overallManager;

    public GameObject worldCanvas;
    public GameObject spellBookCanvas;
    public GameObject combatCanvas;
    public GameObject savesCanvas;
    public GameObject loadCanvas;

    public GameObject combatScripts;
    public GameObject worldScripts;

    public GameObject escMenu; //NOT the canvas, it's child object
    public GameObject menuArrow;
    public GameObject warningTooManySpells;
    public GameObject spellTemplateUI;
    public GameObject combatSpells;

    GameObject spellBook;
    GameObject sceneStateManager;
    Dictionary<string, bool> menus; // dictionary pointing to which menus are active/inactive
    bool isEscMenuOpened;

    // Use this for initialization
    void Awake()
    {
        sceneStateManager = overallManager.GetComponent<overallManager>().sceneStateManager;
        spellBook = overallManager.GetComponent<overallManager>().spellBook;
        menus = new Dictionary<string, bool>();
        if (escMenu != null) menus.Add("escMenu", escMenu.activeSelf);
        if (combatCanvas != null) menus.Add("combatCanvas", combatCanvas.activeSelf);
        if (worldCanvas != null) menus.Add("worldCanvas", worldCanvas.activeSelf);
        // Update is called once per frame
    }
    void Update() {

    }


    public void escMenuArrow()
    {
        if (isEscMenuOpened) closeEscMenu();
        else openEscMenu();
    }
    public void openEscMenu()
    {
        escMenu.SetActive(true);
        isEscMenuOpened = true;
        if (!MainMenu.isSceneMainMenu)
        {
            Time.timeScale = 0;
            if(menus.ContainsKey("combatCanvas")) menus["combatCanvas"] = combatCanvas.activeSelf;
            if(menus.ContainsKey("worldCanvas")) menus["worldCanvas"] = worldCanvas.activeSelf;
            combatScripts.SetActive(false);
            combatCanvas.SetActive(false);
            worldScripts.SetActive(false);
            worldCanvas.SetActive(false);
        }
    }
    public void openSpellCreator()
    {
        Time.timeScale = 0;
        spellBookCanvas.SetActive(true);
    }
    public void showSaves()
    {
        escMenu.SetActive(false);
        savesCanvas.SetActive(true);
    }
    public void showLoad()
    {
        escMenu.SetActive(false);
        loadCanvas.SetActive(true);
    }

    public void closeEscMenu()
    {
        escMenu.SetActive(false);
        loadCanvas.SetActive(false);
        isEscMenuOpened = false;
        if (!MainMenu.isSceneMainMenu)
        {
            Time.timeScale = 1;
            savesCanvas.SetActive(false);
            if (menus.ContainsKey("combatCanvas"))
            {
                combatScripts.SetActive(menus["combatCanvas"]);
                combatCanvas.SetActive(menus["combatCanvas"]);
            }
            if (menus.ContainsKey("worldCanvas"))
            {
                worldScripts.SetActive(menus["worldCanvas"]);
                worldCanvas.SetActive(menus["worldCanvas"]);
            }
        }
    }
    public void startCombat()
    {
        worldScripts.SetActive(false);
        worldCanvas.SetActive(false);
        combatScripts.SetActive(true);
        combatCanvas.SetActive(true);
    }


    public void endCombat()
    {
        combatScripts.SetActive(false);
        combatCanvas.SetActive(false);
        worldScripts.SetActive(true);
        worldCanvas.SetActive(true);
    }
    public void warningPopUp(bool active)
    {
        warningTooManySpells.SetActive(active);
    }

    public void deleteSpell(int i)
    {
        combatSpells.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = "";
        combatSpells.transform.GetChild(i).GetComponent<Image>().sprite = null;
        Destroy(spellBookCanvas.transform.GetChild(0).GetChild(0).GetChild(i + 1).gameObject);
    }
    public bool loadSpell(int counter, string spellName, Sprite spellSprite, int  cost)
    {
        if (combatSpells.transform.GetChild(counter))
        {// This is what will be used to set the ui buttons' image and function
            GameObject spellTemplate = (GameObject)Instantiate(spellTemplateUI);
            spellTemplate.transform.parent = spellBookCanvas.transform.GetChild(0).GetChild(0);
            spellTemplate.transform.GetChild(0).GetComponent<Image>().sprite = spellSprite;
            spellTemplate.transform.GetChild(1).GetComponent<Text>().text = spellName + "";
            spellTemplate.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = cost + "";
            int i = counter;
            spellTemplate.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => sceneStateManager.GetComponent<SceneStateManager>().loadSpellCreationScene(i));
            spellTemplate.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => spellBook.GetComponent<SpellBook>().deleteSpell(i));

            combatSpells.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = cost + "";
            combatSpells.transform.GetChild(i).GetComponent<Image>().sprite = spellSprite;
            return true;
        }
        return false;
    }


    //Greys out a GameObject/ui element
    public static void changeAlpha(GameObject obj, bool isUiElement, float alpha)
    {
        if (isUiElement)
        {
            Color c = obj.GetComponent<Image>().color;
            c.a = alpha;
            obj.GetComponent<Image>().color = c;
        }
        else
        {
            Color c = obj.GetComponent<SpriteRenderer>().color;
            c.a = alpha;
            obj.GetComponent<SpriteRenderer>().color = c;
        }
    } 

    public void changeAlphaSpells(float alpha)
    {
        foreach (Transform child in combatSpells.transform)
        {
            UiManager.changeAlpha(child.gameObject, true, alpha);
        }
    }

}
