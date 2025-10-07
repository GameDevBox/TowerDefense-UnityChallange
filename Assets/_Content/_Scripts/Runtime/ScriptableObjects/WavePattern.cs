using UnityEngine;

[CreateAssetMenu(menuName = "Wave System/Wave Pattern")]
public class WavePattern : ScriptableObject
{
    public WaveData[] waves;
    public bool loopPattern = false;
    public DifficultyScaling difficultyScaling;

    [Header("Pattern Settings")]
    public float timeBetweenWaves = 10f;

    [System.Serializable]
    public class DifficultyScaling
    {
        public float healthMultiplierPerWave = 1.1f;
        public float speedMultiplierPerWave = 1.05f;
        public float spawnRateMultiplier = 0.95f;
        public int extraEnemiesPerWave = 2;
    }
}