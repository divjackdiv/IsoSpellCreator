using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class playerWorld : MonoBehaviour {

    List<GameObject> path;
    int groundLayerMask;
    bool creatingSpell;


    void Start () {
        path = new List<GameObject>();
        groundLayerMask = 1<<gridManager.groundLayerS;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1")) { 
            if (!EventSystem.current.IsPointerOverGameObject()){
        		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        		GameObject g = PathFinding.getTileAt(mousePos);
        		if(g != null){
                    GameObject nextTile = null;
                    if (path.Count > 0)
                    {
                        nextTile = path[0];
                    }
                    path = PathFinding.aStarPathFinding(PathFinding.getTileAt(transform.position), g);
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
