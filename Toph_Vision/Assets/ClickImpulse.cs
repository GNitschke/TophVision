using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickImpulse : MonoBehaviour
{
	public Material mat;
	private Camera main;
	private Transform camT;

	private Vector4[] impulsePoints;
	private float[] offsets;
	private float[] switches;
	private int impulsePointsIndex = 0;

	// Start is called before the first frame update
	void Start()
	{
		main = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		camT = main.GetComponent<Transform>();

		impulsePoints = new Vector4[40];
		offsets = new float[40];
		switches = new float[40];
		for(int i = 0; i < impulsePoints.Length; i++) {
			impulsePoints[i] = new Vector4(0f, 0f, 0f, 0f);
			offsets[i] = 0f;
			switches[i] = 0;
		}
	}

	void Update()
	{
		if(Input.GetMouseButtonDown(0)) {
			mat.SetFloat("_Offset", Time.time);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 1000f)) {
				impulsePoints[impulsePointsIndex] = new Vector4(hit.point.x, hit.point.y, hit.point.z, 1f);
				offsets[impulsePointsIndex] = Time.time;
				switches[impulsePointsIndex] = 1;
				//int myIndex = impulsePointsIndex;
				StartCoroutine(DisableImpulse(impulsePointsIndex, 20f));
				impulsePointsIndex = (impulsePointsIndex + 1) % impulsePoints.Length;

				mat.SetVectorArray("_ImpulseArray", impulsePoints);
				mat.SetFloatArray("_OffsetArray", offsets);
				mat.SetFloatArray("_SwitchArray", switches);
			}
		}
	}

	IEnumerator DisableImpulse(int index, float time=20f) {
		yield return new WaitForSeconds(time);
		switches[index] = 0f;
		mat.SetFloatArray("_SwitchArray", switches);
	}
}
