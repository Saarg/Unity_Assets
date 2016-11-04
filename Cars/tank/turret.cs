using UnityEngine;
using System.Collections;

public class turret : MonoBehaviour {

	private MultiOSControls _controls;

	private float _decalX = 0.0f;
	private float _decalY = 0.0f;

	public GameObject BarrelEnd;
	public GameObject ammo;

	protected float _nextShot;
	public float _fireRate = 0.5f;
	protected float _time = 0.0f;

	// Use this for initialization
	void Start () {
		_controls = GameObject.Find ("Scripts").GetComponent<MultiOSControls> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void FixedUpdate()
	{
		_time = _time + Time.deltaTime;
		if (_controls.getValue ("Fire1") != 0 && _time > _nextShot) {
			GameObject bullet = (GameObject)Instantiate(ammo, BarrelEnd.transform.position, transform.rotation);
			bullet.GetComponent<Rigidbody> ().AddForce(transform.forward * 4000);
			bullet.GetComponent<Rigidbody> ().velocity = transform.parent.GetComponent<Rigidbody> ().velocity;
			_nextShot = _time + _fireRate;
		}

		if (_controls.getValue ("Camera1X") != 0) {
			_decalX += Mathf.Clamp(_controls.getValue ("Camera1X"), -1.0f, 1.0f);
		}

		if (_controls.getValue ("Camera1Y") != 0) {
			_decalY -= Mathf.Clamp(_controls.getValue ("Camera1Y"), -1.0f, 1.0f);
		}
		_decalY = Mathf.Clamp (_decalY, -15.0f, 5.0f);

		Quaternion target = Quaternion.Euler(_decalY, _decalX, 0);

		transform.rotation = transform.parent.rotation * target;
	}
}
