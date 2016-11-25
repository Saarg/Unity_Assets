using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour {

  private MultiOSControls _Controls;

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

  private HingeJoint _Head;
  public Rigidbody _HeadRigidbody;

  [Range(0.0f, 1.0f)]public float _Drag = 0.5f;

  [Header("Controls settings")]
  public float _TurnSpeed = 50.0f;
  public float _ForwardSpeed = 2000.0f;
  private Vector3 _Forward;
  private Vector3 _BallCenter;

  [Header("Jump settings")]
  public float _JumpDelay = 0.5f;
  private float _JumpTimer = 0.0f;
  public float _JumpForce = 30000;

	// Use this for initialization
	void Start () {
    // Init Control script
    _Controls = GameObject.Find ("Scripts") ? GameObject.Find ("Scripts").GetComponent<MultiOSControls> () : null;
    // Init control assistance
    _Forward = new Vector3(0, 0, 1);
    _BallCenter = GetComponent<SphereCollider> () ? GetComponent<SphereCollider> ().center : new Vector3(0, 0, 0);

    // Load main rigidbodys/joints
    _PelvisTransform = transform.parent;
    _PelvisRigidbody = _PelvisTransform ? _PelvisTransform.GetComponent<Rigidbody> () : null;
    _SpineTransform = _SpineRigidbody ? _SpineRigidbody.transform : null;
    _Spine = _SpineTransform ? _SpineTransform.GetComponent<HingeJoint> () : null;

    _LHip = _LHipRigidbody ? _LHipRigidbody.GetComponent<HingeJoint> () : null;
    _RHip = _RHipRigidbody ? _RHipRigidbody.GetComponent<HingeJoint> () : null;
    _LKnee = _LKneeRigidbody ? _LKneeRigidbody.GetComponent<HingeJoint> () : null;
    _RKnee = _RKneeRigidbody ? _RKneeRigidbody.GetComponent<HingeJoint> () : null;

    _Head = _HeadRigidbody ? _HeadRigidbody.GetComponent<HingeJoint> () : null;

    // Init collision to avoid breaking your own leg
    SetUpCollision(_PelvisTransform);

    // Init timers
    _JumpTimer = _JumpDelay;
	}

	// Update is called once per frame
	void Update () {
    // Restore or go full ragdoll
    if(_Controls.getValue("Restore") != 0 || (_Head && !_Head.useSpring)) {
      Restore(transform.parent);
    }

    if(_Controls.getValue("Ragdoll") != 0) {
      Ragdoll(transform.parent);
    }
	}

  void FixedUpdate ()
	{
    Move ();
    Stabilize ();
	}

  void Move () {
    // Update timer
    _JumpTimer += Time.deltaTime;

    // Handle forward
    if(_Controls.getValue("Forward") > 0.0f) {
      _Animator.SetBool("Forward", true);

      // Assistance depending on damages
      if (_Spine.useSpring) {
        if(!_LKnee.useSpring && !_RKnee.useSpring) {
          _PelvisRigidbody.AddForce(_Forward * _ForwardSpeed/2 * _Controls.getValue("Forward"));
        } else {
          _PelvisRigidbody.AddForce(_Forward * _ForwardSpeed * _Controls.getValue("Forward"));
        }
      }
    } else {
      _Animator.SetBool("Forward", false);
    }

    // Turn
    if(_Controls.getValue("Turn") != 0.0f) {
      _Forward = Quaternion.Euler(0, _Controls.getValue("Turn") * _TurnSpeed * Time.deltaTime, 0) * _Forward;
    }

    // Jump
    if(_Controls.getValue("Jump") > 0.0f && _JumpTimer >= _JumpDelay) {
      _JumpTimer = 0.0f;
      _Animator.SetBool("Jump", true);
      _PelvisRigidbody.AddForce(Vector3.up * _JumpForce);
    } else {
      _Animator.SetBool("Jump", false);
    }
  }

  void Stabilize () {
    // Stabilize if the spine isn't broken
    if (_Spine.useSpring) {
      // Position ball depending on damages
      Vector3 center = _BallCenter;
      if(!_LKnee.useSpring && !_RKnee.useSpring) {
        center.y /= 2;
      }
      GetComponent<SphereCollider> ().center = center;
      // Enable ball depending on damages
      if(_LHip.useSpring && _LHip.useSpring && (_LKnee.useSpring || _RKnee.useSpring)) {
        GetComponent<SphereCollider> ().enabled = true;
      } else {
        GetComponent<SphereCollider> ().enabled = false;
      }

      // Pelvis orientatiion
      _PelvisRigidbody.transform.LookAt(_PelvisRigidbody.position + _Forward);

      // Add linear drag to control drift
      float dragXZ = _Drag; // drag value (1 is stop and 0 is no drag)
      Vector3 vel;
      Vector3 locVel;

      locVel = _SpineTransform.InverseTransformDirection(_SpineRigidbody.velocity);
      locVel.x *= 1.0f - dragXZ;
      locVel.z *= 1.0f - dragXZ;
      _SpineRigidbody.velocity = _SpineTransform.TransformDirection(locVel);
    } else {
      GetComponent<SphereCollider> ().enabled = false;
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
      if(child.GetComponent<Collider>()) {
        Physics.IgnoreCollision(child.GetComponent<Collider>(), _SpineTransform.GetComponent<BoxCollider>());
      }
      SetUpCollision(child);
    }
  }
}
