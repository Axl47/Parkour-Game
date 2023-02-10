using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCast : MonoBehaviour
{
    [SerializeField] Transform castPos;
    public Camera cam;
    public GameObject muzzle;
    public GameObject spell;
    public float spellSpeed = 30f;
    public float fireRate = 4f;
    private float timeToFire = 0f;
    private float arcRange = 1f; 

    private Vector3 destination;

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1f / fireRate;
            CastSpell();
        }
    }

    void CastSpell()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) destination = hit.point;
        else destination = ray.GetPoint(1000f);

        InstantiateSpell();
    }
     
    void InstantiateSpell()
    {
        var spellObj = Instantiate(spell, castPos.position, Quaternion.identity) as GameObject;
        spellObj.GetComponent<Rigidbody>().velocity = (destination - castPos.position).normalized * spellSpeed;

        iTween.PunchPosition(spellObj, new Vector3(Random.Range(-arcRange, arcRange), Random.Range(-arcRange, arcRange), 0f), Random.Range(0.5f, 2f));

        var muzzleObj = Instantiate(muzzle, castPos.position, Quaternion.identity) as GameObject;
        Destroy(muzzleObj, 2f);
    }
}
