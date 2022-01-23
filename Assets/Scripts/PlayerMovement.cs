using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform orientation;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float crouchSpeed = 3f;
    [SerializeField] float airMultiplier = 0.4f;
    [SerializeField] float movementMultiplier = 10f;
    [SerializeField] float crouchMultiplier = 5f;

    [Header("Sprinting")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float sprintSpeed = 6f;
    [SerializeField] float acceleration = 10f;

    [Header("Jumping")]
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float jumpRate = 15f;

    [Header("Crouching")]
    [SerializeField] CapsuleCollider playerCollider;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode crouchKey = KeyCode.C;

    [Header("Drag")]
    [SerializeField] float groundDrag = 6f;
    [SerializeField] float airDrag = 2f;
    [SerializeField] float grapplingDrag = 0.001f;

    float horizontalMovement;
    float verticalMovement;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance = 0.2f;
    public bool isGrounded { get; private set; }

    [Header("HeadBob")]
    [SerializeField] Animator headBobAnim;

    [Header("Script References")]
    public GrapplingGun grapplingGun;
    public CalculateSpeed calculateSpeed;

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    Rigidbody rb;

    RaycastHit slopeHit;

    [Header("Debug Variables")]
    [SerializeField] public bool isCrouching;
    [SerializeField] public bool isMoving;

    float crouchYScale = 0.5f;
    float playerHeight = 2f;

    float nextTimeToJump = 0f;

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        MyInput();
        ControlDrag();
        ControlSpeed();
        CheckIfMoving();

        if (Input.GetKey(jumpKey) && isGrounded && Time.time >= nextTimeToJump)
        {
            nextTimeToJump = Time.time + 1f / jumpRate;
            Jump();
        }

        if (Input.GetKeyDown(crouchKey))
        {
            Crouch();
            isCrouching = true;
        }

        if (Input.GetKeyUp(crouchKey))
        {
            UnCrouch();
            isCrouching = false;
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

        AnimateWeapon();
    }

    void AnimateWeapon()
    {
        //if (isMoving)
        //{
        //    headBobAnim.enabled = true;
        //}
        //else
        //{
        //    headBobAnim.enabled = false;
        //}

        //if (!isGrounded)
        //{
        //    headBobAnim.enabled = false;
        //}

        //if (isMoving && isCrouching)
        //{
        //    headBobAnim.enabled = false;
        //}
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    void Crouch()
    {
        transform.localScale = new Vector3(1f, 0.5f, 1f);
        playerCollider.height = 1f;
    }

    void UnCrouch()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        playerCollider.height = 2f;
    }

    void ControlSpeed()
    {
        if (Input.GetKey(sprintKey) && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
    }

    void ControlDrag()
    {
        if (isGrounded)
        {
            if (grapplingGun.IsGrappling())
            {
                rb.drag = grapplingDrag;
            }
            else
            {
                rb.drag = groundDrag;
            }
        }
        else
        {
            
            if (grapplingGun.IsGrappling())
            {
                rb.drag = grapplingDrag;
            }
            else
            {
                rb.drag = airDrag;
            }
        }
    }

    void CheckIfMoving()
    {
        if (isGrounded)
        {
            if (calculateSpeed.speed > 3f)
            {
                isMoving = true;
            }
            else if (calculateSpeed.speed < 3f)
            {
                isMoving = false;
            }
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        if (isGrounded && !OnSlope() && !isCrouching)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }

        if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }

        if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }

        if(isGrounded && isCrouching)
        {
            rb.AddForce(moveDirection.normalized * crouchSpeed * crouchMultiplier, ForceMode.Acceleration);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "JumpBoost")
        {
            rb.AddForce(new Vector3(0f, 150f, 0f), ForceMode.Impulse);
        }
    }
}