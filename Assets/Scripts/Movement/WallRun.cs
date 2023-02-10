using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    #region Vars
    [Header("Movement")]
    [SerializeField] private Transform orientation;
    private Rigidbody rb;

    [Header("Detection")]
    [SerializeField] private float wallDistance = 0.6f;
    [SerializeField] private float minimumJumpHeight = 1.5f;

    [Header("Wall Running")]
    [SerializeField] private float wallRunGravity;
    [SerializeField] private float wallRunJumpForce;

    [Header("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private float camTilt;
    [SerializeField] private float camTiltTime;

    [Header("Field of Vision")]
    [SerializeField] private float fov = 0f;
    [SerializeField] private float basefov = 90f;
    [SerializeField] private float maxfov = 140f;
    [SerializeField] private float addedfov = 0f;
    [SerializeField] private float wallRunfovTime = 10f;


    public float tilt { get; private set; }
    public bool isWallRunning { get; private set; }


    private bool wallLeft = false;
    private bool wallRight = false;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }

    void CheckWall()
    {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance);
    }

    private void FixedUpdate()
    {
        addedfov = rb.velocity.magnitude - 3.44f;
        fov = Mathf.Lerp(fov, basefov + addedfov, wallRunfovTime * Time.deltaTime);
        fov = Mathf.Clamp(fov, basefov, maxfov);
        cam.fieldOfView = fov;
    }

    private void Update()
    {
        CheckWall();

        if (CanWallRun())
        {
            if (wallLeft || wallRight)
                StartWallRun();
            else
                StopWallRun();
        }
        else
            StopWallRun();
    }

    void StartWallRun()
    {
        isWallRunning = true;
        rb.useGravity = false;

        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

        if (wallLeft)
            tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime);
        else if (wallRight)
            tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 wallJumpDirection = transform.up;
            wallJumpDirection += (wallLeft) ? leftWallHit.normal : rightWallHit.normal;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(wallJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
        }
        else
        {
            if (wallLeft)
            {
                Vector3 wallDirection = -orientation.right + leftWallHit.normal;
                rb.AddForce(wallDirection * 2f, ForceMode.Force);
            }
            else if (wallRight)
            {
                Vector3 wallDirection = orientation.right + rightWallHit.normal;
                rb.AddForce(wallDirection * 2f, ForceMode.Force);
            }
        }
    }

    void StopWallRun()
    {
        isWallRunning = false;
        rb.useGravity = true;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, wallRunfovTime * Time.deltaTime);

        tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);
    }
}
