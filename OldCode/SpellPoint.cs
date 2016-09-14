using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpellPoint : MonoBehaviour { 
    public GameObject originalCopy;
    public Point point;
    public float damage;
    public GameObject spellCreator;

    public bool updatePoint(){
    	if(point.duration > 0){
            burn();
            point.duration--;
        }
    	if(point.duration <= 0){
    		applyEffect();
            return true;
    	}
        return false;
    }

    //eg explode, poison, dissapear ect
    void applyEffect(){
    	explode();
    }
    void burn(){
        GameObject currentTile = StaticFunctions.getTileAt(transform.position);
        if(currentTile.GetComponent<tile>().taken){
            GameObject takenBy = currentTile.GetComponent<tile>().takenBy;
            if(takenBy.tag == "mob"){
                takenBy.GetComponent<mobCombat>().takeDamage(damage);
            }
        }
    }
    void explode(){
        foreach(Point child in point.children){
            child.getGameObject().active = true;
        }
    	point.getGameObject().active = false;
        burn();
    }
}