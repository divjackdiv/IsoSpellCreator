using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class gridManager : MonoBehaviour {

	//A grid cell is x = 0.5 and y = 0.25 away from another cell, knowing this, you can get adjacent cells with a raycast
	public GameObject grid;
    public int groundLayer;
    public static int groundLayerS;

    public void Awake()
    {
        groundLayerS = groundLayer;
    }

    public void clearGrid(){
		clearTile(grid);
	}

	public void clearTile(GameObject tile){
		if(tile.GetComponent<tile>() != null){
			tile.GetComponent<tile>().leaveTile();
		}
		foreach(Transform child in tile.transform){
			clearTile(child.gameObject);
		}
	}

    //This is called when dev presses 't' in scene view, creates a tile at mouse pos if there is space
    public static void createTileSceneView(Object tile, Vector2 position)
    {
        position = nearestTilePos(position);
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity);
        if (hit.collider != null)
        {
            return;
        }
        else
        {
            GameObject t = (GameObject) Instantiate(tile, position, Quaternion.identity);
            GameObject g = GameObject.FindGameObjectWithTag("TheGrid");
            if (g != null)
            {
                t.transform.parent = g.transform;
            }
        }
    }

    //creates an object at mouse pos on tile
    public static void createLargeObjSceneView(Object largeObj, Vector2 position)
    {
        position = nearestTilePos(position);
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity);
        if (hit.collider != null)
        {
            Transform tile = hit.collider.transform;
            if (tile.GetComponent<tile>().getTakenBy() == null)
            {
                GameObject t = (GameObject)Instantiate(largeObj, position, Quaternion.identity);
                tile.GetComponent<tile>().takeTile(t);
                t.transform.parent = hit.collider.transform;
            }
        }
    }

    public static void createCharacterSceneView(Object largeObj, Vector2 position)
    {
        position = nearestTilePos(position);
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity);
        if (hit.collider != null)
        {
            Transform tile = hit.collider.transform;
            if (tile.GetComponent<tile>().getTakenBy() == null)
            {
                GameObject t = (GameObject)Instantiate(largeObj, position, Quaternion.identity);
                tile.GetComponent<tile>().takeTile(t);
            }
        }
    }

    public static void createSmallObjSceneView(Object largeObj, Vector2 position)
    {
        position = nearestTilePos(position);
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity);
        if (hit.collider != null)
        {
            Transform tile = hit.collider.transform;
            if (tile.GetComponent<tile>().smallProps.Count < 3)
            {
                GameObject t = (GameObject)Instantiate(largeObj, position, Quaternion.identity);
                t.transform.parent = hit.collider.transform;
                tile.GetComponent<tile>().smallProps.Add(t);
            }
        }
    }

    //returns the nearest tile to the position, even if it's taken
    public static Vector2 nearestTilePos(Vector2 pos)
    {
        Vector2 nearestPos = new Vector2(0,0);
        float tileWidth = 0.5f;
        float tileHeight = 0.25f;
        float ratioX = Mathf.Round((pos.x / tileWidth + pos.y / tileHeight) / 2);
        float ratioY = Mathf.Round((pos.y / tileHeight - (pos.x / tileWidth)) / 2);
        nearestPos.x = (ratioX - ratioY) * tileWidth; 
        nearestPos.y = (ratioX + ratioY ) * tileHeight;
        return nearestPos;
    }
}
