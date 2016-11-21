using UnityEngine;
using System.Collections;

public class ConfigJointTarget : MonoBehaviour {

  private Color _Color = Color.green;

  private ConfigurableJoint _Joint;
  public Transform _Target;
  private Quaternion _InitRotation;

	// Use this for initialization
	void Start () {
    _Joint = GetComponent<ConfigurableJoint> ();
    _InitRotation = transform.localRotation;
	}

	// Update is called once per frame
	void Update () {
    // Calculate the rotation expressed by the joint's axis and secondary axis
		var right = _Joint.axis;
		var forward = Vector3.Cross (_Joint.axis, _Joint.secondaryAxis).normalized;
		var up = Vector3.Cross (forward, right).normalized;
		Quaternion worldToJointSpace = Quaternion.LookRotation (forward, up);

		// Transform into world space
		Quaternion resultRotation = Quaternion.Inverse (worldToJointSpace);

		// Counter-rotate and apply the new local rotation.
		// Joint space is the inverse of world space, so we need to invert our value
		// if (space == Space.World) {
    //  sultRotation *= _InitRotation * Quaternion.Inverse (_Target.rotation);
		// } else {
			resultRotation *= Quaternion.Inverse (_Target.transform.localRotation) * _InitRotation;
		//}

		// Transform back into joint space
		resultRotation *= worldToJointSpace;

		// Set target rotation to our newly calculated rotation
    _Joint.targetRotation = resultRotation;
	}
}
