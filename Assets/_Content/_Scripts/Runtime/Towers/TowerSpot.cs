using UnityEngine;

public class TowerSpot : MonoBehaviour
{
    [Header("Placement Point")]
    public Transform placementPoint;

    [Header("Tower Spot Settings")]
    public float placementRadius = 0.5f;
    public LayerMask towerLayerMask;

    private Tower currentTower;
    private Renderer spotRenderer;
    private Material originalMaterial;

    void Start()
    {
        spotRenderer = GetComponent<Renderer>();
        if (spotRenderer != null)
        {
            originalMaterial = spotRenderer.material;
        }
    }

    public bool IsOccupied()
    {
        return currentTower != null;
    }

    public bool CanPlaceTower()
    {
        if (IsOccupied())
            return false;

        return true;
    }
    public Vector3 GetPlacementPosition()
    {
        return placementPoint != null ? placementPoint.position : transform.position + Vector3.up * 0.1f;
    }

    public bool PlaceTower(Tower tower)
    {
        if (!CanPlaceTower())
            return false;

        currentTower = tower;
        return true;
    }

    public void RemoveTower()
    {
        currentTower = null;
    }

    public Tower GetTower()
    {
        return currentTower;
    }

    public void SetHighlightMaterial(Material material)
    {
        if (spotRenderer != null)
        {
            spotRenderer.material = material;
        }
    }

    public void ResetMaterial()
    {
        if (spotRenderer != null && originalMaterial != null)
        {
            spotRenderer.material = originalMaterial;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, placementRadius);
    }

    private void OnDrawGizmos()
    {
        if (placementPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(placementPoint.position, 0.1f);
            Gizmos.DrawLine(transform.position, placementPoint.position);
        }
    }
}
