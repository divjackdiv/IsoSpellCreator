using UnityEngine;
using System.Collections;

public class gridManager : MonoBehaviour {

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
