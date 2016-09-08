using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
public class SpellCreator : MonoBehaviour {

    public GameObject inputManager;
    public GameObject defaultBranch;
    public List<GameObject> spellsGameObjects;
    public GameObject spellGameObject;
    public GameObject spellBook;
    public GameObject spellCreationCanvas;
    public GameObject combatCanvas;
    public GameObject spellCanvasObject;
    public GameObject spellCreationScripts;
    public GameObject combatScripts;
    public GameObject worldScripts;
    public GameObject worldCanvas;
    public GameObject gridManager;
    public int groundLayer;

    //TEMPORARY
    private int spellIndex;

    private Spell spell;
    private GameObject currentGameObject;
    private int layerMask;
    private int turn;
    
	void Start () {
        layerMask = 1<<groundLayer;
        spellIndex = 0;
	}

	public void openSpellCreator(){
        combatScripts.active = false;
        combatCanvas.active = false;
        spellCreationCanvas.active = true;
        spellCreationScripts.active = true;
        worldScripts.active = false;
        worldCanvas.active = false;
        GameObject spellGO = Instantiate(spellGameObject);
        spell = new Spell(spellGO);
        spellGO.GetComponent<SpellScript>().spell = spell;
	}

	public Branch createBranch(Spell spell, GameObject branch, GameObject g, Vector2 pos){
        Branch b = new Branch(spell, branch);
        spell.branches.Add(b);
        Point p = createPoint(b,g,pos, null);
        b.root = p;
        b.currentPoints.Add(b.root);
        return b;
	}

    public Point createPoint(Branch b, GameObject g, Vector2 pos, Point parent){
        Point point = new Point(b, 1 ,pos, g, parent);
        g.GetComponent<SpellPoint>().point = point;
        if(point.parent != null) parent.getChildren().Add(point);
        return point;
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
                        GameObject branch = (GameObject) Instantiate(defaultBranch, pos, Quaternion.identity);
                        branch.GetComponent<SpellBranch>().branch = createBranch(spell, branch, currentGameObject, pos);  
                        branch.transform.parent =  branch.GetComponent<SpellBranch>().branch.spell.spellGameObject.transform;
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
        if(spell.branches != null && spell.branches.Count > 0 && spell.branches[0].root != null ){
            spell.getGameObject().active = false;
            spellBook.GetComponent<SpellBook>().Spells.Add(spell.getGameObject());
            Sprite sprite = spell.branches[0].root.getGameObject().GetComponent<SpriteRenderer>().sprite;
            spellCanvasObject.transform.GetChild(spellIndex++).GetComponent<Image>().sprite = sprite;
            if(spellIndex == 4) spellIndex = 0;
        }
        spell.spellGameObject.transform.parent = spellBook.transform;
        closeSpellCreator();
    }
    public void closeSpellCreator(){
        inputManager.GetComponent<inputManager>().hideOptions();
        combatScripts.active = false;
        combatCanvas.active = false;
        spellCreationCanvas.active = false;
        spellCreationScripts.active = false;
        worldScripts.active = true;
        worldCanvas.active = true;
        gridManager.GetComponent<gridManager>().clearGrid();
    }
}
