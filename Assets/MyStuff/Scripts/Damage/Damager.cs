using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damager : MonoBehaviour
{
    [SerializeField] protected LayerMask _attackableLayers;
    [SerializeField] protected float _currentDamage;

    private void SetDamage(float damage)
    {
        _currentDamage = damage;
    }
}
