using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Destructible : MonoBehaviour
{
    protected bool destroyed = false;
    public int points = 300;

    virtual protected void Start()
    {
        
    }

    virtual public void Destruct()
    {
        if (!destroyed)
        {
            destroyed = true;
            //ScoreSystem.Instance.score -= points;
        }
    }
}
