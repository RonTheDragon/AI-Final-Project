using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public ObjectPooler OP => GetComponentInChildren<ObjectPooler>();

    public Transform Mine;
    public Vector2 MineralsExtraction;
    public Transform Factory;
    public Transform PirateFreighter;
    public Transform PoliceStation;

    private float _currentSecond = 1;
    public Action OnEverySecond;

    public int WorkersAmount;
    public int PiratesAmount;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        OnEverySecond += Spawners;
    }

    // Update is called once per frame
    void Update()
    {
        OneSecondLoop();
    }

    private void OneSecondLoop()
    {
        if (_currentSecond > 0) 
        {
            _currentSecond -=Time.deltaTime;
            return; 
        }
        _currentSecond = 1;
        OnEverySecond?.Invoke();
    }

    private void Spawners()
    {
        if (WorkersAmount > 0)
        {
            OP.SpawnFromPool("Worker",Factory.position,Quaternion.identity).GetComponent<WorkerHealth>().Spawn();
            WorkersAmount--;
        }
        if (PiratesAmount > 0)
        {
            OP.SpawnFromPool("Pirate", PirateFreighter.position, Quaternion.identity);
            PiratesAmount--;
        }
    }

    public void CallThePoliceToLocation(Vector3 location)
    {

    }

    public float AngleDifference(float angle1, float angle2)
    {
        float diff = (angle2 - angle1 + 180) % 360 - 180;
        return diff < -180 ? diff + 360 : diff;
    }
}
