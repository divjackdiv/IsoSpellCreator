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
            spell.active = false;
            spellBook.GetComponent<SpellBook>().addSpell(spell);
            Sprite sprite = spell.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite;
            spellCanvasObject.transform.GetChild(spellIndex++).GetComponent<Image>().sprite = sprite;
            spell.transform.parent = spellBook.transform;
        }
        else Destroy(spell);
        overallManager.GetComponent<overallManager>().closeSpellCreator();
    }
    public void close(){
    }
}
