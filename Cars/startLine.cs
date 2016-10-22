using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class startLine : MonoBehaviour {

	public float _startTime = 0.0f;

	public int _lapNumer = 1; // 0 if it's not a loop
	public int _lapDone = 0;

	public checkPoint[] _checkpoints;
	public float _current = 0.0f;
	public float _last = 0.0f;

	public bool _started = false;

	public Text _currentText;
	public Text _lastText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (_started) {
			_current = Time.time - _startTime;
		}

		_currentText.text = "Current time: " + _current + "s";
		_lastText.text = "Last time: " + _last + "s";
	}

	void OnTriggerEnter	(Collider other) {
		if (!_started) {
			_startTime = Time.time;
			_started = true;
		} else {
			bool validLap = true;

			foreach (checkPoint point in _checkpoints) {
				if (!point.isPassed ()) {
					validLap = false;
				}
			}

			if (validLap) {
				_last = Time.time - _startTime;
				_startTime = Time.time;

				if (_lapDone < _lapNumer) {
					_lapDone++;
				} else {
					restart ();
				}
			}
		}
	}

	public void restart() {
		_started = false;
	}
}
