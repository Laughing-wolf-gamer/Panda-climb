using UnityEngine;
using GamerWolf.Utils;

public class PlayerController : MonoBehaviour {
    [SerializeField] private CurveSetUp curveSetUp;
    [SerializeField] private SwipeDetection swipeDetection;
    [SerializeField] private PlayerCollision body;
    [SerializeField] private PlayerAnimationHandler animationHandler;
    
    private void Awake(){
        curveSetUp.SetUp(this.transform);
    }
    private void Start(){
        swipeDetection.OnSwipe += (float x,float y) =>{
            Debug.Log("Swipe Detected");
            if(x > 0){
                curveSetUp.TryMoveRight();
                body.TurnRight();
                if(curveSetUp.IsJumping()){
                }
            }else if(x < 0){
                body.TurnLeft();
                curveSetUp.TryMoveLeft();
                if(curveSetUp.IsJumping()){
                }
            }
        };
    }
    public void Climb(bool value){
        animationHandler.Climbing(value);
    }
    
}


