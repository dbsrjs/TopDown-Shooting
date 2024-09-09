using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode
    {
        Auto,   //����
        Burst,  //����
        Single  //�ܹ�
    };

    public FireMode fireMode;

    public Transform[] bulletSpawn;    //�ѱ�(�Ѿ��� ������ ��ġ)
    public Bullet bullet;

    public float bulletSpeed = 35;    //�Ѿ� �ӵ�
    public float shotTime = 100;      //�����
    float nextshottime;               //���� ź�� �߻��� �ð�

    public int burstCount;            //���簡 �߻��� �Ѿ� ����
    int shotsRemainingInBurst;        //�� �� �Ѿ� ����(����)

    public Transform shell;           //ź��
    public Transform shellEjection;   //ź�� ���ⱸ

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
