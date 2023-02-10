using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    private Collider col;
    public GameObject fracture;

    // Start is called before the first frame update
    void Awake()
    {
        col = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Instantiate(fracture, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
