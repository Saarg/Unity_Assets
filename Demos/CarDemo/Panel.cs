using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Panel : MonoBehaviour {

	public TextMesh[] _timeDisplays;

	private float[] _times;

	// Use this for initialization
	void Start () {
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

	void Awake() {
		if (File.Exists (Application.persistentDataPath + "/" + transform.parent.name + ".dat")) {
			print ("loading /" + transform.parent.name + ".dat"); 
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/" + transform.parent.name + ".dat", FileMode.Open);

			_times = (float[])(bf.Deserialize (file));

		} else {
			print ("No save found...");
			_times = new float[_timeDisplays.Length];
		}
	}

	void OnDestroy() {
		print ("saving to /" + transform.parent.name + ".dat"); 
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/" + transform.parent.name + ".dat");

		bf.Serialize(file, _times);
		file.Close();
	}
}
