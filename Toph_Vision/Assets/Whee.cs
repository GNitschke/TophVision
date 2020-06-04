using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whee : MonoBehaviour
{
    public Material mat;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        mat.SetFloat("_Offset", Time.time);
        print("Points colliding: " + other.contacts.Length);
        print("First normal of the point that collide: " + other.contacts[0].normal);
        mat.SetVector("_ImpulsePoint", other.contacts[0].point);
        timer = 5f;
    }

    float timer = 0f;
    private void Update()
    {

        if (timer > 0)
        {
            mat.SetInt("_Switch", 1);
            //mat.SetFloat("_Wavelength", 10 * (5f - timer));
            timer -= Time.deltaTime;
        }
        else
        {
            mat.SetInt("_Switch", 0);
        }
    }
}
