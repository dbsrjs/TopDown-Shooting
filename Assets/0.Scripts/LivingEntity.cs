using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startHealth;
    protected float health;
    protected bool dead;

    public event System.Action OnDeath; //OnDeath는 OnEnemyDeath를 호출해옴.
    //System.Action : 델리게이트 메서드
    //델리게이트 메서드 : 다른 메서드의 위치를 가르키고 불러올 수 있는 타입. (C++에서의 함수 포인터의 역할과 유사)
    //event와 델리케이트에 대한 자세한 설명 : https://kimyir.tistory.com/14

    protected virtual void Start()
    {
        health = startHealth;
    }

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
            Die();
    }

    [ContextMenu("Self Destruct")]
    public virtual void Die()
    {
        dead = true;
        if(OnDeath != null)
            OnDeath();

        Destroy(gameObject);
    }
}
