using UnityEngine;
using Baracuda.Monitoring;
using Baracuda.Monitoring.API;
using System.Collections.Generic;
namespace GamerWolf.Utils{
    
    public class CurveSetUp : MonoBehaviour {
        [Header("Debbuging")]
        [SerializeField] private bool showDebbging = true;
        [SerializeField] private float anchoreGizmosSize,handleGizmosSize;
        [SerializeField] private int numberOfPoint;
        [SerializeField] private float time;
        [Header("Movement")]
        [SerializeField] private float bodyTransitionSpeed = 2f;
        [SerializeField] private Transform secondPoint,secondLastPoint;

        [Header("Move Direction")]
        [SerializeField] private CurvePointData[] rightMovePointData;
        [SerializeField] private CurvePointData[] leftMovePointData;

        #region Monitoring Section...................

        [Monitor] private int currentActiveCurveSetIndex;
        [Monitor] private bool lastMoveRight,lastMoveLeft;
        [Monitor] private bool isJumping;

        #endregion


        private bool canMove;
        private bool isMoving = true;
        private bool moveRight;
        private CurvePointData currentDirectionData;
        private int onFinalPointMoveCount = 1;

        private Transform movingBody;
        private void Awake(){
          //   MonitoringManager.RegisterTarget(this);
        }
        private void OnDestroy(){
          //   MonitoringManager.UnregisterTarget(this);
        }
        private void Start(){
            if(showDebbging){
                foreach(CurvePointData poin in rightMovePointData){
                    poin.debbugingLinePoints = new Vector3[numberOfPoint];
                }
                foreach(CurvePointData movePoint in rightMovePointData){
                    movePoint.lr.positionCount = numberOfPoint;
                }
            }
            for (int i = 0; i < rightMovePointData.Length; i++){
                if(currentActiveCurveSetIndex == i){
                    currentDirectionData = rightMovePointData[currentActiveCurveSetIndex];
                }else{
                    rightMovePointData[i].currentActiveCurveSet = false;
                }
            }
            Debuging();
        }

        public void SetUp(Transform _moveBody){
            movingBody = _moveBody;
        }

        #region Get Move Input.......................

        public void TryMoveRight(){
            if(movingBody != null){
                if(isMoving){
                    if(lastMoveLeft){
                        lastMoveLeft = false;
                        Debug.Log("Only Moving Right");
                        OnlyMoveRight();
                        return;
                    }
                    lastMoveRight = true;
                    moveRight = true;
                    MoveRight();
                }
            }
        }
        public void TryMoveLeft(){
            if(movingBody != null){
                if(isMoving){
                    if(lastMoveRight){
                        lastMoveRight = false;
                        Debug.Log("Only Moving Left");
                        OnlyMoveLeft();
                        return;
                    }
                    lastMoveLeft = true;

                    moveRight = false;
                    MoveLeft();
                }
            }
        }

        #endregion


        #region Debbuging.............................

        private void Debuging(){
            for(int i = 0; i < rightMovePointData.Length; i++){
                for (int d = 1; d < numberOfPoint + 1; d++){
                    float t = d / (float)numberOfPoint;
                    rightMovePointData[i].debbugingLinePoints[d - 1] = CalculateBazierPointMovement(t,rightMovePointData[i].anchre1.position,rightMovePointData[i].handle1.position,rightMovePointData[i].anchre2.position);
                }
                rightMovePointData[i].SetLine();
                
            }
        }
        #endregion



        private void Update(){
            Move();
            if(showDebbging){
                foreach(CurvePointData pointData in rightMovePointData){
                    pointData.lr.enabled = true;
                    if(pointData.anchre1.hasChanged || pointData.anchre1.hasChanged || pointData.anchre2.hasChanged){
                        Debuging();

                    }
                }
            }else{
                foreach(CurvePointData pointData in rightMovePointData){
                    pointData.lr.enabled = false;
                }
            }
            
        }
        private void Move(){
            if(!isMoving){
                if(currentDirectionData != null){
                    currentDirectionData.currentActiveCurveSet = true;
                    if(currentDirectionData.currentActiveCurveSet){
                        time += bodyTransitionSpeed * Time.deltaTime;
                        movingBody.position = CalculateBazierPointMovement(time,currentDirectionData.anchre1.position,currentDirectionData.handle1.position,currentDirectionData.anchre2.position);
                        if(time >= 1f){
                            isMoving = true;
                            currentDirectionData.currentActiveCurveSet = false;
                            currentDirectionData = null;
                            time = 0f;
                        }

                    }
                }
            }
            
        }
        private void MoveRight(){
            if(onFinalPointMoveCount > 0){
                if(currentActiveCurveSetIndex <= 0){
                    currentActiveCurveSetIndex = 0;
                    onFinalPointMoveCount--;
                }else{
                    IncreaseIndex();
                    
                }
            }else{
                onFinalPointMoveCount = 1;

                IncreaseIndex();
                
            }
            Debug.Log("Moving Right");
            OnlyMoveRight();
            
        }
        private void OnlyMoveRight(){
            float dist = Vector3.Distance(movingBody.position,secondPoint.position);
            if(currentActiveCurveSetIndex == 0 && dist <= 1f){
                currentActiveCurveSetIndex++;
            }
            if(currentActiveCurveSetIndex > rightMovePointData.Length - 1){
                currentActiveCurveSetIndex = rightMovePointData.Length - 1;
                isMoving = true;
            }else{
                isMoving = false;
            }
            for (int i = 0; i < rightMovePointData.Length; i++){
                if(currentActiveCurveSetIndex == i){
                    lastMoveRight = true;
                    currentDirectionData = rightMovePointData[currentActiveCurveSetIndex];
                    
                }else{
                    rightMovePointData[i].currentActiveCurveSet = false;
                }
            }
        }
        private void IncreaseIndex(){
            if(!lastMoveLeft){
                currentActiveCurveSetIndex++;
                if(currentActiveCurveSetIndex == 1 || currentActiveCurveSetIndex == 3){
                    isJumping = true;
                }else{
                    isJumping = false;
                }
            }
        }
        
        private void DecreaseIndex(){
            if(!lastMoveRight){
                currentActiveCurveSetIndex--;
                if(currentActiveCurveSetIndex == 1 || currentActiveCurveSetIndex == 3){
                    isJumping = true;
                }else{
                    isJumping = false;
                }
            }
        }
        private void MoveLeft(){
            if(onFinalPointMoveCount > 0){
                if(currentActiveCurveSetIndex >= rightMovePointData.Length - 1){
                    currentActiveCurveSetIndex = rightMovePointData.Length - 1;
                    onFinalPointMoveCount--;
                }else{
                    DecreaseIndex();
                   
                }
            }else{
                onFinalPointMoveCount = 1;
                DecreaseIndex();
                
            }
            OnlyMoveLeft();
        }
        private void OnlyMoveLeft(){
            float dist = Vector3.Distance(movingBody.position,secondLastPoint.position);
            if(currentActiveCurveSetIndex == leftMovePointData.Length - 1 && dist <= 1f){
                currentActiveCurveSetIndex--;
            }
            if(currentActiveCurveSetIndex < 0){
                currentActiveCurveSetIndex = 0;
                isMoving = true;
            }else{
                isMoving = false;
            }
            
            for (int i = 0; i < leftMovePointData.Length; i++){
                if(currentActiveCurveSetIndex == i){
                    // recentlyMovedLeft = true;
                    // recentlyMovedRight = false;
                    lastMoveLeft = true;
                    currentDirectionData = leftMovePointData[currentActiveCurveSetIndex];
                }else{
                    leftMovePointData[i].currentActiveCurveSet = false;
                }
            }
        }
        private Vector3 CalculateBazierPointMovement(float t,Vector3 p0,Vector3 p1,Vector3 p2){
            //B(t) = (1-t)2P0 + 2(1-t)tP1 + t2P2
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            Vector3 point = uu * p0 + 2 * u * t * p1 + tt * p2;
            return point;
            
        }

        private void OnDrawGizmos(){
            if(showDebbging){
                Gizmos.color = Color.green;
                for (int i = 0; i < rightMovePointData.Length; i++){
                    Gizmos.DrawCube(rightMovePointData[i].anchre1.position,Vector3.one * (anchoreGizmosSize / 2f));
                    Gizmos.DrawCube(rightMovePointData[i].anchre2.position,Vector3.one * (anchoreGizmosSize / 2f));
                    
                }
                Gizmos.color = Color.cyan;
                for (int i = 0; i < rightMovePointData.Length; i++){
                    Gizmos.DrawSphere(rightMovePointData[i].handle1.position,handleGizmosSize);
                }
                
            }
        }
        public bool IsJumping(){
            return isJumping;
        }

    }
    [System.Serializable]
    public class CurvePointData{
        public bool currentActiveCurveSet;
        public Transform anchre1,handle1,anchre2;
        public Vector3[] debbugingLinePoints = new Vector3[50];
        public LineRenderer lr;
        public void SetLine(){
            lr.SetPositions(debbugingLinePoints);
        }
    }

}