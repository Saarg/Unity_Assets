using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartMenu : MonoBehaviour {

  private MultiOSControls _Controls;
  public string _MenuKey = "StartMenu";
  public string _MenuScene = "Demos/AllDemos/MainMenu";

	// Use this for initialization
	void Start () {
    // Init Control script
    _Controls = GameObject.Find ("Scripts").GetComponent<MultiOSControls> ();

    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
	}

	// Update is called once per frame
	void Update () {
    if(_Controls.getValue(_MenuKey) == 1) {
      GetComponent<CanvasGroup> ().alpha = 1;
      GetComponent<CanvasGroup> ().interactable = true;
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
    }
	}

  public void Resume () {
    GetComponent<CanvasGroup> ().alpha = 0;
    GetComponent<CanvasGroup> ().interactable = false;
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  public void StartOptions () {

  }

  public void StartMainMenu () {
    SceneManager.LoadScene (_MenuScene, LoadSceneMode.Single);
  }

  public void Quit () {
    Application.Quit();
  }
}
