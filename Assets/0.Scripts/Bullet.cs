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

    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }
    void Update()
    {
        time += Time.deltaTime;
        if(time >= 5f)  //총알이 생성된지 5초 이상이 되면 삭제.
            Destroy(gameObject);

        float moveDistance = speed * Time.deltaTime; 
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.right * moveDistance);
    }

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
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
}