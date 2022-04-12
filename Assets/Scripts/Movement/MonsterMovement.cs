using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement : RigidbodyMovement
{
    MonsterConsumption monsterConsumption;

    [Header("AI")]
    public float wanderDistance = 35f;
    bool agentSetup = false;
    NavMeshAgent agent;
    NavMeshQueryFilter filter;
    int maxQueryAttempts = 2;
    NavMeshHit navHit;
    GameObject chaseTarget = null;
    bool targetOutOfTrigger = false;
    bool targetIsPerson = true;
    MonsterConsumption targetConsumption;
    float timeIdle;
    float maxTimeIdle = 0.3f;


    override protected void Start()
    {
        base.Start();
        monsterConsumption = GetComponent<MonsterConsumption>();
        playerInputEnabled = false;
        if (!agentSetup)
            SetUpAgent();
        timeIdle = 0f;

        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.None;
    }

    public void UpdateForPlayer()
    {
        agent.enabled = false;
        playerInputEnabled = true;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        monsterConsumption.physicalCollider.SetActive(true);
    }

    private void SetUpAgent()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        filter = new NavMeshQueryFilter();
        filter.agentTypeID = agent.agentTypeID;
        filter.areaMask = agent.areaMask;
        agentSetup = true;
    }

    public void UpdateAgentType(float monsterScale)
    {
        if (!agentSetup) // this function may be called before start
            SetUpAgent();
        agent.agentTypeID = NavMeshManager.Instance.GetAgentId(monsterScale);
        filter.agentTypeID = agent.agentTypeID;
        agent.avoidancePriority = Mathf.Clamp(99 - Mathf.RoundToInt(monsterScale), 0, 98);
    }

    override protected void AIInput()
    {
        base.AIInput();

        if (agent.velocity.magnitude < 0.005f)
            timeIdle += Time.deltaTime;
        else
            timeIdle = 0f;

        if (chaseTarget != null && !targetIsPerson && monsterConsumption.GetScale() <= targetConsumption.GetScale())
            chaseTarget = null;
        
        if (chaseTarget != null && NavMesh.SamplePosition(chaseTarget.transform.position, out navHit, 5f, filter)) // chase person
        {
            agent.destination = navHit.position;
        }
        else if (agent.remainingDistance < 0.1f || timeIdle > maxTimeIdle) // wander
        {
            SetDestination(new Vector3(), wanderDistance, true);
        }
    }

    private float RandomizeAround(float mid)
    {
        return Random.Range(mid * 0.75f, mid * 1.25f);
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
            agent.destination = navHit.position;
            return true;
        }
        return false;
    }


    public void TriggerStay(GameObject other)
    {
        if (!playerInputEnabled && other != chaseTarget)
        {
            if (other.tag == "Person" || (other.tag == "Player" && !MonsterConsumption.IsPlayerMonster()))
            {
                if (chaseTarget == null || targetOutOfTrigger || !targetIsPerson || (transform.position - other.transform.position).magnitude < (transform.position - chaseTarget.transform.position).magnitude)
                {
                    chaseTarget = other;
                    targetOutOfTrigger = false;
                    targetIsPerson = true;
                }

                /* if (other.tag == "Player")
                {
                    if ((other.transform.position - transform.position).magnitude < ScoreSystem.Instance.grazeDistance)
                        ScoreSystem.Instance.Graze(gameObject);
                    else
                        ScoreSystem.Instance.EndGraze(gameObject);
                } */
            }
            else if ( (other.tag == "Monster" || (other.tag == "Player" && MonsterConsumption.IsPlayerMonster()))   &&   (monsterConsumption.GetScale() > other.GetComponent<MonsterConsumption>().GetScale()) )
            {
                if (chaseTarget == null || targetOutOfTrigger || (!targetIsPerson && (transform.position - other.transform.position).magnitude < (transform.position - chaseTarget.transform.position).magnitude))
                {
                    chaseTarget = other;
                    targetOutOfTrigger = false;
                    targetIsPerson = false;
                    targetConsumption = other.GetComponent<MonsterConsumption>();
                }
            }
        }
    }

    public void TriggerExit(GameObject other)
    {
        /* if (other.tag == "Player" && !MonsterConsumption.IsPlayerMonster())
        {
            ScoreSystem.Instance.EndGraze(gameObject);
        } */

        if (!playerInputEnabled)
        {
            if (other == chaseTarget)
            {
                targetOutOfTrigger = true;
            }
        }
    }
}
