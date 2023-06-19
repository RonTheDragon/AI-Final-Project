using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerHealth : SpaceshipHealth
{
    protected override void Death()
    {
        base.Death();
        _gm.WorkersAmount++;
    }
}
