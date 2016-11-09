using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour {

  private MultiOSControls _controls;

  private Rigidbody _Head;
  private Rigidbody _Body;
  private Rigidbody _LeftHips, _LeftKnee, _LeftAnkle;
  private Rigidbody _RightHips, _RightKnee, _RightAnkle;
  private Rigidbody _LeftArm, _LeftElbow;
  private Rigidbody _RightArm, _RightElbow;

  void Awake () {
		_controls = GameObject.Find ("Scripts").GetComponent<MultiOSControls> ();

    _Head = transform.Find("Head").gameObject.GetComponent<Rigidbody>();
    _Body = transform.Find("Body").gameObject.GetComponent<Rigidbody>();
    _LeftHips = transform.Find("Left Hips").gameObject.GetComponent<Rigidbody>();
    _LeftKnee = transform.Find("Left Knee").gameObject.GetComponent<Rigidbody>();
    _LeftAnkle = transform.Find("Left Ankle").gameObject.GetComponent<Rigidbody>();
    _RightHips = transform.Find("Right Hips").gameObject.GetComponent<Rigidbody>();
    _RightKnee = transform.Find("Right Knee").gameObject.GetComponent<Rigidbody>();
    _RightAnkle = transform.Find("Right Ankle").gameObject.GetComponent<Rigidbody>();
    _LeftArm = transform.Find("Left Arm").gameObject.GetComponent<Rigidbody>();
    _LeftElbow = transform.Find("Left Elbow").gameObject.GetComponent<Rigidbody>();
    _RightArm = transform.Find("Right Arm").gameObject.GetComponent<Rigidbody>();
    _RightElbow = transform.Find("Right Elbow").gameObject.GetComponent<Rigidbody>();
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
    _Body.AddForce(transform.forward * -30 * _controls.getValue("Forward"));
    //_Body.AddForce(transform.up * 630);
	}
}
