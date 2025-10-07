using UnityEngine;

public class WaypointManager : Singleton<WaypointManager>
{
    protected override bool ShouldPersist => false;

    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Goal Position")]
    public Transform goalTransform; // Reference to the main hall/goal

    protected override void Awake()
    {
        base.Awake();

        // Validate waypoints
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("No waypoints assigned in WaypointManager!");
        }
    }

    public Transform[] GetWaypoints()
    {
        return waypoints;
    }

    public Vector3 GetGoalPosition()
    {
        if (goalTransform != null)
            return goalTransform.position;

        // Fallback: use last waypoint position
        if (waypoints != null && waypoints.Length > 0 && waypoints[waypoints.Length - 1] != null)
            return waypoints[waypoints.Length - 1].position;

        return Vector3.zero;
    }

    // Visualize waypoints in editor
    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.red;

        // Draw path lines
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }

        // Draw waypoint spheres
        Gizmos.color = Color.blue;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawSphere(waypoints[i].position, 0.3f);
            }
        }

        // Draw goal
        if (goalTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(goalTransform.position, Vector3.one * 0.5f);
        }
    }
}