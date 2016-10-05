using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class tank : MonoBehaviour {

	public int playerNumber = 1;
	public float turnRadius = 30f;
	public float _maxHandlingSpeed = 80f;
	public WheelCollider[] LWheel;
	public WheelCollider[] RWheel;
	public AnimationCurve TorqueCurve;
	public int _brakeTorque = 500;

	private string _MovementAxisName;
	private string _TurnAxisName; 
	private Rigidbody _Rigidbody;
	public Transform _centerOfMass;
	private float _MovementInputValue;
	private float _TurnInputValue;

	public int _curGear = 2;
	public float[] _gears = new float[]{-3.833f, 0f, 3.833f, 2.235f, 1.458f, 1.026f};
	private float _nextGear;
	public float _gearChangeTime = 0.5f;
	private float _time = 0.0f;

	public float engineRPM = 600;
	public float speed = 0;
	public int _torqueMultiplier = 4;

	private float _UITime = 0.0f;
	public Text _speedo;
	public Text _gear;
	public Text _rpmGage;

	// Use this for initialization
	void Awake () {
		_Rigidbody = GetComponent<Rigidbody> ();
		_Rigidbody.centerOfMass = _centerOfMass.localPosition;
	}

	private void OnEnable ()
	{
		// When the tank is turned on, make sure it's not kinematic.
		_Rigidbody.isKinematic = false;

		// Also reset the input values.
		_MovementInputValue = 0f;
		_TurnInputValue = 0f;
	}

	private void Start ()
	{
		// The axes names are based on player number.
		_MovementAxisName = "Acceleration" + playerNumber;
		_TurnAxisName = "Steering" + playerNumber;
	}

	void Update () {
		_MovementInputValue = Input.GetAxis (_MovementAxisName);
		_TurnInputValue = Input.GetAxis (_TurnAxisName);

		var localVelocity = transform.InverseTransformDirection(_Rigidbody.velocity);

		_UITime += Time.deltaTime;
		if (_UITime > 0.5f) {
			_UITime = 0.0f;
			_speedo.text = Mathf.Round (localVelocity.z * 3.6f) + "km/h";
			_gear.text = "Gear: " + (_curGear - 1);
			_rpmGage.text = "Rpm: " + Mathf.Round (engineRPM / 100) * 100;
		}
	}

	private void FixedUpdate ()
	{
		// Adjust the rigidbodies position and orientation in FixedUpdate.
		Move ();
		Turn ();
		Gearbox ();
	}

	private void Gearbox ()
	{
		_time = _time + Time.deltaTime;
		if (Input.GetButton ("ShiftUp1") && _time > _nextGear) {
			_curGear++;
			_nextGear = _time + _gearChangeTime;
		}
		if (Input.GetButton ("ShiftDown1") && _time > _nextGear) {
			_curGear--;
			_nextGear = _time + _gearChangeTime;
		}

		if (_curGear < 0) {
			_curGear = 0;
		} else if (_curGear > _gears.Length - 1) {
			_curGear = _gears.Length-1;
		}
	}

	private void Move ()
	{
		float torque = _MovementInputValue * TorqueCurve.Evaluate(engineRPM) * _torqueMultiplier * _gears[_curGear];

		foreach (WheelCollider wheel in LWheel) {
			if (wheel.isGrounded) {
				wheel.motorTorque = torque / LWheel.Length;
			}
			wheel.brakeTorque = _brakeTorque*Input.GetAxis ("Break1");
		}
		foreach (WheelCollider wheel in RWheel) {
			if (wheel.isGrounded) {
				wheel.motorTorque = torque / RWheel.Length;
			}
			wheel.brakeTorque = _brakeTorque*Input.GetAxis ("Break1");
		}


		// Update speed and rpm
		int groundedCount = 0;
		speed = 0;
		engineRPM = 0;
		foreach (WheelCollider wheel in RWheel) {
			if (wheel.isGrounded) {
				groundedCount++;
				speed += 2 * wheel.radius * Mathf.PI * wheel.rpm * 60f / 1000f;
				engineRPM += _torqueMultiplier * wheel.rpm * _gears [_curGear];

				break;
			}
		}
		foreach (WheelCollider wheel in LWheel) {
			if (wheel.isGrounded) {
				groundedCount++;
				speed += 2 * wheel.radius * Mathf.PI * wheel.rpm * 60f / 1000f;
				engineRPM += _torqueMultiplier * wheel.rpm * _gears [_curGear];

				break;
			}
		}

		if (groundedCount > 0) {
			speed = speed / groundedCount;
			engineRPM = engineRPM / groundedCount;
		}

		if (engineRPM < 600) {
			engineRPM = 600;
		}
		if (engineRPM > 10000) {
			engineRPM = 10000;
		}
	}

	private void Turn ()
	{
		LWheel[1].steerAngle += _TurnInputValue - LWheel[1].steerAngle/turnRadius * (speed/_maxHandlingSpeed + 1);
		RWheel[1].steerAngle += _TurnInputValue - RWheel[1].steerAngle/turnRadius * (speed/_maxHandlingSpeed + 1);

		LWheel[4].steerAngle = -_TurnInputValue - LWheel[4].steerAngle/turnRadius * (speed/_maxHandlingSpeed + 1);
		RWheel[4].steerAngle = -_TurnInputValue - RWheel[4].steerAngle/turnRadius * (speed/_maxHandlingSpeed + 1);
	}
}
