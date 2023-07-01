using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Pirate : Spaceship
{
    [Header("Roaming")]

    [SerializeField] private float _randomRoamRange = 5;
    [SerializeField] private LayerMask _scanableLayers;

    [Tooltip("chance of deciding to go back to the Pirate Freighter")]
    [Range(0f, 100f)]
    [SerializeField] private float _homeSick = 20f;

    [SerializeField] private float _scanCooldown = 0.5f;
    private float _scanCD;
    [SerializeField] private float _scanRadius = 5;

    [SerializeField] private float _minimumRob = 5;

    [SerializeField] private float _angleOfVision = 50;

    private Worker _target;

    [SerializeField] private float _fixStuckAfter = 1;

    private float _currentStuck;

    [Header("Attacking")]

    [SerializeField] private string _currentAmmo = "PirateShot";
    [SerializeField] private float _attackDamage = 50;
    [SerializeField] private float _shootCooldown = 1;
    private float _shootCD;
    [SerializeField] private LayerMask _attackableLayers;

    [Header("Icon")]

    [SerializeField] private GameObject _AttackOn;
    [SerializeField] private GameObject _AttackOff;




    private new void Start()
    {
        base.Start();

        _currentState = RoamState;
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

        if (_target.Minerals>0)
        {
            // newPos is on the NavMesh, set it as the destination
            _agent.SetDestination(_target.transform.position);
            ShootingTarget();
        }
        else
        {
            LoseTarget();
        }
    }

    #region Roam
    private void FlyAroundRandomly()
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

    private void SetRandomDestination()
    {
        if (_homeSick >= Random.Range(0,100))
        {
            _agent.SetDestination(_gm.PirateFreighter.transform.position);
            return;
        }

        Vector3 newPos;
        
        float x = Random.Range(-_randomRoamRange, _randomRoamRange);
        float y = Random.Range(-_randomRoamRange, _randomRoamRange);
        newPos = new Vector3(transform.position.x+x,transform.position.y+y,transform.position.z);
            
        _agent.SetDestination(newPos);
    }

    private void Scan()
    {
        // Timer
        if (_scanCD > 0) { _scanCD -= Time.deltaTime; return; }
        _scanCD = _scanCooldown;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _scanRadius, _scanableLayers);
        foreach (Collider2D collider in colliders)
        {
            if (collider.transform.TryGetComponent<Worker>(out Worker w))
            {
                if (w.Minerals >= _minimumRob)
                {
                    if (CheckIfInFront(new Vector2(w.transform.position.x, w.transform.position.y)))
                    {
                        _target = w;
                        _currentState = AttackState;
                        _AttackOn.SetActive(true);
                        _AttackOff.SetActive(false);
                        return;
                    }
                }
            }
        }
    }

    private void FixStuck()
    {
        if (_agent.velocity.magnitude < 1)
        {
            _currentStuck += Time.deltaTime;
        }

        if (_currentStuck> _fixStuckAfter)
        {
            _currentStuck = 0;
            SetRandomDestination();
        }
    }

    private bool CheckIfInFront(Vector2 target)
    {
        float targetAngle = Mathf.Atan2(target.y - transform.position.y, target.x - transform.position.x) * Mathf.Rad2Deg +180;
        float deltaAngleAIAndTarget = _gm.AngleDifference(targetAngle, transform.eulerAngles.x);
        //Debug.Log($"{transform.eulerAngles.x} {targetAngle} = {deltaAngleAIAndTarget}");
        if (deltaAngleAIAndTarget < _angleOfVision / 2 && deltaAngleAIAndTarget > -_angleOfVision / 2)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region Attack

    private void LoseTarget()
    {
        _target = null;
        _currentState = RoamState;
        _AttackOn.SetActive(false);
        _AttackOff.SetActive(true);
    }

    private void ShootingTarget()
    {
        if (_shootCD>0)
        {
            _shootCD -= Time.deltaTime; return;
        }
        _shootCD = _shootCooldown;
        _gm.OP.SpawnFromPool(_currentAmmo, transform.position, transform.rotation,true)
            .GetComponent<ProjectileDamage>().SetDamage(_attackDamage, transform,_attackableLayers);
    }

    #endregion
}
