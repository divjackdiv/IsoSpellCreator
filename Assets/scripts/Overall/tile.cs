using UnityEngine;
using System.Collections;

public class tile : MonoBehaviour {

	public bool taken;
	public GameObject takenBy;
	Animator animator;
	float animCounter;

	int state;
	int nextState;
	bool shouldPlayAnim;

	void Start(){
		animator = gameObject.GetComponent<Animator>();
	}
	void Update(){
		if(shouldPlayAnim){
			playAnimOnce(state, nextState);
		}
	}

	public bool takeTile(GameObject g){
		if (taken) return false;
		taken = true;
		takenBy = g;
		return true;
	}

	public bool leaveTile(){
		if(taken){
			taken = false;
			takenBy = null;
			return true;
		}
		return false;
	}

	public GameObject getTakenBy(){
		return takenBy;
	}

	public void changeAnim(int state, bool playOnlyOnce, int nextState){
		animator.SetInteger("state", state);
		if(playOnlyOnce){
			animCounter = 0;
			shouldPlayAnim = true;
			state = state;
			nextState = nextState;
		}
	}

	public void playAnimOnce (int state, int nextState){
		animCounter += Time.deltaTime;
	    if (animCounter >= animator.GetCurrentAnimatorStateInfo(0).length){
	        animCounter = 0;
	        animator.SetInteger("state", nextState); //If anim is only played once, define which anim is to be played next,      										
	    	shouldPlayAnim = false; //doesnt matter if this played more than once
	    }											 
	}
}
