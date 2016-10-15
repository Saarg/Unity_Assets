using UnityEngine;
using System.Collections;

public enum XboxControllerAxis { Left_X, Left_Y, Right_X, Right_Y, Left_Trigger, Right_Trigger, Cross_X, Cross_Y }
public enum XboxControllerButtons { A, B, X, Y, Left_Bumper, Right_Bumper, Back, Select, Left_Stick, Right_Stick, Up, Down, Left, Right }

[System.Serializable]
public struct InputDefinition {
	public string name;
	public string[] keys;
	public XboxControllerAxis[] axis;
	public XboxControllerButtons[] buttons;
	public float value;
}

public class MultiOSControls : MonoBehaviour {

	public float _deadzone = 0.1f;
	public InputDefinition[] _inputs = new InputDefinition[]{};

	private string[] LinuxAxisNames = new string[]{ "joystick1 axis x", "joystick1 axis y", "joystick1 axis 4", "joystick1 axis 5", "joystick1 axis 3", "joystick1 axis 6", "joystick1 axis 7", "joystick1 axis 8"};
	private KeyCode[] LinuxButtonsCode = new KeyCode[]{ KeyCode.Joystick1Button0, KeyCode.Joystick1Button1, KeyCode.Joystick1Button2, KeyCode.Joystick1Button3, KeyCode.Joystick1Button4, 
												KeyCode.Joystick1Button5, KeyCode.Joystick1Button6, KeyCode.Joystick1Button7, KeyCode.Joystick1Button8, 
												KeyCode.Joystick1Button9, KeyCode.Joystick1Button10, KeyCode.Joystick1Button11, KeyCode.Joystick1Button10};

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0 ; i < _inputs.Length ; i++) {
			_inputs[i].value = 0;

			// get keyboad values
			foreach (string key in _inputs[i].keys) {
				if (Input.GetKey (key)) {
					_inputs [i].value = 1;
				}
			}

			// get controller axis values
			foreach (XboxControllerAxis axis in _inputs[i].axis) {
				if (Input.GetAxis (LinuxAxisNames[(int)axis]) > _deadzone) {
					_inputs [i].value = Input.GetAxis (LinuxAxisNames[(int)axis]);
				}
			}

			// get controller buttons values
			foreach (XboxControllerButtons button in _inputs[i].buttons) {
				if(Input.GetKey (LinuxButtonsCode[(int)button])) {
					_inputs [i].value = 1;
				}
			}
		}
	}

	public float getValue(string name) {
		foreach (InputDefinition i in _inputs) {
			if (i.name == name) {
				return i.value;
			}
		}
		return 0;
	}
}
