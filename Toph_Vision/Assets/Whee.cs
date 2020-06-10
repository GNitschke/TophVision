using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whee : MonoBehaviour
{
    public Material[] materials;

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnCollisionEnter(Collision other)
    {
        print("Points colliding: " + other.contacts.Length);
        print("First normal of the point that collide: " + other.contacts[0].normal);

        Vector4 location = transform.position;

        Debug.Log(other.relativeVelocity.magnitude);

        GlobalImpulse.impulsePoints[GlobalImpulse.impulsePointsIndex] = location;
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

    private void Update()
    {

    }

    IEnumerator DisableImpulse(int index, float time=15f) {
		yield return new WaitForSeconds(time);
		GlobalImpulse.switches[index] = 0f;
		foreach(Material mat in materials) {
			mat.SetFloatArray("_SwitchArray", GlobalImpulse.switches);
		}
	}
}
