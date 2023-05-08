using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spaceship : MonoBehaviour
{
    protected GameManager _gm;
    protected NavMeshAgent agent => GetComponent<NavMeshAgent>();
    // Start is called before the first frame update
    protected void Start()
    {
        _gm=GameManager.Instance;
    }
}