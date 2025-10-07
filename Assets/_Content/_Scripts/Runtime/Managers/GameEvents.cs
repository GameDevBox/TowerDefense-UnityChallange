using System;
using UnityEngine;

public static class GameEvents
{
    // Wave Events
    public static event Action<WaveData> OnWaveStarted;
    public static event Action<WaveData> OnWaveCompleted;
    public static event Action<bool> OnPatternCompleted; // bool for isLooping
    public static event Action OnGameOver;

    // Enemy Events
    public static event Action<Enemy, int> OnEnemyKilled; // Enemy + reward
    public static event Action<Enemy, int> OnEnemyReachedGoal; // Enemy + damage

    // Tower Events
    public static event Action<TowerData, Vector3> OnTowerPlaced;
    public static event Action<Tower> OnTowerSelected;
    public static event Action<TowerData> OnTowerPlacementAttempt;
    public static event Action<TowerData> OnTowerPlacementSuccess; 
    public static event Action<TowerData, string> OnTowerPlacementFailed;

    // Economy & Game State Events
    public static event Action<int> OnMoneyChanged; // new amount
    public static event Action<int> OnScoreChanged; // new amount
    public static event Action<int> OnBaseHealthChanged; // new health
    public static event Action<int> OnCurrentWaveChanged; // new wave number
    public static event Action<int> OnTotalWavesChanged; // new total waves

    // Public methods to raise events
    public static void RaiseWaveStarted(WaveData waveData) => OnWaveStarted?.Invoke(waveData);
    public static void RaiseWaveCompleted(WaveData waveData) => OnWaveCompleted?.Invoke(waveData);
    public static void RaisePatternCompleted(bool isLooping) => OnPatternCompleted?.Invoke(isLooping);
    public static void RaiseGameOver() => OnGameOver?.Invoke();
    public static void RaiseEnemyKilled(Enemy enemy, int reward) => OnEnemyKilled?.Invoke(enemy, reward);
    public static void RaiseEnemyReachedGoal(Enemy enemy, int damage) => OnEnemyReachedGoal?.Invoke(enemy, damage);
    public static void RaiseTowerPlaced(TowerData towerData, Vector3 position) => OnTowerPlaced?.Invoke(towerData, position);
    public static void RaiseTowerPlacementAttempt(TowerData towerData) => OnTowerPlacementAttempt?.Invoke(towerData);
    public static void RaiseTowerPlacementSuccess(TowerData towerData) => OnTowerPlacementSuccess?.Invoke(towerData);
    public static void RaiseTowerPlacementFailed(TowerData towerData, string reason) => OnTowerPlacementFailed?.Invoke(towerData, reason);
    public static void RaiseTowerSelected(Tower tower) => OnTowerSelected?.Invoke(tower);
    public static void RaiseMoneyChanged(int newAmount) => OnMoneyChanged?.Invoke(newAmount);
    public static void RaiseScoreChanged(int newAmount) => OnScoreChanged?.Invoke(newAmount);
    public static void RaiseBaseHealthChanged(int newHealth) => OnBaseHealthChanged?.Invoke(newHealth);
    public static void RaiseCurrentWaveChanged(int newWave) => OnCurrentWaveChanged?.Invoke(newWave);
    public static void RaiseTotalWavesChanged(int totalWaves) => OnTotalWavesChanged?.Invoke(totalWaves);
}