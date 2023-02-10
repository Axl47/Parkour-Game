using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] WallRun wallRun;
    [SerializeField] Transform cam = null;
    [SerializeField] Transform orientation = null;

    [Header("Sensitivity")]
    [SerializeField] private float sensX = 80f;
    [SerializeField] private float sensY = 80f;
    private float mouseX;
    private float mouseY;
    private float rotationX;
    private float rotationY;
    private float multiplier = 0.05f;

    public Quaternion rot { get; private set; }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        MyInput();
        rot = Quaternion.Euler(rotationX, rotationY, wallRun.tilt);
        cam.transform.localRotation = rot;
        orientation.transform.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    void MyInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        rotationY += mouseX * sensX * multiplier;
        rotationX -= mouseY * sensY * multiplier;

        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
    }
}