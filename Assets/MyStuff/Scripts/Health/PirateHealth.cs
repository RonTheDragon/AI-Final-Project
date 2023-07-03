using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateHealth : SpaceshipHealth
{
    private Pirate _pirate => GetComponent<Pirate>();
    protected override void Death()
    {
        base.Death();
        _gm.PiratesAmount++;
    }

    public override void Spawn()
    {
        _pirate.Spawn();
    }
}
