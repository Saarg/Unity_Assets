using UnityEngine;
using System.Collections;

public class suspension : MonoBehaviour {

	public GameObject _wheelModel;
	private WheelCollider _wheelCollider;

	// Use this for initialization
	void Start () {
		_wheelCollider = GetComponent<WheelCollider> ();

		Vector3 scale = new Vector3 (0, 0, 0);
		scale.x = _wheelCollider.radius * 2;
		scale.y = _wheelModel.transform.localScale.y;
		scale.z = _wheelCollider.radius * 2;
		_wheelModel.transform.localScale = scale;

		_wheelModel.transform.Rotate (Vector3.forward * 90);
	}
	
	// Update is called once per frame
	void Update () {
		if (_wheelModel && _wheelCollider) {
			Vector3 pos = new Vector3(0, 0, 0);
			Quaternion quat = new Quaternion();
			_wheelCollider.GetWorldPose (out pos, out quat);

			_wheelModel.transform.rotation = quat;
			_wheelModel.transform.Rotate (Vector3.forward * 90);
			_wheelModel.transform.position = pos;
		}
	}
}
