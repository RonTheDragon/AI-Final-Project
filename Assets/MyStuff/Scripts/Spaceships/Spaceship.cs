using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class Spaceship : MonoBehaviour
{
    protected GameManager _gm;
    protected Transform _attacker;
    [SerializeField] private float runAwayDistance = 20f; // Maximum distance to check for a safe direction
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

    public void RunAwayFromAttacker()
    {
        if (_attacker == null)
        {
            return;
        }

        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            // Worker has reached or is very close to the current destination

            // Check the current agent velocity
            if (_agent.velocity.magnitude < 0.1f)
            {
                // If the velocity is too low, force pick "run to a random direction"
                RunToRandomDirection();
            }
            else
            {
                // Randomly pick between "run away to the opposite direction" and "run to a random direction"
                int randomIndex = UnityEngine.Random.Range(0, 3);
                if (randomIndex < 2)
                {
                    RunToOppositeDirection();
                }
                else
                {
                    RunToRandomDirection();
                }
            }
        }
    }

    private void RunToOppositeDirection()
    {
        Vector2 attackerDirection = (_attacker.position - transform.position).normalized;
        Vector2 oppositeDirection = -attackerDirection;
        Vector2 destination = (Vector2)transform.position + oppositeDirection * runAwayDistance;

        _agent.SetDestination(destination);

    }

    private void RunToRandomDirection()
    {
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
        Vector2 destination = (Vector2)transform.position + randomDirection * runAwayDistance;

        _agent.SetDestination(destination);

    }
}
