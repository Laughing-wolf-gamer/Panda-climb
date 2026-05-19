using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hazards : Interactable {

    public override void Interact(PlayerCollision player){
        base.Interact(player);
        player.CollidedWithHazards();
        MasterController.current.SetGameOver();
    }

}
