using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    Rigidbody rb;
    public float moveSpeed = 9f;
    public float turnSpeed = 115f;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 playerPos = new Vector3(GameManager.Instance.player.transform.position.x, transform.position.y, GameManager.Instance.player.transform.position.z);
        Vector3 offset = playerPos - transform.position;

        transform.rotation = Quaternion.LookRotation(offset, Vector3.up);
        //rb.MovePosition(transform.position + transform.forward * Time.fixedDeltaTime * moveSpeed);
        rb.AddForce(transform.forward * moveSpeed, ForceMode.Acceleration);
    }
}
