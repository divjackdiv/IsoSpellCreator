using UnityEngine;
using System.Collections;

public class mobCombat : MonoBehaviour {

	public float lifePoints;
    public float walkingSpeed; //PURELY COSMETIC
	public GameObject spellCreator;
	public GameObject combatManager;
	GameObject currentTile;

    Animator animator;
    int state;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (state == 0){
			animator.SetInteger("state", 0); 	//State 0 is Idle
		}
		else if(state == 1){
			animator.SetInteger("state", 1);	//State 1 is walking
			moveTo(currentTile.transform.position);
		}
		else if(state == 2){
			animator.SetInteger("state", 2);	//State 2 is attacking
		}
		else if(state == 3){
			animator.SetInteger("state", 3);	//State 3 is dying 
			if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0)){
				die();
				endPlay();
			}
			
		}
	}
	public void takeDamage(float damage){
		lifePoints -= damage;
		if(lifePoints <= 0){
			state = 3;
		}
	}

	public void die(){
		currentTile.GetComponent<tile>().leaveTile();
		combatManager.GetComponent<CombatManager>().characters.Remove(gameObject);
		endPlay();
		Destroy(gameObject);
	}

	public void startCombat(){
		GetComponent<mobWorld>().enabled = false;
	}

	public void play(){
		currentTile = StaticFunctions.getTileAt(transform.position);
		currentTile.GetComponent<tile>().takeTile(gameObject);
        state = 0;
		changePos(new Vector2(transform.position.x - 0.5f, transform.position.y -0.25f));
	}

	public void changePos(Vector2 pos){
		GameObject newTile = StaticFunctions.getTileAt(pos);
		bool moved = newTile.GetComponent<tile>().takeTile(gameObject);
		if (moved){
			currentTile.GetComponent<tile>().leaveTile();
			currentTile = newTile;
			state = 1;
		}
	}

	public void moveTo(Vector2 pos){
		float step = walkingSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, pos, step);
        Vector2 currentPos = transform.position;
        if(currentPos == pos){
        	state = 0;
        	endPlay();
        }
	}


	public void endPlay(){
		animator.SetInteger("state", 0); 	//State 0 is Idle
		combatManager.GetComponent<CombatManager>().finishedPlaying();
	}
}

