using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    protected override bool ShouldPersist => false;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        GameEvents.OnBaseHealthChanged += GameEvents_OnBaseHealthChanged;
        GameEvents.OnGameOver += GameEvents_OnGameOver;
    }

    private void OnDisable()
    {
        GameEvents.OnBaseHealthChanged -= GameEvents_OnBaseHealthChanged;
        GameEvents.OnGameOver -= GameEvents_OnGameOver;
    }


    private void GameEvents_OnBaseHealthChanged(int health)
    {
        CameraShake.Instance.Shake(0.4f, 0.2f);
        //CameraShake.Instance.ShakeAdvanced(ShakePreset.Impact);
    }


    private void GameEvents_OnGameOver()
    {
        Time.timeScale = 0f;
    }
}
