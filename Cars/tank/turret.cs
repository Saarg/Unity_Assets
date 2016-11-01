using UnityEngine;
using System.Collections;

public class turret : MonoBehaviour {

	private MultiOSControls _controls;

	private float _decalX = 0.0f;
	private float _decalY = 0.0f;

	// Use this for initialization
	void Start () {
		_controls = GameObject.Find ("Scripts").GetComponent<MultiOSControls> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void FixedUpdate()
	{
		if (_controls.getValue ("Camera1X") != 0) {
			_decalX += _controls.getValue ("Camera1X");
		}

		if (_controls.getValue ("Camera1Y") != 0) {
			_decalY -= _controls.getValue ("Camera1Y");
		}
		_decalY = Mathf.Clamp (_decalY, -15.0f, 5.0f);

		Quaternion target = Quaternion.Euler(_decalY, _decalX, 0);

		transform.rotation = transform.parent.rotation * target;
	}
}
