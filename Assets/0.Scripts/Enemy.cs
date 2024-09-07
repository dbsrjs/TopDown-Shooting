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
        Idle,       //�ƹ��͵� �� �ϴ� ��
        Chasing,    //�÷��̾� �߰� ��
        Attacking   //�÷��̾� ���� ��    
    };
    State currentState; //���� ����

    NavMeshAgent pathfinder;       //nav
    Transform target;              //Player
    LivingEntity targetEntitiy;

    Material skinMaterial;         //������ �� �� ���׸���

    Color originalColor;

    float attackDistance = 1.5f;   //������ �� �ִ� �Ÿ�

    float timeBetweenAttacks = 1;  //���� ���̿� Ÿ�̸�
    float nextAttackTime;          //���� ���� ���� �ð�

    float damage = 1;

    float myCollisionRadius;       //�� �ݸ��� ������
    float targetCollisionRadius;   //�÷��̾� �ݸ��� ������

    bool hasTarget;    //true : �÷��̾ ����.

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
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude; //��ǥ ��ġ�� �ڽ��� ��ġ�� ���� ����.

                if (sqrDstToTarget < Mathf.Pow(attackDistance + myCollisionRadius + targetCollisionRadius, 2))  //������ �� �ִ����� ���� �Ÿ� ���
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
    /// ����
    /// </summary>
    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPos = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPos = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;  //0~1  //�ִϸ��̼� �Ÿ�

        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false; //������� �����ϴ� �����ΰ�..?

        while(percent <= 1)
        {
            if(percent >= 0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntitiy.TakeDamage(damage);
            }

            percent += Time.deltaTime * attackSpeed;
            //interpolation(����) : �˷��� ������ ��ġ�� �����Ͽ�, ������ ���� ������ ����(��)�� ���Ӱ� �׸��� ����� ���մϴ�.
            //������ -> ������������ �̵��� �� ������ ��Ī ��� ����� �������� �ǹ���.
            float interpolation = ( -Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPos, attackPos, interpolation);   //Lerp : �� ���� ���̿� ��� ������ ������ ������ ��ȯ.

            yield return null;
        }

        currentState = State.Chasing;
        skinMaterial.color = originalColor;
        pathfinder.enabled = true;
    }

    /// <summary>
    /// �̵� ��� ������Ʈ
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