using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class playerWorld : MonoBehaviour {

    List<GameObject> path;
    bool creatingSpell;


    void Start () {
        path = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1")) { 
            if (!EventSystem.current.IsPointerOverGameObject()){
        		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        		GameObject g = PathFinding.getTileAt(mousePos);
        		if(g != null){
                    path = PathFinding.aStarPathFinding(PathFinding.getTileAt(transform.position), g);
                    GameObject currentTile = PathFinding.getTileAt(transform.position);
                    if (path[0].transform.position != currentTile.transform.position) //make sure the palyer cannot diagonally
                    {
                        float tileDist = PathFinding.IsometricDistance(currentTile.transform.position, path[0].transform.position); //distance between current tile and next tile
                        float playerToNextTileDist = PathFinding.IsometricDistance(transform.position, path[0].transform.position); //distance between player and next tile
                        if (tileDist < playerToNextTileDist) {   //ensures that the player does not need to go back to a tile if it is not on it's way
                            path.Insert(0, currentTile);
                        }
                    }
                    transform.GetComponent<playerOverall>().updatePath(path);
        		}
        	}
        }
	}
}
