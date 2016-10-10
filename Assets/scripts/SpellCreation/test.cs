using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class test : MonoBehaviour { 

    Animator animator;
    int state;
    void Start(){
        animator = GetComponent<Animator>();
    }
    void Update(){
        animator.SetInteger("state", 1);
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Appear")){
            animator.SetInteger("state", 0);
        }
      
    }
}