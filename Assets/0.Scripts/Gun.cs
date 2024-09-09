using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle;    //ÃÑ±¸(ÃÑ¾ËÀ» »ý¼ºÇÒ À§Ä¡)
    public Bullet bullet;

    public float bulletSpeed = 35;    //ÃÑ¾Ë ¼Óµµ
    public float shotTime = 100;      //¿¬»ç·Â
    float nextshottime;

    public Transform shell;           //ÅºÇÇ
    public Transform shellEjection;   //ÅºÇÇ ¹èÃâ±¸

    MuzzleFlash muzzleFlash;

    private void Awake()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
    }


    public void Shoot()
    {
        if(Time.time > nextshottime)
        {
            nextshottime = Time.time + shotTime / 1000;
            Bullet newBullet = Instantiate(bullet, muzzle.position, muzzle.rotation);
            newBullet.SetSpeed(bulletSpeed);

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();
        }
    }
}
