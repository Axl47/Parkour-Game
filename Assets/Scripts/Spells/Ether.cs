using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ether : MonoBehaviour
{
    public GameObject EtherImpact;
    private bool collided = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "Player" && !collided)
        {
            Destroy(gameObject);

            var impact = Instantiate(EtherImpact, collision.contacts[0].point, Quaternion.identity) as GameObject;

            Destroy(impact, 2f);

            collided = true;
        }
    }
}
