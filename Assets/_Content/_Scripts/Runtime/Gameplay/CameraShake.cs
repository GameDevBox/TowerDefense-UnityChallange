using UnityEngine;
using DG.Tweening;

public class CameraShake : Singleton<CameraShake>
{
    protected override bool ShouldPersist => false;

    [Header("Shake Settings")]
    public float defaultDuration = 0.5f;
    public float defaultStrength = 0.3f;
    public int defaultVibrato = 10;
    public float defaultRandomness = 90f;

    private Vector3 initialPosition;
    private Tween currentShakeTween;

    protected override void Awake()
    {
        base.Awake();
        initialPosition = transform.localPosition;
    }

    void OnEnable()
    {
        initialPosition = transform.localPosition;
    }

    /// <summary>
    /// Shake the camera with default settings
    /// </summary>
    public void Shake()
    {
        Shake(defaultDuration, defaultStrength, defaultVibrato, defaultRandomness);
    }

    /// <summary>
    /// Shake the camera with custom duration and strength
    /// </summary>
    public void Shake(float duration, float strength)
    {
        Shake(duration, strength, defaultVibrato, defaultRandomness);
    }

    /// <summary>
    /// Shake the camera with full control over parameters
    /// </summary>
    public void Shake(float duration, float strength, int vibrato, float randomness, bool fadeOut = true)
    {
        // Kill any existing shake
        if (currentShakeTween != null && currentShakeTween.IsActive())
        {
            currentShakeTween.Kill();
        }

        // Reset position before starting new shake
        transform.localPosition = initialPosition;

        currentShakeTween = transform.DOShakePosition(
            duration: duration,
            strength: strength,
            vibrato: vibrato,
            randomness: randomness,
            snapping: false,
            fadeOut: fadeOut
        ).SetEase(Ease.OutQuad);

        // Ensure we return to initial position
        currentShakeTween.OnComplete(() => transform.localPosition = initialPosition);
    }

    /// <summary>
    /// Shake with specific axes (useful for 2D games)
    /// </summary>
    public void Shake2D(float duration, float strength, bool shakeX = true, bool shakeY = true)
    {
        if (currentShakeTween != null && currentShakeTween.IsActive())
        {
            currentShakeTween.Kill();
        }

        Vector3 strengthVector = new Vector3(
            shakeX ? strength : 0f,
            shakeY ? strength : 0f,
            0f
        );

        transform.localPosition = initialPosition;

        currentShakeTween = transform.DOShakePosition(
            duration: duration,
            strength: strengthVector,
            vibrato: defaultVibrato,
            randomness: defaultRandomness,
            snapping: false,
            fadeOut: true
        ).SetEase(Ease.OutQuad);

        currentShakeTween.OnComplete(() => transform.localPosition = initialPosition);
    }

    /// <summary>
    /// Advanced shake using preset
    /// </summary>
    public void ShakeAdvanced(ShakePreset preset)
    {
        Shake(preset.duration, preset.strength, preset.vibrato, preset.randomness, preset.fadeOut);
    }

    /// <summary>
    /// Impact shake - quick, intense shake
    /// </summary>
    public void ShakeImpact(float strength)
    {
        if (currentShakeTween != null && currentShakeTween.IsActive())
        {
            currentShakeTween.Kill();
        }

        transform.localPosition = initialPosition;

        currentShakeTween = transform.DOShakePosition(
            duration: 0.3f,
            strength: strength,
            vibrato: 15, // Higher vibrato for more intense shake
            randomness: 90f,
            snapping: false,
            fadeOut: true
        ).SetEase(Ease.OutExpo);

        currentShakeTween.OnComplete(() => transform.localPosition = initialPosition);
    }

    /// <summary>
    /// Rumble effect - longer, sustained shake
    /// </summary>
    public void Rumble(float duration, float intensity)
    {
        if (currentShakeTween != null && currentShakeTween.IsActive())
        {
            currentShakeTween.Kill();
        }

        transform.localPosition = initialPosition;

        currentShakeTween = transform.DOShakePosition(
            duration: duration,
            strength: intensity,
            vibrato: 5, // Lower vibrato for smoother rumble
            randomness: 60f,
            snapping: false,
            fadeOut: false // No fade out for consistent rumble
        ).SetEase(Ease.Linear);

        currentShakeTween.OnComplete(() => transform.localPosition = initialPosition);
    }

    /// <summary>
    /// Rotational shake for more dramatic effects
    /// </summary>
    public void ShakeRotation(float duration, float strength, int vibrato = 10)
    {
        transform.DOShakeRotation(
            duration: duration,
            strength: Vector3.forward * strength, // Shake around Z-axis for 2D
            vibrato: vibrato,
            randomness: 90f,
            fadeOut: true
        );
    }

    /// <summary>
    /// Combined position and rotation shake
    /// </summary>
    public void ShakeAll(float duration, float posStrength, float rotStrength)
    {
        Shake(duration, posStrength);
        ShakeRotation(duration, rotStrength);
    }

    /// <summary>
    /// Stop any active shake immediately
    /// </summary>
    public void StopShake()
    {
        if (currentShakeTween != null && currentShakeTween.IsActive())
        {
            currentShakeTween.Kill();
        }
        transform.localPosition = initialPosition;
        transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Pause current shake
    /// </summary>
    public void PauseShake()
    {
        if (currentShakeTween != null && currentShakeTween.IsActive())
        {
            currentShakeTween.Pause();
        }
    }

    /// <summary>
    /// Resume paused shake
    /// </summary>
    public void ResumeShake()
    {
        if (currentShakeTween != null && currentShakeTween.IsActive())
        {
            currentShakeTween.Play();
        }
    }

    protected override void OnDestroy()
    {
        if (currentShakeTween != null && currentShakeTween.IsActive())
        {
            currentShakeTween.Kill();
        }
    }
}

/// <summary>
/// Preset data for different types of shakes
/// </summary>
[System.Serializable]
public struct ShakePreset
{
    public string name;
    public float duration;
    public float strength;
    public int vibrato;
    public float randomness;
    public bool fadeOut;

    // Common presets
    public static readonly ShakePreset Light = new ShakePreset
    {
        name = "Light",
        duration = 0.3f,
        strength = 0.1f,
        vibrato = 8,
        randomness = 90f,
        fadeOut = true
    };

    public static readonly ShakePreset Medium = new ShakePreset
    {
        name = "Medium",
        duration = 0.5f,
        strength = 0.3f,
        vibrato = 10,
        randomness = 90f,
        fadeOut = true
    };

    public static readonly ShakePreset Heavy = new ShakePreset
    {
        name = "Heavy",
        duration = 0.8f,
        strength = 0.5f,
        vibrato = 12,
        randomness = 90f,
        fadeOut = true
    };

    public static readonly ShakePreset Explosion = new ShakePreset
    {
        name = "Explosion",
        duration = 1.0f,
        strength = 0.7f,
        vibrato = 15,
        randomness = 90f,
        fadeOut = true
    };

    public static readonly ShakePreset Impact = new ShakePreset
    {
        name = "Impact",
        duration = 0.2f,
        strength = 0.4f,
        vibrato = 20,
        randomness = 90f,
        fadeOut = true
    };

    public static readonly ShakePreset Rumble = new ShakePreset
    {
        name = "Rumble",
        duration = 2.0f,
        strength = 0.2f,
        vibrato = 5,
        randomness = 60f,
        fadeOut = false
    };
}