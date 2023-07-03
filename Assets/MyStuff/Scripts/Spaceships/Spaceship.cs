using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public abstract class Spaceship : MonoBehaviour
{
    [Header("Scan")]
    [SerializeField] protected float _scanCooldown = 0.5f;
    protected float _scanCD;
    [SerializeField] protected float _scanRadius = 5;
    [SerializeField] protected float _angleOfVision = 50;
    protected Spaceship _target;
    [SerializeField] protected LayerMask _scanableLayers;
    [SerializeField] protected float _randomRoamRange = 5;

    protected GameManager _gm;
    protected Transform _attacker;
    [SerializeField] private float runAwayDistance = 20f; // Maximum distance to check for a safe direction
    protected NavMeshAgent _agent => GetComponent<NavMeshAgent>();

    protected SpaceshipHealth _health => GetComponent<SpaceshipHealth>();

    protected Action _currentState;

    [SerializeField] protected float _fixStuckAfter = 1;

    protected float _currentStuck;

    protected int _currency;
    protected float _startingSpeed;
    protected float _startingAccel;
    protected float _startingRot;
    protected float _currentSpeed;
    protected float _accelerationSpeed;
    protected float _rotationSpeed;
    protected int _speedLVL;

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

    protected bool CheckIfInFront(Vector2 target)
    {
        float targetAngle = Mathf.Atan2(target.y - transform.position.y, target.x - transform.position.x) * Mathf.Rad2Deg + 180;
        float deltaAngleAIAndTarget = _gm.AngleDifference(targetAngle, transform.eulerAngles.x);
        //Debug.Log($"{transform.eulerAngles.x} {targetAngle} = {deltaAngleAIAndTarget}");
        if (deltaAngleAIAndTarget < _angleOfVision / 2 && deltaAngleAIAndTarget > -_angleOfVision / 2)
        {
            return true;
        }
        return false;
    }

    protected virtual void Scan()
    {
        if (_scanCD > 0) { _scanCD -= Time.deltaTime; return; }
        _scanCD = _scanCooldown;
    }

    protected virtual void FlyAroundRandomly()
    {
        if (_agent.destination.IsUnityNull())
        {
            SetRandomDestination();
        }
        else if (Vector2.Distance(transform.position, _agent.destination) < 1)
        {
            SetRandomDestination();
        }
    }

    protected virtual void SetRandomDestination()
    {

        Vector3 newPos;

        float x = UnityEngine.Random.Range(-_randomRoamRange, _randomRoamRange);
        float y = UnityEngine.Random.Range(-_randomRoamRange, _randomRoamRange);
        newPos = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z);

        _agent.SetDestination(newPos);
    }

    protected void FixStuck()
    {
        if (_agent.velocity.magnitude < 1)
        {
            _currentStuck += Time.deltaTime;
        }

        if (_currentStuck > _fixStuckAfter)
        {
            _currentStuck = 0;
            SetRandomDestination();
        }
    }

    protected void BuySpeed()
    {
        _speedLVL++;
        _currentSpeed = _startingSpeed * (1 + _speedLVL * 0.001f);
        _accelerationSpeed = _startingAccel * (1 + _speedLVL * 0.01f);
        _rotationSpeed = _startingRot * (1 + _speedLVL * 0.1f);

        _agent.acceleration = _accelerationSpeed;
        _agent.angularSpeed = _rotationSpeed;
        _agent.speed = _currentSpeed;
    }

    public virtual void Spawn()
    {
        _currency = 0;
        _speedLVL = 1;
        if (_startingSpeed > 0)
        {
            _currentSpeed = _startingSpeed;
            _rotationSpeed = _startingRot;
            _accelerationSpeed = _startingAccel;
            _agent.speed = _currentSpeed;
            _agent.angularSpeed = _rotationSpeed;
            _agent.acceleration = _accelerationSpeed;
        }
    }
}
