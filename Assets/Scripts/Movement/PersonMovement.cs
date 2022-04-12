using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PersonMovement : RigidbodyMovement
{
    [Header("AI")]
    public float idleTime = 2f;
    float timeIdle;
    float maxTimeIdle;
    public float wanderDistance = 35f;
    public float fleeDistance = 10f;
    NavMeshAgent agent;
    NavMeshQueryFilter filter;
    int maxQueryAttempts = 2;
    NavMeshHit navHit;
    GameObject fleeTarget = null;
    int dirMult;


    override protected void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        filter = new NavMeshQueryFilter();
        filter.agentTypeID = agent.agentTypeID;
        filter.areaMask = agent.areaMask;
        maxTimeIdle = 0f;
        timeIdle = 0f;
        RandomizeDirChoice();

        if (playerInputEnabled)
        {
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
        else
        {
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.None;
        }
    }

    override protected void AIInput() // being called from Update
    {
        base.AIInput();

        if (agent.velocity.magnitude < 0.01f)
            timeIdle += Time.deltaTime;
        else
            timeIdle = 0f;

        if (fleeTarget != null) // flee monster
        {
            Vector3 dir = transform.position - fleeTarget.transform.position;
            if (!SetDestination(dir, fleeDistance)) // try directly away from monster
            {
                // try approx 90 degrees right or left
                if (!SetDestination(Quaternion.Euler(0f, RandomizeAround(90f) * dirMult, 0f) * dir, fleeDistance))
                {
                    dirMult *= -1;
                    SetDestination(Quaternion.Euler(0f, RandomizeAround(90f) * dirMult, 0f) * dir, fleeDistance);
                }
            }
        }
        else if (timeIdle > maxTimeIdle) // wander
        {
            SetDestination(new Vector3(), wanderDistance, true);
        }
    }

    private float RandomizeAround(float mid)
    {
        return Random.Range(mid * 0.75f, mid * 1.25f);
    }

    public void RandomizeDirChoice()
    {
        if (Random.value < 0.5f)
            dirMult = -1;
        else
            dirMult = 1;
    }

    private bool SetDestination(Vector3 dir, float distance, bool randomDir = false)
    {
        int attempts = -1;
        float initDistance = distance;
        do
        {
            attempts++;
            if (randomDir)
            {
                dir = Random.insideUnitSphere;
                //dir.y = 0f;
                distance = RandomizeAround(initDistance);
            }
        }
        while (attempts < maxQueryAttempts && !NavMesh.SamplePosition(transform.position + dir.normalized * distance, out navHit, distance / 2f, filter));

        if (attempts < maxQueryAttempts)
        {
            timeIdle = 0f;
            maxTimeIdle = RandomizeAround(idleTime);
            agent.destination = navHit.position;
            return true;
        }
        return false;
    }

    public void TriggerStay(GameObject other)
    {
        if (!playerInputEnabled)
        {
            if (other != fleeTarget && (other.tag == "Monster" || (other.tag == "Player" && MonsterConsumption.IsPlayerMonster())))
            {
                if (fleeTarget == null || (transform.position - other.transform.position).magnitude < (transform.position - fleeTarget.transform.position).magnitude)
                    fleeTarget = other;
            }
        }
    }

    public void TriggerExit(GameObject other)
    {
        if (!playerInputEnabled)
        {
            if (other == fleeTarget)
            {
                fleeTarget = null;
                RandomizeDirChoice();
            }
        }
    }
}
