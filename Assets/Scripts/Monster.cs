using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Monster : MonoBehaviour
{
    public Transform target;

    NavMeshAgent nmAgent;
    // Start is called before the first frame update
    void Start()
    {
        nmAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        nmAgent.SetDestination(target.position);
    }
}
