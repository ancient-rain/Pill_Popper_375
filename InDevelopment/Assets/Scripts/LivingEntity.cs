using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth;
    public float health { get; protected set; }
    protected bool dead;

    public event System.Action onDeath;

    protected virtual void Start()
    {
        health = startingHealth;
    }

    public virtual void takeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        takeHitFromEnemy(damage);
    }

    [ContextMenu("Self Destruct")]
    public virtual void die()
    {
        this.dead = true;
        if(onDeath != null)
        {
            onDeath();
        }
        GameObject.Destroy(gameObject);
    }

    public virtual void takeHitFromEnemy(float damage)
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            die();
        }
    }

}
