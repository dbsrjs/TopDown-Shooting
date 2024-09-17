using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public LayerMask collisionMask; // 총알이 충돌할 수 있는 레이어 마스크
    public Color trailColor;

    float speed = 10;
    float damage = 1;

    float what_do_I_call_this_variable = 0.1f;  //적의 이동과 총알의 이동이 같은 프레임에서 일어나면 충돌이 안 되는 일을 보안 해주는 놈.

    private void Start()
    {
        Destroy(gameObject, 3f);

        // 총알이 생성된 위치에서 0.1f 반경 내에 충돌할 수 있는 물체가 있는지 확인
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);

        // 만약 생성 시점에 이미 충돌 물체와 겹쳐 있으면 OnHitObject 호출
        if (initialCollisions.Length > 0)   //총알이 생성 됐을 때 어떤 충돌체 오브젝트와 이미 겹친 상태일 때
            OnHitObject(initialCollisions[0], transform.position);

        GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColor);
    }

    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }
    void Update()
    {
        float moveDistance = speed * Time.deltaTime;       // 이동할 거리를 계산 (속도 * 프레임 시간)
        CheckCollisions(moveDistance);                     // 이동 경로에서 충돌 체크
        transform.Translate(Vector3.right * moveDistance); // 총알을 오른쪽으로 이동
    }

    /// <summary>
    /// 이동 거리 내에 충돌할 물체가 있는지 체크하는 함수
    /// </summary>
    void CheckCollisions(float moveDistance)
    {
        // 총알의 현재 위치에서 앞쪽으로 Raycast를 쏴서 충돌을 감지
        Ray ray = new Ray(transform.position, transform.right);
        RaycastHit hit;

        // Ray가 충돌체를 감지하면 OnHitObject 호출
        if (Physics.Raycast(ray, out hit, moveDistance + what_do_I_call_this_variable, collisionMask, QueryTriggerInteraction.Collide))
            OnHitObject(hit.collider, hit.point);
    }

    /// <summary>
    /// 총알이 충돌한 물체에 대한 처리
    /// </summary>
    void OnHitObject(Collider c, Vector3 hitPoint)
    {
        // 충돌한 물체가 IDamageable 인터페이스를 구현하고 있다면 데미지를 가함
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
            damageableObject.TakeHit(damage, hitPoint, transform.right);

        Destroy(gameObject);
    }
}