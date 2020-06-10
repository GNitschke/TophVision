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

	void Update()
	{
		if(Input.GetMouseButtonDown(0)) {
			mat.SetFloat("_Offset", Time.time);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 1000f)) {
				GlobalImpulse.impulsePoints[GlobalImpulse.impulsePointsIndex] = new Vector4(hit.point.x, hit.point.y, hit.point.z, 1f);
				GlobalImpulse.offsets[GlobalImpulse.impulsePointsIndex] = Time.time;
				GlobalImpulse.speeds[GlobalImpulse.impulsePointsIndex] = 5f;
				GlobalImpulse.switches[GlobalImpulse.impulsePointsIndex] = 1;
				StartCoroutine(DisableImpulse(GlobalImpulse.impulsePointsIndex, 20f));
				GlobalImpulse.impulsePointsIndex = (GlobalImpulse.impulsePointsIndex + 1) % GlobalImpulse.impulsePoints.Length;

				mat.SetVectorArray("_ImpulseArray", GlobalImpulse.impulsePoints);
				mat.SetFloatArray("_OffsetArray", GlobalImpulse.offsets);
				mat.SetFloatArray("_SpeedArray", GlobalImpulse.speeds);
				mat.SetFloatArray("_SwitchArray", GlobalImpulse.switches);
			}
		}
	}

	IEnumerator DisableImpulse(int index, float time=15f) {
		yield return new WaitForSeconds(time);
		GlobalImpulse.switches[index] = 0f;
		mat.SetFloatArray("_SwitchArray", GlobalImpulse.switches);
	}
}
