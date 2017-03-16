using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    Rigidbody myRigidbody;
    Vector3 thisVelocity;

	void Start () {
        myRigidbody = GetComponent<Rigidbody>();
	}
	
    public void move(Vector3 velocity){
        thisVelocity = velocity;
    }

    void FixedUpdate()
    {
        myRigidbody.MovePosition(myRigidbody.position + thisVelocity * Time.fixedDeltaTime);
    }

    public void lookAt(Vector3 lookPoint)
    {
        Vector3 newLookPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(newLookPoint);
    }
}
