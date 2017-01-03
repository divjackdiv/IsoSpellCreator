using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Any and all code which the mob should be able to access at any time
public class mobOverall : MonoBehaviour
{

    //Movement Management
    public float walkSpeed;
    List<GameObject> path;
    bool shouldWalk;
    GameObject currentGoal; //next tile normally
    GameObject currentTile;
    int range;

    void Start()
    {
        currentTile = StaticFunctions.getTileAt(transform.position);
        shouldWalk = false;
        path = new List<GameObject>();
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
        range = GetComponent<mobStats>().range;
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
            if (GetComponent<mobCombat>().enabled)
            {
                GetComponent<mobCombat>().finishedWalking();
            }
            else
            {
                GetComponent<mobWorld>().finishedWalking();
            }
            return;
        }
        else if (currentGoal == null)
        {
            currentGoal = path[0];
            GameObject goalTile = StaticFunctions.getTileAt(currentGoal.transform.position);
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
    }
    public bool isWalking()
    {
        return shouldWalk;
    }

    public void moveToNearestTile()
    {
        GameObject currentTile = StaticFunctions.getTileAt(transform.position);
        GameObject nearestTile = currentTile;
        if (currentTile.GetComponent<tile>().taken && !currentTile.GetComponent<tile>().takenBy == gameObject)
        {
            nearestTile = StaticFunctions.findNearestFreeTile(currentTile);
        }           
        path = StaticFunctions.aStarPathFinding(currentTile, nearestTile);
        shouldWalk = true;
    }
    
    public bool takeTile(Vector2 pos)
    {
        GameObject newTile = StaticFunctions.getTileAt(pos);
        bool moved = newTile.GetComponent<tile>().takeTile(gameObject);
        if (moved)
        {
            GameObject currentTile = StaticFunctions.getTileAt(transform.position);
            currentTile.GetComponent<tile>().leaveTile();
            return true;
        }
        return false;
    }
}