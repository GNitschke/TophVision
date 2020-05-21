using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickImpulse : MonoBehaviour
{
	public Material mat;
	private Camera main;
	private Transform camT;

    // Start is called before the first frame update
    void Start()
    {
        main = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        camT = main.GetComponent<Transform>();
    }

    float timer = 0f;

    void Update()
    {
    	if(Input.GetMouseButtonDown(0)) {
    		mat.SetFloat("_Offset", Time.time);
    		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    		Debug.Log(ray);
    		RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f)) {
            	mat.SetVector("_ImpulsePoint", hit.point);
            }
    		timer = 5f;
    	}
        if(timer > 0) {
        	mat.SetInt("_Switch", 1);
        	//mat.SetFloat("_Wavelength", 10 * (5f - timer));
        	timer -= Time.deltaTime;
        } else {
        	mat.SetInt("_Switch", 0);
        }
    }
}
