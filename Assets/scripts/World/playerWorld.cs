using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class playerWorld : MonoBehaviour {

	// Use this for initialization
	public float walkSpeed;
	public GameObject spellCreator;
	List<GameObject> path;
	int groundLayer;
    int groundLayerMask;
    bool shouldWalk;
    bool creatingSpell;


	void Start () {
		path = new List<GameObject>();
        groundLayer = spellCreator.GetComponent<SpellCreator>().groundLayer;
        groundLayerMask = 1<<groundLayer;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1")){
        	if (!EventSystem.current.IsPointerOverGameObject()){
        		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        		GameObject g = StaticFunctions.getTileAt(mousePos);
        		if(g != null){	
        			path = StaticFunctions.getPath(StaticFunctions.getTileAt(transform.position), g);
	        		shouldWalk = true;
        		}
        	}
        }
        if(shouldWalk){
        	walkAlongPath();
        }
	}

	void walkAlongPath(){
		if(path.Count <= 0){
			shouldWalk = false;
			return;
		}
		if(walkTo(path[0].transform.position)){
			path.RemoveAt(0);
		}
	}

	bool walkTo(Vector2 position){
        transform.position = Vector3.MoveTowards(transform.position, position, walkSpeed * Time.deltaTime);
        Vector2 p = new Vector2(transform.position.x, transform.position.y);
        if(p == position){
        	return true;
        }
        return false;
	}
}
