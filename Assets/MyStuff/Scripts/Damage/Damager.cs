using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damager : MonoBehaviour
{
    protected Transform _attacker;
    protected LayerMask _attackableLayers;
    protected float _currentDamage;

    public void SetDamage(float damage,Transform _attacker, LayerMask _attackableLayers)
    {
        _currentDamage = damage;
        this._attacker = _attacker;
        this._attackableLayers = _attackableLayers;
    }
}
