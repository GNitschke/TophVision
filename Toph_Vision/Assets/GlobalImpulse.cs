using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalImpulse : MonoBehaviour
{

	public static Vector4[] impulsePoints;
	public static float[] offsets;
	public static float[] switches;
	public static int impulsePointsIndex = 0;

	static GlobalImpulse() {
		impulsePoints = new Vector4[40];
		offsets = new float[40];
		switches = new float[40];
		for(int i = 0; i < impulsePoints.Length; i++) {
			impulsePoints[i] = new Vector4(0f, 0f, 0f, 0f);
			offsets[i] = 0f;
			switches[i] = 0;
		}
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
