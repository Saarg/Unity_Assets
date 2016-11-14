using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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

  public bool _Assist;

  public float _multipl;
  public float _angle;
  public float _force;

	// Use this for initialization
	void Start () {
    _controls = GameObject.Find ("Scripts").GetComponent<MultiOSControls> ();

    _Forward = new Vector3(0, 0, 1);

    _PelvisTransform = transform.parent;
    _PelvisRigidbody = _PelvisTransform.GetComponent<Rigidbody> ();
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
    if(_controls.getValue("Forward") > 0.0f) {
      _Animator.SetBool("Forward", true);
      _PelvisRigidbody.AddForce(_Forward * 500 * _controls.getValue("Forward"));
      if (_Assist) {
      }
    } else {
      _Animator.SetBool("Forward", false);
    }

    if(_controls.getValue("Jump") > 0.0f) {
      _Animator.SetBool("Jump", true);
      _PelvisRigidbody.AddForce(Vector3.up * 5000);
    } else {
      _Animator.SetBool("Jump", false);
    }

    if(_controls.getValue("Turn") != 0.0f) {
      _Forward = Quaternion.Euler(0, _controls.getValue("Turn") * 50 * Time.deltaTime, 0) * _Forward;
    }
  }

  void Stabilize () {
    Debug.DrawLine(_PelvisRigidbody.position, _PelvisRigidbody.position + _Forward, Color.red);
    Debug.DrawLine(_PelvisRigidbody.position, _PelvisRigidbody.position + _PelvisTransform.forward, Color.red);

    if (_SpineRigidbody.GetComponent<HingeJoint> ().useSpring) {
      _PelvisRigidbody.transform.LookAt(_PelvisRigidbody.position + Vector3.up, -_Forward);
      //_PelvisRigidbody.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_PelvisRigidbody.position + Vector3.up, _Forward), 1.0f);
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

  private float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
  {
    Vector2 diference = vec2 - vec1;
    float sign = (vec2.y < vec1.y)? -1.0f : 1.0f;
    return Vector2.Angle(Vector2.right, diference) * sign;
  }
}
