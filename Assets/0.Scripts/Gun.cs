using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle;    //총구(총알을 생성할 위치)
    public Bullet bullet;

    public float bulletSpeed = 35;    //총알 속도
    public float shotTime = 100;      //연사력
    float nextshottime;

    public void Shoot()
    {
        if(Time.time > nextshottime)
        {
            nextshottime = Time.time + shotTime / 1000;
            Bullet newBullet = Instantiate(bullet, muzzle.position, muzzle.rotation);
            newBullet.SetSpeed(bulletSpeed);
        }
    }
}
