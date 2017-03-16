using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity {
    public CrossHair crossHair;
    public float moveSpeed = 5;
    PlayerController controller;
    Camera viewCamera;
    GunController gunController;

	protected override void Start () {
        base.Start();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
	}

    void Awake()
    {
        FindObjectOfType<Spawner>().onNewWave += onNewWave;
    }
	
	void Update () {
        //Movement
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.move(moveVelocity);
        //Look
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.up * gunController.getHeight);
        float rayDistance;

        if (ground.Raycast(ray, out rayDistance))
        {
            Vector3 pointOfIntersection = ray.GetPoint(rayDistance);
            controller.lookAt(pointOfIntersection);
            crossHair.transform.position = pointOfIntersection;
            crossHair.detectTargets(ray);
            if((new Vector2(pointOfIntersection.x, pointOfIntersection.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
            {
                gunController.aim(pointOfIntersection);
            }
            
        }
        //Weapon
        if(Input.GetMouseButton(0))
        {
            gunController.onTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            gunController.onTriggerRelease();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            gunController.reload();
        }

        if(transform.position.y < -10)
        {
            die();
        }
    }

    void onNewWave(int waveNumber)
    {
        health = startingHealth;
        gunController.equipWeapon(waveNumber - 1);
    }

    public override void die()
    {
        AudioManager.instance.playSound("Player Death", transform.position);
        base.die();
    }
}
