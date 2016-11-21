using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Human2 : MonoBehaviour {

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
	}

	// Update is called once per frame
	void Update () {

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
