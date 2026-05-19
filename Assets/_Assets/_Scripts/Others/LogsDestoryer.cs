using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogsDestoryer : MonoBehaviour {
    [SerializeField] private LevelVariations variations;
    private void OnTriggerEnter(Collider coli){
        if(coli.TryGetComponent<PlayerCollision>(out PlayerCollision player)){
            variations.DestroyNow();
            SpawnManager.current.InvokeSpawnNewSection();
        }
    }

}
