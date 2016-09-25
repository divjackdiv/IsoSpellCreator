using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpellPoint : MonoBehaviour { 
    public int duration;
    public float damage;
    public float movementSpeed = 1;
    public int cost;
    bool moving;
    bool playing;
    Vector2 target;

    GameObject branch;

    void Update(){
        if(moving){
            if(moveTo(target)){
                moving = false;
            }
        }
        else if(playing){
            burn();
            finished();  
        }
    }

    public void updatePoint(GameObject b){
        branch = b;
        b.GetComponent<SpellBranch>().currentPoints.Add(transform);
        target = transform.position;
        if(!gameObject.activeSelf){
            transform.position = transform.parent.GetComponent<SpellPoint>().target;
            transform.parent = b.transform;
            gameObject.active = true;
            moving = true; 
        } 
        playing = true;
        duration--;
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

    public bool moveTo(Vector2 pos){
        transform.position = Vector3.MoveTowards(transform.position, pos, movementSpeed * Time.deltaTime);
        Vector2 p = new Vector2(transform.position.x, transform.position.y);
        if( p == pos) return true;
        return false;
    }

    void finished(){
        if(duration >= 0) branch.GetComponent<SpellBranch>().shouldDelete = false;
        branch.GetComponent<SpellBranch>().currentPoints.Remove(transform);
        if(duration <= 0){
            List<Transform> temp = new List<Transform>();
            foreach(Transform child in transform){
                temp.Add(child);
            }
            foreach(Transform child in temp){
                child.GetComponent<SpellPoint>().updatePoint(branch);
            }
            Destroy(gameObject);
        }
    }
}
