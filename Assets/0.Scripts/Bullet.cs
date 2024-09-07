using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public LayerMask collisionMask;

    float speed = 10;
    float time = 0;

    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }
    void Update()
    {
        time += Time.deltaTime;
        if(time >= 5f)
        {
            Destroy(gameObject);
        }

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
        print(hit.collider.gameObject.name);
        Destroy(gameObject);
    }
}