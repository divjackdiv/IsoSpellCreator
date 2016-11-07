using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpellPoint : MonoBehaviour { 
    public int duration;
    public int damage;
    public float movementSpeed = 1;
    public int cost;
    bool moving;
    bool playing;
    Vector2 target;
    Animator animator;
    GameObject branch;
    GameObject currentTile;
    int state;
    bool appearing;
    float animCounter;
    int s;

    void Start()
    {
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
        state = 0;
        s = -4;
        animator = GetComponent<Animator>();
    }

    void Update(){
        if(s != state ){
            s = state;
        } 
        if(moving){
            if(moveTo(target)){
                moving = false;
                appearing = true;
            }
            else {
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                //state = 2;
                // animator.SetInteger("state", 2);    // 2 is moving
                GameObject tile = StaticFunctions.getTileAt(transform.position);
                if (tile != currentTile){
                    currentTile = tile;
                    currentTile.GetComponent<tile>().changeAnim(1,true,0);
                }
            }
            GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1; 
        }
        else if(appearing){
            if (state != 1)
            {
                animCounter = 0;
                state = 1;
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                animator.SetInteger("state", 1);
            }
            else
            {
                animCounter += Time.deltaTime;
            }
            if (animCounter >= animator.GetCurrentAnimatorStateInfo(0).length +0.2){
                appearing = false;
            }
        }
        else if(playing){
            state = 0;
            animator.SetInteger("state", 0);    // 0 is idle
            burn();
            finished();
        }
    }

    public void updatePoint(GameObject b){
        state = 0;
        branch = b;
        b.GetComponent<SpellBranch>().currentPoints.Add(transform);
        target = transform.position;
        if (transform.parent.GetComponent<SpellPoint>())
        {
            transform.position = transform.parent.GetComponent<SpellPoint>().target;
        }
        transform.parent = b.transform;
        gameObject.SetActive(true);
        moving = true; 
        appearing = true;
        currentTile = StaticFunctions.getTileAt(transform.position);
        playing = true;
        duration--;
    }

    public void initPoint(){
        animator = GetComponent<Animator>();
        state = 0;
        target = transform.position;
        appearing = false;
        moving = false;
        playing = false;
    }

    void burn(){
        currentTile = StaticFunctions.getTileAt(transform.position);
        if(currentTile.GetComponent<tile>().taken){
            GameObject takenBy = currentTile.GetComponent<tile>().takenBy;
            if (takenBy.tag == "mob"){
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
        if (duration > 0)
        {
            branch.GetComponent<SpellBranch>().shouldDelete = false;
        }
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
