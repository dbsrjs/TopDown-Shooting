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

    public static event Action OnDeathStatic;

    NavMeshAgent pathfinder;       //nav
    Transform target;              //Player
    LivingEntity targetEntity;

    Material skinMaterial;         //������ �� �� ���׸���
    public ParticleSystem deathEffect; //��� ��ƼŬ

    Color originalColor;

    float attackDistance = 0.5f;   //������ �� �ִ� �Ÿ�

    float timeBetweenAttacks = 1;  //���� ���̿� Ÿ�̸�
    float nextAttackTime;          //���� ���� ���� �ð�

    float damage = 1;

    float myCollisionRadius;       //�� �ݸ��� ������
    float targetCollisionRadius;   //�÷��̾� �ݸ��� ������

    bool hasTarget;    //true : �÷��̾ ����.

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
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude; //��ǥ ��ġ�� �ڽ��� ��ġ�� ���� ����.

                if (sqrDstToTarget < Mathf.Pow(attackDistance + myCollisionRadius + targetCollisionRadius, 2))  //������ �� �ִ����� ���� �Ÿ� ���
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    AudioManager.instance.PlaySound("Enemy Attack", transform.position);
                    StartCoroutine(Attack());
                }
            }
        }
    }

    /// <summary>
    /// �� Ư¡ ���� ����
    /// moveSpeed : �̵� �ӵ�
    /// hitsToKillPlayer : �÷��̾� HP / hitsToKillPlayer
    /// enemyHealth : ��(�ڽ�) HP
    /// skinColor : ����
    /// </summary>
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
    /// �ǰ� ����
    /// </summary>
    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        print("Hit");
        AudioManager.instance.PlaySound("Impact", transform.position);
        if (damage >= health && !dead)
        {
            if (OnDeathStatic != null)
                OnDeathStatic();

            AudioManager.instance.PlaySound("Enemy Death", transform.position);
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.startLifetime);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    /// <summary>
    /// Ÿ�� ���
    /// </summary>
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
                targetEntity.TakeDamage(damage);
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
        float refreshRate = 0.2f;   //���ʿ� �ѹ���?

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