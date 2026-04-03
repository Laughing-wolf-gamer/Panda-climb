using System;
using Unity.Cinemachine;
using UnityEngine;
using GamerWolf.Utils;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class SpawnManager : MonoBehaviour {
	[Header("Log Spawning")]
[SerializeField] private Vector2 logSpawnIntervalRange = new Vector2(1.5f, 0.6f); // max, min seconds
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
	[SerializeField] private float distanceToMaxDifficulty = 300f;
	[SerializeField] private AnimationCurve difficultyCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[Header("Enemy Difficulty")]
	[SerializeField] private Vector2 enemySpawnIntervalRange = new Vector2(3.5f, 0.8f);
	[SerializeField] private Vector2Int enemiesPerWaveRange = new Vector2Int(1, 4);
	[SerializeField] private Vector2 foodChanceRange = new Vector2(0.65f, 0.2f);

	private float GetDifficulty01()
	{
		float distance = MasterController.current.CurrentDistance;
		float progress = Mathf.Clamp01(distance / distanceToMaxDifficulty);
		return difficultyCurve.Evaluate(progress);
	}
    private void Awake(){
        currentEnemySpawnTime = spawnTimes;
        current = this;
        poolingManager = ObjectPoolingManager.current;
    }

    private void Start(){
		onMaxLogsDestroyed += SpawnOtherLogs;
		Init();
		StartCoroutine(SpawnLogsContinuously());
	}
	private IEnumerator SpawnLogsContinuously()
	{
		// Wait for game to start
		while (MasterController.current == null || !MasterController.current.isGamePlaying)
			yield return null;

		while (MasterController.current.isGamePlaying)
		{
			// Spawn another log section ahead
			SpawnOtherLogs();

			// Randomized interval so it doesn’t look too uniform
			float t = Random.value;
			float delay = Mathf.Lerp(logSpawnIntervalRange.x, logSpawnIntervalRange.y, t);
			yield return new WaitForSeconds(delay);
		}
	}
	private void OnDestroy() {
		onMaxLogsDestroyed -= SpawnOtherLogs;
	}
	public void Init(){
        int rand = Random.Range(0,poolName.Length);
        GameObject variations = poolingManager.SpawnFromPool(poolName[rand],intialSpawnPoint.position,intialSpawnPoint.rotation,variationsParent);
        if(variations.TryGetComponent(out LevelVariations newVaritaionsRight)){
            currentVariations = newVaritaionsRight;

        }
        for (int i = 0; i < 2; i++){
            SpawnOtherLogs();
        }
        StartCoroutine(SpawnEnemy());
    }
    public void SpawnOtherLogs(){
		nextLogSpawnAmount = 0;
        if (currentVariations == null)
        {
            Debug.LogWarning("SpawnManager: currentVariations is null, cannot spawn new log section.");
            return;
        }

        Transform spawnPoint = currentVariations.GetNextObstacleSpawnPoint();
        if (spawnPoint == null)
        {
            Debug.LogWarning("SpawnManager: GetNextObstacleSpawnPoint returned null, cannot spawn new log section.");
            return;
        }

        int rand = Random.Range(0, poolName.Length);
        GameObject variations = poolingManager.SpawnFromPool(
            poolName[rand],
            spawnPoint.position,
            spawnPoint.rotation,
            variationsParent);

        if (variations != null && variations.TryGetComponent(out LevelVariations newVaritaionsRight)){
            currentVariations = newVaritaionsRight;
        }
        
    }
    private IEnumerator SpawnEnemy(){
		while (!MasterController.current.isGamePlaying)
		{
			yield return null; // Wait until the game starts
		}
		Debug.Log("Enemy spawn coroutine started." + $" MasterController.current.isGamePlaying: {MasterController.current.isGamePlaying}");
        while (MasterController.current.isGamePlaying)
        {
            float difficulty = GetDifficulty01();

            float delay = Mathf.Lerp(enemySpawnIntervalRange.x, enemySpawnIntervalRange.y, difficulty);
			Debug.Log($"Spawning enemies in {delay:F2} seconds (Difficulty: {difficulty:F2})");
            yield return new WaitForSeconds(delay);
			Debug.Log("Spawning enemies now!");
            int spawnCount = Mathf.RoundToInt(Mathf.Lerp(enemiesPerWaveRange.x, enemiesPerWaveRange.y, difficulty));
            float foodChance = Mathf.Lerp(foodChanceRange.x, foodChanceRange.y, difficulty);

            for (int i = 0; i < spawnCount; i++)
            {
				Debug.Log($"Spawning enemy {i + 1}/{spawnCount} (Food Chance: {foodChance:F2})");
                int spawnPointIndex = Random.Range(0, enemySpawnPoints.Length);
                bool spawnFood = Random.value < foodChance;

                if (spawnFood)
                {
                    int foodIndex = Random.Range(0, foodNames.Length);
                    poolingManager.SpawnFromPool(
                        foodNames[foodIndex],
                        enemySpawnPoints[spawnPointIndex].position,
                        enemySpawnPoints[spawnPointIndex].rotation);
                }
                else
                {
                    int enemyIndex = Random.Range(0, enemeyNames.Length);
                    poolingManager.SpawnFromPool(
                        enemeyNames[enemyIndex],
                        enemySpawnPoints[spawnPointIndex].position,
                        enemySpawnPoints[spawnPointIndex].rotation);
                }

                yield return new WaitForSeconds(Mathf.Lerp(1f, 0.25f, difficulty));
            }
        }
    }
    public void InvokeSpawnNewSection(){
        nextLogSpawnAmount++;
        if(nextLogSpawnAmount >= maxLogDestoryToSpawn){
            onMaxLogsDestroyed?.Invoke();
        }
    }
    
    
}
