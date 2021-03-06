﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Class which contains functions relating to tile overlays, for example when a tile is enflamed
public class tileOverlay : MonoBehaviour {

    Animator animator;
    List<int> states;
    float animCounter;
    bool shouldPlayAnim;
    bool destroyAfterAnimation;
    // Use this for initialization
    void Awake()
    {
        animator = GetComponent<Animator>();
        states = new List<int>();
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
    }
	
	// Update is called once per frame
	void Update () {
        if (shouldPlayAnim)
        {
            playAnimOnce(states);
        }
    }

    public void changeAnim(List<int> animationStates, bool destroyAfterAnim)
    {
        if (animationStates != null && animationStates.Count != 0)
        {
            animator.SetInteger("state", animationStates[0]);
            destroyAfterAnimation = destroyAfterAnim;
            animCounter = 0;
            shouldPlayAnim = true;
            states = animationStates;

        }
    }

    public void playAnimOnce(List<int> states)
    {
        animCounter += Time.deltaTime;
        if (animCounter >= animator.GetCurrentAnimatorStateInfo(0).length)
        {
            if (destroyAfterAnimation) Destroy(gameObject);
            animCounter = 0;
            states.RemoveAt(0);
            if (states.Count > 0) animator.SetInteger("state", states[0]); //If anim is only played once, define which anim is to be played next,      										
            else shouldPlayAnim = false; 
        }
    }
}
