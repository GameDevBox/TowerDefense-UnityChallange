using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Components")]
    public HealthBar healthBar;
    public EnemyMovement movement;
    public Animator animator;

    [Header("Death Settings")]
    public float deathAnimationDuration = 2f;
    public float destroyDelay = 3f;

    private EnemyData data;
    private int currentHealth;
    private float currentSpeed;
    private bool isFlying = false;
    private int reward;
    private bool isAlive = true;
    private Vector3 spawnPosition;
    private Vector3 goalPosition;

    public event Action<Enemy, int> OnDeath;
    public event Action<Enemy, int> OnReachedGoal;

    public void Initialize(EnemyData enemyData, int health, float speed, int enemyReward)
    {
        data = enemyData;
        currentHealth = health;
        currentSpeed = speed;
        reward = enemyReward;
        isAlive = true;

        isFlying = data.isFlying;

        spawnPosition = transform.position;
        goalPosition = WaypointManager.Instance.GetGoalPosition();

        if (movement != null)
        {
            movement.Initialize(currentSpeed, WaypointManager.Instance.GetWaypoints());
        }

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(health);
            healthBar.SetHealth(health);
            healthBar.gameObject.SetActive(false);
        }

        if (animator != null)
        {
            animator.SetBool("Death", false);
        }
    }

    void Update()
    {
        if (!isAlive) return;

        if (Vector3.Distance(transform.position, goalPosition) < 1f)
        {
            ReachedGoal();
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive) return;

        int actualDamage = Mathf.RoundToInt(damage);

        currentHealth -= actualDamage;

        healthBar?.SetHealth(currentHealth);

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (!isAlive) return;

        isAlive = false;

        if (movement != null)
        {
            movement.StopMovement();
        }

        if (animator != null)
        {
            animator.SetBool("Death", true);
        }

        Collider enemyCollider = GetComponent<Collider>();
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        StartCoroutine(DeathSequence());

        OnDeath?.Invoke(this, reward);
    }

    private IEnumerator DeathSequence()
    {
        if (data.deathEffect != null)
        {
            Instantiate(data.deathEffect, transform.position, Quaternion.identity);
        }

        // Wait for death animation to play
        yield return new WaitForSeconds(deathAnimationDuration);

        StartCoroutine(FadeOutBeforeDestruction());

        yield return new WaitForSeconds(destroyDelay - deathAnimationDuration);

        Destroy(gameObject);
    }

    private IEnumerator FadeOutBeforeDestruction()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = renderer.material;
            Color originalColor = material.color;
            float fadeDuration = 1f;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
        }
    }

    public void ReachedGoal()
    {
        if (!isAlive) return;

        OnReachedGoal?.Invoke(this, data.damage);
        Destroy(gameObject);
    }

    public bool IsFlying()
    {
        return isFlying;
    }

    public float GetPathProgress()
    {
        float totalDistance = Vector3.Distance(spawnPosition, goalPosition);
        float currentDistance = Vector3.Distance(transform.position, goalPosition);
        return 1f - (currentDistance / totalDistance);
    }

    public EnemyData GetEnemyData()
    {
        return data;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    public void ForceDestroy()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}