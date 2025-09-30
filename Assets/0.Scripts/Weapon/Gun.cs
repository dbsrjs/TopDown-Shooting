using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode
    {
        Auto,   //자동
        Burst,  //연사
        Single  //단발
    };

    public FireMode fireMode;          //사격모드

    public Bullet bullet;              //총알
    public Transform[] bulletSpawn;    //총구(총알이 생성될 위치)

    public float shotTime = 100;      //발사속도
    public float bulletSpeed = 35;    //총알 속도
    public int burstCount;            //연사시 발사될 총알 개수
    public int bulletPerMag;          //탄창 최대 크기

    int shotsRemainingInBurst;        //한 번에 쏠 탄약 개수(연사)

    bool triggerReleasedSinceLastShot;//총 쏠 준비 됨?
    public int bulletRemainingInMag;   //현재 탄창에 남아 있는 총알 개수

    [Header("재장전")]
    bool isReloading;                 //재장전 중?
    public float reloadTime;          //재장전 시간

    Vector3 recoilSmoothDampvelocity;
    float recoilRotSmoothDampVelocity;

    [Header("총 효과")]
    public Transform shell;           //탄피
    public Transform shellEjection;   //탄피 배출구
    MuzzleFlash muzzleFlash;          //총구 화염
    float nextshottime;               //다음 탄환 발사될 시간


    [Header("반동")]
    public Vector2 kickMinMax = new Vector2(0.05f, 0.2f);
    public Vector2 recoilAngleMinMax = new Vector2(3, 5);
    public float recoilMoveSettleTime = 0.1f;
    public float recoilRotationSettleTime = 0.1f;
    float recoilAngle;  //반동 각도

    [Header("Audio")]
    public AudioClip shootAudio;    //발포음
    public AudioClip reloadAudio;   //재장전음

    [Header("Random Gun Settings")]
    public RandomGunSettings randomGunSettings;

    private void Awake()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
    }

    private void Start()
    {
        // 랜덤 모드일 때 총 스탯을 랜덤으로 설정
        if (GameSettings.Instance != null && GameSettings.Instance.IsRandomMapMode())
        {
            ApplyRandomGunStats();
            // 웨이브가 바뀔 때마다 총 스탯도 새로 설정
            Spawner.Instance.OnNewWave += OnNewWave;
        }

        shotsRemainingInBurst = burstCount;
        bulletRemainingInMag = bulletPerMag;
    }

    /// <summary>
    /// 새 웨이브 시작 시 총 스탯 재설정
    /// </summary>
    void OnNewWave(int waveNumber)
    {
        if (GameSettings.Instance != null && GameSettings.Instance.IsRandomMapMode())
        {
            ApplyRandomGunStats();
            // 탄창도 새로 리셋
            bulletRemainingInMag = bulletPerMag;
            shotsRemainingInBurst = burstCount;
        }
    }

    private void LateUpdate()
    {
        //반동의 애니메이션 복구하기.
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampvelocity, 0.1f);    //SmoothDamp : 부드럽게 이동을 만듦
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if(!isReloading && bulletRemainingInMag == 0)   //재장전이 아니고, 현재 탄창에 총알이 없다면.
            Reload();
    }

    /// <summary>
    /// 발포
    /// </summary>
    void Shoot()
    {
        //장전중 아님, 다음 발사 시간 지남, 탄창에 총알 남아있음
        if (!isReloading && Time.time > nextshottime && bulletRemainingInMag > 0)
        {
            if(fireMode == FireMode.Burst)  //점사
            {
                if (shotsRemainingInBurst == 0)
                    return;

                shotsRemainingInBurst--;
            }
            else if (fireMode == FireMode.Single)   //단밣
            {
                if (!triggerReleasedSinceLastShot)
                    return;
            }

            for (int i = 0; i < bulletSpawn.Length; i++)
            {
                if (bulletRemainingInMag == 0)
                {
                    break;
                }
                bulletRemainingInMag--;
                nextshottime = Time.time + shotTime / 1000;
                Bullet newBullet = Instantiate(bullet, bulletSpawn[i].position, bulletSpawn[i].rotation);
                newBullet.SetSpeed(bulletSpeed);

                // 조준점이 적을 가리키고 있으면 치명타로 설정
                Crosshairs crosshairs = FindObjectOfType<Crosshairs>();
                if (crosshairs != null && crosshairs.isTargetingEnemy)
                {
                    newBullet.SetCriticalHit(true);
                }
            }

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();
            transform.localPosition -= Vector3.right * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

            AudioManager.instance.PlaySound(shootAudio, transform.position);
        }
    }

    /// <summary>
    /// 재장전
    /// </summary>
    public void Reload()
    {
        if(!isReloading && bulletRemainingInMag != bulletPerMag)    //재장전이 아니고, 현재 탄창이 최대 탄환과 같지 않다면
        {
            StartCoroutine(AnimateReload());
            AudioManager.instance.PlaySound(reloadAudio, transform.position);
        }
    }

    /// <summary>
    /// 재장전 애니메이션
    /// </summary>
    IEnumerator AnimateReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(0.2f);

        float reloadSpeed = 1f / reloadTime;
        float percent = 0;  //애니메이션이 얼마나 진행되었나요.
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
        bulletRemainingInMag = bulletPerMag;    //탄환 리셋됨.
    }

    /// <summary>
    /// 총구를 바라봄
    /// </summary>
    public void Aim(Vector3 aimPoint)
    {
        if(!isReloading)
        {
            transform.LookAt(aimPoint);
        }
    }

    /// <summary>
    /// 트리거 누름
    /// </summary>
    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    /// <summary>
    /// 트리거 놓음
    /// </summary>
    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }

    /// <summary>
    /// 랜덤 총 스탯 적용
    /// </summary>
    void ApplyRandomGunStats()
    {
        System.Random rand = new System.Random(System.DateTime.Now.Millisecond);

        // 사격 모드 랜덤 선택
        System.Array fireModes = System.Enum.GetValues(typeof(FireMode));
        fireMode = (FireMode)fireModes.GetValue(rand.Next(fireModes.Length));

        // 발사 속도 랜덤 설정 (낮을수록 빠름)
        shotTime = randomGunSettings.minShotTime +
            (float)rand.NextDouble() * (randomGunSettings.maxShotTime - randomGunSettings.minShotTime);

        // 총알 속도 랜덤 설정
        bulletSpeed = randomGunSettings.minBulletSpeed +
            (float)rand.NextDouble() * (randomGunSettings.maxBulletSpeed - randomGunSettings.minBulletSpeed);

        // 연사 개수 랜덤 설정 (Burst 모드일 때만 의미있음)
        burstCount = rand.Next(randomGunSettings.minBurstCount, randomGunSettings.maxBurstCount + 1);

        // 탄창 크기 랜덤 설정
        bulletPerMag = rand.Next(randomGunSettings.minBulletPerMag, randomGunSettings.maxBulletPerMag + 1);

        // 재장전 시간 랜덤 설정
        reloadTime = randomGunSettings.minReloadTime +
            (float)rand.NextDouble() * (randomGunSettings.maxReloadTime - randomGunSettings.minReloadTime);

        Debug.Log($"랜덤 총 설정 적용: 모드={fireMode}, 발사속도={shotTime:F1}, 총알속도={bulletSpeed:F1}, 연사개수={burstCount}, 탄창={bulletPerMag}, 재장전={reloadTime:F1}");
    }

    [System.Serializable]
    public class RandomGunSettings
    {
        [Header("Shot Time Range (ms, 낮을수록 빠름)")]
        public float minShotTime = 50f;
        public float maxShotTime = 200f;

        [Header("Bullet Speed Range")]
        public float minBulletSpeed = 20f;
        public float maxBulletSpeed = 50f;

        [Header("Burst Count Range")]
        public int minBurstCount = 2;
        public int maxBurstCount = 5;

        [Header("Magazine Size Range")]
        public int minBulletPerMag = 10;
        public int maxBulletPerMag = 30;

        [Header("Reload Time Range")]
        public float minReloadTime = 1f;
        public float maxReloadTime = 4f;
    }
}