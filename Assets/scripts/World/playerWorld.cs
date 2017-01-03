using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class playerWorld : MonoBehaviour {
    
	int groundLayer;
    int groundLayerMask;
    bool creatingSpell;
    List<GameObject> path;


    void Start () {
        path = new List<GameObject>();
        groundLayer = overallManager.groundLayerS;
        groundLayerMask = 1<<groundLayer;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1")) { 
            if (!EventSystem.current.IsPointerOverGameObject()){
        		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        		GameObject g = StaticFunctions.getTileAt(mousePos);
        		if(g != null){
                    GameObject nextTile = null;
                    if (path.Count > 0)
                    {
                        nextTile = path[0];
                    }
                    path =  StaticFunctions.aStarPathFinding(StaticFunctions.getTileAt(transform.position), g);
                    if (nextTile != null)
                    {
                        path.Insert(0,nextTile);
                    }
                    transform.GetComponent<playerOverall>().updatePath(path);
        		}
        	}
        }
	}
}
