using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpellPoint : MonoBehaviour {

    public GameObject tileOverlay;
    public float movementSpeed = 1;
    public int duration;
    public int damage;
    public int cost;
    public int spriteIndex;
    public int gameObjectIndex;

    Animator animator;
    GameObject branch;
    GameObject currentTile;
    Vector2 target;
    float animCounter;
    int s;
    int state;
    bool moving;
    bool playing;
    bool appearing;

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
                GameObject tile = PathFinding.getTileAt(transform.position);
                if (tile != currentTile && tileOverlay != null)
                {
                    currentTile = tile;

                    GameObject tileOvl = (GameObject) Instantiate(tileOverlay);
                    tileOvl.transform.parent = currentTile.transform;
                    tileOvl.transform.localPosition = new Vector3(0, 0, 0);
                    List<int> animationStates = new List<int>();
                    animationStates.Add(1);
                    tileOvl.GetComponent<tileOverlay>().changeAnim(animationStates,true);
                }
            }
            GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1; 
        }
        else if(animator != null && appearing){
            if (state != 1)
            {
                animCounter = 0;
                state = 1;
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                if (animator != null) animator.SetInteger("state", 1);
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
            if (animator != null) animator.SetInteger("state", 0);    // 0 is idle
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
        currentTile = PathFinding.getTileAt(transform.position);
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
        currentTile = PathFinding.getTileAt(transform.position);
        if(currentTile.GetComponent<tile>().taken){
            GameObject takenBy = currentTile.GetComponent<tile>().takenBy;
            if (takenBy != null && takenBy.tag == "mob"){
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
