using System;
using Unity.Cinemachine;
using UnityEngine;
using GamerWolf.Utils;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class SpawnManager : MonoBehaviour {
    [Header("Spawning Logs")]
    [SerializeField] private Transform variationsParent;
    [SerializeField] private CinemachineCamera followCam;
    [SerializeField] private string[] poolName;
    [SerializeField] private Transform intialSpawnPoint;
    [SerializeField] private int maxLogDestoryToSpawn = 1;

    [Header("Enemy Spawning")]
    [SerializeField] private float spawnTimes;
    [SerializeField] private Transform[] enemySpawnPoints;
    [SerializeField] private string[] enemeyNames,foodNames;
    private LevelVariations currentVariations;
    private ObjectPoolingManager poolingManager;
    private int nextLogSpawnAmount;
    private float currentEnemySpawnTime;
    public Action onMaxLogsDestroyed;
    public static SpawnManager current;
    private void Awake(){
        currentEnemySpawnTime = spawnTimes;
        current = this;
        poolingManager = ObjectPoolingManager.current;
    }

    private void Start(){
        onMaxLogsDestroyed += SpawnOtherLogs;
        Init();
    }
	private void OnDestroy() {
		onMaxLogsDestroyed -= SpawnOtherLogs;
	}
	public void Init(){
        int rand = Random.Range(0,poolName.Length);
        GameObject variations = poolingManager.SpawnFromPool(poolName[rand],intialSpawnPoint.position,intialSpawnPoint.rotation,variationsParent);
        if(variations.TryGetComponent<LevelVariations>(out LevelVariations newVaritaionsRight)){
            currentVariations = newVaritaionsRight;

        }

        for (int i = 0; i < 10; i++){
            SpawnOtherLogs();
        }
        StartCoroutine(SpawnEnemy());
    }
    public void SpawnOtherLogs(){
		nextLogSpawnAmount = 0;
        int rand = Random.Range(0,poolName.Length);
        GameObject variations = poolingManager.SpawnFromPool(poolName[rand],currentVariations.GetNextObstacleSpawnPoint().position,currentVariations.GetNextObstacleSpawnPoint().rotation,variationsParent);
        
        if(variations.TryGetComponent<LevelVariations>(out LevelVariations newVaritaionsRight)){
            currentVariations = newVaritaionsRight;
        }
        
    }
    private IEnumerator SpawnEnemy(){
        yield return new WaitForSeconds(currentEnemySpawnTime);
        currentEnemySpawnTime -= 0.5f;
        currentEnemySpawnTime = Mathf.Clamp(currentEnemySpawnTime,1,spawnTimes);
        int randomSpawnNumber = Random.Range(1,5);
        for (int i = 0; i < randomSpawnNumber; i++){
            int randomSpawnPoint = Random.Range(0,enemySpawnPoints.Length);
            int randomEnemy = Random.Range(0,enemeyNames.Length);
            int randomFood = Random.Range(0,foodNames.Length);
            if(Random.Range(0,10) > 5){
                poolingManager.SpawnFromPool(enemeyNames[randomEnemy],enemySpawnPoints[randomSpawnPoint].position,enemySpawnPoints[randomSpawnPoint].rotation);
            }else{
                poolingManager.SpawnFromPool(foodNames[randomFood],enemySpawnPoints[randomSpawnPoint].position,enemySpawnPoints[randomSpawnPoint].rotation);
            }
            yield return new WaitForSeconds(1f);
        }
        yield return StartCoroutine(SpawnEnemy());
    }
    public void InvokeSpawnNewSection(){
        nextLogSpawnAmount++;
        if(nextLogSpawnAmount >= maxLogDestoryToSpawn){
            onMaxLogsDestroyed?.Invoke();
        }
    }
    
}
