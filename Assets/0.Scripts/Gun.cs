using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle;    //�ѱ�(�Ѿ��� ������ ��ġ)
    public Bullet bullet;

    public float bulletSpeed = 35;    //�Ѿ� �ӵ�
    public float shotTime = 100;      //�����
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
