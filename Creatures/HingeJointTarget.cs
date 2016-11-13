using UnityEngine;
using System.Collections;

public class HingeJointTarget : MonoBehaviour {

  public HingeJoint hj;
  public Transform target;
  [Tooltip("Only use one of these values at a time. Toggle invert if the rotation is backwards.")]
  public bool x, y, z, invert;

  public RigidbodyConstraints _Constraints;

  void Start ()
  {
    GetComponent<Rigidbody>().constraints = _Constraints;
    hj.useSpring = true;
  }

  void Update ()
  {
    if (hj != null)
    {
      if (x)
      {
        JointSpring js;
        js = hj.spring;

        js.targetPosition = target.transform.localEulerAngles.x;

        if (js.targetPosition > 180)
          js.targetPosition = js.targetPosition - 360;
        if (invert)
          js.targetPosition = js.targetPosition * -1;

        js.targetPosition = Mathf.Clamp(js.targetPosition, hj.limits.min + 5, hj.limits.max - 5);

        hj.spring = js;
      }
      else if (y)
      {
        JointSpring js;
        js = hj.spring;
        js.targetPosition = target.transform.localEulerAngles.y;
        if (js.targetPosition > 180)
          js.targetPosition = js.targetPosition - 360;
        if (invert)
          js.targetPosition = js.targetPosition * -1;

        js.targetPosition = Mathf.Clamp(js.targetPosition, hj.limits.min + 5, hj.limits.max - 5);

        hj.spring = js;
      }
      else if (z)
      {
        JointSpring js;
        js = hj.spring;
        js.targetPosition = target.transform.localEulerAngles.z;
        if (js.targetPosition > 180)
          js.targetPosition = js.targetPosition - 360;
        if (invert)
          js.targetPosition = js.targetPosition * -1;

        js.targetPosition = Mathf.Clamp(js.targetPosition, hj.limits.min + 5, hj.limits.max - 5);

        hj.spring = js;
      }
    }
  }

  public void Restore () {
    if (!hj.useSpring) {
      Debug.Log("Restoring " + name);
      GetComponent<Rigidbody>().constraints = _Constraints;
      hj.useSpring = true;
    }
  }

  void OnCollisionEnter(Collision collision) {
    if (collision.relativeVelocity.magnitude > 10 && hj.useSpring) {
      Debug.Log(name + " received collision velocity: " + collision.relativeVelocity.magnitude);
      GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
      hj.useSpring = false;
    }
  }
}
