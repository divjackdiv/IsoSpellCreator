using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class inputManager : MonoBehaviour {

	// Use this for initialization
    public GameObject spellCreator;
	public int layerToPickUp;
    public List<GameObject> durationGameObjects;
    public float hoverDist; 
    //gameobjects that will be seen when hovering above a point, when pressed they change that point's duration
    List<GameObject> durations;
    bool durationsAreShown;
    Point hoveredPoint;

    int groundLayer;
    int groundLayerMask;
	int layerMask; //this is the spell layer mask
	bool isDragging;
    GameObject oldTile;
	bool draggingFromBar;
    bool extending;
    
    int currentVertex;
    GameObject currentGameObject;

	void Start () {
        groundLayer = spellCreator.GetComponent<SpellCreator>().groundLayer;
        groundLayerMask = 1<<groundLayer;
		layerMask = 1<<layerToPickUp;
        durations = new List<GameObject>();
        for (int i = 0; i < durationGameObjects.Count; i++){
            GameObject dur = (GameObject) Instantiate (durationGameObjects[i], new Vector3(0,0,0), Quaternion.identity);
            dur.GetComponent<durationOfPoint>().inputManager = gameObject;
            dur.active = false;
            dur.transform.localScale = new Vector3(0.07F, 0.07F, 0.07F);
            durations.Add(dur);
        }
        durationsAreShown = false;
	}
	
	// Update is called once per frame
	void Update () {

        //Drag an already existing GameObject
        if (Input.GetButton("Fire1")){
            if (EventSystem.current.IsPointerOverGameObject() || draggingFromBar){
                draggingFromBar = true;
                return;
            }
        	movePoint();
            hideOptions();
        }
        //if you were previously dragging, but not anymore DROP
        else if(isDragging){
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
                        GameObject takenBy = hit.collider.gameObject.GetComponent<tile>().getTakenBy();
                        if(takenBy.GetComponent<SpellPoint>() != null){
                            hoveredPoint = takenBy.GetComponent<SpellPoint>().point;
                            showOptions(hoveredPoint);
                        } 
                    }
                }
            }
            else {
                if(durationsAreShown){
                    if(Vector3.Distance(hoveredPoint.getGameObject().transform.position, mousePos) > hoverDist){
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
            GameObject tile = StaticFunctions.getObjectAtMousePos(groundLayerMask);
            if (tile != null)
            {
                GameObject p = tile.GetComponent<tile>().getTakenBy();
                if(p != null){
                    extending = true;
                    GameObject clickedPoint = p;
                    Branch branch = clickedPoint.GetComponent<SpellPoint>().point.getBranch();   
                    currentGameObject = (GameObject) Instantiate(clickedPoint, mousePos, Quaternion.identity);
                    currentGameObject.transform.parent = branch.getGameObject().transform;
                    spellCreator.GetComponent<SpellCreator>().createPoint(branch, currentGameObject, clickedPoint.transform.position, clickedPoint.GetComponent<SpellPoint>().point);
                    enableChildrenLineRenderers(currentGameObject.GetComponent<SpellPoint>().point.getBranch().getGameObject(), true);
                    createLineRenderer(currentGameObject, clickedPoint.transform.position);
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
        return lineRenderer;
    }

    void extendLineRenderer(LineRenderer lineRenderer, GameObject currentObj){
        lineRenderer.enabled = false;
        enableChildrenLineRenderers(currentGameObject.GetComponent<SpellPoint>().point.getBranch().getGameObject(), false);
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);  
        GameObject tile = StaticFunctions.getObjectAtMousePos(groundLayerMask);
        bool take = false;
        if (tile !=null) take = tile.GetComponent<tile>().takeTile(currentGameObject);
        if(!take){
            //severe the link from the parent 
            Point p = currentObj.GetComponent<SpellPoint>().point;
            deletePoint(p);
            return;
        }
        else{
            MonoBehaviour[] scripts = currentObj.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts) script.enabled = true;
            currentObj.transform.position = tile.transform.position;
            currentObj.GetComponent<SpellPoint>().point.changePos(mousePos);
        }
        currentObj = null;
    }

    void lineFollow (GameObject currentObj, Vector2 pos, int index){
        LineRenderer lr = currentObj.GetComponent<LineRenderer>(); 
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
            GameObject tile = StaticFunctions.getObjectAtMousePos(groundLayerMask);
            if (tile != null)
            {
                GameObject p = tile.GetComponent<tile>().getTakenBy();
                if(p != null){
                    isDragging = true;
                    currentGameObject = p;
                    oldTile = tile;
                }
            }
        }
        else{
            currentGameObject.transform.position = mousePos;

            Point currentPoint = currentGameObject.GetComponent<SpellPoint>().point;
            List<Point> children = currentGameObject.GetComponent<SpellPoint>().point.getChildren();
            Branch b = currentPoint.getBranch();

            //Update line renderer pos for parent
            if(currentPoint.parent != null && currentPoint.parent.getGameObject() != null) lineFollow(currentPoint.getGameObject(), currentGameObject.transform.position, 1);

            //update line Renderer Positions for children
            foreach(Point p in children){
                GameObject nextObj = p.getGameObject();
                lineFollow(nextObj, currentGameObject.transform.position, 0);
            }    

            //enable the branch line renderer
            enableChildrenLineRenderers(b.getGameObject(), true);
        }
    }

    void dropPoint(){
        GameObject tile = StaticFunctions.getObjectAtMousePos(groundLayerMask); 
        if(tile == null){
            deletePoint(currentGameObject.GetComponent<SpellPoint>().point);
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
        Point currentPoint = currentGameObject.GetComponent<SpellPoint>().point;
        List<Point> children = currentGameObject.GetComponent<SpellPoint>().point.getChildren();
        Branch b = currentPoint.getBranch();

        //Update line renderer pos for parent
        if(currentPoint.parent != null && currentPoint.parent.getGameObject() != null) lineFollow(currentPoint.getGameObject(), currentGameObject.transform.position, 1);

        //update line Renderer Positions for children
        foreach(Point p in children){
            GameObject nextObj = p.getGameObject();
            lineFollow(nextObj, currentGameObject.transform.position, 0);
        }    

        //disable the branch line renderer
        enableChildrenLineRenderers(b.getGameObject(), false);
        oldTile = null;
        isDragging = false;
        currentGameObject = null;   
    }

    void deletePoint(Point point){
        if(point == point.branch.root) deleteBranch(point.branch);
        List<Point> children = point.getChildren();
        for(int i = 0; i < children.Count;){
            deletePoint(children[0]);
        }  
        if (point.parent != null){
            point.parent.children.Remove(point);
        } 
        else{
            Destroy(point.getBranch().getGameObject());
        }
        GameObject tile = StaticFunctions.getObjectAt(point.getPosition(), groundLayerMask);
        tile.GetComponent<tile>().leaveTile();
        Destroy(point.getGameObject());
    }

    void deleteBranch(Branch branch){
        branch.spell.branches.Remove(branch);
        Destroy(branch.getGameObject());
    }

    void showOptions(Point point){
        Vector3 center = point.getGameObject().transform.position;
        for (int i = 0; i < durations.Count; i++){

            float j = (i * 1.0f) / 6;
            float angle = j * Mathf.PI * 2;

            float x = Mathf.Sin(angle)/2;
            float y = Mathf.Cos(angle)/2;

            Vector3 pos = new Vector3(x, y, 0) + center;

            durations[i].transform.position = pos;
            durations[i].active = true;
            durationsAreShown = true;
        }
    }
    public void hideOptions(){
        for (int i = 0; i < durations.Count; i++){
            durations[i].active = false;
        }
        durationsAreShown = false;
    }
    
    public void modifyPointDuration(int i){
        hoveredPoint.changeDuration(i);
    }

}
