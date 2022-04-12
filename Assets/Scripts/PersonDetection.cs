using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonDetection : MonoBehaviour
{
    public PersonMovement personMovement;

    private void OnTriggerStay(Collider other)
    {
        personMovement.TriggerStay(other.gameObject.transform.parent.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        personMovement.TriggerExit(other.gameObject.transform.parent.gameObject);
    }
}
