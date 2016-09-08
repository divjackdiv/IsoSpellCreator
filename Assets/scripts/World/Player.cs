using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	// Use this for initialization
	public GameObject player;
	public float speed;
	public GameObject spellCreator;
	float step;
	int groundLayer;
    int groundLayerMask;
    public bool shouldWalk;
    Vector2 target;

	void Start () {
		step = speed * Time.deltaTime;   
        groundLayer = spellCreator.GetComponent<SpellCreator>().groundLayer;
        groundLayerMask = 1<<groundLayer;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Fire1")){
        	target = StaticFunctions.getObjectAtMousePos(groundLayerMask).transform.position;
        	shouldWalk = true;
        }
        if(shouldWalk){
        	walkTo(target);
        }
	}

	void walkTo(Vector2 position){
        player.transform.position = Vector3.MoveTowards(player.transform.position, position, step);
        Vector2 p = new Vector2(player.transform.position.x, player.transform.position.y);
        if( p == target) shouldWalk = false;
	}
}
