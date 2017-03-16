using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity {

    public static event System.Action onDeathStatic;

    NavMeshAgent pathFinder;
    Transform target;
    public enum State
    {
        Idle,
        Chasing,
        Attacking
    };
    State currentState;
    LivingEntity targetEntity;
    Material skinMat;
    Color original;

    float attackDistanceThreshold = .5f;
    float timeBetweenAttacks = 1;
    float nextAttack;
    float damage = 0;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;
    public ParticleSystem deathEffect;

    void Awake()
    {
        pathFinder = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
            targetEntity = target.GetComponent<LivingEntity>();
        }
    }

	protected override void Start () {
        base.Start();

        if (hasTarget)
        {
            currentState = State.Chasing;
            targetEntity.onDeath += onTargetDeath;
            StartCoroutine(updatePath());
        }
        
	}

    void onTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }
	
	void Update () {
        if (hasTarget)
        {
            if (Time.time > nextAttack)
            {
                float squareDistanceToTarget = (target.position - transform.position).sqrMagnitude;

                if (squareDistanceToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttack = Time.time + timeBetweenAttacks;
                    AudioManager.instance.playSound("Enemy Attack", transform.position);
                    StartCoroutine(attack());
                }
            }
        }
	}

    IEnumerator attack()
    {

        currentState = State.Attacking;
        pathFinder.enabled = false;

        Vector3 oriPos = transform.position;
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 attackPos = target.position - directionToTarget * (myCollisionRadius);
        float percent = 0;
        float attackSpeed = 3;
        bool damaging = false;
        skinMat.color = Color.red;

        while(percent <= 1)
        {
            if(percent >= .5f && !damaging)
            {
                damaging = true;
                targetEntity.takeHitFromEnemy(damage);
            }

            percent += Time.deltaTime * attackSpeed;
            float interpol = 4 * (-percent * percent + percent);
            transform.position = Vector3.Lerp(oriPos, attackPos, interpol);
            yield return null;
        }

        skinMat.color = original;

        currentState = State.Chasing;
        pathFinder.enabled = true;
    }

    public void setCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor)
    {
        pathFinder.speed = moveSpeed;
        if (hasTarget)
        {
            damage += Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
        }
        deathEffect.startColor = new Color(skinColor.r, skinColor.g, skinColor.b, 1);
        startingHealth = enemyHealth;
        skinMat = GetComponent<Renderer>().material;
        skinMat.color = skinColor;
        original = skinMat.color;
    }

    public override void takeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        AudioManager.instance.playSound("Impact", transform.position);
        if (damage >= health)
        {
            if(onDeathStatic != null)
            {
                onDeathStatic();
            }
            Destroy((GameObject)Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)), deathEffect.main.startLifetime.constant);
            AudioManager.instance.playSound("Enemy Death", transform.position);
        }
        base.takeHit(damage, hitPoint, hitDirection);
    }

    IEnumerator updatePath()
    {
        float refreshRate = .25f;

        while (hasTarget)
        {
            if(currentState == State.Chasing)
            {
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - directionToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
                if (!dead)
                {
                    pathFinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
