using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Vector3 _direction;
    [SerializeField] private float _speed;
    // Update is called once per frame


    void Update()
    {
        transform.position += (transform.rotation * _direction).normalized * _speed * Time.deltaTime;
    }
}
