using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camSwitch : MonoBehaviour
{
    public GameObject Cam1;
    public GameObject Cam2;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (Cam1.activeSelf)
            {
                Cam1.SetActive(false);
                Cam2.SetActive(true);
            }
            else
            {
                Cam2.SetActive(false);
                Cam1.SetActive(true);
            }
        }
    }
}
