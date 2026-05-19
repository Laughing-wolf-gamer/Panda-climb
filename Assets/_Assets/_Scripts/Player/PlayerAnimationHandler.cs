using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(Animator))]
public class PlayerAnimationHandler : MonoBehaviour {
    
    private Animator animator;

    private void Awake(){
        animator = GetComponent<Animator>();
    }
    public void Climbing(bool climb){
        animator.SetBool("Climbing",climb);
    }

}
