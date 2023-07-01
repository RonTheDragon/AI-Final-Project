using System;
using UnityEngine;

public class Worker : Spaceship
{
    public int Minerals;
    private int _currency;
    private float _currentSpeed;
    private float _startingSpeed;
    private float _accelerationSpeed;
    private float _startingRotation;
    private WorkerHealth _health => GetComponent<WorkerHealth>();


    [Header("Icon")]
    [SerializeField] private GameObject _mineralOff;
    [SerializeField] private GameObject _mineralOn;

    private new void Start()
    {
        base.Start();
        _startingSpeed = _agent.speed;
        _currentSpeed = _startingSpeed;
        _startingRotation = _agent.angularSpeed;
        _accelerationSpeed = _agent.acceleration;
        _agent.SetDestination(_gm.Mine.position);
        _currentState = WorkState;
    }

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

    private void Panic(Transform Attacker)
    {

    }

    private void CallThePolice()
    {

    }

}
