using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Human2 : MonoBehaviour {

  private Rigidbody _PelvisRigidbody;
  private Rigidbody _SpineRigidbody;

  public List<ConfigurableJoint> _Joints = new List<ConfigurableJoint>();
  public JointDrive _JointDrive;
  public int positionSpring = 1000;
  public int positionDamper = 0;
  public int maximumForce = 5000;

	// Use this for initialization
	void Start () {
    _JointDrive.positionSpring = positionSpring;
    _JointDrive.positionDamper = positionDamper;
    _JointDrive.maximumForce = maximumForce;

    LoadJoints(transform);

    _PelvisRigidbody = GetComponent<Rigidbody> ();
    foreach (Transform child in transform)
    {
      if(child.name == "Spine") {
        _SpineRigidbody = child.GetComponent<Rigidbody> ();
      }
    }
	}

	// Update is called once per frame
	void Update () {
    _SpineRigidbody.transform.LookAt(_SpineRigidbody.position + Vector3.forward, Vector3.up);

    float dragXZ = 0.5f; // drag value (1 is stop and 0 is no drag)
    Vector3 vel;
    Vector3 locVel;

    locVel = _SpineRigidbody.transform.InverseTransformDirection(_SpineRigidbody.velocity);
    locVel.x *= 1.0f - dragXZ;
    locVel.z *= 1.0f - dragXZ;
    _SpineRigidbody.velocity = _SpineRigidbody.transform.TransformDirection(locVel);
	}

  void FixedUpdate ()	{

	}

  void LoadJoints (Transform t) {
    foreach (Transform child in t)
    {
      ConfigurableJoint joint = child.GetComponent<ConfigurableJoint> ();
      if (joint != null) {
        joint.angularXDrive = _JointDrive;
        joint.angularYZDrive = _JointDrive;
        _Joints.Add(joint);
      }
      LoadJoints(child);
    }
  }
}
