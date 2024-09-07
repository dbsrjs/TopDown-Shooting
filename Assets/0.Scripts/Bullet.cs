using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
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

        transform.Translate(Vector3.right * Time.deltaTime * speed);
    }
}