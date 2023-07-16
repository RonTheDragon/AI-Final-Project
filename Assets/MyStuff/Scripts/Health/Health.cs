using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    protected bool _isAlive = true;
    [SerializeField] protected float _maxHealth;
    [SerializeField] protected float _currentHealth;
    protected float _startMaxHealth;
    protected GameManager _gm;
    public Action<Transform> OnTakeDamage;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _gm = GameManager.Instance;
        _startMaxHealth = _maxHealth;
        Spawn();
    }

    public virtual void Spawn()
    {
        _isAlive = true;
        if (_startMaxHealth != 0)
        {
            _maxHealth = _startMaxHealth;
        }
        _currentHealth = _maxHealth;
    }

    public virtual void TakeDamage(float damage,Transform Attacker)
    {
        if (!_isAlive) 
            return;

        OnTakeDamage?.Invoke(Attacker);
        _currentHealth -= damage;
        if (_currentHealth <= 0) 
        { 
            Death();
        }
    }

    protected virtual void Death()
    {
        _isAlive = false;
    }
}
