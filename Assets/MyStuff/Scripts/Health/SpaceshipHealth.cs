using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpaceshipHealth : Health
{
    [SerializeField] protected float _healthUpgrade = 50;

    protected override void Death()
    {
        base.Death();
        gameObject.SetActive(false);
    }

    public void Heal()
    {
        _currentHealth = _maxHealth;
    }

    public void UpgradeMaxHealth(int upgradeLevel)
    {
        _maxHealth = _startMaxHealth + _healthUpgrade* upgradeLevel;
        Heal();
    }
}
