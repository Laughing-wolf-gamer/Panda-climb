using UnityEngine;
using GamerWolf.Utils;
using System.Collections;
using UnityEngine.Events;
using Baracuda.Monitoring;
using Baracuda.Monitoring.API;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
public class MasterController : MonoBehaviour {
    [SerializeField] private CinemachineCamera followCamera;
    [SerializeField] private Transform moveBody;
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private UnityEvent OnGameStart;
    [SerializeField] private UnityEvent OnGamePlaying,OnGameEnd,OnGamePaused,OnGameResume,OnAfterEndDealy;

    [SerializeField] private bool isGamePause;
    [Monitor] public bool isGamePlaying{get;private set;}

    private Vector3 lastPosition;
    private float longestDistance;
    private float currentSpeed;
    private float speedFactordelay;
    private float UpdateDelay = 0.1f;
    private int collectedCoins;
    private UIManager uIManager;
    
    public static MasterController current;
    private void Awake(){
        current = this;
     //    MonitoringManager.RegisterTarget(this);
    }
    private void OnDestory(){
     //    MonitoringManager.UnregisterTarget(this);
    }

    private void Start(){
        Time.timeScale = 1f;
        uIManager = UIManager.current;
        lastPosition = moveBody.transform.position;
        StartCoroutine(SpeedReckoner());
        StartCoroutine(GameStartRoutine());
    }

    private void Update(){
        if(isGamePlaying){
            float currentDistance = Vector3.Distance(lastPosition,moveBody.transform.position);
            // currentSpeed = Vector3.Distance(lastPosition,player.transform.position) / 100f;
            longestDistance += currentDistance;
            lastPosition = moveBody.transform.position;
            uIManager.SetcurrentDistance(longestDistance);
            if(Input.GetKeyDown(KeyCode.Escape)){
                if(isGamePause){
                    Resume();
                }else{
                    Pause();
                }
            }
        }
    }
    private IEnumerator SpeedReckoner() {

        YieldInstruction timedWait = new WaitForSeconds(0.1f);
        Vector3 lastPosition = moveBody.transform.position;
        float lastTimestamp = Time.time;

        while (enabled) {
            yield return timedWait;

            var deltaPosition = (moveBody.transform.position - lastPosition).magnitude;
            var deltaTime = Time.time - lastTimestamp;

            if (Mathf.Approximately(deltaPosition, 0f)) // Clean up "near-zero" displacement
                deltaPosition = 0f;

            currentSpeed = deltaPosition / deltaTime;

            uIManager.SetSpeed(currentSpeed);
            // Debug.Log(currentSpeed.ToString("F2"));
            lastPosition = moveBody.transform.position;
            lastTimestamp = Time.time;
        }
    }
    
    private IEnumerator GameStartRoutine(){
        OnGameStart?.Invoke();
        while(!isGamePlaying){
            yield return null;
        }
        yield return StartCoroutine(GamePlayingRoutine());
        
    }
    private IEnumerator GamePlayingRoutine(){
        OnGamePlaying?.Invoke();
        while(isGamePlaying){
            uIManager.SetCoinsAmount(collectedCoins);
            yield return null;
        }
        gameData.SetNewLongestDistance(longestDistance);
        gameData.IncreaseCash(collectedCoins);
        uIManager.SetLongestDistance();
        followCamera.Follow = null;
        OnGameEnd?.Invoke();
        yield return new WaitForSeconds(1f);
        OnAfterEndDealy?.Invoke();

    }
    
    public void GameStart(){
        isGamePlaying = true;
    }
    public void SetGameOver(){
        isGamePlaying = false;
    }
    public void RestartGame(){
        if(LevelLoader.instance != null){
            LevelLoader.instance.PlayNextLevel();
        }else{
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
    }
    
    
    public void Resume(){
        isGamePause = false;
        Time.timeScale = 1f;
        OnGameResume?.Invoke();
    }
    public void Pause(){
        Time.timeScale = 0f;
        OnGamePaused?.Invoke();
        isGamePause = true;
    }
    public void SetCoins(int amount){
        collectedCoins += amount;
        
    }
}
