using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class car : MonoBehaviour {

	public int playerNumber = 1;
	public float turnRadius = 30f;
	public float _maxHandlingSpeed = 80f;
	public WheelCollider FLWheel;
	public WheelCollider FRWheel;
	public WheelCollider RLWheel;
	public WheelCollider RRWheel;
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

	public enum DriveWheel { Front, Back, All }
	public DriveWheel _driveWheel;

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

		if (_driveWheel == DriveWheel.Front) {
			FLWheel.motorTorque = torque/2;
			FRWheel.motorTorque = torque/2;
		}
		if (_driveWheel == DriveWheel.Back) {
			RLWheel.motorTorque = torque/2;
			RRWheel.motorTorque = torque/2;
		}
		if (_driveWheel == DriveWheel.All) {
			FLWheel.motorTorque = torque/4;
			FRWheel.motorTorque = torque/4;
			RLWheel.motorTorque = torque/4;
			RRWheel.motorTorque = torque/4;
		}


		FLWheel.brakeTorque = _brakeTorque*Input.GetAxis ("Break1");
		FRWheel.brakeTorque = _brakeTorque*Input.GetAxis ("Break1");
		RLWheel.brakeTorque = _brakeTorque*Input.GetAxis ("Break1");
		RRWheel.brakeTorque = _brakeTorque*Input.GetAxis ("Break1");

		speed = 2 * RLWheel.radius * Mathf.PI * RLWheel.rpm * 60f / 1000f;
		engineRPM = _torqueMultiplier * RLWheel.rpm * _gears [_curGear];
	}


	private void Turn ()
	{
		FLWheel.steerAngle += _TurnInputValue - FLWheel.steerAngle/turnRadius * (speed/_maxHandlingSpeed + 1);
		FRWheel.steerAngle += _TurnInputValue - FLWheel.steerAngle/turnRadius * (speed/_maxHandlingSpeed + 1);


	}
}
