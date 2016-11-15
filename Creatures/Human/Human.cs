using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour {

  private MultiOSControls _controls;

  private Transform _PelvisTransform;
  private Rigidbody _PelvisRigidbody;

  public Vector3 _Forward;

  public Animator _Animator;

  public HingeJoint _Spine;
  public Transform _SpineTransform;
  public Rigidbody _SpineRigidbody;
  public HingeJoint _LShoulder;
  public Rigidbody _LShoulderRigidbody;
  public HingeJoint _RShoulder;
  public Rigidbody _RShoulderRigidbody;

  public HingeJoint _LKnee;
  public Rigidbody _LKneeRigidbody;
  public HingeJoint _RKnee;
  public Rigidbody _RKneeRigidbody;

  public bool _Assist;

  [Range(0, 500)]public float _multipl;
  [Range(-180, 180)]public float _angle;
  [Range(0, 500)]public float _force;

  private float _JumpTimer = 0.0f;
  public float _JumpDelay = 0.5f;

	// Use this for initialization
	void Start () {
    _controls = GameObject.Find ("Scripts").GetComponent<MultiOSControls> ();

    _Forward = new Vector3(0, 0, 1);

    _PelvisTransform = transform.parent;
    _PelvisRigidbody = _PelvisTransform.GetComponent<Rigidbody> ();

    SetUpCollision(_PelvisTransform);
	}

	// Update is called once per frame
	void Update () {
    GetComponent<CapsuleCollider> ().enabled = _Assist;

    if(_controls.getValue("Toggle Assist") != 0) {
      _Assist = !_Assist;
    }

    if(_controls.getValue("Restore") != 0) {
      Restore(transform.parent);
    }

    if(_controls.getValue("Ragdoll") != 0) {
      Ragdoll(transform.parent);
    }
	}

  void FixedUpdate ()
	{
    Move ();
    Stabilize ();
	}

  void Move () {
    _JumpTimer += Time.deltaTime;
    if(_controls.getValue("Forward") > 0.0f) {
      _Animator.SetBool("Forward", true);
      _PelvisRigidbody.AddForce(_Forward * 1000 * _controls.getValue("Forward"));
    } else {
      _Animator.SetBool("Forward", false);
    }

    if(_controls.getValue("Jump") > 0.0f && _JumpTimer >= _JumpDelay) {
      _JumpTimer = 0.0f;
      _Animator.SetBool("Jump", true);
      _PelvisRigidbody.AddForce(Vector3.up * 30000);
    } else {
      _Animator.SetBool("Jump", false);
    }

    if(_controls.getValue("Turn") != 0.0f) {
      _Forward = Quaternion.Euler(0, _controls.getValue("Turn") * 50 * Time.deltaTime, 0) * _Forward;
    }
  }

  void Stabilize () {
    if (_SpineRigidbody.GetComponent<HingeJoint> ().useSpring) {
      if(_LKnee.useSpring || _RKnee.useSpring) {
        _SpineRigidbody.GetComponent<WheelCollider> ().enabled = true;
      } else {
        _SpineRigidbody.GetComponent<WheelCollider> ().enabled = false;
      }

      _PelvisRigidbody.transform.LookAt(_PelvisRigidbody.position + Vector3.up, -_Forward);

      float dragXZ = 0.5f; // drag value (1 is stop and 0 is no drag)
      Vector3 vel;
      Vector3 locVel;

      locVel = _SpineTransform.InverseTransformDirection(_SpineRigidbody.velocity);
      locVel.x *= 1.0f - dragXZ;
      locVel.z *= 1.0f - dragXZ;
      _SpineRigidbody.velocity = _SpineTransform.TransformDirection(locVel);
    } else {
      _SpineRigidbody.GetComponent<WheelCollider> ().enabled = false;
    }
  }

  void Restore (Transform t) {
    foreach (Transform child in t)
    {
      HingeJointTarget hj = child.GetComponent<HingeJointTarget> ();
      if (hj != null) { hj.Restore(); }
      Restore(child);
    }
  }

  void Ragdoll (Transform t) {
    foreach (Transform child in t)
    {
      HingeJointTarget hj = child.GetComponent<HingeJointTarget> ();
      if (hj != null) { hj.Ragdoll(); }
      Ragdoll(child);
    }
  }

  void SetUpCollision (Transform t) {
    foreach (Transform child in t)
    {
      Debug.Log(child.name);
      if(child.GetComponent<Collider>()) {
        Physics.IgnoreCollision(child.GetComponent<Collider>(), _SpineTransform.GetComponent<WheelCollider>());
      }
      SetUpCollision(child);
    }
  }

  private float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
  {
    Vector2 diference = vec2 - vec1;
    float sign = (vec2.y < vec1.y)? -1.0f : 1.0f;
    return Vector2.Angle(Vector2.right, diference) * sign;
  }
}
