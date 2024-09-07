using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startHealth;
    protected float health;
    protected bool dead;

    protected virtual void Start()
    {
        health = startHealth;
    }

    public void TakeHit(float damage, RaycastHit hit)
    {
        health -= damage;

        if(health <= 0 && !dead)
            Die();
    }

    protected void Die()
    {
        dead = true;
        Destroy(gameObject);
    }
}
