using UnityEngine;
using System.Collections;

public class tile : MonoBehaviour {


    public GameObject takenBy;
    public bool taken;

	void Start(){
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
	}

	public bool takeTile(GameObject g){
		if (taken) return false;
		taken = true;
		takenBy = g;
		return true;
	}

	public bool leaveTile(){
		if(taken){
			taken = false;
			takenBy = null;
			return true;
		}
		return false;
	}

	public GameObject getTakenBy(){
		return takenBy;
	}


}
