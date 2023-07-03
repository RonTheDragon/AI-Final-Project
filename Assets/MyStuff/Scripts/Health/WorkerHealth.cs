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

    public override void TakeDamage(float damage, Transform Attacker)
    {
        if (!_isAlive)
            return;

        OnTakeDamage?.Invoke(Attacker);
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            if(Attacker.TryGetComponent<Pirate>(out Pirate p))
            {
                p.StealMinerals(_worker.GetMinerals());
            }
            Death();
        }
    }
}
