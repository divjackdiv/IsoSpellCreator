using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mobWorld : MonoBehaviour {

    public int roamingRange; //how far should the mob be able to roam from it's anchor, in tiles
    public float waitTime;

    private GameObject overallManager;
    GameObject roamingAnchor;
    GameObject roamingTarget;
    GameObject player;
    float detectionRange;
    int state;
    bool waiting;

    void Start ()
    {
        overallManager = GetComponent<mobOverall>().overallManager;
        player = overallManager.GetComponent<overallManager>().player;
        roamingAnchor = PathFinding.getTileAt(transform.position);
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
                roam();   
            }
		}
	}

	void startCombat(){
        GetComponent<mobOverall>().updatePath(new List<GameObject>());
        overallManager.GetComponent<overallManager>().startCombat(transform.parent.gameObject);
	}

	bool ennemyDetected(){
		if(PathFinding.IsometricDistance(player.transform.position,transform.position) < detectionRange) return true;
		else return false;
	}

    public void finishedWalking()
    {
        waiting = false;
    }

    void roam()
    {
        state = 0;
        waiting = true;
        GameObject currentTile = PathFinding.getTileAt(transform.position);
        Vector2 roamingTargetPos = roamingAnchor.transform.position;
        roamingTargetPos.x += (Random.Range(-roamingRange * 100, roamingRange * 100) / 200f);
        roamingTargetPos.y += (Random.Range(-roamingRange * 100, roamingRange * 100) / 400f);
        GameObject newTarget= PathFinding.getTileAt(roamingTargetPos);
        if (newTarget != null)
            roamingTarget = PathFinding.findNearestFreeTile(newTarget);
        else
            roamingTarget = PathFinding.findNearestFreeTile(roamingAnchor);
        List<GameObject> path = PathFinding.aStarPathFinding(currentTile, roamingTarget);
        GetComponent<mobOverall>().updatePath(path);
    }

    IEnumerator waitThenWalk(){
		waiting = true;
		yield return new WaitForSeconds(Random.Range(waitTime - waitTime/3, waitTime + waitTime / 3));
		waiting = false;
		state = 1;
	}
}
