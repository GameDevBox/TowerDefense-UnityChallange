using UnityEngine;
using UnityEngine.UI;

public class TowerSelectionUI : MonoBehaviour
{
    [Header("UI References")]
    public Button[] towerButtons;
    public TowerData[] towerDataList;

    private TowerPlacementController towerController;

    void Start()
    {
        towerController = FindFirstObjectByType<TowerPlacementController>();

        for (int i = 0; i < towerButtons.Length; i++)
        {
            int index = i; // Important: capture the index
            towerButtons[i].onClick.AddListener(() => SelectTower(index));

            // Update button visuals with tower data
            if (i < towerDataList.Length)
            {
                UpdateButtonVisuals(towerButtons[i], towerDataList[i]);
            }
        }
    }

    void SelectTower(int index)
    {
        if (index < towerDataList.Length)
        {
            towerController.SelectTower(towerDataList[index]);
        }
    }

    void UpdateButtonVisuals(Button button, TowerData towerData)
    {
        // Set button icon
        GameObject iconobj = button.transform.Find("Icon").gameObject;
        if (iconobj != null)
        {
            Image icon = iconobj.GetComponent<Image>();
            if (icon != null && towerData.icon != null)
            {
                icon.sprite = towerData.icon;
            }
        }

        // Add tooltip or description if needed
        TowerTooltip tooltip = button.GetComponent<TowerTooltip>();
        if (tooltip != null)
        {
            tooltip.SetTowerData(towerData);
        }
    }
}