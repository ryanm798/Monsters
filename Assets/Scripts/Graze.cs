using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Graze : MonoBehaviour
{
    public TextMeshProUGUI timer;
    bool grazing = false;
    float grazeSeconds = 0f;
    float lastTriggerTime;
    public float maxWait = 0.2f;
    public int pointsPerSec = 2000;

    void Start()
    {
        lastTriggerTime = Time.fixedTime - maxWait - 1f;
        grazing = false;
        timer.enabled = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Monster")
        {
            if (!grazing)
            {
                grazing = true;
                grazeSeconds = 0f;
                timer.enabled = true;
            }
            
            lastTriggerTime = Time.fixedTime;
            grazeSeconds += Time.fixedDeltaTime;
        }
    }

    void FixedUpdate()
    {
        if (grazing && Time.fixedTime - lastTriggerTime > maxWait)
        {
            grazing = false;
            timer.enabled = false;
            ScoreSystem.Instance.score += Mathf.CeilToInt(grazeSeconds * pointsPerSec);
        }
    }

    void Update()
    {
        if (grazing)
        {
            UpdateTimer(grazeSeconds);
        }
    }

    void UpdateTimer(float seconds)
    {
        timer.text = seconds.ToString("n2");
    }

    void OnDestroy()
    {
        timer.enabled = false;
    }
}
