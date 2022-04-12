using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDetection : MonoBehaviour
{
    public MonsterMovement monsterMovement;

    private void OnTriggerStay(Collider other)
    {
        monsterMovement.TriggerStay(other.gameObject.transform.parent.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        monsterMovement.TriggerExit(other.gameObject.transform.parent.gameObject);
    }
}
