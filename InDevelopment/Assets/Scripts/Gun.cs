using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
    public enum FireMode {auto, burst, single};
    public FireMode firemode;
    public Transform[] muzzles;
    public Projectile bullet;
    public Transform shell;
    public Transform shellEjection;
    public float reloadTime;
    public float msBetweenShot = 100;
    public float muzzleVelocity = 35;
    public float recoilReturnSpeed = .1f;
    public float kickReturnSpeed = .1f;
    public int burstCount;
    public int bulletsPerMag;
    public Vector2 kickMinMax = new Vector2(.05f, .2f);
    public Vector2 recoilMinMax = new Vector2(3, 5);
    public AudioClip shootAudio;
    public AudioClip reloadAudio;

    MuzzleFlash muzzleFlash;
    bool triggerRelease;
    bool reloading;
    Vector3 recoilSmoothDampVelocity;
    float recoilAngleSmoothVelocity;
    Vector3 startingPos;
    float nextShotTime;
    float recoilAngle;
    int shotsRemainingInBurst;
    int remainingBulletsInMag;

    void Start()
    {
        startingPos = transform.localPosition;
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
        remainingBulletsInMag = bulletsPerMag;
        reloading = false;
    }

    void LateUpdate()
    {
        if (!reloading)
        {
            if(remainingBulletsInMag <= 0)
            {
                reload();
            }
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, startingPos, ref recoilSmoothDampVelocity, kickReturnSpeed);
            recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilAngleSmoothVelocity, recoilReturnSpeed);
            transform.localEulerAngles = Vector3.left * recoilAngle;
        }
    }

    void shoot()
    {
        if(!reloading && Time.time > nextShotTime && remainingBulletsInMag > 0)
        {
            if(firemode == FireMode.burst)
            {
                if(shotsRemainingInBurst == 0)
                {
                    return;
                }
                shotsRemainingInBurst--;
            }
            else if(firemode == FireMode.single)
            {
                if (!triggerRelease)
                {
                    return;
                }
            }

            for(int i = 0; i < muzzles.Length; i++)
            {
                if(remainingBulletsInMag == 0)
                {
                    break;
                }
                remainingBulletsInMag--;
                nextShotTime = Time.time + msBetweenShot / 1000;
                Projectile newProjectile = (Projectile)Instantiate(bullet, muzzles[i].position, muzzles[i].rotation);
                newProjectile.setSpeed(muzzleVelocity);
            }

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.activate();
            recoilAngle += Random.Range(recoilMinMax.x, recoilMinMax.y);
            Mathf.Clamp(recoilAngle, 0, 30);
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            AudioManager.instance.playSound(shootAudio, transform.position);
        }
    }

    public void reload()
    {
        if (!reloading && remainingBulletsInMag != bulletsPerMag)
        {
            StartCoroutine(animateReload());
            AudioManager.instance.playSound(reloadAudio, transform.position);
        }
    }

    IEnumerator animateReload()
    {
        reloading = true;
        yield return new WaitForSeconds(.2f);

        float reloadSpeed = 1f / reloadTime;
        float percent = 0;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30;

        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

            yield return null;
        }


        reloading = false;
        remainingBulletsInMag = bulletsPerMag;
    }

    public void onTriggerHold()
    {
        shoot();
        triggerRelease = false;
    }

    public void onTriggerRelease()
    {
        triggerRelease = true;
        shotsRemainingInBurst = burstCount;
    }

    public void aim(Vector3 aimPoint)
    {
        if (!reloading)
        {
            transform.LookAt(aimPoint);
        }
    }

}
