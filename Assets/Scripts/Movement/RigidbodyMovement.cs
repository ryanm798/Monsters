using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyMovement : MonoBehaviour
{
    protected Rigidbody rb;

    [Header("Controls")]
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";

    [Header("Movement")]
    public bool playerInputEnabled = true;
    public float moveSpeed = 15f;
    public float turnSpeed = 120f;
    float airSlowMultiplier = 0.4f;
    protected float hMov;
    protected float vMov;
    public bool autoForward = false;
    public float turnSlow = 0f;
    public bool backstepEnabled = true;
    public bool backstepSlow = true;
    bool backstepping;
    Vector3 moveDirection;

    [Header("Jumping")]
    public bool jumpEnabled = true;
    public float jumpForce = 15f;
    float jumpCooldown = 0.1f;
    float lastJumpTime;

    [Header("Drag")]
    public float groundDrag = 20f;
    public float airDrag = 0.5f;
    public float angularDrag = 23.0f;

    [Header("Ground Detection")]
    public LayerMask groundMask;
    bool isGrounded;
    public float groundDistance = 0.1f;
    RaycastHit slopeHit;
    Vector3 slopeMoveDirection;
    

    virtual protected void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.angularDrag = angularDrag;

        if (groundMask.value == 0)
            groundMask |= (1 << LayerMask.NameToLayer("Walkable"));
        
        lastJumpTime = 0f - jumpCooldown;
        
        hMov = 0f;
        vMov = 0f;

        if (autoForward)
            backstepEnabled = false;
        turnSlow = Mathf.Clamp(turnSlow, 0f, 1f);
    }

    virtual protected void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundDistance, groundMask);
        ControlDrag();

        if (playerInputEnabled)
        {
            ProcessInputs();

            if (!backstepEnabled)
                vMov = Mathf.Max(0f, vMov);
            if (vMov < 0)
                backstepping = true;
            else
                backstepping = false;
            
            moveDirection = transform.forward * vMov;
            slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
        }
        else
        {
            AIInput();
        }
    }

    void ProcessInputs()
    {
        hMov = Input.GetAxis(horizontalAxis);
        if (autoForward)
        {
            vMov = 1f - turnSlow * Mathf.Pow(Mathf.Abs(hMov), 2f);
        }
        else
            vMov = Input.GetAxis(verticalAxis);
        
        if (Input.GetKey(KeyCode.Space))
            Jump();
    }

    virtual protected void AIInput()
    {

    }

    void ControlDrag()
    {
        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = airDrag;
    }

    virtual protected void FixedUpdate()
    {
        if (playerInputEnabled)
        {
            Turn();
            Move();
        }
    }

    virtual protected void Turn()
    {
        rb.AddTorque(0f, hMov * turnSpeed * Mathf.Deg2Rad * angularDrag * 1.7f, 0f, ForceMode.Acceleration);
    }

    virtual protected void Move()
    {
        float speed = moveSpeed;
        if (backstepSlow && backstepping)
            speed *= 0.3f;
        if (!isGrounded)
            speed *= airSlowMultiplier * airDrag / groundDrag;
        Vector3 dir = moveDirection;
        if (isGrounded && OnSlope())
            dir = slopeMoveDirection;
        rb.AddForce(dir * speed * groundDrag * 1.7f, ForceMode.Acceleration);
    }

    bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 0.5f))
        {
            return slopeHit.normal != Vector3.up;
        }
        return false;
    }

    virtual protected void Jump()
    {
        if (jumpEnabled && isGrounded && (Time.time - lastJumpTime > jumpCooldown))
        {
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Max(0f, rb.velocity.y), rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            lastJumpTime = Time.time;
        }
    }

}
