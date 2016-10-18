using UnityEngine;
using System.Collections;

public enum XboxControllerAxis { Left_X, Left_Y, Right_X, Right_Y, Left_Trigger, Right_Trigger, Cross_X, Cross_Y }
public enum XboxControllerButtons { A, B, X, Y, Left_Bumper, Right_Bumper, Back, Start, Left_Stick, Right_Stick, Up, Down, Left, Right }

[System.Serializable]
public struct InputDefinition {
	public string name;
	public string[] posKeys;
	public string[] negKeys;
	public XboxControllerAxis[] axis;
	public XboxControllerButtons[] buttons;
	public float deadzone;
	public float value;
}

public class MultiOSControls : MonoBehaviour {

	public bool _linuxEditor;
	public InputDefinition[] _inputs = new InputDefinition[]{};

	private string[] WindowsAxisNames = new string[]{ "joystick1 axis x", "joystick1 axis y", "joystick1 axis 4", "joystick1 axis 5", "joystick1 axis 9", "joystick1 axis 10", "joystick1 axis 6", "joystick1 axis 7"};
	private KeyCode[] WindowsButtonsCode = new KeyCode[]{ KeyCode.Joystick1Button0, KeyCode.Joystick1Button1, KeyCode.Joystick1Button2, KeyCode.Joystick1Button3, KeyCode.Joystick1Button4, 
														  KeyCode.Joystick1Button5, KeyCode.Joystick1Button6, KeyCode.Joystick1Button7, KeyCode.Joystick1Button8, KeyCode.Joystick1Button9, 
														  KeyCode.Joystick1Button13, KeyCode.Joystick1Button14, KeyCode.Joystick1Button11, KeyCode.Joystick1Button12};// no D-Pad on windows
	// No Dpad axis on mac
	private string[] MacAxisNames = new string[]{ "joystick1 axis x", "joystick1 axis y", "joystick1 axis 3", "joystick1 axis 4", "joystick1 axis 5", "joystick1 axis 6", "joystick1 axis 5", "joystick1 axis 6"};
	private KeyCode[] MacButtonsCode = new KeyCode[]{ KeyCode.Joystick1Button16, KeyCode.Joystick1Button17, KeyCode.Joystick1Button18, KeyCode.Joystick1Button19, KeyCode.Joystick1Button13, 
													  KeyCode.Joystick1Button14, KeyCode.Joystick1Button10, KeyCode.Joystick1Button9, KeyCode.Joystick1Button11, KeyCode.Joystick1Button12, 
													  KeyCode.Joystick1Button5, KeyCode.Joystick1Button6, KeyCode.Joystick1Button7, KeyCode.Joystick1Button8};

	private string[] LinuxAxisNames = new string[]{ "joystick1 axis x", "joystick1 axis y", "joystick1 axis 4", "joystick1 axis 5", "joystick1 axis 3", "joystick1 axis 6", "joystick1 axis 7", "joystick1 axis 8"};
	private KeyCode[] LinuxButtonsCode = new KeyCode[]{ KeyCode.Joystick1Button0, KeyCode.Joystick1Button1, KeyCode.Joystick1Button2, KeyCode.Joystick1Button3, KeyCode.Joystick1Button4, 
														KeyCode.Joystick1Button5, KeyCode.Joystick1Button6, KeyCode.Joystick1Button7, KeyCode.Joystick1Button9, KeyCode.Joystick1Button10, 
														KeyCode.Joystick1Button13, KeyCode.Joystick1Button14, KeyCode.Joystick1Button11, KeyCode.Joystick1Button12};

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0 ; i < _inputs.Length ; i++) {
			_inputs[i].value = 0;

			// get keyboad values
			foreach (string key in _inputs[i].posKeys) {
				if (Input.GetKey (key)) {
					_inputs [i].value = 1;
				}
			}
			foreach (string key in _inputs[i].negKeys) {
				if (Input.GetKey (key)) {
					_inputs [i].value = -1;
				}
			}

			if (!_linuxEditor) {
				// Manage controller depending on the os (you should use linux man!)
				#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
				// get controller axis values
				foreach (XboxControllerAxis axis in _inputs[i].axis) {
					if (Input.GetAxis (WindowsAxisNames [(int)axis]) > _inputs [i].deadzone || Input.GetAxis (WindowsAxisNames [(int)axis]) < -_inputs [i].deadzone) {
						_inputs [i].value = Input.GetAxis (WindowsAxisNames [(int)axis]);
					}
				}

				// get controller buttons values
				foreach (XboxControllerButtons button in _inputs[i].buttons) {
					if (Input.GetKey (WindowsButtonsCode [(int)button])) {
						_inputs [i].value = 1;
					}
				}
				#elif (UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX)
				// get controller axis values
				foreach (XboxControllerAxis axis in _inputs[i].axis) {
					if (Input.GetAxis (MacAxisNames [(int)axis]) > _inputs [i].deadzone || Input.GetAxis (MacAxisNames [(int)axis]) < -_inputs [i].deadzone) {
						_inputs [i].value = Input.GetAxis (MacAxisNames [(int)axis]);
					}
				}

				// get controller buttons values
				foreach (XboxControllerButtons button in _inputs[i].buttons) {
					if (Input.GetKey (MacButtonsCode [(int)button])) {
						_inputs [i].value = 1;
					}
				}
				#else
				// get controller axis values
				foreach (XboxControllerAxis axis in _inputs[i].axis) {
					if (Input.GetAxis (LinuxAxisNames [(int)axis]) > _inputs [i].deadzone || Input.GetAxis (LinuxAxisNames [(int)axis]) < -_inputs [i].deadzone) {
						_inputs [i].value = Input.GetAxis (LinuxAxisNames [(int)axis]);
					}
				}

				// get controller buttons values
				foreach (XboxControllerButtons button in _inputs[i].buttons) {
					if (Input.GetKey (LinuxButtonsCode [(int)button])) {
						_inputs [i].value = 1;
					}
				}
				#endif
			} else {
				// get controller axis values
				foreach (XboxControllerAxis axis in _inputs[i].axis) {
					if (Input.GetAxis (LinuxAxisNames [(int)axis]) > _inputs [i].deadzone || Input.GetAxis (LinuxAxisNames [(int)axis]) < -_inputs [i].deadzone) {
						_inputs [i].value = Input.GetAxis (LinuxAxisNames [(int)axis]);
					}
				}

				// get controller buttons values
				foreach (XboxControllerButtons button in _inputs[i].buttons) {
					if (Input.GetKey (LinuxButtonsCode [(int)button])) {
						_inputs [i].value = 1;
					}
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
