using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Any and all code which the player should be able to use at any time
public class playerOverall : MonoBehaviour {

    //Movement Management
    public float walkSpeed;
    List<GameObject> path;
    bool shouldWalk;
    float step;

    void Start()
    {
        shouldWalk = false;
        path = new List<GameObject>();
        step = walkSpeed * Time.deltaTime;
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
            return;
        }
        if (walkTo(path[0].transform.position))
        {
            path.RemoveAt(0);
        }
    }

    bool walkTo(Vector2 position)
    {
        transform.position = Vector3.MoveTowards(transform.position, position, step);
        Vector2 p = new Vector2(transform.position.x, transform.position.y);
        if (p == position)
        {
            return true;
        }
        return false;
    }
    public void updatePath(List<GameObject> newPath)
    {
        path = newPath;
        shouldWalk = true;
    }
    public bool isMoving()
    {
        return shouldWalk;
    }

    public void moveToNearestTile()
    {
        GameObject nearestTile = StaticFunctions.getTileAt(transform.position);
        nearestTile.GetComponent<tile>().takeTile(gameObject);
        path = new List<GameObject>();
        path.Add(nearestTile);
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
