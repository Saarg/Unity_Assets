using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour {

  private MultiOSControls _controls;

  public Rigidbody _Head;
  public Rigidbody _Pelvis;
  public Rigidbody _Spine;
  public Rigidbody _LeftHips, _LeftKnee, _LeftAnkle;
  public Rigidbody _RightHips, _RightKnee, _RightAnkle;
  public Rigidbody _LeftArm, _LeftElbow;
  public Rigidbody _RightArm, _RightElbow;

  void Awake () {
		_controls = GameObject.Find ("Scripts").GetComponent<MultiOSControls> ();
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
    //_Body.AddForce(transform.forward * -30 * _controls.getValue("Forward"));
    //_Body.AddForce(transform.up * 630);
	}
}
