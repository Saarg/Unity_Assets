using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour {

  private MultiOSControls _controls;

  private Transform _PelvisTransform;
  private Rigidbody _PelvisRigidbody;

  public Animator _Animator;

	// Use this for initialization
	void Start () {
    _controls = GameObject.Find ("Scripts").GetComponent<MultiOSControls> ();

    _PelvisTransform = transform.parent;
    _PelvisRigidbody = _PelvisTransform.GetComponent<Rigidbody> ();
	}

	// Update is called once per frame
	void Update () {

	}

  void FixedUpdate ()
	{
    if(_controls.getValue("Forward") > 0.0f) {
      _Animator.SetBool("Forward", true);
      _PelvisRigidbody.AddForce(_PelvisTransform.up * -50 * _controls.getValue("Forward"));
    } else {
      _Animator.SetBool("Forward", false);
    }

    if(_controls.getValue("Turn") != 0.0f) {
      _PelvisRigidbody.transform.Rotate(new Vector3(0, 0, _controls.getValue("Turn") * 50 * Time.deltaTime));
    }
	}
}
