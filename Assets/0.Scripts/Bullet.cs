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
    bool isCriticalHit = false; // 치명타 여부

    float what_do_I_call_this_variable = 0.1f;  //고속 이동 시 총알이 이동할 것으로 예상되는 대상을 넘어 충돌을 놓칠 수 있는 문제를 방지하는 값.

    private void Start()
    {
        Accuracy.Instance.IncrementShots();
        Destroy(gameObject, 3f);

        // 총알이 생성된 위치에서 0.1f 반경 안에 충돌할 수 있는 물체가 있는지 확인
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);

        // 만약 거기에 어떤 충돌체 오브젝트가 이미 닿은 상태라면 OnHitObject 호출
        if (initialCollisions.Length > 0)   //총알이 생성 직후 어떤 충돌체 오브젝트와 이미 닿은 상태라면
            OnHitObject(initialCollisions[0], transform.position);

        GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColor);
    }

    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }

    /// <summary>
    /// 치명타 설정
    /// </summary>
    public void SetCriticalHit(bool criticalHit)
    {
        isCriticalHit = criticalHit;
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;       // 이동할 거리를 계산 (속도 * 델타타임)
        CheckCollisions(moveDistance);                     // 이동 경로에서 충돌 체크
        transform.Translate(Vector3.right * moveDistance); // 총알을 오른쪽으로 이동
    }

    /// <summary>
    /// 이동 거리 내에 충돌하는 물체가 있는지 체크하는 함수
    /// </summary>
    void CheckCollisions(float moveDistance)
    {
        // 총알의 현재 위치에서 진행방향으로 Raycast를 사용해 충돌을 감지
        Ray ray = new Ray(transform.position, transform.right);
        RaycastHit hit;

        // Ray가 충돌체와 교차하면 OnHitObject 호출
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
        {
            float finalDamage = damage;

            // 치명타일 경우 데미지 2배
            if (isCriticalHit)
            {
                finalDamage *= 2f;
                Debug.Log("치명타! 데미지: " + finalDamage);
            }

            damageableObject.TakeHit(finalDamage, hitPoint, transform.right, isCriticalHit);
            Accuracy.Instance.IncrementHits();

            // 치명타였다면 치명타 명중도 증가
            if (isCriticalHit)
            {
                Accuracy.Instance.IncrementCriticalHits();
            }
        }

        Destroy(gameObject);
    }
}