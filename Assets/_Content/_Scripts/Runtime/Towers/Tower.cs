using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Tower : MonoBehaviour
{
    [Header("Components")]
    public TowerData data;
    public Transform rotationBase;
    public Transform firePoint;
    public GameObject rangeIndicator;
    public AudioSource audioSource;
    public ParticleSystem muzzleFlashParticle;

    [Header("Rotation Settings")]
    public bool rotateOnlyBase = true;
    public float rotationSpeed = 8f;

    [Header("Runtime Data")]
    public Enemy currentTarget;

    private float fireCooldown;
    private List<Enemy> enemiesInRange = new List<Enemy>();
    private Coroutine shootingCoroutine;
    private GameData gameData;
    private SphereCollider detectionCollider;

    public float CurrentRange { get; private set; }
    public float CurrentFireRate { get; private set; }
    public int CurrentDamage { get; private set; }
    public int CurrentCost { get; private set; }

    void Start()
    {
        if (data != null)
        {
            Initialize(data);
        }
    }

    public void Initialize(TowerData towerData)
    {
        data = towerData;
        gameData = FindFirstObjectByType<GameData>();

        CurrentRange = data.range;
        CurrentFireRate = data.fireRate;
        CurrentDamage = data.damage;
        CurrentCost = data.cost;

        // Setup detection collider
        SetupDetectionCollider();

        // Setup range indicator
        SetupRangeIndicator();

        // Start shooting routine
        if (shootingCoroutine != null)
            StopCoroutine(shootingCoroutine);

        shootingCoroutine = StartCoroutine(ShootingRoutine());
    }

    void SetupDetectionCollider()
    {
        detectionCollider = GetComponent<SphereCollider>();
        if (detectionCollider == null)
        {
            detectionCollider = gameObject.AddComponent<SphereCollider>();
        }
        detectionCollider.radius = CurrentRange;
        detectionCollider.isTrigger = true;
    }

    void SetupRangeIndicator()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.transform.localScale = Vector3.one * CurrentRange * 2f;
            rangeIndicator.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        UpdateTarget();
        UpdateRotation();
    }

    void UpdateTarget()
    {
        // Clear null or dead enemies
        enemiesInRange.RemoveAll(enemy => enemy == null || !enemy.isActiveAndEnabled || !enemy.IsAlive());

        if (enemiesInRange.Count == 0)
        {
            currentTarget = null;
            return;
        }

        currentTarget = GetPriorityTarget();
    }

    Enemy GetPriorityTarget()
    {
        if (enemiesInRange.Count == 0) return null;

        Enemy priorityTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Enemy enemy in enemiesInRange)
        {
            if (enemy == null || !IsValidTarget(enemy)) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                priorityTarget = enemy;
            }
        }

        return priorityTarget;
    }

    void UpdateRotation()
    {
        if (currentTarget != null)
        {
            Vector3 direction = currentTarget.transform.position - transform.position;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                if (rotateOnlyBase && rotationBase != null)
                {
                    rotationBase.rotation = Quaternion.Slerp(rotationBase.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }
                else
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }
            }
        }
    }

    IEnumerator ShootingRoutine()
    {
        while (true)
        {
            if (currentTarget != null && fireCooldown <= 0 && IsValidTarget(currentTarget))
            {
                Shoot();
                fireCooldown = 1f / CurrentFireRate;
            }

            if (fireCooldown > 0)
            {
                fireCooldown -= Time.deltaTime;
            }

            yield return null;
        }
    }

    void Shoot()
    {
        if (currentTarget == null || !IsValidTarget(currentTarget))
        {
            DebugLogsManager.Log("Shoot cancelled: invalid target");
            return;
        }

        if (data.projectileData.projectilePrefab != null)
        {
            // Use firePoint rotation if we have one, otherwise use tower rotation
            Quaternion spawnRotation = firePoint != null ? firePoint.rotation :
                (rotateOnlyBase && rotationBase != null ? rotationBase.rotation : transform.rotation);

            GameObject projectileObj = Instantiate(data.projectileData.projectilePrefab, firePoint.position, spawnRotation);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Initialize(data.projectileData, currentTarget, CurrentDamage);
            }
            else
            {
                DebugLogsManager.LogError("Projectile prefab doesn't have Projectile component!");
            }
        }
        else
        {
            DebugLogsManager.LogError("No projectile prefab assigned in TowerData!");
        }

        // Play sound
        if (audioSource != null && data.shootSound != null)
        {
            audioSource.PlayOneShot(data.shootSound);
        }

        PlayMuzzleFlash();
    }

    void PlayMuzzleFlash()
    {
        if (muzzleFlashParticle != null)
        {
            muzzleFlashParticle.Play();
        }
        else if (firePoint != null)
        {
            StartCoroutine(SimpleMuzzleFlash());
        }
    }

    IEnumerator SimpleMuzzleFlash()
    {
        GameObject flash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        flash.transform.position = firePoint.position;
        flash.transform.localScale = Vector3.one * 0.2f;
        flash.GetComponent<Renderer>().material.color = Color.yellow;
        Destroy(flash, 0.1f);
        yield return null;
    }

    bool IsValidTarget(Enemy enemy)
    {
        if (enemy == null || !enemy.isActiveAndEnabled || !enemy.IsAlive())
        {
            return false;
        }

        bool canTarget = true;
        if (enemy.IsFlying() && !data.canAttackFlying) canTarget = false;
        if (!enemy.IsFlying() && !data.canAttackGround) canTarget = false;

        bool inRange = Vector3.Distance(transform.position, enemy.transform.position) <= CurrentRange;

        return canTarget && inRange;
    }

    void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Remove(enemy);

            if (currentTarget == enemy)
            {
                currentTarget = null;
            }
        }
    }

    public void ShowRange(bool show)
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.gameObject.SetActive(show);
        }
    }

    public void SetRotationMode(bool rotateBaseOnly)
    {
        rotateOnlyBase = rotateBaseOnly;
    }

    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }

    void OnDestroy()
    {
        if (shootingCoroutine != null)
            StopCoroutine(shootingCoroutine);
    }
}