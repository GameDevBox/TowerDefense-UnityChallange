using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private float speed;
    private bool hasPath = false;
    private bool isStopped = false;
    private Enemy enemy;

    public void Initialize(float enemySpeed, Transform[] pathWaypoints)
    {
        speed = enemySpeed;
        waypoints = pathWaypoints;
        hasPath = waypoints != null && waypoints.Length > 0;
        isStopped = false;
        enemy = GetComponent<Enemy>();

        if (hasPath && waypoints[0] != null)
        {
            transform.position = waypoints[0].position;
            currentWaypointIndex = 1;
        }
    }

    void Update()
    {
        if (!hasPath || waypoints == null || currentWaypointIndex >= waypoints.Length || isStopped)
            return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];

        if (targetWaypoint == null)
        {
            currentWaypointIndex++;
            return;
        }

        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;
        transform.position = newPosition;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        float distanceToWaypoint = Vector3.Distance(transform.position, targetWaypoint.position);
        if (distanceToWaypoint < 0.1f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                hasPath = false;
                if (enemy != null)
                {
                    enemy.ReachedGoal();
                }
            }
        }
    }

    public void StopMovement()
    {
        isStopped = true;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public bool HasReachedGoal()
    {
        return !hasPath && currentWaypointIndex >= waypoints.Length;
    }
}