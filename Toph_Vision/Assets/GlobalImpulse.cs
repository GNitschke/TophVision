using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalImpulse : MonoBehaviour
{

	//public static Vector4[] impulsePoints;
	//public static float[] offsets;
	//public static float[] switches;
	//public static int impulsePointsIndex = 0;

	public static List<Impulse> allImpulses;
	public static List<Group> groups;

	static GlobalImpulse() {
		//impulsePoints = new Vector4[40];
		//offsets = new float[40];
		//switches = new float[40];
		//for(int i = 0; i < impulsePoints.Length; i++) {
		//	impulsePoints[i] = new Vector4(0f, 0f, 0f, 0f);
		//	offsets[i] = 0f;
		//	switches[i] = 0;
		//}

		allImpulses = new List<Impulse>();
		groups = new List<Group>();
	}

	public class Impulse {
		public Vector4 location;
		public Vector3 location3;
		public float offset;
		public float speed;

		public List<ImpulseManager> participators;

		public Impulse(Vector4 location, float offset, float speed) {
			this.location = location;
			this.location3 = new Vector3(location.x, location.y, location.z);
			this.offset = offset;
			this.speed = speed;

			participators = new List<ImpulseManager>();
		}

		public Impulse(Impulse i) {
			this.location = i.location;
			this.location3 = i.location3;
			this.offset = i.offset;
			this.speed = i.speed;

			this.participators = new List<ImpulseManager>(i.participators);
		}
	}

	public class Group {
		public List<ImpulseManager> participators;
		public List<Impulse> impulses;
		public Group() {
			participators = new List<ImpulseManager>();
			impulses = new List<Impulse>();
		}
	}

	public static Impulse AddImpulse(Impulse impulse) {
		Impulse existingImpulse = null;
		for(int i = 0; i < allImpulses.Count; i++) {
			if(allImpulses[i].offset == impulse.offset && allImpulses[i].location == impulse.location) {
				existingImpulse = allImpulses[i];
				break;
			}
		}
		if(existingImpulse != null) {
			existingImpulse.participators.Add(impulse.participators[0]);
			return existingImpulse;
		}

		allImpulses.Add(impulse);
		impulse.participators[0].myGroup.impulses.Add(impulse);
		return impulse;
	}

	// Merge groups of im1 and im2 after collision
	public static void MergeGroups(ImpulseManager im1, ImpulseManager im2) {
		// If they are the same group, do nothing
		if(im1.myGroup != im2.myGroup) {
			// Merge the participators
			foreach(ImpulseManager im in im1.myGroup.participators) {
				im2.myGroup.participators.Add(im);
			}
			// Merge the impulses
			foreach(Impulse i in im1.myGroup.impulses) {
				im2.myGroup.impulses.Add(i);
			}
			// Remove the old group
			groups.Remove(im1.myGroup);
			Debug.Log("Removing group because of merge between "+im1.gameObject.name+" and "+im2.gameObject.name+". There are now "+groups.Count);
			// Set group of im1 to be im2's group, which has all the im1 participators/impulses
			im1.myGroup = im2.myGroup;
		}
		//Debug.Log(im1.myGroup.participators.Count);
	}

	// Possibly separate one group into two after im exits collision with a participator
	public static void UpdateGroup(ImpulseManager im) {
		// Get the first group
		List<ImpulseManager> touched = new List<ImpulseManager>();
		List<Impulse> impulses = new List<Impulse>();
		CheckCollidingWith(im, touched, impulses);
		// If there are now two groups
		if(touched.Count < im.myGroup.participators.Count) {

			// Get the second group
			List<ImpulseManager> untouched = new List<ImpulseManager>();
			List<Impulse> otherImpulses = new List<Impulse>();
			foreach(ImpulseManager p in im.myGroup.participators) {
				if(!touched.Contains(p)) {
					CheckCollidingWith(p, untouched, otherImpulses);
				}
			}

			// Remove second group participators from impulses of the first group
			for(int i = 0; i < impulses.Count; i++) {
				// Collect a list of ImpulseManagers to remove from this impulse
				List<ImpulseManager> toRemove = new List<ImpulseManager>();
				foreach(ImpulseManager p in untouched) {
					if(impulses[i].participators.Contains(p)) {
						toRemove.Add(p);
					}
				}
				// If some need to be removed
				if(toRemove.Count > 0) {
					// Remove this impulse from the list
					allImpulses.Remove(impulses[i]);
					// Make a copy
					impulses[i] = new Impulse(impulses[i]);
					// Add the copy
					allImpulses.Add(impulses[i]);
					// Remove all toRemove ImpulseManagers from the copy
					foreach(ImpulseManager p in toRemove) {
						impulses[i].participators.Remove(p);
					}
				}
			}

			// Remove participators of second group from all impulses in first group
			DivideImpulses(impulses, untouched);
			// Remove participators of the first group from all impulses in the second group
			DivideImpulses(otherImpulses, touched);

			// Make a new group for the first group
			Group newGroup = new Group();
			newGroup.participators = touched;
			newGroup.impulses = impulses;
			groups.Add(newGroup);
			Debug.Log("Added a new group because "+im.gameObject.name+" exited collision. There are now "+groups.Count);
			// Remove the old group, because the member of that that im exited collision with will make another one
			if(groups.Remove(im.myGroup)) {
				Debug.Log("Successfully removed "+im.gameObject.name+"'s old group. There are now "+groups.Count);
			} else {
				Debug.Log("Tried to remove "+im.gameObject.name+"'s old group, but it was already removed. There are now "+groups.Count);
			}
			foreach(ImpulseManager p in newGroup.participators) {
				p.myGroup = newGroup;
			}
		}
	}
	private static List<ImpulseManager> CheckCollidingWith(ImpulseManager im, List<ImpulseManager> touched, List<Impulse> impulses) {
		touched.Add(im);
		foreach(Impulse impulse in im.myImpulses) {
			impulses.Add(impulse);
		}
		for(int i = 0; i < im.collidingWith.Count; i++) {
			if(!touched.Contains(im.collidingWith[i])) {
				CheckCollidingWith(im.collidingWith[i], touched, impulses);
			}
		}
		return touched;
	}
	private static void DivideImpulses(List<Impulse> impulses, List<ImpulseManager> nonparticipators) {
		// Go through each impulse
		for(int i = 0; i < impulses.Count; i++) {
			// Collect a list of ImpulseManagers to remove from this impulse
			List<ImpulseManager> toRemove = new List<ImpulseManager>();
			foreach(ImpulseManager p in nonparticipators) {
				if(impulses[i].participators.Contains(p)) {
					toRemove.Add(p);
				}
			}
			// If some need to be removed
			if(toRemove.Count > 0) {
				// Remove this impulse from the list
				allImpulses.Remove(impulses[i]);
				// Make a copy
				impulses[i] = new Impulse(impulses[i]);
				// Add the copy
				allImpulses.Add(impulses[i]);
				// Remove all toRemove ImpulseManagers from the copy
				foreach(ImpulseManager p in toRemove) {
					impulses[i].participators.Remove(p);
				}
			}
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		GameObject[] all = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
		List<ImpulseManager> allImpulseManagers = new List<ImpulseManager>();
		foreach(GameObject g in all) {
			if(g.GetComponent<Collider>() && g.GetComponent<MeshRenderer>()) {
				allImpulseManagers.Add(g.AddComponent<ImpulseManager>());
			}
		}
		StartCoroutine(LateStart(allImpulseManagers));
	}

	IEnumerator LateStart(List<ImpulseManager> allImpulseManagers) {
		yield return new WaitForEndOfFrame();
		Group firstGroup = new Group();
		groups.Add(firstGroup);
		foreach(ImpulseManager im in allImpulseManagers) {
			foreach(ImpulseManager otherIM in allImpulseManagers) {
				im.collidingWith.Add(otherIM);
			}
			groups.Remove(im.myGroup);
			im.myGroup = firstGroup;
			firstGroup.participators.Add(im);
		}
	}

	// Update is called once per frame
	void Update()
	{
		// Go through each group
		foreach(Group g in groups) {
			// Each participator
			foreach(ImpulseManager im in g.participators) {
				// Each impulse of this group
				for(int i = 0; i < g.impulses.Count; i++) {
					Impulse impulse = g.impulses[i];
					// If participator is not participating in this impulse
					if(!impulse.participators.Contains(im)) {
						// Check if it should be now
						Vector3 closestPoint = im.GetClosestPoint(impulse.location3);
						if((closestPoint - impulse.location3).magnitude < impulse.speed * (Time.time - impulse.offset)) {
							im.AddExistingImpulse(impulse);
						}
					}
				}
			}
		}
	}
}
