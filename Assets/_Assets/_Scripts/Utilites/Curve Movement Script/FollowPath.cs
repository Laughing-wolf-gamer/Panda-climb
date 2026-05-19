using UnityEngine;

namespace GamerWolf.Utils{
    public class FollowPath : MonoBehaviour {
        
        [SerializeField] private CurveSetUp curveSetUp;
        [SerializeField] private SwipeDetection swipeDetection;
        private void Awake(){
            curveSetUp.SetUp(this.transform);
        }
        private void Start(){
            swipeDetection.OnSwipe += (float x,float y) =>{
                Debug.Log("Swipe Detected");
                if(x > 0){
                    curveSetUp.TryMoveRight();
                    
                }else if(x < 0){
                    curveSetUp.TryMoveLeft();
                    
                }
            };
        }
    }

}

