using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour {

  private MultiOSControls _Controls;
  private Animator _Animator;

  public Transform _ObjSpine;

  public Rigidbody _Head;
  public Rigidbody _Pelvis;
  public Rigidbody _Spine;
  public Rigidbody _LeftHips, _LeftKnee, _LeftAnkle;
  public Rigidbody _RightHips, _RightKnee, _RightAnkle;
  public Rigidbody _LeftArm, _LeftElbow;
  public Rigidbody _RightArm, _RightElbow;

  protected float _time = 0.0f;
  public float _WalkTime = 0.2f;

  void Awake () {
		_Controls = GameObject.Find ("Scripts").GetComponent<MultiOSControls> ();
    _Animator = GetComponent<Animator> ();
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
    _time += Time.deltaTime;

    _LeftElbow.AddForce(transform.up * 100.0f);
    _RightElbow.AddForce(transform.up * 100.0f);
	}
}
