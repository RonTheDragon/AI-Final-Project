using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Vector3 _direction;
    [SerializeField] private float _speed;
    // Update is called once per frame

    private void Start()
    {
        Destroy(gameObject,5);
    }

    void Update()
    {
        transform.position += (transform.rotation * _direction).normalized * _speed * Time.deltaTime;
    }
}
