using UnityEngine;

public class GameData : Singleton<GameData>
{
    protected override bool ShouldPersist => false;

    [Header("Player Data")]
    [SerializeField] private int baseHealth = 100;
    [SerializeField] private int money = 100;

    private int score = 0;
    private int currentWave = 1;
    private int totalWaves = 10;

    protected override void Awake()
    {
        base.Awake();
    }

    public int Money
    {
        get => money;
        set
        {
            if (money != value)
            {
                money = value;
                GameEvents.RaiseMoneyChanged(money);
            }
        }
    }

    public int Score
    {
        get => score;
        set
        {
            if (score != value)
            {
                score = value;
                GameEvents.RaiseScoreChanged(score);
            }
        }
    }

    public int BaseHealth
    {
        get => baseHealth;
        set
        {
            int newHealth = Mathf.Max(0, value);
            if (baseHealth != newHealth)
            {
                baseHealth = newHealth;
                GameEvents.RaiseBaseHealthChanged(baseHealth);

                // Check for game over when health changes
                if (baseHealth <= 0)
                {
                    GameEvents.RaiseGameOver();
                }
            }
        }
    }

    public int CurrentWave
    {
        get => currentWave;
        set
        {
            if (currentWave != value)
            {
                currentWave = value;
                GameEvents.RaiseCurrentWaveChanged(currentWave);
            }
        }
    }

    public int TotalWaves
    {
        get => totalWaves;
        set
        {
            if (totalWaves != value)
            {
                totalWaves = value;
                GameEvents.RaiseTotalWavesChanged(totalWaves);
            }
        }
    }

    // Public methods
    public void AddMoney(int amount) => Money += amount;
    public void AddScore(int amount) => Score += amount;
    public void DamageBase(int damage) => BaseHealth -= damage;
    public void HealBase(int healAmount) => BaseHealth += healAmount;
    public void SetTotalWaves(int total) => TotalWaves = total;

    // Reset game data
    public void ResetGame()
    {
        Money = 100;
        Score = 0;
        BaseHealth = 100;
        CurrentWave = 1;
        TotalWaves = 10;
    }

    // Check if player can afford something
    public bool CanAfford(int cost) => money >= cost;

    // Try to spend money - returns true if successful
    public bool TrySpendMoney(int amount)
    {
        if (CanAfford(amount))
        {
            Money -= amount;
            return true;
        }
        return false;
    }
}