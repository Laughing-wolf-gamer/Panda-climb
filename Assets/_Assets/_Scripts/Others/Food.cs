using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Food : Interactable{

    [SerializeField] private float speedMultiplier = 1f;
    private MasterController masterController;
    private void Start(){
        masterController = MasterController.current;
    }
    private void Update(){
        if (masterController != null && masterController.isGamePlaying)
        {
            float speed = masterController.WorldSpeed * speedMultiplier;
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }
    public override void Interact(PlayerCollision player){
        base.Interact(player);
        Debug.Log("Collided With Food");
        DestroyNow();
    }
    

}
