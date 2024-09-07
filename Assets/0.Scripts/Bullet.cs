using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public LayerMask collisionMask;

    float time = 0;

    float speed = 10;
    float damage = 1;

    float what_do_I_call_this_variable = 0.1f;  //���� �̵��� �Ѿ��� �̵��� ���� �����ӿ��� �Ͼ�� �浹�� �� �Ǵ� ���� ���� ���ִ� ��.

    private void Start()
    {
        Destroy(gameObject, 3f);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);

        if (initialCollisions.Length > 0)   //�Ѿ��� ���� ���� �� � �浹ü ������Ʈ�� �̹� ��ģ ������ ��
        {
            OnHitObject(initialCollisions[0]);
        }
    }

    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }
    void Update()
    {
        float moveDistance = speed * Time.deltaTime; 
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.right * moveDistance);
    }

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, moveDistance + what_do_I_call_this_variable, collisionMask, QueryTriggerInteraction.Collide))
            OnHitObject(hit);
    }

    void OnHitObject(RaycastHit hit)
    {
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hit);
        }
        Destroy(gameObject);
    }

    void OnHitObject(Collider c)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}