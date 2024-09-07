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

    float what_do_I_call_this_variable = 0.1f;  //적의 이동과 총알의 이동이 같은 프레임에서 일어나면 충돌이 안 되는 일을 보안 해주는 놈.

    private void Start()
    {
        Destroy(gameObject, 3f);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);

        if (initialCollisions.Length > 0)   //총알이 생성 됐을 때 어떤 충돌체 오브젝트와 이미 겹친 상태일 때
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