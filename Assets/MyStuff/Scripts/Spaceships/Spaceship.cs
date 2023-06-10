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
}
