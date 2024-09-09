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

    public FireMode fireMode;        //����

    public Bullet bullet;            //�Ѿ�
    public Transform bulletSpawn;    //�ѱ�(�Ѿ��� ������ ��ġ)

    public float shotTime = 100;      //�����
    public float bulletSpeed = 35;    //�Ѿ� �ӵ�

    public int burstCount;            //���簡 �߻��� �Ѿ� ����
    int shotsRemainingInBurst;        //�� �� �Ѿ� ����(����)

    bool triggerReleasedSinceLastShot;//�� �غ� ��?
    public int bulletPerMag;    //źâ �ִ� ũ��
    [SerializeField] int bulletRemainingInMag;   //���� ź�忡 ���� �ִ� �Ѿ� ����

    [Header("����")]
    bool isReloading;                 //���� ��?
    public float reloadTime;

    Vector3 recoilSmoothDampvelocity;
    float recoilRotSmoothDampVelocity;

    [Header("�� ȿ��")]
    public Transform shell;           //ź��
    public Transform shellEjection;   //ź�� ���ⱸ
    MuzzleFlash muzzleFlash;          //��
    float nextshottime;               //���� ź�� �߻��� �ð�


    [Header("�ݵ�")]
    public Vector2 kickMinMax = new Vector2(0.05f, 0.2f);
    public Vector2 recoilAngleMinMax = new Vector2(3, 5);
    float recoilAngle;  //�ݵ� ����
    public float recoilMoveSettleTime = 0.1f;
    public float recoilRotationSettleTime = 0.1f;

    private void Awake()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
    }

    private void Start()
    {
        shotsRemainingInBurst = burstCount;
        bulletRemainingInMag = bulletPerMag;
    }

    private void LateUpdate()
    {
        //�ݵ��� �ִϸ��̼� �����ϱ�.
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampvelocity, 0.1f);    //SmoothDamp : �ε巯�� �̵��� ����
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if(!isReloading && bulletRemainingInMag == 0)   //�������� �ƴϰ�, ���� źâ�� �Ѿ��� ���ٸ�.
            Reload();
    }

    /// <summary>
    /// ���
    /// </summary>
    void Shoot()
    {
        if(!isReloading && Time.time > nextshottime && bulletRemainingInMag > 0)
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

            if (bulletRemainingInMag != 0)
            {
                bulletRemainingInMag--;
                nextshottime = Time.time + shotTime / 1000;
                Bullet newBullet = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
                newBullet.SetSpeed(bulletSpeed);
            }

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);
        }
    }

    /// <summary>
    /// ������
    /// </summary>
    public void Reload()
    {
        if(!isReloading && bulletRemainingInMag != bulletPerMag)    //�������� �ƴϰ�, ���� ź���� �ִ� ź��� ���� �ʴٸ�
            StartCoroutine(AnimateReload());
    }

    IEnumerator AnimateReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(0.2f);

        float reloadSpeed = 1f / reloadTime;
        float percent = 0;  //�ִϸ��̼��� �󸶳� ����Ǿ�����. 
        Vector3 initialRot = transform.localEulerAngles;    //�ʱ� ȸ��
        float maxReloadAngle = 30;  //�ִ� ������ ����

        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

            yield return null;
        }

        isReloading = false;
        bulletRemainingInMag = bulletPerMag;    //���� ��Ű��.
    }

    public void Aim(Vector3 aimPoint)
    {
        if(!isReloading)
        {
            transform.LookAt(aimPoint);

            transform.Rotate(0, -90, 0);
        }
    }

    /// <summary>
    /// ��� ����
    /// </summary>
    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    /// <summary>
    /// ��� ����
    /// </summary>
    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }
}
