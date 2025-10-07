using System.Collections;
using UnityEngine;

public class EnemyVisual : MonoBehaviour
{
    [Header("Visual Components")]
    public Renderer meshRenderer;
    public Animator animator;
    public ParticleSystem trailParticles;
    public ParticleSystem damageParticles;
    public ParticleSystem healParticles;

    [Header("Visual Settings")]
    public float damageFlashDuration = 0.1f;
    public Color damageFlashColor = Color.red;
    public Color healFlashColor = Color.green;

    private Material originalMaterial;
    private Material instanceMaterial;
    private Color originalColor;
    private Coroutine flashCoroutine;

    public void Initialize(EnemyData data)
    {
        if (meshRenderer != null)
        {
            originalMaterial = meshRenderer.material;
            instanceMaterial = Instantiate(originalMaterial);
            meshRenderer.material = instanceMaterial;
            originalColor = instanceMaterial.color;
        }

        ApplyEnemyVisuals(data);

        if (animator != null)
        {
            animator.SetFloat("MoveSpeed", data.baseSpeed);

            if (data.isBoss)
            {
                animator.SetBool("IsBoss", true);
            }
        }
    }

    private void ApplyEnemyVisuals(EnemyData data)
    {
        transform.localScale = Vector3.one * data.size;

        if (data.isBoss)
        {
            if (instanceMaterial != null)
            {
                instanceMaterial.SetColor("_EmissionColor", Color.red * 2f);
                instanceMaterial.EnableKeyword("_EMISSION");
            }
        }
    }

    public void OnDamage(int damageAmount)
    {
        // Flash effect
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(DamageFlash());

        // Damage particles
        if (damageParticles != null)
        {
            damageParticles.Play();
        }

        // Shake animation
        if (animator != null)
        {
            animator.SetTrigger("TakeDamage");
        }

        // Screen shake for big damage
        if (damageAmount > 50)
        {
            CameraShake.Instance?.Shake(0.3f, 0.2f);
        }
    }

    public void OnDeath()
    {
        // Death animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Stop trail particles
        if (trailParticles != null)
        {
            trailParticles.Stop();
        }

        // Disable collider immediately
        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }

    public void UpdateMovementVisual(float speed, bool isMoving)
    {
        if (animator != null)
        {
            animator.SetFloat("MoveSpeed", speed);
            animator.SetBool("IsMoving", isMoving);
        }
    }

    private IEnumerator DamageFlash()
    {
        if (instanceMaterial != null)
        {
            instanceMaterial.color = damageFlashColor;
            yield return new WaitForSeconds(damageFlashDuration);
            instanceMaterial.color = originalColor;
        }
    }

    public void SetTintColor(Color color)
    {
        if (instanceMaterial != null)
        {
            instanceMaterial.color = color;
            originalColor = color;
        }
    }

    void OnDestroy()
    {
        if (instanceMaterial != null)
        {
            Destroy(instanceMaterial);
        }
    }
}