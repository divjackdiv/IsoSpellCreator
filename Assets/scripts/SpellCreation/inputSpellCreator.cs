using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class inputSpellCreator : MonoBehaviour {

    public List<GameObject> durationGameObjects;
    public GameObject overallManager;
    public float hoverDist; 

    
    List<GameObject> durations; //gameobjects that will be seen when hovering above a point, when pressed they change that point's duration
    List<GameObject> currentChildren;
    GameObject spellbook;
    GameObject currentGameObject;
    GameObject hoveredPoint;
    GameObject takenBy;
    GameObject oldTile;
    int groundLayer;
    int groundLayerMask;
    int currentVertex;
    bool durationsAreShown;
	bool isDragging;
	bool draggingFromBar;
    bool extending;

    void Awake()
    {
        spellbook = overallManager.GetComponent<overallManager>().spellBook;
    }
	void Start () {
        groundLayer = gridManager.groundLayerS;
        groundLayerMask = 1<<groundLayer;
        durations = new List<GameObject>();
        for (int i = 0; i < durationGameObjects.Count; i++){
            GameObject dur = (GameObject) Instantiate (durationGameObjects[i], new Vector3(0,0,0), Quaternion.identity);
            dur.GetComponent<durationOfPoint>().inputSpellCreator = gameObject;
            dur.SetActive(false);
            dur.transform.localScale = new Vector3(0.07F, 0.07F, 0.07F);
            durations.Add(dur);
        }
        durationsAreShown = false;
	}
	
	// Update is called once per frame
	void Update () {

        //Drag an already existing GameObject
        if (Input.GetButton("Fire1")){
            if ((EventSystem.current.IsPointerOverGameObject() && !isDragging)|| draggingFromBar){
                draggingFromBar = true;
                return;
            }
        	movePoint();
            hideOptions();
        }
        //if you were previously dragging, but not anymore DROP
        else if(isDragging){
            print("dropping");
            dropPoint();  
        }
        //Create new point in the branch
        else if (Input.GetButton("Fire2") && !isDragging){
            if (EventSystem.current.IsPointerOverGameObject() || draggingFromBar){
                draggingFromBar = true;
                return;
            }
            extendBranch();   
            hideOptions();       
        }
        else {
            if(draggingFromBar){
                draggingFromBar = false;
            }
            if(extending){
                extending = false;
                LineRenderer lr = currentGameObject.GetComponent<LineRenderer>();
                extendLineRenderer(lr, currentGameObject);
            }
            if(currentGameObject != null) currentGameObject = null;
            //On Hover 
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);      
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, groundLayerMask);
            if (hit){
                if (hit.collider != null){
                    if(hit.collider.gameObject.GetComponent<tile>().taken == true){
                        takenBy = hit.collider.gameObject.GetComponent<tile>().getTakenBy();
                        if(takenBy.GetComponent<SpellPoint>() != null){
                            hoveredPoint = takenBy;
                            showOptions(takenBy);
                        } 
                    }
                }
            }
            else {
                if(durationsAreShown){
                    if(Vector3.Distance(takenBy.transform.position, mousePos) > hoverDist){
                        hideOptions();
                    }
                }
            }
        }
	}

    void extendBranch(){
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);  
        if (currentGameObject != null){
                lineFollow(currentGameObject, mousePos , 1);
                currentGameObject.transform.position = mousePos;
        } 
        else{
            GameObject tile = PathFinding.getObjectAtMousePos();
            if (tile != null)
            {
                GameObject p = tile.GetComponent<tile>().getTakenBy();
                if(p != null && p.tag == "Spell"){
                    extending = true;
                    currentGameObject = (GameObject) Instantiate(p, mousePos, Quaternion.identity);
                    foreach(Transform child in currentGameObject.transform){
                        Destroy(child.gameObject);
                    }
                    currentGameObject.transform.parent = p.transform;
                    currentGameObject.transform.localScale = new Vector3(1,1,1);
                    createLineRenderer(currentGameObject, p.transform.position);
                }
            }
        }
    }


    LineRenderer createLineRenderer(GameObject currentObj, Vector3 startPos){
        LineRenderer lineRenderer = currentObj.GetComponent<LineRenderer>();
        if(lineRenderer != null){    
            lineRenderer.enabled = true;
        }
        else{
            lineRenderer = currentObj.AddComponent<LineRenderer>();
        }
        lineRenderer.SetVertexCount(2);
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, startPos);
        lineRenderer.sortingLayerName = currentObj.GetComponent<SpriteRenderer>().sortingLayerName; 
        return lineRenderer;
    }

    void extendLineRenderer(LineRenderer lineRenderer, GameObject currentObj){
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);  
        GameObject tile = PathFinding.getObjectAtMousePos();
        bool take = false;
        if (tile !=null) take = tile.GetComponent<tile>().takeTile(currentGameObject);
        if(!take){
            Destroy(currentGameObject);
        }
        else{
            MonoBehaviour[] scripts = currentObj.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts) script.enabled = true;
            currentObj.transform.position = tile.transform.position;
            lineFollow(currentGameObject, currentGameObject.transform.position , 1);
        }
        currentObj = null;
    }

    void lineFollow (GameObject currentObj, Vector2 pos, int index){
        LineRenderer lr = currentObj.GetComponent<LineRenderer>();
        lr.sortingOrder = currentObj.GetComponent<SpriteRenderer>().sortingOrder;
        lr.SetPosition(index, pos);
    }

    void enableChildrenLineRenderers(GameObject g, bool enable){
        LineRenderer[] lrs = g.GetComponentsInChildren<LineRenderer>(true);
        foreach (LineRenderer lr in lrs){
            lr.enabled = enable;
        }
    }

	void objLookAt(GameObject g, Vector3 pos){
        pos.x = pos.x - g.transform.position.x;
        pos.z = pos.z - g.transform.position.z;
        float angle = (int) (Mathf.Atan2(pos.x, pos.z) * Mathf.Rad2Deg);
        g.transform.rotation  = Quaternion.Slerp(g.transform.rotation, Quaternion.Euler(new Vector3(0, angle, 0)),1);
    }

    void movePoint(){
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);  
        if(!isDragging){
            GameObject tile = PathFinding.getObjectAtMousePos();
            if (tile != null)
            {
                GameObject p = tile.GetComponent<tile>().getTakenBy();
                if(p != null){
                    isDragging = true;
                    currentGameObject = p;
                    currentChildren = new List<GameObject>();
                    foreach(Transform child in currentGameObject.transform){
                        currentChildren.Add(child.gameObject);
                    }
                    foreach(GameObject child in currentChildren){ 
                        child.transform.parent = currentGameObject.transform.parent;
                    }
                    oldTile = tile;
                }
            }
        }
        else{
            currentGameObject.transform.position = mousePos;

            //Update line renderer pos for parent
            if(currentGameObject.transform.parent != null && currentGameObject.transform.parent.GetComponent<LineRenderer>() != null)  lineFollow(currentGameObject, currentGameObject.transform.position, 1);

            //update line Renderer Positions for children
            foreach(GameObject child in currentChildren){
                lineFollow(child, currentGameObject.transform.position, 0);
            }    
        }
    }

    void dropPoint(){
        GameObject tile = PathFinding.getObjectAtMousePos(); 
        if(tile == null){
            deletePoint(currentGameObject, currentChildren);
            isDragging = false;
            currentGameObject = null;
            oldTile = null;
            return;
        }
        if (tile.GetComponent<tile>().takeTile(currentGameObject)){
            currentGameObject.transform.position = tile.transform.position;
            oldTile.GetComponent<tile>().leaveTile();
        }
        else {
            currentGameObject.transform.position = oldTile.transform.position;
        }

        //reset children to be attached to the spell 
        foreach(GameObject child in currentChildren){
            child.transform.parent = currentGameObject.transform;
        }

        //Update line renderer pos for parent
        if(currentGameObject.transform.parent != null && currentGameObject.transform.parent.GetComponent<LineRenderer>() != null)  lineFollow(currentGameObject, currentGameObject.transform.position, 1);
        //update line Renderer Positions for children
        foreach(GameObject child in currentChildren){
            lineFollow(child, currentGameObject.transform.position, 0);
        }  

        oldTile = null;
        isDragging = false;
        currentGameObject = null;   
    }

    void deletePoint(GameObject point, List<GameObject> children)
    {
        foreach (GameObject child in children)
        {
            lineFollow(child, point.transform.parent.position, 0);
        }
        oldTile.GetComponent<tile>().leaveTile();
        Destroy(point);
        return;
    }

    void showOptions(GameObject g){
        Vector3 center = g.transform.position;
        for (int i = 0; i < durations.Count; i++){

            float j = (i * 1.0f) / 6;
            float angle = j * Mathf.PI * 2;

            float x = Mathf.Sin(angle)/2;
            float y = Mathf.Cos(angle)/2;

            Vector3 pos = new Vector3(x, y, 0) + center;

            durations[i].transform.position = pos;
            durations[i].SetActive(true);
            durationsAreShown = true;
        }
    }
    public void hideOptions(){
        for (int i = 0; i < durations.Count; i++){
            durations[i].SetActive(false);
        }
        durationsAreShown = false;
    }
    
    public void modifyPointDuration(int i){
        hoveredPoint.GetComponent<SpellPoint>().duration = i;
    }
}
