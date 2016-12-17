using UnityEngine;
using System.Collections;

public class StartMenu : MonoBehaviour {

  private MultiOSControls _Controls;
  public string _MenuKey = "StartMenu";
  public string _MenuScene = "Demos/AllDemos/MainMenu";

	// Use this for initialization
	void Start () {
    // Init Control script
    _Controls = GameObject.Find ("Scripts").GetComponent<MultiOSControls> ();

    Screen.lockCursor = true;
	}

	// Update is called once per frame
	void Update () {
    if(_Controls.getValue(_MenuKey) == 1) {
      GetComponent<CanvasGroup> ().alpha = 1;
      GetComponent<CanvasGroup> ().interactable = true;
      Screen.lockCursor = false;
    }
	}

  public void Resume () {
    GetComponent<CanvasGroup> ().alpha = 0;
    GetComponent<CanvasGroup> ().interactable = false;
    Screen.lockCursor = true;
  }

  public void StartOptions () {

  }

  public void StartMainMenu () {
    Application.LoadLevel(_MenuScene);
  }

  public void Quit () {
    Application.Quit();
  }
}
