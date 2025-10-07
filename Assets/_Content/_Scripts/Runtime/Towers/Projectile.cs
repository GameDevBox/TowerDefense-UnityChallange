using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody rb;
    public ParticleSystem trailParticles;

    [Header("Basic Stats")]
    public float RotationSpeed = 25f;

    private ProjectileData data;
    private Enemy target;
    private int damage;
    private bool hasHit = false;

    void Start()
    {
        // Ensure we have a Rigidbody
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    public void Initialize(ProjectileData projectileData, Enemy targetEnemy, int projectileDamage)
    {
        data = projectileData;
        target = targetEnemy;
        damage = projectileDamage;
        hasHit = false;

        // Setup visuals
        if (trailParticles != null) trailParticles.Play();

        // Start lifetime coroutine
        StartCoroutine(LifetimeRoutine());

        Debug.Log($"Projectile initialized targeting: {target.GetEnemyData().enemyName}");
    }

    void Update()
    {
        if (hasHit || target == null)
        {
            // If target is null, destroy projectile
            if (target == null && !hasHit)
            {
                Destroy(gameObject);
            }
            return;
        }

        // Move towards target
        Vector3 direction = (target.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * data.speed; // FIXED: Use velocity instead of linearVelocity

        // Rotate towards target
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);
        }
    }

    IEnumerator LifetimeRoutine()
    {
        yield return new WaitForSeconds(data.lifetime);
        if (!hasHit)
        {
            Debug.Log("Projectile lifetime expired");
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && enemy == target)
        {
            HitTarget(enemy);
            hasHit = true;
            Destroy(gameObject);
        }
    }

    void HitTarget(Enemy enemy)
    {
        // Apply damage
        enemy.TakeDamage(damage);

        if (data.impactEffect != null)
        {
            Instantiate(data.impactEffect, transform.position, Quaternion.identity);
        }

        // Impact sound
        if (data.impactSound != null)
        {
            AudioSource.PlayClipAtPoint(data.impactSound, transform.position);
        }
    }
}