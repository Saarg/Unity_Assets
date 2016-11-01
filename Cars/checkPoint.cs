using UnityEngine;
using System.Collections;

public class checkPoint : MonoBehaviour {

	private float _startTime = 0.0f;
	private float _checkTime = 0.0f;
	private bool _passed = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool isPassed() {
		return _passed;
	}

	public void setStart(float startTime) {
		_startTime = startTime;
	}

	public void reset(){
		_checkTime = 0.0f;
		_passed = false;
	}

	void OnTriggerEnter	(Collider other) {
		if (!_passed) {
			_checkTime = Time.time;
		}
		_passed = true;
	}
}
