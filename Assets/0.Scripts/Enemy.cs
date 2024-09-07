using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    NavMeshAgent pathfinder;
    Transform target;

    private void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        StartCoroutine(UpdatePath());
        
    }

    void Update()
    {
        
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.2f;

        while(target != null)
        {
            Vector3 targetPos = new Vector3(target.position.x, 0, target.position.z);
            pathfinder.SetDestination(targetPos);
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
