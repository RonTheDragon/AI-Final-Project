using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Spaceship
{
    private int _currency;
    public int Minerals;
    private float _currentSpeed;
    private float _startingSpeed;
    private float _accelerationSpeed;
    private float _startingRotation;

    [SerializeField] private GameObject _mineralOff;
    [SerializeField] private GameObject _mineralOn;

    private new void Start()
    {
        base.Start();
        _startingSpeed = agent.speed;
        _currentSpeed = _startingSpeed;
        _startingRotation = agent.angularSpeed;
        _accelerationSpeed = agent.acceleration;
        agent.SetDestination(_gm.Mine.position);
    }

    private void Update()
    {
        if(Vector2.Distance(transform.position, agent.destination) < 1)
        {
            agent.enabled = false;
            agent.enabled = true;
            if (Minerals > 0)
            {
                _currency += Minerals*40;
                Minerals = 0;
                _currentSpeed = _startingSpeed * (1 + _currency * 0.0001f);
                agent.speed = _currentSpeed;
                agent.acceleration = _accelerationSpeed * (1 + _currency * 0.001f);
                agent.angularSpeed = _startingRotation * (1 + _currency * 0.01f);
                agent.SetDestination(_gm.Mine.position);
                _mineralOff.SetActive(true);
                _mineralOn.SetActive(false);
            }
            else
            {
                Minerals = (int)Random.Range(_gm.MineralsExtraction.x, _gm.MineralsExtraction.y);
                agent.SetDestination(_gm.Factory.position);
                agent.speed = _currentSpeed*0.5f;
                _mineralOff.SetActive(false);
                _mineralOn.SetActive(true);
            }
        }
    }
}
