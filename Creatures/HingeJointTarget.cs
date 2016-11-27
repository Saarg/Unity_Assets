using UnityEngine;
using System.Collections;

public class HingeJointTarget : MonoBehaviour {

  private Color _Color = Color.green;

  public HingeJoint hj;
  public Transform target;
  [Tooltip("Only use one of these values at a time. Toggle invert if the rotation is backwards.")]
  public bool x, y, z, invert;
  public float _BreakingVel = 15.0f;

  public RigidbodyConstraints _Constraints;

  void Start ()
  {
    GetComponent<Rigidbody>().constraints = _Constraints;
    hj.useSpring = true;
  }

  void Update ()
  {
    foreach (Transform child in target)
    {
      Debug.DrawLine(child.parent.position, child.position, _Color);
    }

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
    _Color = Color.green;

    GetComponent<Rigidbody>().constraints = _Constraints;
    hj.useSpring = true;
  }

  public void Ragdoll () {
    _Color = Color.red;

    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    hj.useSpring = false;
  }

  void OnCollisionEnter(Collision collision) {
    if (collision.relativeVelocity.magnitude > _BreakingVel && hj.useSpring) {
      Debug.Log(name + " received collision velocity: " + collision.relativeVelocity.magnitude + " from " + collision.gameObject.name);
      Ragdoll();
    }
  }
}
