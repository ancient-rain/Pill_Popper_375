using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {

    void takeHit(float damage, Vector3 hitPoint, Vector3 hitDirection);

    void takeHitFromEnemy(float damage);

}
