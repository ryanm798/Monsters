using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.AI;

public class MonsterConsumption : MonoBehaviour
{
    Rigidbody rb;
    MonsterMovement monsterMovement;
    
    [Header("Scaling")]
    public float scaleTime = 1.0f;
    float monsterScale;
    public float GetScale() { return monsterScale; }
    static float startScale = 3.0f;
    public static float GetStartScale() { return startScale; }
    float startScaleForAnimation;
    

    [Header("Player Transition")]
    public GameObject camPrefab;
    CinemachineVirtualCamera cam;
    CinemachineTransposer transposer;
    Vector3 baseCamOffset;
    Vector3 startCamOffset;
    Vector3 endCamOffset;
    public Transform camFollow;
    bool isPlayer;
    static MonsterConsumption playerMonster = null;
    public static bool IsPlayerMonster() { return playerMonster != null; }
    public Collider modelCollider;
    public GameObject physicalCollider;

    private void Start()
    {
        isPlayer = false;
        rb = GetComponent<Rigidbody>();
        monsterMovement = GetComponent<MonsterMovement>();
        monsterScale = startScale;
        physicalCollider.SetActive(false);
        ScaleUp(0f);
    }

    public void ProcessTrigger(GameObject other)
    {
        if (other.tag == "Person" || (other.tag == "Player" && !IsPlayerMonster()))
        {
            if (other.tag == "Player") // player consumed - this monster becomes player
            {
                isPlayer = true;
                playerMonster = this;
                modelCollider.gameObject.tag = "Player";
                gameObject.tag = "Player";
                gameObject.name = "Player";
                GameManager.Instance.player = gameObject;

                monsterMovement.UpdateForPlayer();

                cam = Instantiate(camPrefab).GetComponent<CinemachineVirtualCamera>();
                cam.Follow = camFollow;
                cam.LookAt = camFollow;
                transposer = cam.GetCinemachineComponent<CinemachineTransposer>();
                baseCamOffset = transposer.m_FollowOffset;
            }

            ScaleUp(1f);
            Destroy(other);
        }
        else if (other.tag == "Monster" || (other.tag == "Player" && IsPlayerMonster()))
        {
            float otherScale = other.GetComponent<MonsterConsumption>().monsterScale;
            if (monsterScale > otherScale)
            {
                ScaleUp(otherScale);
            }
            else
            {
                if (isPlayer)
                {
                    GameManager.Instance.Restart();
                }

                Destroy(gameObject);
            }
        }
        /* else if (other.tag == "Destructible" && Mathf.RoundToInt(monsterScale) >= NavMeshManager.Instance.destructiveMonsterSize)
        {
            other.GetComponent<Destructible>().Destruct();
        } */
    }

    private void ScaleUp(float scaleIncrease)
    {
        StopCoroutine("ScaleGradually");
        monsterScale += scaleIncrease;
        monsterMovement.UpdateAgentType(monsterScale);
        rb.mass = Mathf.Max(monsterScale - 1f, 0f) * 0.1f + 1f;
        startScaleForAnimation = transform.localScale.x;
        if (isPlayer)
        {
            //cam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = baseCamOffset + new Vector3(0f, monsterScale * 0.9f, monsterScale * -0.2f);
            ScoreSystem.Instance.score -= Mathf.RoundToInt(scaleIncrease) * 500;
            startCamOffset = transposer.m_FollowOffset;
            endCamOffset = baseCamOffset + new Vector3(0f, monsterScale * 0.9f, monsterScale * -0.4f);
        }
        StartCoroutine(ScaleGradually(scaleTime));
    }

    IEnumerator ScaleGradually(float maxTime)
    {
        float t = 0f;
        float scale;
        while (t < maxTime)
        {
            t += Time.deltaTime;
            scale = Mathf.SmoothStep(startScaleForAnimation, monsterScale, t / maxTime);
            transform.localScale = new Vector3(scale, scale, scale);
            if (isPlayer)
            {
                transposer.m_FollowOffset = Vector3.Slerp(startCamOffset, endCamOffset, t / maxTime);
            }

            yield return null;
        }
        transform.localScale = new Vector3(monsterScale, monsterScale, monsterScale);
        if (isPlayer)
        {
            transposer.m_FollowOffset = endCamOffset;
        }
    }

}
