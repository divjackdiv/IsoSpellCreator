﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mobWorld : MonoBehaviour {

	// Use this for initialization
	public GameObject spellCreator;
	public GameObject overallManager;
	public GameObject player;
	private float detectionRange;
    int state;
    public List<GameObject> roamingTargets;
    public int targetIndex = 0;
    bool waiting;

    void Start () {
        detectionRange = GetComponent<mobStats>().detectionRange;
		state = 0;
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
                state = 0;
                waiting = true;
                GameObject currentTile = StaticFunctions.getTileAt(transform.position);
                List<GameObject> path = StaticFunctions.aStarPathFinding(currentTile, roamingTargets[targetIndex++]); //Can be stored for more efficiency //
                if (targetIndex >= roamingTargets.Count) targetIndex = 0;
                GetComponent<mobOverall>().updatePath(path);                
            }
		}
	}

	void startCombat(){
        GetComponent<mobOverall>().updatePath(new List<GameObject>());
        overallManager.GetComponent<overallManager>().startCombat(transform.parent.gameObject);
	}

	bool ennemyDetected(){
		if(Vector2.Distance(player.transform.position,transform.position) < detectionRange) return true;
		else return false;
	}

    public void finishedWalking()
    {
        waiting = false;
    }

    IEnumerator waitThenWalk(){
		waiting = true;
		yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
		waiting = false;
		state = 1;
	}
}