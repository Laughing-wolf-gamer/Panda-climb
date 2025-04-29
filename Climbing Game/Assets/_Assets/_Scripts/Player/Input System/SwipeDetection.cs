using System;
using UnityEngine;
namespace GamerWolf.Utils{
    public class SwipeDetection : MonoBehaviour {
        [SerializeField] private bool onPc = true;
        [SerializeField] private bool inputEnable = true;
        [SerializeField] private float direcitonMoveThreshold = 0.1f;
        [SerializeField] private PlayerInputSystem inputSystem;
        private Vector3 swipeStartPosition,swipeEndPosition;

        #region Events.....................
        public Action<float,float> OnSwipe;

        #endregion

        #region Singelton...................

        public static SwipeDetection current;

        private void Awake(){
            current = this;
        }

        #endregion


        #region Initializing Input Scripts....................
        
        private void OnEnable(){
            inputSystem.onStartTouch += SwipeStart;
            inputSystem.onEndTouch += SwipeEnd;
        }

        private void OnDisable(){
            inputSystem.onStartTouch -= SwipeStart;
            inputSystem.onEndTouch -= SwipeEnd;
        }
        #endregion
        private void SwipeStart(Vector3 positon,float time){
            swipeStartPosition = positon;
        }
        private void SwipeEnd(Vector3 position,float time){
            swipeEndPosition = position;
            DetectSwipe();
        }
        private void DetectSwipe(){
            if(inputEnable){
                SwipeDirection(swipeStartPosition,swipeEndPosition);
            }
        }
		private void Update() {
			if(inputEnable){
				if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)){
					OnSwipe?.Invoke(-1f,0);
				}else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)){
					OnSwipe?.Invoke(1f,0);
				}
			}
		}
		private void SwipeDirection(Vector3 first,Vector3 end){
            if(Mathf.Abs(end.x - first.x) > direcitonMoveThreshold || Mathf.Abs(end.y - first.y) > direcitonMoveThreshold){
                if(Mathf.Abs(end.x - first.x) > Mathf.Abs(end.y - first.y)){
                    if(end.x > first.x){
                        OnSwipe?.Invoke(1f,0);
                        Debug.Log("Swipe Right");
                    }else{
                        OnSwipe?.Invoke(-1f,0);
                        Debug.Log("Swipe Left");
                    }
                }else {
                    if(end.y > first.y){
                        OnSwipe?.Invoke(0f,1f);
                        Debug.Log("Swipe Up");
                    }else{
                        OnSwipe?.Invoke(0f,-1f);
                        Debug.Log("Swipe Down");
                    }
                }
            }
            
        }
        public void ToggleInputs(bool value){
            inputEnable = value;
        }
        public bool OnPC(){
            return onPc;
        }
        
        
    }

}
