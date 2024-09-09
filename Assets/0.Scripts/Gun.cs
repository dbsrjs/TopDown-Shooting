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

    public FireMode fireMode;        //상태

    public Bullet bullet;            //총알
    public Transform bulletSpawn;    //총구(총알을 생성할 위치)

    public float shotTime = 100;      //연사력
    public float bulletSpeed = 35;    //총알 속도

    public int burstCount;            //점사가 발사할 총알 개수
    int shotsRemainingInBurst;        //더 쏠 총알 개수(점사)

    bool triggerReleasedSinceLastShot;//쏠 준비 됨?
    public int bulletPerMag;    //탄창 최대 크기
    [SerializeField] int bulletRemainingInMag;   //현재 탄장에 남아 있는 총알 개수

    [Header("장전")]
    bool isReloading;                 //장전 중?
    public float reloadTime;

    Vector3 recoilSmoothDampvelocity;
    float recoilRotSmoothDampVelocity;

    [Header("총 효과")]
    public Transform shell;           //탄피
    public Transform shellEjection;   //탄피 배출구
    MuzzleFlash muzzleFlash;          //빛
    float nextshottime;               //다음 탄을 발사할 시간


    [Header("반동")]
    public Vector2 kickMinMax = new Vector2(0.05f, 0.2f);
    public Vector2 recoilAngleMinMax = new Vector2(3, 5);
    float recoilAngle;  //반동 각도
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
        //반동에 애니메이션 적용하기.
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampvelocity, 0.1f);    //SmoothDamp : 부드러운 이동을 구현
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if(!isReloading && bulletRemainingInMag == 0)   //장전중이 아니고, 현재 탄창에 총알이 없다면.
            Reload();
    }

    /// <summary>
    /// 사격
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
    /// 재장전
    /// </summary>
    public void Reload()
    {
        if(!isReloading && bulletRemainingInMag != bulletPerMag)    //장전중이 아니고, 현재 탄약이 최대 탄약과 같지 않다면
            StartCoroutine(AnimateReload());
    }

    IEnumerator AnimateReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(0.2f);

        float reloadSpeed = 1f / reloadTime;
        float percent = 0;  //애니메이션이 얼마나 진행되었는지. 
        Vector3 initialRot = transform.localEulerAngles;    //초기 회전
        float maxReloadAngle = 30;  //최대 재장전 각도

        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

            yield return null;
        }

        isReloading = false;
        bulletRemainingInMag = bulletPerMag;    //장전 시키기.
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
    /// 사격 시작
    /// </summary>
    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    /// <summary>
    /// 사격 종료
    /// </summary>
    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }
}
