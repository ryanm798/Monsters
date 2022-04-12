using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterConsumptionTrigger : MonoBehaviour
{
    public MonsterConsumption monsterConsumption;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Model")
            monsterConsumption.ProcessTrigger(other.gameObject.transform.parent.gameObject);
    }
}
