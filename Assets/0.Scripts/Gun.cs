using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode
    {
        Auto,   //연사
        Burst,  //점사
        Single  //단발
    };

    public FireMode fireMode;

    public Transform[] bulletSpawn;    //총구(총알을 생성할 위치)
    public Bullet bullet;

    public float bulletSpeed = 35;    //총알 속도
    public float shotTime = 100;      //연사력
    float nextshottime;               //다음 탄을 발사할 시간

    public int burstCount;            //점사가 발사할 총알 개수
    int shotsRemainingInBurst;        //더 쏠 총알 개수(점사)

    public Transform shell;           //탄피
    public Transform shellEjection;   //탄피 배출구

    MuzzleFlash muzzleFlash;

    bool triggerReleasedSinceLastShot;
    

    private void Awake()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
    }

    private void Start()
    {
        shotsRemainingInBurst = burstCount;
    }

    void Shoot()
    {
        if(Time.time > nextshottime)
        {
            if(fireMode == FireMode.Burst)
            {
                if (shotsRemainingInBurst == 0)
                    return;

                shotsRemainingInBurst--;
            }
            else if (fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                    return;
            }

            for(int i = 0; i < bulletSpawn.Length; i++)
            {
                nextshottime = Time.time + shotTime / 1000;
                Bullet newBullet = Instantiate(bullet, bulletSpawn[i].position, bulletSpawn[i].rotation);
                newBullet.SetSpeed(bulletSpeed);
            }

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();
        }
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }
}
