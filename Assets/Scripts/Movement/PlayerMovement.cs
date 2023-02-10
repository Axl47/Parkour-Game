using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    private float playerHeight;

    [Header("References")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform player;
    [SerializeField] WallRun wallRun;
    [SerializeField] CapsuleCollider col;
    Rigidbody rb;
    RaycastHit slopeHit;

    [Header("Speeds")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] public float moveSpeed = 6f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float slideSpeed = 7f;
    [SerializeField] private float wallSpeed = 8f;
    [SerializeField] private float addedSpeed;
    [SerializeField] private float maxSpeed = 40f;
    [SerializeField] private float acceleration = 10f;

    [Header("Ground")]
    private float movementMultiplayer = 10f;
    private float horizontalMovement;
    private float verticalMovement;

    [Header("Jumping")]
    [SerializeField] public float jumpForce = 15f;
    [SerializeField] private float airMultiplier = 0.4f;

    [Header("Sliding")]
    private float slideMultiplier = 1.2f;
    private bool isSliding = false;
    private bool reduceSpeed = false;

    [Header("Drag")]
    float groundDrag = 6f;
    float slideDrag = 4f;
    float airDrag = 2f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode slideKey = KeyCode.LeftControl;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    bool isGrounded;
    float groundDistance = 0.4f;

    [Header("Directions")]
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 slideDirection;
    private Vector3 slopeMoveDirection;
    #endregion

    #region Main Functions

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponentInChildren<CapsuleCollider>();

        rb.freezeRotation = true;
    }

    private void Update()
    {
        moveDirection = Direction();

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        ControlDrag();

        ControlSpeed();

        if (Input.GetKeyDown(jumpKey) && isGrounded)
            Jump();

        if (Input.GetKeyDown(slideKey) && isGrounded)
            StartSlide();
        else if (Input.GetKeyUp(slideKey))
            StopSlide();

        if (isGrounded && OnSlope())
            slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    private void FixedUpdate()
    {
        if (isSliding)
            col.height = Mathf.Max(0.6f, col.height - Time.deltaTime * 10f);
        else
            col.height = Mathf.Min(1.8f, col.height + Time.deltaTime * 10f);

        MovePlayer();

        addedSpeed = rb.velocity.magnitude / 3f;
    }

    private Vector3 Direction()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");
        moveDirection = orientation.forward * vAxis + orientation.right * hAxis;
        return moveDirection;
    }

    void MovePlayer()
    {
        if (!isSliding)
        {
            if (isGrounded && !OnSlope())
                rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplayer, ForceMode.Acceleration);
            else if (isGrounded && OnSlope())
                rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplayer, ForceMode.Acceleration);
            else if (!isGrounded && !wallRun.isWallRunning)
                rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplayer * airMultiplier, ForceMode.Acceleration);
            else if (!isGrounded && wallRun.isWallRunning)
                rb.AddForce(moveDirection.normalized * wallSpeed * movementMultiplayer, ForceMode.Acceleration);
        }
        else if (isSliding)
            rb.AddForce(slideDirection.normalized * moveSpeed * movementMultiplayer * slideMultiplier, ForceMode.Acceleration);
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
            if (slopeHit.normal != Vector3.up)
                return true;
        return false;
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
    }

    void StartSlide()
    {
        //playerHeight = 0.5f;
        //player.transform.localScale = new Vector3(player.transform.localScale.x, playerHeight, player.transform.localScale.z);
        //this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.5f, this.transform.position.z);
        slideDirection = moveDirection;
        isSliding = true;
        Invoke(nameof(ReduceSlide), 5f);
    }

    void ReduceSlide()
    {
        reduceSpeed = true;
        Invoke(nameof(StopSlide), 5f);
    }

    void StopSlide()
    {
        //playerHeight = 1f;
        //player.transform.localScale = new Vector3(player.transform.localScale.x, playerHeight, player.transform.localScale.z);
        //this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z);
        isSliding = false;
    }

    void ControlSpeed()
    {
        if (wallRun.isWallRunning)
            moveSpeed = Mathf.Lerp(moveSpeed, wallSpeed + addedSpeed, acceleration * Time.deltaTime);
        else if (Input.GetKey(sprintKey) && isGrounded)
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed + addedSpeed, acceleration * Time.deltaTime);
        else if (isGrounded && !isSliding)
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed + addedSpeed, acceleration * Time.deltaTime);
        else if (isSliding && !reduceSpeed)
            moveSpeed = Mathf.Lerp(moveSpeed, slideSpeed + addedSpeed, acceleration * Time.deltaTime);
        else if (isSliding && reduceSpeed)
            moveSpeed = Mathf.Lerp(moveSpeed, 0, acceleration * Time.deltaTime);
        moveSpeed = Mathf.Clamp(moveSpeed, walkSpeed, maxSpeed);
    }

    void ControlDrag()
    {
        rb.drag = (isGrounded) ? groundDrag : airDrag;
        if (isSliding)
            rb.drag = slideDrag;
    }

    #endregion
}