using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class playerWorld : MonoBehaviour {

	// Use this for initialization
	public GameObject spellCreator;
	int groundLayer;
    int groundLayerMask;
    bool creatingSpell;


	void Start () {
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
                    transform.GetComponent<playerOverall>().updatePath(StaticFunctions.getPath(StaticFunctions.getTileAt(transform.position), g));
        		}
        	}
        }
	}
}
