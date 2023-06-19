using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpaceshipHealth : Health
{
    protected override void Death()
    {
        base.Death();
        gameObject.SetActive(false);
    }
}
