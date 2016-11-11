using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour {

  private Rigidbody _Pelvis;
  public Rigidbody _Spine;

	// Use this for initialization
	void Start () {
    _Pelvis = GetComponent<Rigidbody> ();
	}

	// Update is called once per frame
	void Update () {

	}

  void OnTriggerEnter	(Collider other) {
    if (other.name == "Terrain") {
      _Pelvis.constraints  |= RigidbodyConstraints.FreezePositionY;
      _Spine.constraints  |= _Pelvis.constraints;
    }
  }
}
