using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mobWorld : MonoBehaviour {

	// Use this for initialization
	public float speed;
	public GameObject spellCreator;
	public GameObject overallManager;
	public GameObject player;
	public float range;
	int groundLayer;
    int groundLayerMask;
    int state;
    public List<GameObject> roamingTargets;
    public int targetIndex = 0;
    Animator animator;
    bool waiting;

	void Start () {
		animator = GetComponent<Animator>();
		state = 0;
        groundLayer = spellCreator.GetComponent<SpellCreator>().groundLayer;
        groundLayerMask = 1<<groundLayer;
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
    }
	
	void Update () {
		if(ennemyDetected()){
    		startCombat();
    	}
    	else{
			if (state == 0){
				if(!waiting) StartCoroutine(waitThenWalk());
			}
			else if(state == 1){
				moveTo(roamingTargets[targetIndex].transform.position);
                GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
            }
		}
	}

	void moveTo(Vector2 position){
        transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
        Vector2 p = new Vector2(transform.position.x, transform.position.y);
		animator.SetInteger("state", 1);	//State 1 is walking
        if(p == position){
        	state = 0;
			animator.SetInteger("state", 0); 	//State 0 is Idle
        	if(targetIndex < roamingTargets.Count - 1) targetIndex++;
			else targetIndex = 0;
        }
	}


	void startCombat(){
		overallManager.GetComponent<overallManager>().startCombat(transform.parent.gameObject);
	}

	bool ennemyDetected(){
		if(Vector2.Distance(player.transform.position,transform.position) < range) return true;
		else return false;
	}


	IEnumerator waitThenWalk(){
		waiting = true;
		yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
		waiting = false;
		state = 1;
	}
}
