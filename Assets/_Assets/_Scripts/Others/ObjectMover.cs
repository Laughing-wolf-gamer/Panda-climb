using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectMover : MonoBehaviour {
    

    [SerializeField] private float speedMultiplier = 1f;
    
    private void Update(){
        if (MasterController.current != null && MasterController.current.isGamePlaying)
        {
            float speed = MasterController.current.WorldSpeed * speedMultiplier;
            transform.position += Vector3.down * speed * Time.deltaTime;
        }
    }

}
