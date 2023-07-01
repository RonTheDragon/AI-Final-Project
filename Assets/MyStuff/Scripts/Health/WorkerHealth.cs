using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerHealth : SpaceshipHealth
{
    private Worker _worker => GetComponent<Worker>();


    private new void Start()
    {
        base.Start();
        OnTakeDamage += _worker.Panic;
    }

    public override void Spawn()
    {
        base.Spawn();
        _worker.Spawn();
    }

    protected override void Death()
    {
        base.Death();
        _gm.WorkersAmount++;
    }
}
