using UnityEngine;
using System.Collections;

public class mobWorld : MonoBehaviour {

	// Use this for initialization
	public float speed;
	public GameObject spellCreator;
	public GameObject overallManager;
	public GameObject player;
	public float range;
	float step;
	int groundLayer;
    int groundLayerMask;
    bool shouldWalk;
    Vector2 target;

	void Start () {
		step = speed * Time.deltaTime;   
        groundLayer = spellCreator.GetComponent<SpellCreator>().groundLayer;
        groundLayerMask = 1<<groundLayer;
	}
	
	// Update is called once per frame
	void Update () {
    	if(ennemyDetected()){
    		startCombat();
    	}
	}

	void walkTo(Vector2 position){
        transform.position = Vector3.MoveTowards(transform.position, position, step);
        Vector2 p = new Vector2(transform.position.x, transform.position.y);
        if(p == target){
        	shouldWalk = false;
        }
	}

	void startCombat(){
		GetComponent<mobCombat>().enabled = true;
		overallManager.GetComponent<overallManager>().startCombat(transform.parent.gameObject);
	}
	bool ennemyDetected(){
		if(Vector2.Distance(player.transform.position,transform.position) < range) return true;
		else return false;
	}
}
