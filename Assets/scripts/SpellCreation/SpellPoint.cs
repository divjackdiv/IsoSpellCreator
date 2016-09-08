using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpellPoint : MonoBehaviour { 
    public GameObject originalCopy;
    public Point point;

    public bool updatePoint(){
        print("             Point " + point.duration);
    	if(point.duration > 0) point.duration--;
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
    void explode(){
        foreach(Point child in point.children){
            child.getGameObject().active = true;
        }
    	point.getGameObject().active = false;
        print("exploded");
    }
}