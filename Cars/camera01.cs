using UnityEngine;
using System.Collections;

public class camera01 : MonoBehaviour {

	private MultiOSControls _controls;

	public float height = 2;
	public float distance = 5;

	private Vector3 _velocity = Vector3.zero;
	private Rigidbody _parentBody;

	private float _decalX = 0.0f;
	private float _decalY = 0.0f;

	// Use this for initialization
	void Start () {
		_controls = GameObject.Find ("Scripts").GetComponent<MultiOSControls> ();
		_parentBody = transform.parent.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate()
	{
		if (_controls.getValue ("Camera1X") != 0) {
			_decalX -= Mathf.Clamp(_controls.getValue ("Camera1X"), -1.0f, 1.0f);
		} else {
			_decalX = 0;
		}

		if (_controls.getValue ("Camera1Y") != 0) {
			_decalY -= Mathf.Clamp(_controls.getValue ("Camera1Y"), -1.0f, 1.0f);
		} else {
			_decalY = 0;
		}
		_decalY = Mathf.Clamp (_decalY, -15.0f, 180.0f);

		Vector3 target = transform.parent.position + transform.parent.up * height;
		target -= Quaternion.AngleAxis(_decalX, transform.parent.up) * Quaternion.AngleAxis(_decalY, transform.parent.right) * transform.parent.GetComponent<Rigidbody> ().velocity.normalized * distance;

		if (_parentBody.velocity.magnitude < 0.2 && _parentBody.velocity.magnitude > -0.2) {
			target = transform.parent.position + transform.parent.up * height;
			target -= Quaternion.AngleAxis(_decalX, transform.parent.up) * Quaternion.AngleAxis(_decalY, transform.parent.right) * transform.parent.forward * distance;
		}

		//target = Quaternion.AngleAxis(_decalX, transform.parent.up) * target;

		transform.position = Vector3.SmoothDamp(transform.position, target, ref _velocity, 0.3f);
		transform.LookAt (transform.parent);
	}
}
