using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State
    {
        Idle,       //아무것도 안 하는 상태
        Chasing,    //플레이어 추격 상태
        Attacking   //플레이어 공격 상태
    };
    State currentState; //현재 상태

    public static event Action OnDeathStatic;

    NavMeshAgent pathfinder;       //nav
    Transform target;              //Player
    LivingEntity targetEntity;

    Material skinMaterial;             //적의 스킨 색 머티리얼
    public ParticleSystem deathEffect; //죽음 파티클

    Color originalColor;

    float attackDistance = 0.5f;   //공격할 수 있는 거리

    float timeBetweenAttacks = 1;  //공격 사이의 타이밍
    float nextAttackTime;          //다음 공격 가능 시간

    float damage = 1;

    float myCollisionRadius;       //내 콜라이더 반지름
    float targetCollisionRadius;   //플레이어 콜라이더 반지름

    bool hasTarget;    //true : 플레이어 있음.

    public void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
    }

    protected override void Start()
    {
        base.Start();

        if (hasTarget)
        {
            currentState = State.Chasing;

            targetEntity.OnDeath += OnTargetDeath;
            StartCoroutine(UpdatePath());
        }
    }

    void Update()
    {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude; //목표 위치와 자신의 위치의 거리 제곱.

                if (sqrDstToTarget < Mathf.Pow(attackDistance + myCollisionRadius + targetCollisionRadius, 2))  //공격할 수 있는지에 대한 거리 검사
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    AudioManager.instance.PlaySound("Enemy Attack", transform.position);
                    StartCoroutine(Attack());
                }
            }
        }
    }

    /// <summary>
    /// 적 특징 세팅 함수
    /// </summary>
    /// <param name="moveSpeed">이동 속도</param>
    /// <param name="hitsToKillPlayer">플레이어 HP / hitsToKillPlayer</param>
    /// <param name="enemyHealth">적(자신) HP</param>
    /// <param name="skinColor">색상</param>
    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor)
    {
        pathfinder.speed = moveSpeed;

        if(hasTarget)
            damage = Mathf.Ceil(targetEntity.startHealth / hitsToKillPlayer);


        startHealth = enemyHealth;

        deathEffect.startColor = new Color(skinColor.r, skinColor.g, skinColor.b, 1);

        skinMaterial = GetComponent<Renderer>().material;
        skinMaterial.color = skinColor;
        originalColor = skinMaterial.color;
    }

    /// <summary>
    /// 피격 처리
    /// </summary>
    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        TakeHit(damage, hitPoint, hitDirection, false);
    }

    /// <summary>
    /// 치명타 여부를 포함한 피격 처리
    /// </summary>
    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection, bool isCriticalHit)
    {
        AudioManager.instance.PlaySound("Impact", transform.position);
        if (damage >= health && !dead)
        {
            if (OnDeathStatic != null)
                OnDeathStatic();

            AudioManager.instance.PlaySound("Enemy Death", transform.position);

            // 파티클 효과 생성
            GameObject deathEffectInstance = Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject;

            // 치명타면 파티클 3배로 증가
            if (isCriticalHit)
            {
                ParticleSystem deathParticle = deathEffectInstance.GetComponent<ParticleSystem>();
                if (deathParticle != null)
                {
                    var main = deathParticle.main;
                    main.maxParticles = main.maxParticles * 3;

                    var emission = deathParticle.emission;
                    emission.rateOverTime = emission.rateOverTime.constant * 3;

                    // Burst가 있다면 그것도 3배로
                    if (emission.burstCount > 0)
                    {
                        for (int i = 0; i < emission.burstCount; i++)
                        {
                            var burst = emission.GetBurst(i);
                            burst.count = burst.count.constant * 3;
                            emission.SetBurst(i, burst);
                        }
                    }
                }
            }

            Destroy(deathEffectInstance, deathEffect.startLifetime);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    /// <summary>
    /// 타겟 죽음
    /// </summary>
    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }


    /// <summary>
    /// 공격
    /// </summary>
    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPos = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPos = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;  //0~1  //애니메이션 거리

        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false; //데미지를 적용했는 상황인가..?

        while(percent <= 1)
        {
            if(percent >= 0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }

            percent += Time.deltaTime * attackSpeed;
            //interpolation(보간) : 알려진 지점의 위치를 참조하여, 지점의 사이 지점의 값(높이) 등을 추정해 구하는 과정입니다.
            //중간값 -> 아래위로써 이동을 한 다음에 다시 돌아가 가격을 가한다는 의미임.
            float interpolation = ( -Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPos, attackPos, interpolation);   //Lerp : 두 벡터 사이의 주어진 비율로 계산된 벡터를 반환.

            yield return null;
        }

        currentState = State.Chasing;
        skinMaterial.color = originalColor;
        pathfinder.enabled = true;
    }

    /// <summary>
    /// 이동 경로 업데이트
    /// </summary>
    IEnumerator UpdatePath()
    {
        float refreshRate = 0.2f;   //초당에 몇번씩?

        while(hasTarget)
        {
            if(currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPos = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistance /  2);
                if (!dead)
                    pathfinder.SetDestination(targetPos);
            }

            yield return new WaitForSeconds(refreshRate);
        }
    }
}