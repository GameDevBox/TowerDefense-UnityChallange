using UnityEngine;

[CreateAssetMenu(menuName = "Tower Defense/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("Basic Stats")]
    public string towerName;
    public string description;
    public GameObject towerPrefab;
    public ProjectileData projectileData;
    public int cost = 100;
    public float range = 5f;
    public float fireRate = 1f;
    public int damage = 10;

    [Header("Targeting")]
    public bool canAttackFlying = true;
    public bool canAttackGround = true;

    [Header("Visuals")]
    public Sprite icon;
    public AudioClip shootSound;
}