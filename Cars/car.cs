using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class car : MonoBehaviour {

	private MultiOSControls _controls;

	public int playerNumber = 1;
	[Range(1.0f, 10.0f)] public float _powerStearing = 1.0f;
	public float turnRadius = 30f;
	public float _maxHandlingSpeed = 80f;
	public WheelCollider FLWheel;
	public WheelCollider FRWheel;
	public WheelCollider RLWheel;
	public WheelCollider RRWheel;

	[Range(0.1f, 1.0f)] public float _madness = 0.5f;
	[Range(0.1f, 0.8f)] public float _tractionControl = 0.5f;
	[Range(0, 1000)] public int _downforce = 50;
	public int _engineRedline = 7500;
	public int _engineIdle = 600;
	public AnimationCurve TorqueCurve;
	public int _brakeTorque = 500;

	[Range(0, 500)] public int _drag = 50;

	protected string _MovementAxisName;
	protected string _TurnAxisName; 
	protected Rigidbody _Rigidbody;
	public Transform _centerOfMass;
	protected float _MovementInputValue;
	protected float _TurnInputValue;

	public int _curGear = 2;
	public float[] _gears = new float[]{-3.833f, 0f, 3.833f, 2.235f, 1.458f, 1.026f};
	protected float _nextGear;
	public float _gearChangeTime = 0.5f;
	protected float _time = 0.0f;

	public enum DriveWheel { Front, Back, All }
	public DriveWheel _driveWheel;

	public float engineRPM = 600;
	public float speed = 0;
	public int _torqueMultiplier = 4;

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

		Vector3 localVelocity = transform.InverseTransformDirection(_Rigidbody.velocity);

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

		// Aply torque
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

		// Aply brake
		FLWheel.brakeTorque = _brakeTorque*_controls.getValue ("Break1");
		FRWheel.brakeTorque = _brakeTorque*_controls.getValue ("Break1");
		RLWheel.brakeTorque = _brakeTorque*_controls.getValue ("Break1");
		RRWheel.brakeTorque = _brakeTorque*_controls.getValue ("Break1");

		// Compute RPM
		switch (_driveWheel) {
		case DriveWheel.All:
			engineRPM = _torqueMultiplier * FLWheel.rpm * _gears [_curGear];
			engineRPM += _torqueMultiplier * FRWheel.rpm * _gears [_curGear];
			engineRPM += _torqueMultiplier * RLWheel.rpm * _gears [_curGear];
			engineRPM += _torqueMultiplier * RRWheel.rpm * _gears [_curGear];
			engineRPM /= 4;
			break;

		case DriveWheel.Back:
			engineRPM = _torqueMultiplier * RLWheel.rpm * _gears [_curGear];
			engineRPM += _torqueMultiplier * RRWheel.rpm * _gears [_curGear];
			engineRPM /= 2;
			break;

		case DriveWheel.Front:
			engineRPM = _torqueMultiplier * FLWheel.rpm * _gears [_curGear];
			engineRPM += _torqueMultiplier * FRWheel.rpm * _gears [_curGear];
			engineRPM /= 2;
			break;
		}

		engineRPM = Mathf.Lerp (oldEngineRPM, engineRPM, 0.4f);

		// Lock idle < rpm < redline
		if (engineRPM >= _engineRedline)
		{
			engineRPM = _engineRedline;
		} else if (engineRPM < _engineIdle) {
			engineRPM = _engineIdle;
		}

		// Car behavior and skids
		if (_controls.getValue ("Handbreak1") != 0) { // Mad mode!!!
			WheelFrictionCurve tmp = RLWheel.sidewaysFriction;
			tmp.extremumValue = 1-_madness;
			RLWheel.sidewaysFriction = tmp;
			RRWheel.sidewaysFriction = tmp;

			tmp = RLWheel.sidewaysFriction;
			tmp.extremumValue = 1+_madness;
			FLWheel.sidewaysFriction = tmp;
			FRWheel.sidewaysFriction = tmp;
		} else { // Easy mode...
			WheelFrictionCurve tmp = RLWheel.sidewaysFriction;
			tmp.extremumValue = 1;
			RLWheel.sidewaysFriction = tmp;
			RRWheel.sidewaysFriction = tmp;

			tmp = RLWheel.sidewaysFriction;
			tmp.extremumValue = 1;
			FLWheel.sidewaysFriction = tmp;
			FRWheel.sidewaysFriction = tmp;

			// Adjust torque
			switch (_driveWheel) {
			case DriveWheel.All:
				AdjustTorque (FLWheel);
				AdjustTorque (FRWheel);
				AdjustTorque (RLWheel);
				AdjustTorque (RRWheel);
				break;

			case DriveWheel.Back:
				AdjustTorque (RLWheel);
				AdjustTorque (RRWheel);
				break;

			case DriveWheel.Front:
				AdjustTorque (FLWheel);
				AdjustTorque (FRWheel);
				break;
			}
		}

		// Downforce
		_Rigidbody.AddForce(-transform.up*_downforce*_Rigidbody.velocity.magnitude);
		// Drag
		_Rigidbody.AddForce(-transform.forward*_drag*_Rigidbody.velocity.magnitude);
		// Speed
		speed = transform.InverseTransformDirection(_Rigidbody.velocity).z * 3.6f;
	}

	protected void AdjustTorque(WheelCollider wheel)
	{
		WheelHit wheelHit;
		wheel.GetGroundHit (out wheelHit);
		float forwardSlip = wheelHit.forwardSlip;

		if (Mathf.Abs(forwardSlip) >= _tractionControl)
		{
			wheel.motorTorque = Mathf.Lerp(0, wheel.motorTorque, _tractionControl);
		}
	}

	protected void Turn ()
	{
		if (_controls.getValue ("Handbreak1") != 0) { // Mad mode!!!
			FLWheel.steerAngle = _TurnInputValue * (turnRadius+20) * Mathf.Clamp ((1.0f - (Mathf.Abs (speed) / _maxHandlingSpeed)), 0.05f, 1.0f);
			FRWheel.steerAngle = _TurnInputValue * (turnRadius+20) * Mathf.Clamp ((1.0f - (Mathf.Abs (speed) / _maxHandlingSpeed)), 0.05f, 1.0f);
		} else {
			float turn = _TurnInputValue * turnRadius * Mathf.Clamp ((1.0f - (Mathf.Abs (speed) / _maxHandlingSpeed)), 0.05f, 1.0f);
			float turnSpeed = 1.0f-Mathf.Clamp (Mathf.Abs (speed) / _maxHandlingSpeed, 0.0f, 0.9f);

			float turnMultiplier = _powerStearing;
			if ((turn < 0) != (FLWheel.steerAngle < 0)) {
				turnMultiplier *= 4.0f;
			}
			if (turn == 0.0f) {
				FLWheel.steerAngle = 0;
				FRWheel.steerAngle = 0;
			} else {
				FLWheel.steerAngle = Mathf.Lerp (FLWheel.steerAngle, turn, (turnMultiplier * turnSpeed) * Time.deltaTime);
				FRWheel.steerAngle = Mathf.Lerp (FRWheel.steerAngle, turn, (turnMultiplier * turnSpeed) * Time.deltaTime);
			}
		}
	}
}
