using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource movementAudioSource;
    public AudioSource effectAudioSource;
    public AudioSource voiceAudioSource;

    [Header("Audio Clips")]
    public AudioClip[] spawnClips;
    public AudioClip[] deathClips;
    public AudioClip[] damageClips;
    public AudioClip[] movementClips;

    [Header("Audio Settings")]
    public float movementVolume = 0.3f;
    public float minMovementPitch = 0.8f;
    public float maxMovementPitch = 1.2f;
    public float footstepInterval = 0.5f;

    private EnemyData enemyData;
    private Coroutine footstepCoroutine;
    private bool isMoving = false;

    public void Initialize(EnemyData data)
    {
        enemyData = data;

        // Set up audio sources
        InitializeAudioSources();

        // Play spawn sound
        PlaySpawnSound();
    }

    private void InitializeAudioSources()
    {
        if (movementAudioSource == null)
            movementAudioSource = gameObject.AddComponent<AudioSource>();

        if (effectAudioSource == null)
            effectAudioSource = gameObject.AddComponent<AudioSource>();

        if (voiceAudioSource == null)
            voiceAudioSource = gameObject.AddComponent<AudioSource>();

        // Configure audio sources
        movementAudioSource.spatialBlend = 1.0f; // 3D sound
        movementAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        movementAudioSource.maxDistance = 20f;

        effectAudioSource.spatialBlend = 1.0f;
        effectAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        effectAudioSource.maxDistance = 25f;

        voiceAudioSource.spatialBlend = 1.0f;
        voiceAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        voiceAudioSource.maxDistance = 30f;
    }

    public void PlaySpawnSound()
    {
        if (enemyData != null && enemyData.spawnSound != null)
        {
            effectAudioSource.PlayOneShot(enemyData.spawnSound);
        }
        else if (spawnClips.Length > 0)
        {
            var clip = spawnClips[Random.Range(0, spawnClips.Length)];
            effectAudioSource.PlayOneShot(clip);
        }
    }

    public void OnDamage()
    {
        PlayDamageSound();
    }

    public void OnDeath()
    {
        PlayDeathSound();
        StopMovementSounds();
    }

    private void PlayDamageSound()
    {
        if (enemyData != null)
        {
            if (damageClips.Length > 0)
            {
                var clip = damageClips[Random.Range(0, damageClips.Length)];
                voiceAudioSource.pitch = Random.Range(0.8f, 1.2f);
                voiceAudioSource.PlayOneShot(clip);
            }
        }
    }

    private void PlayDeathSound()
    {
        if (enemyData != null && enemyData.deathSound != null)
        {
            effectAudioSource.PlayOneShot(enemyData.deathSound);
        }
        else if (deathClips.Length > 0)
        {
            var clip = deathClips[Random.Range(0, deathClips.Length)];
            effectAudioSource.PlayOneShot(clip);
        }
    }

    private void StopMovementSounds()
    {
        if (footstepCoroutine != null)
        {
            StopCoroutine(footstepCoroutine);
            footstepCoroutine = null;
        }

        if (movementAudioSource != null)
        {
            movementAudioSource.Stop();
        }
    }

    public void UpdateMovementAudio(float speed)
    {
        // Adjust audio based on movement speed
        if (movementAudioSource != null)
        {
            movementAudioSource.pitch = Mathf.Lerp(0.8f, 1.2f, speed / 5f);
            movementAudioSource.volume = Mathf.Lerp(0.1f, movementVolume, speed / 5f);
        }
    }

    void OnDestroy()
    {
        StopMovementSounds();
    }
}