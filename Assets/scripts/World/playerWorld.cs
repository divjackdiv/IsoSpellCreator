using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class playerWorld : MonoBehaviour {

	// Use this for initialization
	public GameObject player;
	public float walkSpeed;
	public GameObject spellCreator;
	float step;
	int groundLayer;
    int groundLayerMask;
    bool shouldWalk;
    Vector2 target;

	void Start () {
		step = walkSpeed * Time.deltaTime;   
        groundLayer = spellCreator.GetComponent<SpellCreator>().groundLayer;
        groundLayerMask = 1<<groundLayer;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Fire1")){
        	if (!EventSystem.current.IsPointerOverGameObject()){
        		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        		GameObject g = StaticFunctions.getTileAt(mousePos);
        		if(g != null){	
	        		target = g.transform.position;
	        		shouldWalk = true;
        		}
        	}
        }
        if(shouldWalk){
        	walkTo(target);
        }
	}

	void walkTo(Vector2 position){
        player.transform.position = Vector3.MoveTowards(player.transform.position, position, step);
        Vector2 p = new Vector2(player.transform.position.x, player.transform.position.y);
        if( p == position) shouldWalk = false;
	}
}
