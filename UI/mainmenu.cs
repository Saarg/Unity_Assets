using UnityEngine;
using System.Collections;

public class mainmenu : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

  public void StartCarDemo () {
    Application.LoadLevel("Demos/CarDemo/map01");
  }

  public void StartActiveRagdoll () {
    Application.LoadLevel("Demos/livingCreatures/map02");
  }

  public void StartOptions () {

  }

  public void Quit () {
    Application.Quit();
  }
}
