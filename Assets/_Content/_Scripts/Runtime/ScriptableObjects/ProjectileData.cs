using UnityEngine;

[CreateAssetMenu(menuName = "Tower Defense/Projectile Data")]
public class ProjectileData : ScriptableObject
{
    [Header("Basic Stats")]
    public GameObject projectilePrefab;
    public float speed = 10f;

    [Header("Effects")]
    public GameObject impactEffect;
    public AudioClip impactSound;

    [Header("Visual")]
    public float lifetime = 5f;
}