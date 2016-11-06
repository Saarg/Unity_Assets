using UnityEngine;
using System.Collections;

public class Panel : MonoBehaviour {

	public TextMesh[] _timeDisplays;

	private float[] _times;

	// Use this for initialization
	void Start () {
		_times = new float[_timeDisplays.Length];

		for (int i = 0 ; i < _times.Length ; i++) {
			_timeDisplays [i].text = (i+1) + ": " + _times [i] + "s";
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void addTime(float t) {
		float tmpLast = -1.0f;
		for (int i = 0 ; i < _times.Length ; i++) {
			if (t < _times[i] || _times[i] == 0.0f) {
				float tmp = tmpLast;
				tmpLast = _times[i];
				if (tmp > 0) {
					_times[i] = tmp;
				} else {
					_times[i] = t;
				}
			}

			_timeDisplays [i].text = (i+1) + ": " + _times [i] + "s";
		}
	}
}
