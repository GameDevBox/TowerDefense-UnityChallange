using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Wave System/Wave Data")]
public class WaveData : ScriptableObject
{
    [Header("Wave Configuration")]
    public string waveName;
    public WaveSequence[] sequences;
    public float timeBeforeWave = 5f;

    [Header("Completion Rewards")]
    public int moneyReward = 100;
    public int scoreReward = 500;

    public int TotalEnemies
    {
        get
        {
            int total = 0;
            foreach (var sequence in sequences)
            {
                total += sequence.enemyCount;
            }
            return total;
        }
    }
}

[Serializable]
public class WaveSequence
{
    public EnemyData enemyType;
    public int enemyCount;
    public float spawnInterval = 1f;
    public float delayBeforeSequence = 0f;

    [Header("Scaling")]
    public float healthMultiplier = 1f;
    public float speedMultiplier = 1f;
    public int bonusReward = 0;
}