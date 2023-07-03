using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopHealth : SpaceshipHealth
{
    private Cop _cop => GetComponent<Cop>();

    public void Spawn(Vector3 location)
    {
        Spawn();
        _cop.Spawn(location);
    }

    private new void Start()
    {
        base.Start();
        OnTakeDamage += _cop.Agro;
    }
}
