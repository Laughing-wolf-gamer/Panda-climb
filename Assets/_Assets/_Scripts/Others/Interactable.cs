using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamerWolf.Utils;

public class Interactable : MonoBehaviour,IPooledObject {
    

    [SerializeField] private float lifeTime = 10f;
    
    protected virtual void OnTriggerEnter(Collider coli){
        if(coli.TryGetComponent<PlayerCollision>(out PlayerCollision player)){
            Interact(player);
        }
    }

    public virtual void Interact(PlayerCollision player){
        
    }
    public void DestroyMySelfWithDelay(float delay = 1f){
        Invoke(nameof(DestroyNow),delay);    
    }

    public void DestroyNow(){
        CancelInvoke(nameof(DestroyNow));
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
    }

    public void OnObjectReuse(){
        DestroyMySelfWithDelay(lifeTime);
        gameObject.SetActive(true);
    }

}
