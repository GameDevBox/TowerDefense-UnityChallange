using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    public Transform spawnPoint;
    public Transform enemyContainer;
    public WavePattern currentPattern;

    [Header("Debug")]
    [SerializeField] private int currentWaveIndex = 0;
    [SerializeField] private int currentSequenceIndex = 0;
    [SerializeField] private int enemiesAlive = 0;
    [SerializeField] private int enemiesSpawnedThisWave = 0;
    [SerializeField] private WaveState currentState = WaveState.Idle;

    private WaveData currentWave;
    private Coroutine currentWaveCoroutine;
    private List<Enemy> activeEnemies = new List<Enemy>();
    private GameData gameData;

    public enum WaveState
    {
        Idle,
        Spawning,
        WaitingForCompletion,
        Completed,
        GameOver
    }

    void Start()
    {
        gameData = FindFirstObjectByType<GameData>();

        if (gameData == null)
        {
            DebugLogsManager.LogError("GameData not found in scene!", gameObject);
            return;
        }

        if (currentPattern != null)
        {
            InitializeWaveSystem();
            StartNextWave();
        }
        else
        {
            DebugLogsManager.LogWarning("No WavePattern assigned to WaveManager!", gameObject);
        }
    }

    public void InitializeWaveSystem()
    {
        currentState = WaveState.Idle;
        currentWaveIndex = 0;
        UpdateGameData();
    }

    public void StartNextWave()
    {
        if (currentState != WaveState.Idle || currentWaveIndex >= currentPattern.waves.Length)
            return;

        currentWave = currentPattern.waves[currentWaveIndex];
        currentState = WaveState.Spawning;
        enemiesSpawnedThisWave = 0;

        GameEvents.RaiseWaveStarted(currentWave);

        if (currentWaveCoroutine != null)
            StopCoroutine(currentWaveCoroutine);

        currentWaveCoroutine = StartCoroutine(ExecuteWave());
    }

    private IEnumerator ExecuteWave()
    {
        // Wait before wave starts
        yield return new WaitForSeconds(currentWave.timeBeforeWave);

        // Execute each sequence in the wave
        for (currentSequenceIndex = 0; currentSequenceIndex < currentWave.sequences.Length; currentSequenceIndex++)
        {
            var sequence = currentWave.sequences[currentSequenceIndex];

            // Wait before sequence
            if (sequence.delayBeforeSequence > 0)
                yield return new WaitForSeconds(sequence.delayBeforeSequence);

            // Spawn sequence enemies
            yield return StartCoroutine(SpawnSequence(sequence));
        }

        currentState = WaveState.WaitingForCompletion;
        yield return new WaitUntil(() => enemiesAlive <= 0);

        CompleteWave();
    }

    private IEnumerator SpawnSequence(WaveSequence sequence)
    {
        for (int i = 0; i < sequence.enemyCount; i++)
        {
            SpawnEnemy(sequence);
            enemiesSpawnedThisWave++;
            enemiesAlive++;

            if (i < sequence.enemyCount - 1)
                yield return new WaitForSeconds(sequence.spawnInterval);
        }
    }

    private void SpawnEnemy(WaveSequence sequence)
    {
        var enemyObj = Instantiate(sequence.enemyType.prefab, spawnPoint.position, Quaternion.identity, enemyContainer);
        var enemy = enemyObj.GetComponent<Enemy>();

        if (enemy != null)
        {
            // Apply sequence scaling
            var scaledHealth = Mathf.RoundToInt(sequence.enemyType.baseHealth * sequence.healthMultiplier);
            var scaledSpeed = sequence.enemyType.baseSpeed * sequence.speedMultiplier;
            var scaledReward = sequence.enemyType.reward + sequence.bonusReward;

            enemy.Initialize(sequence.enemyType, scaledHealth, scaledSpeed, scaledReward);
            enemy.OnDeath += OnEnemyDeath;
            enemy.OnReachedGoal += OnEnemyReachedGoal;

            activeEnemies.Add(enemy);
        }
    }

    private void OnEnemyDeath(Enemy enemy, int reward)
    {
        enemiesAlive--;
        activeEnemies.Remove(enemy);

        gameData.AddMoney(reward);
        gameData.AddScore(10);

        // Raise enemy killed event
        GameEvents.RaiseEnemyKilled(enemy, reward);

        if (enemiesAlive <= 0 && currentState == WaveState.WaitingForCompletion)
        {
            CompleteWave();
        }
    }

    private void OnEnemyReachedGoal(Enemy enemy, int damage)
    {
        enemiesAlive--;
        activeEnemies.Remove(enemy);

        gameData.DamageBase(damage);

        // Raise enemy reached goal event
        GameEvents.RaiseEnemyReachedGoal(enemy, damage);

        if (gameData.BaseHealth <= 0)
        {
            GameOver();
        }
    }

    private void CompleteWave()
    {
        currentState = WaveState.Completed;

        // Give rewards
        gameData.AddMoney(currentWave.moneyReward);
        gameData.AddScore(currentWave.scoreReward);

        // Raise wave completed event
        GameEvents.RaiseWaveCompleted(currentWave);

        // Move to next wave
        currentWaveIndex++;

        if (currentWaveIndex >= currentPattern.waves.Length)
        {
            if (currentPattern.loopPattern)
            {
                currentWaveIndex = 0;
                ApplyDifficultyScaling();
                GameEvents.RaisePatternCompleted(true);

                // Auto-start next wave after delay
                StartCoroutine(AutoStartNextWaveAfterDelay());
            }
            else
            {
                GameEvents.RaisePatternCompleted(false);
                currentState = WaveState.Idle;
                return;
            }
        }
        else
        {
            // Auto-start next wave after delay
            StartCoroutine(AutoStartNextWaveAfterDelay());
        }

        currentState = WaveState.Idle;
        UpdateGameData();
    }

    private IEnumerator AutoStartNextWaveAfterDelay()
    {
        float delay = currentPattern != null ? currentPattern.timeBetweenWaves : 3f;

        yield return new WaitForSeconds(delay);

        if (currentState == WaveState.Idle)
        {
            StartNextWave();
        }
    }

    private void ApplyDifficultyScaling()
    {
        // Apply pattern scaling to all waves
        foreach (var wave in currentPattern.waves)
        {
            foreach (var sequence in wave.sequences)
            {
                sequence.healthMultiplier *= currentPattern.difficultyScaling.healthMultiplierPerWave;
                sequence.speedMultiplier *= currentPattern.difficultyScaling.speedMultiplierPerWave;
                sequence.spawnInterval *= currentPattern.difficultyScaling.spawnRateMultiplier;
                sequence.enemyCount += currentPattern.difficultyScaling.extraEnemiesPerWave;
            }
        }
    }

    private void GameOver()
    {
        currentState = WaveState.GameOver;

        // Raise game over event
        GameEvents.RaiseGameOver();

        if (currentWaveCoroutine != null)
            StopCoroutine(currentWaveCoroutine);
    }

    private void UpdateGameData()
    {
        if (gameData != null)
        {
            gameData.CurrentWave = currentWaveIndex + 1;
            gameData.TotalWaves = currentPattern.waves.Length;
        }
    }

    // Public API
    public WaveState GetCurrentState() => currentState;
    public int GetEnemiesAlive() => enemiesAlive;
    public int GetCurrentWaveNumber() => currentWaveIndex + 1;
    public WaveData GetCurrentWaveData() => currentWave;

    public void SetWavePattern(WavePattern newPattern)
    {
        currentPattern = newPattern;
        InitializeWaveSystem();
    }

    void OnDestroy()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null)
            {
                enemy.OnDeath -= OnEnemyDeath;
                enemy.OnReachedGoal -= OnEnemyReachedGoal;
            }
        }
    }
}