using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    /***** SINGLETON SETUP *****/
    private static ScoreSystem _instance;
    public static ScoreSystem Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    private void OnDestroy()
    {
        if (this == _instance)
        {
            _instance = null;
        }
    }
    /******************************/

    [HideInInspector] public int score;
    public ScoreUI scoreUI;

    [Header("Score Over Time")]
    public float timeInterval = 1.0f;
    public int timedPoints = 50;
    bool waiting;

    /* [Header("Grazing")]
    public float grazeDistance = 1.5f;
    public int grazePointsPerSec = 2000;
    Dictionary<GameObject, float> grazing = new Dictionary<GameObject, float>(); */


    private void Start()
    {
        score = 0;
        waiting = false;
    }

    private void Update()
    {
        if (!waiting)
        {
            waiting = true;
            StartCoroutine("ConstScoreUp");
        }
    }
    IEnumerator ConstScoreUp()
    {
        yield return new WaitForSeconds(timeInterval);
        if (!MonsterConsumption.IsPlayerMonster())
        {
            score += timedPoints;
            scoreUI.TimedPoints(timedPoints);
        }
        else
        {
            score -= timedPoints;
            scoreUI.TimedPoints(-timedPoints);
        }
        waiting = false;
    }

    /* public void Graze(GameObject monster)
    {
        if (!grazing.ContainsKey(monster))
        {
            grazing[monster] = Time.time;
        }
    }

    public void EndGraze(GameObject monster)
    {
        if (grazing.ContainsKey(monster))
        {
            score += Mathf.RoundToInt(Time.time - grazing[monster]) * grazePointsPerSec;
            grazing.Remove(monster);
        }
    } */
}
