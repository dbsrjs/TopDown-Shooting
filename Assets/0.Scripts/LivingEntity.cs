using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startHealth;
    protected float health;
    protected bool dead;

    public event System.Action OnDeath; //OnDeath�� OnEnemyDeath�� ȣ���ؿ�.
    //System.Action : ��������Ʈ �޼���
    //��������Ʈ �޼��� : �ٸ� �޼����� ��ġ�� ����Ű�� �ҷ��� �� �ִ� Ÿ��. (C++������ �Լ� �������� ���Ұ� ����)
    //event�� ��������Ʈ�� ���� �ڼ��� ���� : https://kimyir.tistory.com/14

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
