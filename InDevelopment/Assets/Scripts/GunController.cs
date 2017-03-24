 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    public Transform weaponHold;
    public Gun[] guns;
    Gun equipedGun;

    void Start()
    {
        
    }

    public void equipWeapon(Gun gunToEquip)
    {
        if(equipedGun != null)
        {
            Destroy(equipedGun.gameObject);
        }
        equipedGun = (Gun)Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation);
        equipedGun.transform.parent = weaponHold;
    }

    public void equipWeapon(int weaponIndex)
    {
        equipWeapon(guns[weaponIndex]);
    }

    public void onTriggerHold()
    {
        if(equipedGun != null)
        {
            equipedGun.onTriggerHold();
        }
    }

    public void onTriggerRelease()
    {
        if (equipedGun != null)
        {
            equipedGun.onTriggerRelease();
        }
    }

    public float getHeight
    {
        get {
            return weaponHold.position.y;
        }
    }

    public void aim(Vector3 aimPoint)
    {
        if(equipedGun != null)
        {
            equipedGun.aim(aimPoint);
        }
    }

    public void reload()
    {
        if(equipedGun != null)
        {
            equipedGun.reload();
        }
    }

}
