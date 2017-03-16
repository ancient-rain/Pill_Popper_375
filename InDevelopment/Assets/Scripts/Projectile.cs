using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public LayerMask collisionMask;
    public Color trailColor;
    public float speed = 10;
    public float damage = 1;
    float life = 3;
    float offSet = .1f;

    void Start()
    {
        GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColor);
        Destroy(gameObject, life);
        Collider[] collisionsOnSpawn = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if (collisionsOnSpawn.Length > 0)
        {
            onHitObject(collisionsOnSpawn[0], transform.position);
        }
    }

	void Update () {
        float moveDistance = speed * Time.deltaTime;

        checkCollisions(moveDistance);
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
	}

    public void setSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void checkCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance + offSet, collisionMask, QueryTriggerInteraction.Collide))
        {
            onHitObject(hit.collider, hit.point);
        }
    }

    void onHitObject(Collider col, Vector3 hitPoint)
    {
        IDamageable damageableObject = col.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.takeHit(damage, hitPoint, transform.forward);
        }
        GameObject.Destroy(gameObject);
    }
}
