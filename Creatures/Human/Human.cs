using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Human : MonoBehaviour {

  private MultiOSControls _controls;

  private Transform _PelvisTransform;
  private Rigidbody _PelvisRigidbody;

  public Animator _Animator;

  public HingeJoint _Spine;
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
      int scene = SceneManager.GetActiveScene().buildIndex;
      SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
	}

  void FixedUpdate ()
	{
    Move ();
	}

  void Move () {
    if(_controls.getValue("Forward") > 0.0f) {
      _Animator.SetBool("Forward", true);
      if (_Assist) {
        _PelvisRigidbody.AddForce(_PelvisTransform.up * -50 * _controls.getValue("Forward"));
      }
    } else {
      _Animator.SetBool("Forward", false);
    }

    if(_controls.getValue("Jump") > 0.0f) {
      _Animator.SetBool("Jump", true);
      _PelvisRigidbody.AddForce(Vector3.up * 200);
    } else {
      _Animator.SetBool("Jump", false);
    }

    if(_controls.getValue("Turn") != 0.0f) {
      _PelvisRigidbody.transform.Rotate(new Vector3(0, 0, _controls.getValue("Turn") * 50 * Time.deltaTime));
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
}
