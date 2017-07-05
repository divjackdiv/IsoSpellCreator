using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Any and all code which the mob should be able to access at any time
public class mobOverall : MonoBehaviour
{
    
    public float walkSpeed;

    public GameObject overallManager;
    GameObject combatManager;
    Animator animator;
    List<GameObject> path;
    GameObject currentGoal; //next tile normally
    GameObject currentTile;
    bool shouldWalk;
    bool movingToNearestTile;
    
    void Start()
    {
        if(overallManager == null)
            overallManager = GameObject.Find("overallManager");
        combatManager = overallManager.GetComponent<overallManager>().combatManager;
        animator = GetComponent<Animator>();
        currentTile = PathFinding.getTileAt(transform.position);
        shouldWalk = false;
        path = new List<GameObject>();
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
    }

    void Update()
    {
        if (shouldWalk)
        {
            walkAlongPath();
            GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1; 
        }
    }

    void walkAlongPath()
    {
        if (path.Count <= 0)
        {
            shouldWalk = false;
            animator.SetInteger("state", 0);
            if (GetComponent<mobCombat>().enabled)
            {
                GetComponent<mobCombat>().finishedWalking();
            }
            else
            {
                GetComponent<mobWorld>().finishedWalking();
            }
            if (movingToNearestTile)
            {
                combatManager.GetComponent<CombatManager>().characterReady();
                movingToNearestTile = false;
            }
            return;
        }
        else if (currentGoal == null)
        {
            currentGoal = path[0];
            GameObject goalTile = PathFinding.getTileAt(currentGoal.transform.position);
            if (goalTile.GetComponent<tile>().takeTile(gameObject))
            {
                if (currentTile != goalTile) currentTile.GetComponent<tile>().leaveTile();
            }
            else
            {
                path.Clear();
                currentGoal = null;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, currentGoal.transform.position, walkSpeed * Time.deltaTime);
            if (transform.position == currentGoal.transform.position)
            {
                currentTile = currentGoal;
                path.RemoveAt(0);
                currentGoal = null;
            }
        }
    }

    public void updatePath(List<GameObject> newPath)
    {
        path = newPath;
        shouldWalk = true;
        animator.SetInteger("state", 1);
    }
    public bool isWalking()
    {
        return shouldWalk;
    }

    public void moveToNearestTile()
    {
        GameObject currentTile = PathFinding.getTileAt(transform.position);
        GameObject nearestTile = currentTile;
        if (currentTile.GetComponent<tile>().taken && !currentTile.GetComponent<tile>().takenBy == gameObject)
        {
            nearestTile = PathFinding.findNearestFreeTile(currentTile);
        }           
        path = PathFinding.aStarPathFinding(currentTile, nearestTile);
        shouldWalk = true;
        movingToNearestTile = true;
    }
    
    public bool takeTile(Vector2 pos)
    {
        GameObject newTile = PathFinding.getTileAt(pos);
        bool moved = newTile.GetComponent<tile>().takeTile(gameObject);
        if (moved)
        {
            GameObject currentTile = PathFinding.getTileAt(transform.position);
            currentTile.GetComponent<tile>().leaveTile();
            return true;
        }
        return false;
    }
}