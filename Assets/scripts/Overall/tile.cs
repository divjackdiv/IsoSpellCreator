using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class tile : MonoBehaviour {


    public GameObject takenBy;
    public bool taken;
    public List<GameObject> smallProps;

	void Start(){
        if (taken)
        {
            if (takenBy == null)
                taken = false;
        }
        resetSortingOrder(transform);
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
    
    public void resetSortingOrder(Transform t)
    {
        if(t.GetComponent<SpriteRenderer>())
            t.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
        foreach(Transform child in t)
        {
            resetSortingOrder(child);
        }
    }
}
