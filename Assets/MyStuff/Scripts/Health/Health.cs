using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    protected bool _isAlive = true;
    [SerializeField] protected float _maxHealth;
    [SerializeField] protected float _currentHealth;
    protected GameManager _gm;
    public Action<Transform> OnTakeDamage;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _gm = GameManager.Instance;
        Spawn();
    }

    public void Spawn()
    {
        _isAlive = true;
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
