﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class tank : MonoBehaviour {

	private MultiOSControls _controls;

	public int playerNumber = 1;

  [Header("General settings")]
  [Range(0, 1000)] public int _downforce = 50;
  public int _engineRedline = 7500;
	public int _engineIdle = 600;
	public AnimationCurve TorqueCurve;
	public int _brakeTorque = 500;
  public Transform _centerOfMass;

  [Header("Wheels settings")]
	public float turnRadius = 30f;
	public float _maxHandlingSpeed = 80f;
	public WheelCollider[] LWheel;
	public WheelCollider[] RWheel;

  [Header("Controls")]
	protected string _MovementAxisName;
	protected string _TurnAxisName;
	protected Rigidbody _Rigidbody;
	protected float _MovementInputValue;
	protected float _TurnInputValue;

  [Header("Gearbox")]
	public int _curGear = 2;
	public float[] _gears = new float[]{-3.833f, 0f, 3.833f, 2.235f, 1.458f, 1.026f};
	protected float _nextGear;
	public float _gearChangeTime = 0.5f;
	protected float _time = 0.0f;
  public int _torqueMultiplier = 4;

  [Header("Infos")]
	public float engineRPM = 600;
	public float speed = 0;

  [Header("UI")]
	protected float _UITime = 0.0f;
	[HideInInspector]public Text _speedo;
	[HideInInspector]public Text _gear;
	[HideInInspector]public Text _rpmGage;
	public TextMesh _MeshSpeedo;
	public TextMesh _MeshGear;
	public TextMesh _MeshRpmGage;

	// Use this for initialization
	void Awake () {
		_controls = GameObject.Find ("Scripts").GetComponent<MultiOSControls> ();

		_Rigidbody = GetComponent<Rigidbody> ();
		_Rigidbody.centerOfMass = _centerOfMass.localPosition;
	}

	protected void OnEnable ()
	{
		// When the car is turned on, make sure it's not kinematic.
		_Rigidbody.isKinematic = false;

		// Also reset the input values.
		_MovementInputValue = 0f;
		_TurnInputValue = 0f;
	}

	protected void Start ()
	{
		// The axes names are based on player number.
		_MovementAxisName = "Acceleration" + playerNumber;
		_TurnAxisName = "Steering" + playerNumber;

		engineRPM = _engineIdle;
	}

	void Update () {
		_MovementInputValue = _controls.getValue (_MovementAxisName);
		_TurnInputValue = _controls.getValue (_TurnAxisName);

		var localVelocity = transform.InverseTransformDirection(_Rigidbody.velocity);

		_UITime += Time.deltaTime;
		if (_UITime > 0.5f) {
			_UITime = 0.0f;
			_speedo.text = Mathf.Round (localVelocity.z * 3.6f) + "km/h";
			_gear.text = "Gear: " + (_curGear - 1);
			_rpmGage.text = "Rpm: " + Mathf.Round (engineRPM / 100) * 100;
			_MeshSpeedo.text = Mathf.Round (localVelocity.z * 3.6f) + "km/h";
			_MeshGear.text = "Gear: " + (_curGear - 1);
			_MeshRpmGage.text = "Rpm: " + Mathf.Round (engineRPM / 100) * 100;
		}
	}

	protected void FixedUpdate ()
	{
		// Adjust the rigidbodies position and orientation in FixedUpdate.
		Move ();
		Turn ();
		Gearbox ();
	}

	protected void Gearbox ()
	{
		_time = _time + Time.deltaTime;
		if (_controls.getValue ("ShiftUp1") == 1 && _time > _nextGear) {
			_curGear++;
			_nextGear = _time + _gearChangeTime;
		}
		if (_controls.getValue ("ShiftDown1") == 1 && _time > _nextGear) {
			_curGear--;
			_nextGear = _time + _gearChangeTime;
		}

		if (_curGear < 0) {
			_curGear = 0;
		} else if (_curGear > _gears.Length - 1) {
			_curGear = _gears.Length-1;
		}
	}

	protected void Move ()
	{
		float oldEngineRPM = engineRPM;
		// Compute torque with engineRPM and torquecurve
		float torque = _MovementInputValue * TorqueCurve.Evaluate(engineRPM) * _torqueMultiplier * _gears[_curGear];


		// Aply torque and brake
		foreach (WheelCollider wheel in LWheel) {
			if (wheel.isGrounded) {
				wheel.motorTorque = torque / LWheel.Length;
			}
			wheel.brakeTorque = _brakeTorque*_controls.getValue ("Break1");
		}
		foreach (WheelCollider wheel in RWheel) {
			if (wheel.isGrounded) {
				wheel.motorTorque = torque / RWheel.Length;
			}
			wheel.brakeTorque = _brakeTorque*_controls.getValue ("Break1");
		}

		// Compute RPM
		int groudedCount = 0;
		float avgRPM = 0;
		foreach (WheelCollider w in RWheel) {
			if (w.isGrounded) {
				groudedCount++;
				avgRPM += w.rpm;
			}
		}
		foreach (WheelCollider w in LWheel) {
			if (w.isGrounded) {
				groudedCount++;
				avgRPM += w.rpm;
			}
		}
		if (groudedCount > 0) {
			avgRPM = avgRPM / groudedCount;
		} else {
			avgRPM = _engineIdle;
		}
		engineRPM = _torqueMultiplier * avgRPM * _gears [_curGear];

		engineRPM = Mathf.Lerp (oldEngineRPM, engineRPM, 0.1f);

		// Lock idle < rpm < redline
		if (engineRPM >= _engineRedline)
		{
			engineRPM = _engineRedline;
		} else if (engineRPM < _engineIdle) {
			engineRPM = _engineIdle;
		}

		// Downforce
		_Rigidbody.AddForce(-transform.up*_downforce*_Rigidbody.velocity.magnitude);
		// Speed
		speed = transform.InverseTransformDirection(_Rigidbody.velocity).z * 3.6f;
	}

	protected void Turn ()
	{
		LWheel[1].steerAngle = _TurnInputValue * turnRadius * Mathf.Clamp ((1.0f - (Mathf.Abs (speed) / (2.0f * _maxHandlingSpeed))), 0.1f, 1.0f);
		RWheel[1].steerAngle = _TurnInputValue * turnRadius * Mathf.Clamp ((1.0f - (Mathf.Abs (speed) / (2.0f * _maxHandlingSpeed))), 0.1f, 1.0f);

		LWheel[4].steerAngle = -_TurnInputValue * turnRadius * Mathf.Clamp ((1.0f - (Mathf.Abs (speed) / (2.0f * _maxHandlingSpeed))), 0.1f, 1.0f);
		RWheel[4].steerAngle = -_TurnInputValue * turnRadius * Mathf.Clamp ((1.0f - (Mathf.Abs (speed) / (2.0f * _maxHandlingSpeed))), 0.1f, 1.0f);
	}
}
