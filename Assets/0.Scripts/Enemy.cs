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
        Idle,       //아무것도 안 하는 중
        Chasing,    //플레이어 추격 중
        Attacking   //플레이어 공격 중    
    };
    State currentState; //현재 상태

    NavMeshAgent pathfinder;       //nav
    Transform target;              //Player
    LivingEntity targetEntitiy;

    Material skinMaterial;         //공격할 때 쓸 메테리얼

    Color originalColor;

    float attackDistance = 1.5f;   //공격할 수 있는 거리

    float timeBetweenAttacks = 1;  //공격 사이에 타이머
    float nextAttackTime;          //다음 공격 가능 시간

    float damage = 1;

    float myCollisionRadius;       //적 콜리션 반지름
    float targetCollisionRadius;   //플레이어 콜리션 반지름

    bool hasTarget;    //true : 플레이어가 없음.

    public void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;

        if (GameObject.FindGameObjectsWithTag("Player") != null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntitiy = target.GetComponent<LivingEntity>();
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
    }

    protected override void Start()
    {
        base.Start();
        originalColor = skinMaterial.color;

        if (GameObject.FindGameObjectsWithTag("Player") != null)
        {
            currentState = State.Chasing;
            hasTarget = true;

            targetEntitiy.OnDeath += OnTargetDeath;
            StartCoroutine(UpdatePath());
        }

    }
    
    void Update()
    {
        if (hasTarget) 
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude; //목표 위치와 자신의 위치의 차에 제곱.

                if (sqrDstToTarget < Mathf.Pow(attackDistance + myCollisionRadius + targetCollisionRadius, 2))  //공격할 수 있는지에 대한 거리 계산
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
    }

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
        bool hasAppliedDamage = false; //대미지를 적용하는 도중인가..?

        while(percent <= 1)
        {
            if(percent >= 0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntitiy.TakeDamage(damage);
            }

            percent += Time.deltaTime * attackSpeed;
            //interpolation(보간) : 알려진 점들의 위치를 참조하여, 집합의 일정 범위의 점들(선)을 새롭게 그리는 방법을 말합니다.
            //원지점 -> 공격지점으로 이동할 때 참조할 대칭 곡선을 만드는 참조점을 의미함.
            float interpolation = ( -Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPos, attackPos, interpolation);   //Lerp : 두 벡터 사이에 비례 값으로 내분점 지점을 반환.

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
        float refreshRate = 0.2f;

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