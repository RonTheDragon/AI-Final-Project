using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Pirate : Spaceship
{

    [Tooltip("chance of deciding to go back to the Pirate Freighter")]
    [Range(0f, 100f)]
    [SerializeField] private float _homeSick = 20f;

    [SerializeField] private float _minimumRob = 5;

    private bool _untargetable;

    private int _stolenMinerals;
    

    PirateAttackSystem _attackSystem => GetComponent<PirateAttackSystem>();

    

    [Header("Icon")]

    [SerializeField] private GameObject _AttackOn;
    [SerializeField] private GameObject _AttackOff;




    private new void Start()
    {
        base.Start();
        _startingSpeed = _agent.speed;
        _startingAccel = _agent.acceleration;
        _startingRot = _agent.angularSpeed;
        _currentState = RoamState;
        Spawn();
    }

    private void RoamState()
    {
        FlyAroundRandomly();
        Scan();
        FixStuck();
    }

    private void AttackState()
    {
        if (_target == null) 
        {
            LoseTarget();
            return; 
        }

        if (!_target.isActiveAndEnabled)
        {
            LoseTarget();
            return;
        }

        if ((_target as Worker).GetMinerals()>0)
        {
            // newPos is on the NavMesh, set it as the destination
            _agent.SetDestination(_target.transform.position);
            _attackSystem.ShootingTarget();
        }
        else
        {
            LoseTarget();
        }
    }

    #region Roam
    protected override void SetRandomDestination()
    {
        if (_homeSick >= UnityEngine.Random.Range(0, 100))
        {
            _agent.SetDestination(_gm.PirateFreighter.transform.position);
            return;
        }

        Vector3 newPos;

        float x = UnityEngine.Random.Range(-_randomRoamRange, _randomRoamRange);
        float y = UnityEngine.Random.Range(-_randomRoamRange, _randomRoamRange);
        newPos = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z);

        _agent.SetDestination(newPos);
    }

    protected override void FlyAroundRandomly()
    {
        if (_agent.destination.IsUnityNull())
        {
            SetRandomDestination();
        }
        else if (Vector2.Distance(transform.position, _agent.destination) < 1)
        {
            SetRandomDestination();
            if (Vector3.Distance(_agent.destination, _gm.PirateFreighter.position) < 0.5f)
            {
                PirateShop();
            }
        }
    }

    private void PirateShop()
    {
        _untargetable = true;
        _health.Heal();
        _currency += _stolenMinerals * 40;
        _stolenMinerals = 0;


    }

    protected override void Scan()
    {
        // Timer
        base.Scan();
        if (_scanCD > 0) return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _scanRadius, _scanableLayers);
        foreach (Collider2D collider in colliders)
        {
            if (collider.transform.TryGetComponent<Worker>(out Worker w))
            {
                if (w.GetMinerals() >= _minimumRob)
                {
                    if (CheckIfInFront(new Vector2(w.transform.position.x, w.transform.position.y)))
                    {
                        _target = w;
                        _currentState = AttackState;
                        _AttackOn.SetActive(true);
                        _AttackOff.SetActive(false);
                        _untargetable = false;
                        return;
                    }
                }
            }
        }
    }

    

    
    #endregion


    private void LoseTarget()
    {
        _target = null;
        _currentState = RoamState;
        _AttackOn.SetActive(false);
        _AttackOff.SetActive(true);
        _agent.SetDestination(_gm.PirateFreighter.transform.position);
    }

    public bool IsAttacking()
    {
        return _currentState == AttackState;
    }

    public bool IsTargetable()
    {
        return !_untargetable;
    }

    public void StealMinerals(int minerals)
    {
        _stolenMinerals += minerals;
    }

    public override void Spawn()
    {
        base.Spawn();
        _stolenMinerals = 0;
    }
}
