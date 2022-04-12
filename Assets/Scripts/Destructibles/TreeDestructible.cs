using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TreeDestructible : Destructible
{
    NavMeshAgent agent;
    public float fallTime = 3f;
    Vector3 collisionNorm;
    public AnimationCurve fallCurve;

    override protected void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
    }

    override public void Destruct()
    {
        if (!destroyed)
        {
            base.Destruct();
            //Destroy(gameObject);
        
            agent.enabled = false;
            foreach (Collider collider in GetComponentsInChildren<Collider>())
                collider.enabled = false;

            StartCoroutine(Fall());
        }
    }

    IEnumerator Fall()
    {
        float t = 0f;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.FromToRotation(transform.up, collisionNorm) * transform.rotation;
        while (t < fallTime)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRot, endRot, fallCurve.Evaluate(t / fallTime));
            yield return null;
        }
        transform.rotation = endRot;
    }

    private void OnTriggerStay(Collider other)
    {
        MonsterConsumption monster = other.gameObject.GetComponentInParent<MonsterConsumption>();
        if (monster != null)
        {
            if (Mathf.RoundToInt(monster.GetScale()) >= NavMeshManager.Instance.destructiveMonsterSize)
            {
                collisionNorm = transform.position - other.transform.position;
                collisionNorm.y = 0f;
                collisionNorm.Normalize();
                Destruct();
                if (other.gameObject.tag == "Player" || (other.transform.parent != null && other.transform.parent.tag == "Player"))
                    ScoreSystem.Instance.score -= points;
            }
        }
    }
}
