using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TowerTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltipPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI statsText;

    private TowerData currentTowerData;

    public void SetTowerData(TowerData towerData)
    {
        currentTowerData = towerData;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipPanel != null && currentTowerData != null)
        {
            nameText.text = currentTowerData.towerName;
            descriptionText.text = currentTowerData.description;
            statsText.text = $"Cost: {currentTowerData.cost}\nDamage: {currentTowerData.damage}\nRange: {currentTowerData.range}";
            tooltipPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }
}