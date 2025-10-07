using UnityEngine;
using static Codice.Client.Common.Connection.AskCredentialsToUser;

public class TowerPlacementController : MonoBehaviour
{
    [Header("References")]
    public LayerMask placementLayerMask;
    public Transform towersContainer;
    public Material validMaterial;
    public Material invalidMaterial;

    public AudioSource buildSoundEffect; // It's better to move this to TowerData!

    private TowerData selectedTower;
    private GameData gameData;
    private Camera mainCamera;
    private TowerSpot currentHighlightSpot;

    void Start()
    {
        gameData = FindFirstObjectByType<GameData>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleTowerPlacement();
        HandleGridHighlight();
    }

    void HandleTowerPlacement()
    {
        if (Input.GetMouseButtonDown(0) && selectedTower != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Raise attempt event
            GameEvents.RaiseTowerPlacementAttempt(selectedTower);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayerMask))
            {
                TowerSpot spot = hit.collider.GetComponent<TowerSpot>();

                if (spot != null && IsSpotValid(spot))
                {
                    if (gameData.Money >= selectedTower.cost)
                    {
                        PlaceTower(spot, selectedTower);
                        gameData.Money -= selectedTower.cost;

                        // Raise success event
                        GameEvents.RaiseTowerPlacementSuccess(selectedTower);
                        DebugLogsManager.Log($"Tower placed!", gameObject);

                        ClearHighlight();
                    }
                    else
                    {
                        // Raise failed event - not enough money
                        string reason = $"Not enough money! Need ${selectedTower.cost} but have ${gameData.Money}";
                        GameEvents.RaiseTowerPlacementFailed(selectedTower, reason);
                        DebugLogsManager.Log(reason, gameData);
                    }
                }
                else
                {
                    // Raise failed event - invalid spot
                    string reason = "Cannot place tower here - spot is occupied or invalid";
                    GameEvents.RaiseTowerPlacementFailed(selectedTower, reason);
                    DebugLogsManager.Log(reason, gameData);
                }
            }
            else
            {
                // Raise failed event - no valid spot under mouse
                string reason = "No valid tower spot under cursor";
                GameEvents.RaiseTowerPlacementFailed(selectedTower, reason);
            }
        }

        // Right click to cancel selection
        if (Input.GetMouseButtonDown(1))
        {
            CancelSelection();
        }
    }

    void HandleGridHighlight()
    {
        if (selectedTower == null)
        {
            ClearHighlight();
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayerMask))
        {
            TowerSpot spot = hit.collider.GetComponent<TowerSpot>();

            if (spot != null)
            {
                bool isValid = IsSpotValid(spot) && gameData.Money >= selectedTower.cost;
                UpdateHighlight(spot, isValid);
            }
            else
            {
                ClearHighlight();
            }
        }
        else
        {
            ClearHighlight();
        }
    }

    bool IsSpotValid(TowerSpot spot)
    {
        if (spot == null)
            return false;

        return spot.CanPlaceTower();
    }

    void UpdateHighlight(TowerSpot spot, bool isValid)
    {
        if (currentHighlightSpot != spot)
        {
            ClearHighlight();
            currentHighlightSpot = spot;
        }

        spot.SetHighlightMaterial(isValid ? validMaterial : invalidMaterial);
    }

    void ClearHighlight()
    {
        if (currentHighlightSpot != null)
        {
            currentHighlightSpot.ResetMaterial();
            currentHighlightSpot = null;
        }
    }

    void PlaceTower(TowerSpot spot, TowerData towerData)
    {
        if (towerData.towerPrefab != null)
        {
            Vector3 spawnPosition = spot.GetPlacementPosition();
            GameObject towerObj = Instantiate(towerData.towerPrefab, spawnPosition, Quaternion.identity);
            towerObj.transform.parent = towersContainer;
            Tower tower = towerObj.GetComponent<Tower>();

            if (tower != null)
            {
                buildSoundEffect.Play();
                tower.Initialize(towerData);

                // Register the tower with the spot
                if (spot.PlaceTower(tower))
                {
                    // Raise tower placed event
                    GameEvents.RaiseTowerPlaced(towerData, spawnPosition);
                    DebugLogsManager.Log($"Tower successfully placed on spot!", this);
                }
                else
                {
                    DebugLogsManager.LogWarning("Failed to register tower with spot!", this);
                }
            }
            else
            {
                Debug.LogWarning("Tower prefab doesn't have Tower component!");
            }
        }
        else
        {
            Debug.LogError("No tower prefab assigned in TowerData!");
        }
    }

    public void SelectTower(TowerData towerData)
    {
        if (towerData != null)
        {
            selectedTower = towerData;
            DebugLogsManager.Log($"Selected: {towerData.towerName} (Cost: {towerData.cost})", this);
        }
    }

    public void SelectTowerFromArray(TowerData[] availableTowers, int towerIndex)
    {
        if (availableTowers != null && towerIndex >= 0 && towerIndex < availableTowers.Length)
        {
            SelectTower(availableTowers[towerIndex]);
        }
    }

    public bool HasTowerSelected()
    {
        return selectedTower != null;
    }

    public TowerData GetSelectedTower()
    {
        return selectedTower;
    }

    public void CancelSelection()
    {
        selectedTower = null;
        ClearHighlight();
    }

    void OnDisable()
    {
        ClearHighlight();
    }

    void OnDestroy()
    {
        ClearHighlight();
    }

    // Optional: Method to get all valid tower spots (for AI or other systems)
    public TowerSpot[] GetAllValidTowerSpots()
    {
        TowerSpot[] allSpots = FindObjectsByType<TowerSpot>(FindObjectsSortMode.None);
        System.Collections.Generic.List<TowerSpot> validSpots = new System.Collections.Generic.List<TowerSpot>();

        foreach (TowerSpot spot in allSpots)
        {
            if (spot.CanPlaceTower())
            {
                validSpots.Add(spot);
            }
        }

        return validSpots.ToArray();
    }

    // Optional: Method to get all occupied tower spots
    public TowerSpot[] GetAllOccupiedTowerSpots()
    {
        TowerSpot[] allSpots = FindObjectsByType<TowerSpot>(FindObjectsSortMode.None);
        System.Collections.Generic.List<TowerSpot> occupiedSpots = new System.Collections.Generic.List<TowerSpot>();

        foreach (TowerSpot spot in allSpots)
        {
            if (spot.IsOccupied())
            {
                occupiedSpots.Add(spot);
            }
        }

        return occupiedSpots.ToArray();
    }
}