using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    NavMeshAgent pathfinder;
    Transform target;

    public void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(UpdatePath());
        
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.2f;

        while(target != null)
        {
            Vector3 targetPos = new Vector3(target.position.x, 0, target.position.z);
            if(!dead)
            pathfinder.SetDestination(targetPos);

            yield return new WaitForSeconds(refreshRate);
        }
    }
}
