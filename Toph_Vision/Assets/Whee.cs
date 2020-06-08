using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whee : MonoBehaviour
{
    public Material[] materials;

    /*private Vector4[] impulsePoints;
	private float[] offsets;
	private float[] switches;
	private int impulsePointsIndex = 0;*/

    // Start is called before the first frame update
    void Start()
    {
        /*impulsePoints = new Vector4[40];
		offsets = new float[40];
		switches = new float[40];
		for(int i = 0; i < impulsePoints.Length; i++) {
			impulsePoints[i] = new Vector4(0f, 0f, 0f, 0f);
			offsets[i] = 0f;
			switches[i] = 0;
		}*/
    }

    void OnCollisionEnter(Collision other)
    {
        print("Points colliding: " + other.contacts.Length);
        print("First normal of the point that collide: " + other.contacts[0].normal);

        Debug.Log(other.relativeVelocity.magnitude);

        GlobalImpulse.impulsePoints[GlobalImpulse.impulsePointsIndex] = new Vector4(other.contacts[0].point.x, other.contacts[0].point.y, other.contacts[0].point.z, 1f);
		GlobalImpulse.offsets[GlobalImpulse.impulsePointsIndex] = Time.time;
		GlobalImpulse.speeds[GlobalImpulse.impulsePointsIndex] = 4f + (other.relativeVelocity.magnitude / 10f);
		GlobalImpulse.switches[GlobalImpulse.impulsePointsIndex] = 1;

		StartCoroutine(DisableImpulse(GlobalImpulse.impulsePointsIndex));
		GlobalImpulse.impulsePointsIndex = (GlobalImpulse.impulsePointsIndex + 1) % GlobalImpulse.impulsePoints.Length;

		foreach(Material mat in materials) {
			mat.SetVectorArray("_ImpulseArray", GlobalImpulse.impulsePoints);
			mat.SetFloatArray("_OffsetArray", GlobalImpulse.offsets);
			mat.SetFloatArray("_SpeedArray", GlobalImpulse.speeds);
			mat.SetFloatArray("_SwitchArray", GlobalImpulse.switches);
		}
    }

    //float timer = 0f;
    private void Update()
    {

        /* if (timer > 0)
        {
            mat.SetInt("_Switch", 1);
            //mat.SetFloat("_Wavelength", 10 * (5f - timer));
            timer -= Time.deltaTime;
        }
        else
        {
            mat.SetInt("_Switch", 0);
        }*/
    }

    IEnumerator DisableImpulse(int index, float time=10f) {
		yield return new WaitForSeconds(time);
		GlobalImpulse.switches[index] = 0f;
		foreach(Material mat in materials) {
			mat.SetFloatArray("_SwitchArray", GlobalImpulse.switches);
		}
	}
}
