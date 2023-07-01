using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackSystem : MonoBehaviour
{
    protected GameManager _gm;

    [Header("Attacking")]
    [SerializeField] private string _currentAmmo = "PirateShot";
    [SerializeField] private float _attackDamage = 50;
    [SerializeField] private float _shootCooldown = 1;
    private float _shootCD;
    [SerializeField] private LayerMask _attackableLayers;

    protected void Start()
    {
        _gm = GameManager.Instance;
    }

    public void ShootingTarget()
    {
        if (_shootCD > 0)
        {
            _shootCD -= Time.deltaTime; return;
        }
        _shootCD = _shootCooldown;
        _gm.OP.SpawnFromPool(_currentAmmo, transform.position, transform.rotation, true)
            .GetComponent<ProjectileDamage>().SetDamage(_attackDamage, transform, _attackableLayers);
    }
}
