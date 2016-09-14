using UnityEngine;
using System.Collections;

public class gridManager : MonoBehaviour {

	//A grid cell is x = 0.5 and y = 0.25 away from another cell, knowing this, you can get adjacent cells with a raycast
	public GameObject grid;

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
}
