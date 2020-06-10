using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launch : MonoBehaviour
{
    public GameObject projectile;
    public float strength;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            GameObject p = Instantiate(projectile, transform.position, projectile.transform.rotation);
            p.GetComponent<Rigidbody>().AddForce(transform.forward * strength);
        }
    }
}
