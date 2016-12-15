using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpellCreator : MonoBehaviour {


    public List<GameObject> spellsGameObjects;
    public List<Sprite> spellSprites;
    public GameObject spellBook;
    public GameObject player;

    public GameObject spellCanvasObject;
    public GameObject overallManager;
    public int groundLayer;

    private GameObject spell;
    private int spellIndex;

    private GameObject currentGameObject;
    private int layerMask;
    private int turn;
    private GameObject defaultBranch;
    private GameObject spellGameObject;

    private bool shouldOpen;

    void Start () {
        spellGameObject = overallManager.GetComponent<overallManager>().defaultSpellGameObject;
        defaultBranch = overallManager.GetComponent<overallManager>().defaultBranchGameObject;
        layerMask = 1<<groundLayer;
        spellIndex = 0;

    }
    void Update()
    {
        if (shouldOpen)
        {
            if (!player.GetComponent<playerOverall>().isMoving())
            {
                spell = (GameObject)Instantiate(spellGameObject, player.transform.position, Quaternion.identity);
                shouldOpen = false;
            } 
        }
    }
    public void open(){
        shouldOpen = true;
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
        else
        {
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
                        spell.GetComponent<SpellScript>().spriteIndex = currentGameObject.GetComponent<SpellPoint>().spriteIndex;
                        currentGameObject = null;
                        return;
                    }
                }
            }
            Destroy(currentGameObject);
            currentGameObject = null;
        }
    }

    public void saveSpell()
    {
        if(spell.transform.childCount > 0 && spell.transform.GetChild(0).childCount > 0  && spellIndex < 4)
        {
            enableChildrenLineRenderers(spell, false);
            spell.SetActive(false);
            int cost = calculateSpellCost(spell);
            spell.GetComponent<SpellScript>().player = player;
            spell.GetComponent<SpellScript>().cost = cost;
            spellBook.GetComponent<SpellBook>().addSpell(spell);
            Sprite sprite = spell.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite;
            spellCanvasObject.transform.GetChild(spellIndex).GetChild(0).GetComponent<Text>().text = cost + "";
            spellCanvasObject.transform.GetChild(spellIndex).GetComponent<Image>().sprite = sprite;
            spell.transform.parent = spellBook.transform;
            saveSpellData(spell.transform);
            spellIndex++;
        }
        else Destroy(spell);
        overallManager.GetComponent<overallManager>().closeSpellCreator(true);
    }

    public void saveSpellData(Transform s)
    {

        if (s.GetComponent<SpellScript>())
        {
            int spriteIndex = 0;
            List<BranchData> branches = new List<BranchData>();
            foreach (Transform branch in s.transform)
            {
                if (branch.GetComponent<SpellBranch>())
                {
                    List<PointData> points = new List<PointData>();
                    int index = -1;
                    foreach (Transform point in branch)
                    {
                        spriteIndex = point.GetComponent<SpellPoint>().spriteIndex;
                        recursiveSaveSpellPoint(point, index, points);
                    }
                    BranchData b = new BranchData(points);
                    branches.Add(b);
                }
            }
            SpellData spellData = new SpellData(s.transform.position, branches, s.GetComponent<SpellScript>().cost, spellIndex, spriteIndex);
            s.GetComponent<SpellScript>().setSpellData(spellData);
        }
    }
    void recursiveSaveSpellPoint(Transform point, int parentIndex, List<PointData> points)
    {
        int index = -1;
        if (point.GetComponent<SpellPoint>())
        {
            int duration = point.GetComponent<SpellPoint>().duration;
            int damage = point.GetComponent<SpellPoint>().damage;
            float movementSpeed = point.GetComponent<SpellPoint>().movementSpeed;
            int cost = point.GetComponent<SpellPoint>().cost;
            index = points.Count;
            int spriteIndex = point.GetComponent<SpellPoint>().spriteIndex;
            PointData p = new PointData(duration, damage, movementSpeed, cost, point.position, parentIndex, point.GetComponent<SpellPoint>().gameObjectIndex, spriteIndex);
            points.Add(p);
        }
        foreach (Transform child in point)
        {
            recursiveSaveSpellPoint(child, index, points);
        }

    }
    void saveSpells(Transform g, Game game)
    {
        foreach (Transform child in g)
        {
            saveSpells(child, game);
        }
        if (g.gameObject.GetComponent<SpellScript>() != null)
        {
            SpellData s = g.GetComponent<SpellScript>().getSpellData();
            game.spells.Add(s);
        }
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
