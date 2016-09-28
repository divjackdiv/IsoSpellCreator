using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpellCreator : MonoBehaviour {

    public GameObject defaultBranch;
    public List<GameObject> spellsGameObjects;
    public GameObject spellGameObject;
    public GameObject spellBook;
    public GameObject player;

    public GameObject spellCanvasObject;
    public GameObject overallManager;
    public int groundLayer;

    private GameObject spell;
    //TEMPORARY
    private int spellIndex;

    private GameObject currentGameObject;
    private int layerMask;
    private int turn;
    
	void Start () {
        layerMask = 1<<groundLayer;
        spellIndex = 0;
	}

    public void open(){        
        spell = (GameObject) Instantiate(spellGameObject, player.transform.position, Quaternion.identity);
    }

	public void OnDragSpell(int i){
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (currentGameObject == null ){
                currentGameObject = (GameObject) Instantiate(spellsGameObjects[i], mousePosition, Quaternion.identity);
                MonoBehaviour[] scripts = currentGameObject.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour script in scripts) script.enabled = false;
        }
        else currentGameObject.transform.position = mousePosition;
    }

    public void OnDropSpell(){
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if(currentGameObject != null){
                Destroy(currentGameObject);
            } 
        }
        else{
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, layerMask);
            if (hit){
                if (hit.collider != null){
                    GameObject tile = hit.collider.gameObject;
                    bool take = tile.GetComponent<tile>().takeTile(currentGameObject);
                    if (take){
                        MonoBehaviour[] scripts = currentGameObject.GetComponents<MonoBehaviour>();
                        foreach (MonoBehaviour script in scripts) script.enabled = true;

                        Vector2 pos =  tile.transform.position;
                        currentGameObject.transform.position = pos;  

                        GameObject branch = (GameObject) Instantiate(defaultBranch);
                        branch.transform.parent = spell.transform;
                        currentGameObject.transform.parent = branch.transform;
                        currentGameObject = null;
                        return;
                    }
                }
            }
            Destroy(currentGameObject);
            currentGameObject = null;
        }
    }

    public void saveSpell(){
        if(spell.transform.childCount > 0 && spell.transform.GetChild(0).childCount > 0  && spellIndex < 4){
            enableChildrenLineRenderers(spell, false);
            spell.active = false;
            int cost = calculateSpellCost(spell);
            spell.GetComponent<SpellScript>().player = player;
            spell.GetComponent<SpellScript>().cost = cost;
            spellBook.GetComponent<SpellBook>().addSpell(spell);
            Sprite sprite = spell.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite;
            spellCanvasObject.transform.GetChild(spellIndex).GetChild(0).GetComponent<Text>().text = cost + "";
            spellCanvasObject.transform.GetChild(spellIndex++).GetComponent<Image>().sprite = sprite;
            spell.transform.parent = spellBook.transform;
        }
        else Destroy(spell);
        overallManager.GetComponent<overallManager>().closeSpellCreator(true);
    }

    public int calculateSpellCost(GameObject s){
        int cost = 0;
        foreach(Transform branch in s.transform){
            if(branch.GetChild(0) != null){
                cost += calculatePointCost(branch.GetChild(0).gameObject, 0);
            }
        }
        return cost;
    }

    public int calculatePointCost(GameObject sp, int currentCost){
        int cost = currentCost;
        foreach(Transform child in sp.transform){
            cost += calculatePointCost(child.gameObject, currentCost); 
        }        
        cost += sp.gameObject.GetComponent<SpellPoint>().cost;
        cost += sp.gameObject.GetComponent<SpellPoint>().duration;
        return cost;
    }

    public void close(){
        Destroy(spell);
    }


    void enableChildrenLineRenderers(GameObject g, bool enable){
        LineRenderer[] lrs = g.GetComponentsInChildren<LineRenderer>(true);
        foreach (LineRenderer lr in lrs){
            lr.enabled = enable;
        }
    }
}
