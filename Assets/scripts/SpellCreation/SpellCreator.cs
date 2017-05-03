using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpellCreator : MonoBehaviour {


    public GameObject overallManager;
    public List<GameObject> durationGameObjects;
    public float hoverDist;

    List<GameObject> spellsGameObjects;
    List<Sprite> spellsSprites;
    List<GameObject> durations; //gameobjects that will be seen when hovering above a point, when pressed they change that point's duration
    List<GameObject> currentChildren;
    GameObject sceneStateManager;
    GameObject spellBook;
    GameObject player;
    GameObject spell;
    GameObject branch;
    GameObject currentGameObject;
    GameObject hoveredPoint;
    GameObject defaultBranch;//static copies of above variables
    GameObject defaultSpell;
    GameObject oldTile;
    string spellName;
    int groundLayerMask;
    int turn;
    int currentVertex;
    bool shouldOpen;
    bool editingExistingSpell;
    bool isDragging;
    bool extending;


    void Start ()
    {
        spellBook = overallManager.GetComponent<overallManager>().spellBook;
        player = overallManager.GetComponent<overallManager>().player;
        sceneStateManager = overallManager.GetComponent<overallManager>().sceneStateManager;
        spellsGameObjects = spellBook.GetComponent<SpellBook>().spellGameObjects;
        spellsSprites = spellBook.GetComponent<SpellBook>().spellsSprites;
        defaultBranch = spellBook.GetComponent<SpellBook>().defaultBranch;
        defaultSpell = spellBook.GetComponent<SpellBook>().defaultSpell;
        groundLayerMask = 1 << gridManager.groundLayerS;
        if (Game.current != null && Game.current.editingSpell >= 0 && Game.current.editingSpell < Game.current.spells.Count) {
            editingExistingSpell = true;
            spell = SpellBook.loadSpell(Game.current.spells[Game.current.editingSpell], true, spellsSprites, spellsGameObjects, defaultSpell, defaultBranch, player);
        }
        else {
            spell = (GameObject)Instantiate(defaultSpell, player.transform.position, Quaternion.identity);
            branch = (GameObject)Instantiate(defaultBranch);
            branch.transform.parent = spell.transform;
        }
        durations = new List<GameObject>();
        for (int i = 0; i < durationGameObjects.Count; i++)
        {
            GameObject dur = (GameObject)Instantiate(durationGameObjects[i], new Vector3(0, 0, 0), Quaternion.identity);
            dur.GetComponent<durationOfPoint>().spellCreator = gameObject;
            dur.SetActive(false);
            dur.transform.localScale = new Vector3(0.07F, 0.07F, 0.07F);
            durations.Add(dur);
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            goBackToOldTile();
        }
        else if (Input.GetKeyUp(KeyCode.Delete))
        {
            deleteCurrentPoint();
        }
        else if (Input.GetButton("Fire1"))
        {
            movePoint();
            hideOptions();
        }
        //Create new point in the branch
        else if (Input.GetButton("Fire2"))
        {
            extendBranch();
            hideOptions();
        }
        //if you were previously dragging, but not anymore DROP
        else if (isDragging)
        {
            dropPoint();
        }
        else
        {
            onHover();
            if (currentGameObject != null) currentGameObject = null;
        }
    }

    public void createPointFromBar(int i)
    {
        createNewPoint(spellsGameObjects[i]);
        currentGameObject.transform.parent = branch.transform;
    }

    void createNewPoint(GameObject g){
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentGameObject = (GameObject) Instantiate(g, mousePosition, Quaternion.identity);//spellsGameObjects[i]
        MonoBehaviour[] scripts = currentGameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts) script.enabled = false;
        isDragging = true;
    }
    

    //Extends a point created
    void extendBranch()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (currentGameObject != null)
        {
            if (currentGameObject.transform.parent != null && currentGameObject.transform.parent.GetComponent<LineRenderer>() != null) lineFollow(currentGameObject, mousePos, 1);
            currentGameObject.transform.position = mousePos;
        }
        else
        {
            GameObject tile = PathFinding.getObjectAtMousePos();
            if (tile != null)
            {
                GameObject p = tile.GetComponent<tile>().getTakenBy();
                if (p != null && p.tag == "Spell")
                {
                    createNewPoint(p);
                    foreach (Transform child in currentGameObject.transform)
                    {
                        Destroy(child.gameObject); 
                    }
                    currentGameObject.transform.parent = p.transform; 
                    currentGameObject.transform.localScale = new Vector3(1, 1, 1);
                    createLineRenderer(currentGameObject, p.transform.position);
                    currentChildren = null;
                }
            }
        }
    }
    LineRenderer createLineRenderer(GameObject currentObj, Vector3 startPos)
    {
        LineRenderer lineRenderer = currentObj.GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer = currentObj.AddComponent<LineRenderer>();
        }
        lineRenderer.SetVertexCount(2);
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, startPos);
        lineRenderer.sortingLayerName = currentObj.GetComponent<SpriteRenderer>().sortingLayerName;
        return lineRenderer;
    }
 

    public void saveSpell(InputField saveField)
    {
        spellName = saveField.text;
        if (spellName == null)
            return;
        if (spell.transform.childCount > 0 && spell.transform.GetChild(0).childCount > 0)
        {
            enableChildrenLineRenderers(spell, false);
            spell.SetActive(false);
            int cost = calculateSpellCost(spell);
            spell.GetComponent<SpellScript>().player = player;
            spell.GetComponent<SpellScript>().cost = cost;
            spell.transform.parent = spellBook.transform;
            SpellData s = saveSpellData(spell.transform);
            if(Game.current != null)
            {
                if (Game.current.spells == null) Game.current.spells = new List<SpellData>();
                if (s != null)
                {
                    if (editingExistingSpell) Game.current.spells[Game.current.editingSpell] = s;
                    else Game.current.spells.Add(s);
                }
            }
        }
        else Destroy(spell);
        closeSpellCreator();
    }

    public SpellData saveSpellData(Transform s)
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
            SpellData spellData = new SpellData(s.transform.position, branches, s.GetComponent<SpellScript>().cost, spriteIndex, spellName);
            return spellData;
        }
        return null;
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
    
    //change this to scene state manager in the future
    public void closeSpellCreator()
    {
        int i = 1;
        if (Game.current != null)
            i = Game.current.sceneIndex;
        sceneStateManager.GetComponent<SceneStateManager>().loadScene(i);
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


    //cosmetic, enables/disables the line renderers of all children of Gameobject g
    void enableChildrenLineRenderers(GameObject g, bool enable){
        LineRenderer[] lrs = g.GetComponentsInChildren<LineRenderer>(true);
        foreach (LineRenderer lr in lrs){
            lr.enabled = enable;
        }
    }

    //Shows the durations ui on hover
    void onHover()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, groundLayerMask);
        if (hit)
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.GetComponent<tile>().taken == true)
                {
                    hoveredPoint = hit.collider.gameObject.GetComponent<tile>().getTakenBy();
                    if (hoveredPoint.GetComponent<SpellPoint>() != null)
                    {
                        showOptions(hoveredPoint);
                    }
                }
            }
        }
        if (hoveredPoint != null)
        {
            if (Vector3.Distance(hoveredPoint.transform.position, mousePos) > hoverDist)
            {
                hideOptions();
                hoveredPoint = null;
            }
        }
    }
    void showOptions(GameObject g)
    {
        Vector3 center = g.transform.position;
        for (int i = 0; i < durations.Count; i++)
        {

            float j = (i * 1.0f) / 6;
            float angle = j * Mathf.PI * 2;

            float x = Mathf.Sin(angle) / 2;
            float y = Mathf.Cos(angle) / 2;

            Vector3 pos = new Vector3(x, y, 0) + center;

            durations[i].transform.position = pos;
            durations[i].SetActive(true);
        }
    }
    public void hideOptions()
    {
        for (int i = 0; i < durations.Count; i++)
        {
            durations[i].SetActive(false);
        }
    }
      
    void lineFollow(GameObject currentObj, Vector2 pos, int index)
    {
        LineRenderer lr = currentObj.GetComponent<LineRenderer>();
        lr.sortingOrder = currentObj.GetComponent<SpriteRenderer>().sortingOrder;
        lr.SetPosition(index, pos);
    }
    void objLookAt(GameObject g, Vector3 pos)
    {
        pos.x = pos.x - g.transform.position.x;
        pos.z = pos.z - g.transform.position.z;
        float angle = (int)(Mathf.Atan2(pos.x, pos.z) * Mathf.Rad2Deg);
        g.transform.rotation = Quaternion.Slerp(g.transform.rotation, Quaternion.Euler(new Vector3(0, angle, 0)), 1);
    }
    void movePoint()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (currentGameObject == null && !isDragging)
        {
            GameObject tile = PathFinding.getObjectAtMousePos();
            if (tile != null)
            {
                GameObject p = tile.GetComponent<tile>().getTakenBy();
                if (p != null)
                {
                    isDragging = true;
                    currentGameObject = p;
                    currentChildren = new List<GameObject>();
                    foreach (Transform child in currentGameObject.transform)
                    {
                        currentChildren.Add(child.gameObject);
                    }
                    foreach (GameObject child in currentChildren)
                    {
                        child.transform.parent = currentGameObject.transform.parent;
                    }
                    oldTile = tile;
                }
            }
        }
        else
        {
            currentGameObject.transform.position = mousePos;

            //Update line renderer pos for parent
            if (currentGameObject.transform.parent != null && currentGameObject.transform.parent.GetComponent<LineRenderer>() != null) lineFollow(currentGameObject, currentGameObject.transform.position, 1);

            //update line Renderer Positions for children
            if (currentChildren != null)
            {
                foreach (GameObject child in currentChildren)
                {
                    lineFollow(child, currentGameObject.transform.position, 0);
                }
            }
        }
    }
    void goBackToOldTile()
    {
        if (oldTile == null)
        {
            deleteCurrentPoint();
        }
        else
        {
            currentGameObject.transform.position = oldTile.transform.position;
        }
    }
    public void dropPoint()
    {
        GameObject tile = PathFinding.getObjectAtMousePos();
        if (tile == null)
        {
            deleteCurrentPoint();
            return;
        }
        if (tile.GetComponent<tile>().takeTile(currentGameObject))
        {
            currentGameObject.transform.position = tile.transform.position;
            MonoBehaviour[] scripts = currentGameObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts) script.enabled = true;
            if (oldTile != null)
            {
                oldTile.GetComponent<tile>().leaveTile();
            }
        }
        else
        {
            goBackToOldTile();
        }

        if (currentGameObject != null)
        {
            if (currentChildren != null)
            {
                //reset children to be attached to the spell 
                foreach (GameObject child in currentChildren)
                {
                    child.transform.parent = currentGameObject.transform;
                }
                //update line Renderer Positions for children
                foreach (GameObject child in currentChildren)
                {
                    lineFollow(child, currentGameObject.transform.position, 0);
                }
            }

            //Update line renderer pos for parent
            if (currentGameObject.transform.parent != null && currentGameObject.transform.parent.GetComponent<LineRenderer>() != null) lineFollow(currentGameObject, currentGameObject.transform.position, 1);
        }

        oldTile = null;
        isDragging = false;
        currentGameObject = null;
        currentChildren = null;
    }
    public void deleteCurrentPoint()
    {
        if (currentChildren != null)
        {
            if (currentGameObject.transform.parent.GetComponent<SpellPoint>() != null)
            {
                foreach (GameObject child in currentChildren)
                {
                    lineFollow(child, currentGameObject.transform.parent.position, 0);
                }
            }
            else
            {
                foreach (GameObject child in currentChildren)
                {
                    lineFollow(child, child.transform.position, 0);
                }
            }
        }
        if (oldTile != null)
        {
            oldTile.GetComponent<tile>().leaveTile();
        }
        isDragging = false;
        oldTile = null;
        currentChildren = null;
        Destroy(currentGameObject);
        return;
    }
    public void modifyPointDuration(int i)
    {
        hoveredPoint.GetComponent<SpellPoint>().duration = i;
        for(int d = 0; d <durations.Count; d++) 
        {
            if(d == i)
                durations[d].GetComponent<SpriteRenderer>().color = new Vector4(0.2f,1f,1f,1f);
            else
                durations[d].GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f, 1f, 1f);
        }
    }
}
