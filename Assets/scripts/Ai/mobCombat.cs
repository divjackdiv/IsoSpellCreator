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
    GameObject nextTile;
	//List<GameObject> path;
    //GameObject currentGoal;
	bool shouldAttack;
    Animator animator;
    int state;
    bool playing;
    float animCounter;
	// Use this for initialization
    void Awake()
    {
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
            if (state == -1) ;
			else if (state == 0){
				animator.SetInteger("state", 0); 	//State 0 is Idle
			}
			else if(state == 1){
                currentTile = StaticFunctions.getTileAt(transform.position);
                List<GameObject> path = StaticFunctions.aStarPathFinding(currentTile, nextTile);
                int howMuchShouldIWalk = currentMovementPoints;
                if (howMuchShouldIWalk > path.Count - range ) howMuchShouldIWalk = path.Count - range;
                if (howMuchShouldIWalk < 0) howMuchShouldIWalk = 0;
                path = path.GetRange(0, howMuchShouldIWalk);
                GetComponent<mobOverall>().updatePath(path);
                state = -1;
			}
			else if(state == 2){
				attack();
			}
			else if(state == 4){	//To avoid any confusion between the animation states and the states used for this AI i will not use the value 3 for state
									// as it is the value for the animation state of dying (see line 45)
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
		damage = gameObject.GetComponent<mobStats>().damage;
		range = gameObject.GetComponent<mobStats>().range;
	}

	public void play(){
		playing = true;
		currentTile = StaticFunctions.getTileAt(transform.position);
        state = 0;
		playMovement();
	}

	void playMovement()
    {
        nextTile = StaticFunctions.getTileAt(target.transform.position);
        currentMovementPoints = gameObject.GetComponent<mobStats>().movementPoints;
        currentTile = StaticFunctions.getTileAt(transform.position);
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
    public void finishedWalking()
    {
        state = 2;
    }

	bool isInRange(){
        List<GameObject> pathToTarget = StaticFunctions.aStarPathFinding(StaticFunctions.getTileAt(transform.position), StaticFunctions.getTileAt(target.transform.position));
		if(pathToTarget.Count > range) return false;
		return true;
	}

	public void endPlay(){
		playing = false;
		animator.SetInteger("state", 0); 	//State 0 is Idle
		combatManager.GetComponent<CombatManager>().finishedPlaying();
	}

}