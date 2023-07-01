using System;
using UnityEngine;
using UnityEngine.AI;

public class Worker : Spaceship
{
    public int Minerals;
    private int _currency;
    private float _currentSpeed;
    private float _startingSpeed;
    private float _accelerationSpeed;
    private float _startingRotation;

    [Header("Panic")]
    [SerializeField] private float _panicDuration = 10;
    private float _panicTimeLeft;
    [SerializeField] private float _callThePoliceCooldown = 5;
    private float _callThePoliceCD;
    [SerializeField] private float _panicSpeedMult = 2f;

    [Header("Icon")]
    [SerializeField] private GameObject _mineralOff;
    [SerializeField] private GameObject _mineralOn;
    [SerializeField] private GameObject _underAttack;

    private new void Start()
    {
        base.Start();
        _startingSpeed = _agent.speed;
        Spawn();
    }

    #region States
    private void WorkState()
    {
        if (Vector2.Distance(transform.position, _agent.destination) < 1)
        {
            _agent.enabled = false;
            _agent.enabled = true;
            if (Minerals > 0)
            {
                SellMinerals();
            }
            else
            {
                MineMinerals();
            }
        }
    }

    private void PanicState()
    {
        if (_attacker == null)
        {
            StopPanic();
            return;
        }

        if (_callThePoliceCD > 0)
        {
            _callThePoliceCD -= Time.deltaTime;
        }

        if (_panicTimeLeft > 0)
        {
            _panicTimeLeft -= Time.deltaTime;
        }
        else
        {
            StopPanic();
            return;
        }
        RunAwayFromAttacker();
    }
    #endregion

    private void MineMinerals()
    {
        Minerals = (int)UnityEngine.Random.Range(_gm.MineralsExtraction.x, _gm.MineralsExtraction.y);
        _agent.SetDestination(_gm.Factory.position);
        _agent.speed = _currentSpeed * 0.5f;
        _mineralOff.SetActive(false);
        _mineralOn.SetActive(true);
    }

    private void SellMinerals()
    {
        _currency += Minerals * 40;
        Minerals = 0;
        _currentSpeed = _startingSpeed * (1 + _currency * 0.0001f);
        _agent.speed = _currentSpeed;
        _agent.acceleration = _accelerationSpeed * (1 + _currency * 0.001f);
        _agent.angularSpeed = _startingRotation * (1 + _currency * 0.01f);
        _agent.SetDestination(_gm.Mine.position);
        _mineralOff.SetActive(true);
        _mineralOn.SetActive(false);
    }

    public void Panic(Transform attacker)
    {
        _attacker = attacker;
        _panicTimeLeft = _panicDuration;
        _currentState = PanicState;
        _underAttack.SetActive(true);
        _agent.speed = _currentSpeed * _panicSpeedMult;
        if (_callThePoliceCD <= 0) { CallThePolice(); _callThePoliceCD = _callThePoliceCooldown; }
    }

    private void CallThePolice()
    {
        if (_attacker == null)
        {
            return; 
        }
        _gm.CallThePoliceToLocation(_attacker.transform.position);
    }


    private void StopPanic()
    {
        _attacker = null;
        _underAttack.SetActive(false);
        _currentState = WorkState;
    }

    public void Spawn()
    {
        StopPanic(); 
        _gm = GameManager.Instance;
        _currentSpeed = _startingSpeed;
        _startingRotation = _agent.angularSpeed;
        _accelerationSpeed = _agent.acceleration;
        _agent.SetDestination(_gm.Mine.position);
    }

}
