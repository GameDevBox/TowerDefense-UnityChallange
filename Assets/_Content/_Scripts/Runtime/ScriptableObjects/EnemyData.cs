using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Stats")]
    public string enemyName;
    public GameObject prefab;
    public int baseHealth = 100;
    public float baseSpeed = 2f;
    public int damage = 10;
    public int reward = 25;
    public float size = 1;

    [Header("Visual & Audio")]
    public AudioClip spawnSound;
    public AudioClip deathSound;
    public ParticleSystem deathEffect;

    [Header("Advanced")]
    public bool isFlying = false;
    public bool isBoss = false;
}