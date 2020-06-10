using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalImpulse : MonoBehaviour
{

	public static Vector4[] impulsePoints;
	public static float[] offsets;
	public static float[] speeds;
	public static float[] switches;
	public static int impulsePointsIndex = 0;

	public static Camera currentCam;

	static GlobalImpulse() {
		impulsePoints = new Vector4[1000];
		offsets = new float[1000];
		speeds = new float[1000];
		switches = new float[1000];
		for(int i = 0; i < impulsePoints.Length; i++) {
			impulsePoints[i] = new Vector4(0f, 0f, 0f, 0f);
			offsets[i] = 0f;
			speeds[i] = 0f;
			switches[i] = 0f;
		}

		currentCam = Camera.main;
	}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
