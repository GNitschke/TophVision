using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseManager : MonoBehaviour
{
	public Material mat;
	public MeshRenderer mr;

	public Vector4[] impulsePoints;
	public float[] offsets;
	public float[] speeds;
	public float[] switches;
	public int impulsePointsIndex = 0;

	private int maxImpulsePoints = 40;

	public List<GlobalImpulse.Impulse> myImpulses;
	public GlobalImpulse.Group myGroup;
	public List<ImpulseManager> collidingWith;

	private BoxCollider boxCollider;
	private SphereCollider sphereCollider;
	private CapsuleCollider capsuleCollider;
	private MeshCollider meshCollider;

	// Start is called before the first frame update
	void Start()
	{
		mr = GetComponent<MeshRenderer>();
		mat = new Material(mr.material);
		mr.material = mat;

		impulsePoints = new Vector4[maxImpulsePoints];
		offsets = new float[maxImpulsePoints];
		speeds = new float[maxImpulsePoints];
		switches = new float[maxImpulsePoints];
		impulsePointsIndex = 0;

		myImpulses = new List<GlobalImpulse.Impulse>();
		myGroup = new GlobalImpulse.Group();
		GlobalImpulse.groups.Add(myGroup);
		myGroup.participators.Add(this);
		collidingWith = new List<ImpulseManager>();

		boxCollider = GetComponent<BoxCollider>();
		sphereCollider = GetComponent<SphereCollider>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		meshCollider = GetComponent<MeshCollider>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public Vector3 GetClosestPoint(Vector3 location) {
		if(boxCollider) return boxCollider.ClosestPoint(location);
		else if(sphereCollider) return sphereCollider.ClosestPoint(location);
		else if(capsuleCollider) return capsuleCollider.ClosestPoint(location);
		else if(meshCollider) return meshCollider.ClosestPoint(location);
		else {
			Debug.Log("Uh oh no collider");
			return Vector3.zero;
		}
	}

	public void AddImpulse(Vector4 location, float offset, float speed) {
		GlobalImpulse.Impulse newImpulse = new GlobalImpulse.Impulse(location, offset, speed);
		newImpulse.participators.Add(this);
		myImpulses.Add(GlobalImpulse.AddImpulse(newImpulse));

		AddExistingImpulse(newImpulse);
	}

	public void AddExistingImpulse(GlobalImpulse.Impulse impulse) {
		impulsePoints[impulsePointsIndex] = impulse.location;
		offsets[impulsePointsIndex] = impulse.offset;
		speeds[impulsePointsIndex] = impulse.speed;
		switches[impulsePointsIndex] = 1;

		StartCoroutine(DisableImpulse(impulsePointsIndex));
		impulsePointsIndex = (impulsePointsIndex + 1) % impulsePoints.Length;

		mat.SetVectorArray("_ImpulseArray", impulsePoints);
		mat.SetFloatArray("_OffsetArray", offsets);
		mat.SetFloatArray("_SpeedArray", speeds);
		mat.SetFloatArray("_SwitchArray", switches);
	}

	void OnCollisionEnter(Collision other) {
		ImpulseManager newColliding = other.gameObject.GetComponent<ImpulseManager>();
		if(!newColliding) return;
		//if(newColliding) collidingWith.Add(newColliding);
		//else Debug.Log("!!!!"+other.gameObject.name);

		//print("Points colliding: " + other.contacts.Length);
		//print("First normal of the point that collide: " + other.contacts[0].normal);

		Vector4 impulseLocation = new Vector4(other.contacts[0].point.x, other.contacts[0].point.y, other.contacts[0].point.z, 1f);
		//Debug.Log(impulseLocation);
		float impulseTime = Time.time;

		AddImpulse(impulseLocation, impulseTime, 5f);
		GlobalImpulse.MergeGroups(this, newColliding);
		collidingWith.Add(newColliding);
	}

	void OnCollisionExit(Collision other) {
		ImpulseManager exitColliding = other.gameObject.GetComponent<ImpulseManager>();
		if(exitColliding) {
			collidingWith.Remove(exitColliding);
			Debug.Log("Calling updagegroup because collision exit between "+name+" and "+other.gameObject.name);
			GlobalImpulse.UpdateGroup(this);
		}
	}

	IEnumerator DisableImpulse(int index, float time=7f) {
		yield return new WaitForSeconds(time);
		switches[index] = 0f;
		mat.SetFloatArray("_SwitchArray", switches);
	}
}
