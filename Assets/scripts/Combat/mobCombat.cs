using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class mobCombat : MonoBehaviour {

	public int mobType;
	/*Mob types are : 
		0 = passive
		1 = CAC
		2 = purely ranged attacker
		3 = 1 and 2
	*/
	//Following are the mobs' stats
	int currentMovementPoints;
	int currentLifePoints;
	int damage;
	int range;

	public GameObject target;
    public float walkingSpeed; //PURELY COSMETIC
	public GameObject spellCreator;
	public GameObject combatManager;

	GameObject currentTile;
	List<GameObject> path;
	bool shouldAttack;
    Animator animator;
    int state;
    bool playing;
    float animCounter;
	// Use this for initialization
	void Start () {		
		animCounter = 0;
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if(currentLifePoints <= 0){
			animCounter += Time.deltaTime;
			animator.SetInteger("state", 3);	 //animator State 3 is dying 
			if (animCounter >= animator.GetCurrentAnimatorStateInfo(0).length * 2){
				animCounter = 0;
				die();
			}
		}
		else {
			if (state == 0){
				animator.SetInteger("state", 0); 	//State 0 is Idle
			}
			else if(state == 1){
				animator.SetInteger("state", 1);	//State 1 is walking
				walkAlongPath();
			}
			else if(state == 2){
				attack();
			}
			else if(state == 4){	//To avoid any confusion between the animation states and the states used for this AI i will not use the value 3 for state
									// as it is the value for the animation state of dying (see line 41)
				if(playing) endPlay();		   		//State 4 is finishing your turn
				else state = 0;
			}
		}
	} 
	public void takeDamage(int dmg){
		currentLifePoints -= dmg;
	}

	public void die(){
		currentTile.GetComponent<tile>().leaveTile();
		combatManager.GetComponent<CombatManager>().characters.Remove(gameObject);
		if(playing)combatManager.GetComponent<CombatManager>().finishedPlaying();
		Destroy(gameObject);
	}

	public void startCombat(){
		currentLifePoints = gameObject.GetComponent<mobStats>().lifePoints;
		currentMovementPoints = gameObject.GetComponent<mobStats>().movementPoints;
		damage = gameObject.GetComponent<mobStats>().damage;
		range = gameObject.GetComponent<mobStats>().range;
		GetComponent<mobWorld>().enabled = false;
		currentTile = StaticFunctions.getTileAt(transform.position);
		currentTile.GetComponent<tile>().takeTile(gameObject);
		path = new List<GameObject>();
		path.Add(currentTile);
		state = 1;
	}

	public void play(){
		playing = true;
		currentTile = StaticFunctions.getTileAt(transform.position);
		currentTile.GetComponent<tile>().takeTile(gameObject);
        state = 0;
		playMovement();
	}

	void playMovement(){
		path = new List<GameObject>();
        path = StaticFunctions.getPath(StaticFunctions.getTileAt(transform.position), target);
        state = 1;
        if(mobType > 0) shouldAttack = true;
	}
	void attack(){
		if(shouldAttack && isInRange()){
			animator.SetInteger("state", 2);	//State 2 is attacking
			animCounter += Time.deltaTime;
			if (animCounter >= animator.GetCurrentAnimatorStateInfo(0).length){
				animCounter = 0;
				target.GetComponent<playerCombat>().takeDamage(damage);
				shouldAttack = false;
				state = 4;
			}
		}
		else state = 4;
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
        transform.position = Vector2.MoveTowards(transform.position, pos, walkingSpeed * Time.deltaTime);
        Vector2 currentPos = transform.position;
        if(currentPos == pos){
        	state = 0;
        	if (playing) endPlay();
        }
	}

	void walkAlongPath(){
		if(playing && currentMovementPoints <= 0 || playing && path.Count <= 1 || path.Count <= 0){
			if(playing)	state = 2;
			else state = 0;
		}
		else if(walkTo(path[0].transform.position)){
			path.RemoveAt(0);
			if(playing) currentMovementPoints--;
		}
	}

	bool walkTo(Vector2 position){
        transform.position = Vector3.MoveTowards(transform.position, position, walkingSpeed * Time.deltaTime);
        Vector2 p = new Vector2(transform.position.x, transform.position.y);
        if(p == position){
        	return true;
        }
        return false;
	}

	bool isInRange(){
		int cost = StaticFunctions.movementCost(StaticFunctions.getTileAt(transform.position),StaticFunctions.getTileAt(target.transform.position));
		if(cost > range) return false;
		return true;
	}

	public void endPlay(){
		playing = false;
		animator.SetInteger("state", 0); 	//State 0 is Idle
		combatManager.GetComponent<CombatManager>().finishedPlaying();
	}
}

