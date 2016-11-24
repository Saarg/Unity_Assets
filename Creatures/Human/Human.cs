using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour {

  private MultiOSControls _controls;

  public Animator _Animator;

  [Header("Body settings")]
  public Rigidbody _SpineRigidbody;
  private HingeJoint _Spine;
  private Transform _SpineTransform;
  private Transform _PelvisTransform;
  private Rigidbody _PelvisRigidbody;

  private HingeJoint _LHip;
  public Rigidbody _LHipRigidbody;
  private HingeJoint _RHip;
  public Rigidbody _RHipRigidbody;

  private HingeJoint _LKnee;
  public Rigidbody _LKneeRigidbody;
  private HingeJoint _RKnee;
  public Rigidbody _RKneeRigidbody;

  [Range(0.0f, 1.0f)]public float _Drag = 0.5f;

  [Header("Controls settings")]
  public float _TurnSpeed = 50.0f;
  public float _ForwardSpeed = 2000.0f;
  private Vector3 _Forward;

  [Header("Jump settings")]
  public float _JumpDelay = 0.5f;
  private float _JumpTimer = 0.0f;
  public float _JumpForce = 30000;

	// Use this for initialization
	void Start () {
    _controls = GameObject.Find ("Scripts").GetComponent<MultiOSControls> ();

    _Forward = new Vector3(0, 0, 1);

    _PelvisTransform = transform.parent;
    _PelvisRigidbody = _PelvisTransform.GetComponent<Rigidbody> ();
    _SpineTransform = _SpineRigidbody.transform;

    _Spine = _SpineTransform.GetComponent<HingeJoint> ();
    _LHip = _LHipRigidbody.GetComponent<HingeJoint> ();
    _RHip = _RHipRigidbody.GetComponent<HingeJoint> ();
    _LKnee = _LKneeRigidbody.GetComponent<HingeJoint> ();
    _RKnee = _RKneeRigidbody.GetComponent<HingeJoint> ();

    SetUpCollision(_PelvisTransform);

    _JumpTimer = _JumpDelay;
	}

	// Update is called once per frame
	void Update () {
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
      if (_Spine.useSpring) {
        _PelvisRigidbody.AddForce(_Forward * _ForwardSpeed * _controls.getValue("Forward"));
      }
    } else {
      _Animator.SetBool("Forward", false);
    }

    if(_controls.getValue("Jump") > 0.0f && _JumpTimer >= _JumpDelay) {
      _JumpTimer = 0.0f;
      _Animator.SetBool("Jump", true);
      _PelvisRigidbody.AddForce(Vector3.up * _JumpForce);
    } else {
      _Animator.SetBool("Jump", false);
    }

    if(_controls.getValue("Turn") != 0.0f) {
      _Forward = Quaternion.Euler(0, _controls.getValue("Turn") * _TurnSpeed * Time.deltaTime, 0) * _Forward;
    }
  }

  void Stabilize () {
    if (_Spine.useSpring) {
      if(_LHip.useSpring && _LHip.useSpring && (_LKnee.useSpring || _RKnee.useSpring)) {
        _SpineRigidbody.GetComponent<BoxCollider> ().enabled = true;
      } else {
        _SpineRigidbody.GetComponent<BoxCollider> ().enabled = false;
      }

      _PelvisRigidbody.transform.LookAt(_PelvisRigidbody.position + _Forward);

      float dragXZ = _Drag; // drag value (1 is stop and 0 is no drag)
      Vector3 vel;
      Vector3 locVel;

      locVel = _SpineTransform.InverseTransformDirection(_SpineRigidbody.velocity);
      locVel.x *= 1.0f - dragXZ;
      locVel.z *= 1.0f - dragXZ;
      _SpineRigidbody.velocity = _SpineTransform.TransformDirection(locVel);
    } else {
      _SpineRigidbody.GetComponent<BoxCollider> ().enabled = false;
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
    Physics.IgnoreCollision(_LKneeRigidbody.GetComponent<Collider>(), _RKneeRigidbody.GetComponent<Collider>());
    foreach (Transform child in t)
    {
      Debug.Log(child.name);
      if(child.GetComponent<Collider>()) {
        Physics.IgnoreCollision(child.GetComponent<Collider>(), _SpineTransform.GetComponent<BoxCollider>());
      }
      SetUpCollision(child);
    }
  }
}
