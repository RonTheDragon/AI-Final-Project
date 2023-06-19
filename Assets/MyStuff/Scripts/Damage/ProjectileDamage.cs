using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : Damager
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_attackableLayers == (_attackableLayers | (1 << collision.gameObject.layer)))
        {
            if (collision.transform.TryGetComponent<Health>(out Health health))
            {
                health.TakeDamage(_currentDamage);
                gameObject.SetActive(false);
            }
        }
    }
}
