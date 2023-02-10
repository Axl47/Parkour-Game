using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillation : MonoBehaviour
{
    [SerializeField] private float maxAmount = 0f;
    private float amount;
    private float oldAmount;
    private Transform scale;
    private Vector3 oldScale;

    void Awake()
    {
        scale = GetComponent<Transform>();
        oldScale = scale.localScale;
    }

    void Update()
    {
        oldAmount = amount;
        amount = Mathf.PingPong(Time.time * 0.15f, maxAmount);
        scale.localScale = new Vector3(oldScale.x + amount, oldScale.y + amount, oldScale.z + amount);
    }
}
