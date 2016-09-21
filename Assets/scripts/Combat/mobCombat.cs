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
    bool playing;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if(lifePoints <= 0){
			animator.SetInteger("state", 3);	//State 3 is dying 
			//if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0)){
				die();
			//}
		}
		else {
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
		}
	} 
	public void takeDamage(float damage){
		print("'arggghh tu fais mal' - Vous avez infligé " +damage +" dégats");
		lifePoints -= damage;
	}

	public void die(){
		currentTile.GetComponent<tile>().leaveTile();
		combatManager.GetComponent<CombatManager>().characters.Remove(gameObject);
		if(playing)combatManager.GetComponent<CombatManager>().finishedPlaying();
		Destroy(gameObject);
	}

	public void startCombat(){
		GetComponent<mobWorld>().enabled = false;
		currentTile = StaticFunctions.getTileAt(transform.position);
		currentTile.GetComponent<tile>().takeTile(gameObject);
		state = 1;
	}

	public void play(){
		playing = true;
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
        	if (playing) endPlay();
        }
	}


	public void endPlay(){
		playing = false;
		animator.SetInteger("state", 0); 	//State 0 is Idle
		combatManager.GetComponent<CombatManager>().finishedPlaying();
	}
}

