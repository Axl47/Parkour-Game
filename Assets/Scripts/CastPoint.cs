using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPoint : MonoBehaviour
{
    [SerializeField] private Camera cam;

    void Update()
    {
        this.transform.forward = cam.transform.forward;
    }
}
