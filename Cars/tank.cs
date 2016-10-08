using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class tank : MonoBehaviour {

	public int playerNumber = 1;
	public float turnRadius = 30f;
	public float _maxHandlingSpeed = 80f;
	public WheelCollider[] LWheel;
	public WheelCollider[] RWheel;

	public int _engineRedline = 7500;
	public int _engineIdle = 600;
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

		// Downforce
		_Rigidbody.AddForce(-transform.up*100*_Rigidbody.velocity.magnitude);


		WheelHit wheelHit;
		foreach (WheelCollider wheel in RWheel) {
			wheel.GetGroundHit(out wheelHit);
			AdjustTorque(wheelHit.forwardSlip);
		}
		foreach (WheelCollider wheel in LWheel) {
			wheel.GetGroundHit(out wheelHit);
			AdjustTorque(wheelHit.forwardSlip);
		}

		speed = 2 * RWheel[3].radius * Mathf.PI * RWheel[3].rpm * 60f / 1000f;
	}

	private void AdjustTorque(float forwardSlip)
	{
		if (forwardSlip >= 0.3f && engineRPM >= _engineIdle)
		{
			engineRPM = Mathf.Lerp(_engineIdle, engineRPM, 0.5f);
		}
		else
		{
			int groudedCount = 0;
			float avgRPM = 0;
			foreach (WheelCollider wheel in RWheel) {
				if (wheel.isGrounded) {
					groudedCount++;
					avgRPM += wheel.rpm;
				}
			}
			foreach (WheelCollider wheel in LWheel) {
				if (wheel.isGrounded) {
					groudedCount++;
					avgRPM += wheel.rpm;
				}
			}
			avgRPM = avgRPM / groudedCount;

			engineRPM = _torqueMultiplier * avgRPM * _gears [_curGear];
			if (engineRPM > _engineRedline) {
				engineRPM = _engineRedline;
			} else if (engineRPM > _engineIdle) {
				//engineRPM = _engineIdle;
			} else {
				engineRPM = _engineIdle;
			}
		}
	}

	private void Turn ()
	{
		LWheel[1].steerAngle = _TurnInputValue*turnRadius;
		RWheel[1].steerAngle = _TurnInputValue*turnRadius;
	}
}
