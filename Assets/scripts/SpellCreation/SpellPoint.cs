using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpellPoint : MonoBehaviour { 
    public int duration;
    public float damage;
    public int cost;
    
    public bool updatePoint(){
        if(duration > 0){
            burn();
            duration--;
        }
        if(duration <= 0){
            //applyEffect();
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
        foreach(Transform child in transform){
            child.gameObject.active = true;
        }
        gameObject.active = false;
        burn();
    }
}
