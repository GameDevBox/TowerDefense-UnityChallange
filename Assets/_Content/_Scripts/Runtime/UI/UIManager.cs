using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI currentWaveText;
    public TextMeshProUGUI currentMoney;
    public TextMeshProUGUI currentScore;
    public Slider healthSlider;
    public Button speedUpButton;
    public TextMeshProUGUI speedUpButtonText;

    [Header("Tower Placement UI")]
    public TextMeshProUGUI towerErrorText;
    public TextMeshProUGUI towerSuccessText;
    public float errorDisplayDuration = 2f;
    public float successDisplayDuration = 1.5f;

    [Header("Game Speed Settings")]
    public float[] speedLevels = { 1f, 2f, 4f, 8f };

    [Header("Game Result UI")]
    public TextMeshProUGUI gameResultText;
    public Color winColor = Color.green;
    public Color loseColor = Color.red;

    private GameData gameData;
    private int currentSpeedIndex = 0;

    private void Start()
    {
        gameData = GameData.Instance;
        if (gameData == null)
        {
            Debug.LogError("GameData instance not found!");
            return;
        }

        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        yield return null; // Wait one frame for everything to be ready

        // Initialize health slider
        healthSlider.maxValue = gameData.BaseHealth;
        healthSlider.minValue = 0;
        healthSlider.value = gameData.BaseHealth;

        // Initialize speed up button
        speedUpButton.onClick.AddListener(OnSpeedUpButtonClicked);
        UpdateSpeedButtonText();

        // Initialize all UI elements
        UpdateCurrentWave(gameData.CurrentWave);
        UpdateHealth(gameData.BaseHealth);
        UpdateMoney(gameData.Money);
        UpdateScore(gameData.Score);

        DebugLogsManager.Log("UIManager initialized successfully");
    }

    private void OnEnable()
    {
        GameEvents.OnCurrentWaveChanged += GameEvents_OnCurrentWaveChanged;
        GameEvents.OnBaseHealthChanged += GameEvents_OnBaseHealthChanged;
        GameEvents.OnMoneyChanged += GameEvents_OnMoneyChanged;
        GameEvents.OnScoreChanged += GameEvents_OnScoreChanged;
        GameEvents.OnGameOver += GameEvents_OnGameOver;
        GameEvents.OnPatternCompleted += GameEvents_OnPatternCompleted;
        GameEvents.OnTowerPlacementFailed += GameEvents_OnTowerPlacementFailed;
        GameEvents.OnTowerPlacementSuccess += GameEvents_OnTowerPlacementSuccess;
        GameEvents.OnTowerPlaced += GameEvents_OnTowerPlaced;
    }

    private void OnDisable()
    {
        GameEvents.OnCurrentWaveChanged -= GameEvents_OnCurrentWaveChanged;
        GameEvents.OnBaseHealthChanged -= GameEvents_OnBaseHealthChanged;
        GameEvents.OnMoneyChanged -= GameEvents_OnMoneyChanged;
        GameEvents.OnScoreChanged -= GameEvents_OnScoreChanged;
        GameEvents.OnGameOver -= GameEvents_OnGameOver;
        GameEvents.OnPatternCompleted -= GameEvents_OnPatternCompleted;
        GameEvents.OnTowerPlacementFailed -= GameEvents_OnTowerPlacementFailed;
        GameEvents.OnTowerPlacementSuccess -= GameEvents_OnTowerPlacementSuccess;
        GameEvents.OnTowerPlaced -= GameEvents_OnTowerPlaced;
    }

    private void Update()
    {
        HandleKeyboardShortcuts();
    }

    #region Event Handlers

    private void GameEvents_OnCurrentWaveChanged(int currentWave)
    {
        UpdateCurrentWave(currentWave);
    }

    private void GameEvents_OnBaseHealthChanged(int health)
    {
        UpdateHealth(health);
    }

    private void GameEvents_OnMoneyChanged(int money)
    {
        UpdateMoney(money);
    }

    private void GameEvents_OnScoreChanged(int score)
    {
        UpdateScore(score);
    }

    private void GameEvents_OnGameOver()
    {
        speedUpButton.interactable = false;

        currentSpeedIndex = 0;
        UpdateSpeedButtonText();

        ShowLoseMessage();
    }

    private void GameEvents_OnPatternCompleted(bool didLoop)
    {
        if (!didLoop)
        {
            ShowWinMessage();
        }
    }

    private void GameEvents_OnTowerPlacementFailed(TowerData towerData, string reason)
    {
        ShowTowerError(reason);
    }

    private void GameEvents_OnTowerPlacementSuccess(TowerData towerData)
    {
        // CODE
    }

    private void GameEvents_OnTowerPlaced(TowerData towerData, Vector3 position)
    {
        ShowTowerSuccess($"{towerData.towerName} placed!");
    }

    #endregion

    #region Game Speed Control

    private void OnSpeedUpButtonClicked()
    {
        CycleGameSpeed();
    }

    private void CycleGameSpeed()
    {
        currentSpeedIndex = (currentSpeedIndex + 1) % speedLevels.Length;

        Time.timeScale = speedLevels[currentSpeedIndex];

        UpdateSpeedButtonText();

        DebugLogsManager.Log($"Game speed changed to: {Time.timeScale}X");
    }

    private void UpdateSpeedButtonText()
    {
        if (speedUpButtonText != null)
        {
            speedUpButtonText.text = $"{Time.timeScale}X";
        }
    }

    private void HandleKeyboardShortcuts()
    {
        // Space bar to cycle through speeds
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CycleGameSpeed();
        }

        // Number keys for direct speed selection
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            SetGameSpeed(0); // 1x
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            SetGameSpeed(1); // 2x
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            SetGameSpeed(2); // 4x
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            SetGameSpeed(3); // 8x
        }

        // Escape to pause (1x speed)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetGameSpeed(0);
        }
    }

    private void SetGameSpeed(int speedIndex)
    {
        if (speedIndex >= 0 && speedIndex < speedLevels.Length)
        {
            currentSpeedIndex = speedIndex;
            Time.timeScale = speedLevels[currentSpeedIndex];
            UpdateSpeedButtonText();
        }
    }

    #endregion

    #region UI Update Methods

    private void ShowGameResult(string result, Color color)
    {
        if (gameResultText != null)
        {
            gameResultText.text = result;
            gameResultText.color = color;
            gameResultText.gameObject.SetActive(true);
        }
    }

    private void UpdateCurrentWave(int currentWave)
    {
        if (currentWaveText != null)
        {
            currentWaveText.text = $"Wave: {currentWave}/{gameData.TotalWaves}";
        }
    }

    private void UpdateHealth(int health)
    {
        if (healthSlider != null)
        {
            healthSlider.value = health;

            UpdateHealthBarColor(health / healthSlider.maxValue);
        }
    }

    private void UpdateHealthBarColor(float healthPercentage)
    {
        var fillImage = healthSlider.fillRect?.GetComponent<Image>();
        if (fillImage != null)
        {
            if (healthPercentage > 0.6f)
                fillImage.color = Color.green;
            else if (healthPercentage > 0.3f)
                fillImage.color = Color.yellow;
            else
                fillImage.color = Color.red;
        }
    }

    private void UpdateMoney(int money)
    {
        if (currentMoney != null)
        {
            currentMoney.text = $"${money}";
        }
    }

    private void UpdateScore(int score)
    {
        if (currentScore != null)
        {
            currentScore.text = $"Score: {score}";
        }
    }

    private void ShowTowerError(string errorMessage)
    {
        if (towerErrorText != null)
        {
            towerErrorText.text = errorMessage;
            towerErrorText.color = Color.red;
            towerErrorText.gameObject.SetActive(true);

            // Make sure success text is hidden
            if (towerSuccessText != null)
                towerSuccessText.gameObject.SetActive(false);

            StartCoroutine(HideErrorAfterDelay());
        }
    }

    private void ShowTowerSuccess(string successMessage)
    {
        if (towerSuccessText != null)
        {
            towerSuccessText.text = successMessage;
            towerSuccessText.color = Color.green;
            towerSuccessText.gameObject.SetActive(true);

            // Make sure error text is hidden
            if (towerErrorText != null)
                towerErrorText.gameObject.SetActive(false);

            StartCoroutine(HideSuccessAfterDelay());
        }
    }

    private IEnumerator HideErrorAfterDelay()
    {
        yield return new WaitForSeconds(errorDisplayDuration);

        if (towerErrorText != null)
        {
            towerErrorText.gameObject.SetActive(false);
        }
    }

    private IEnumerator HideSuccessAfterDelay()
    {
        yield return new WaitForSeconds(successDisplayDuration);

        if (towerSuccessText != null)
        {
            towerSuccessText.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Public Methods

    public void ShowGameOverScreen()
    {
        DebugLogsManager.Log("Game Over - Final Score: " + gameData.Score);
    }

    public void ShowWaveCompleteScreen(int waveNumber)
    {
        DebugLogsManager.Log($"Wave {waveNumber} completed!");
    }

    public void ShowWinMessage()
    {
        ShowGameResult("VICTORY!", winColor);
        DebugLogsManager.Log("WIN: All waves completed successfully!");
    }

    public void ShowLoseMessage()
    {
        ShowGameResult("DEFEAT!", loseColor);
        DebugLogsManager.Log("LOSE: Base was destroyed!");
    }

    #endregion

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        speedUpButton.onClick.RemoveAllListeners();
    }
}