using System;
using UnityEngine;
using UnityEngine.AI;

public class Spaceship : MonoBehaviour
{
    protected GameManager _gm;
    protected NavMeshAgent _agent => GetComponent<NavMeshAgent>();

    protected Action _currentState;

    // Start is called before the first frame update
    protected void Start()
    {
        _gm=GameManager.Instance;
    }

    protected void Update()
    {
        _currentState?.Invoke();
    }

    protected Vector3 FindFarthestDirection(Vector3 targetPosition, Vector3 enemyPosition, float radius, int numDirections)
    {
        Vector3[] directions = new Vector3[numDirections];
        float angleIncrement = 360f / numDirections;

        for (int i = 0; i < numDirections; i++)
        {
            float angle = i * angleIncrement;
            Vector3 direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            directions[i] = direction;
        }

        Vector3 farthestDirection = Vector3.zero;
        float farthestDistance = 0f;

        foreach (Vector3 direction in directions)
        {
            Vector3 targetPositionWithOffset = targetPosition + direction * radius;
            NavMeshHit navMeshHit;
            if (NavMesh.SamplePosition(targetPositionWithOffset, out navMeshHit, 2f, NavMesh.AllAreas))
            {
                float distance = Vector3.Distance(targetPositionWithOffset, enemyPosition);
                if (distance > farthestDistance)
                {
                    farthestDistance = distance;
                    farthestDirection = direction;
                }
            }
        }

        return farthestDirection;
    }
}
