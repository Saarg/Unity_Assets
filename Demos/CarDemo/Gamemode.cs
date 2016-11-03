using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Gamemode : MonoBehaviour {

	private MultiOSControls _controls;

	public GameObject[] _cars = {};
	public float _carChangeTime = 0.5f;
	private float _nextTime = 0.0f;
	private float _time = 0.0f;
	private int _curCar = 0;

	public Text _speedo;
	public Text _gear;
	public Text _rpmGage;
	public Text _name;

	// Use this for initialization
	void Start () {
		_controls = GameObject.Find ("Scripts").GetComponent<MultiOSControls> ();

		foreach (GameObject c in _cars) {
			disable(c);
		}
		enable (_cars [_curCar]);
	}

	// Update is called once per frame
	void Update () {
		_time = _time + Time.deltaTime;
		if (_controls.getValue ("SwitchCar1") != 0 && _time > _nextTime) {
			_nextTime = _time + _carChangeTime;

			disable(_cars [_curCar]);

			if (_controls.getValue ("SwitchCar1") < 0) {
				_curCar--;
				if (_curCar < 0) {
					_curCar = _cars.Length - 1;
				}
			} else if (_controls.getValue ("SwitchCar1") > 0) {
				_curCar++;
				if (_curCar > _cars.Length-1) {
					_curCar = 0;
				}
			}

			enable (_cars [_curCar]);
		}
	}

	void enable(GameObject o) {
		o.GetComponent<Rigidbody> ().isKinematic = false;
		o.GetComponent<MonoBehaviour> ().enabled = true;
		o.GetComponentInChildren<Camera> ().enabled = true;

		car c = o.GetComponent<MonoBehaviour> () as car;
		if (c != null) {
			c._speedo = _speedo;
			c._gear = _gear;
			c._rpmGage = _rpmGage;
		}
		tank t = o.GetComponent<MonoBehaviour> () as tank;
		if (t != null) {
			t._speedo = _speedo;
			t._gear = _gear;
			t._rpmGage = _rpmGage;
		}

		_name.text = o.name;
	}

	void disable(GameObject o) {
		//o.GetComponent<Rigidbody> ().isKinematic = true;
		o.GetComponent<MonoBehaviour> ().enabled = false;
		o.GetComponentInChildren<Camera> ().enabled = false;

		car c = o.GetComponent<MonoBehaviour> () as car;
		if (c != null) {
			c._speedo = null;
			c._gear = null;
			c._rpmGage = null;
		}
		tank t = o.GetComponent<MonoBehaviour> () as tank;
		if (t != null) {
			t._speedo = null;
			t._gear = null;
			t._rpmGage = null;
		}
	}
}
