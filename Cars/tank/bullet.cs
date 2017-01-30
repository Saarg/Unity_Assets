using UnityEngine;
using System.Collections;

public class bullet : MonoBehaviour {

	public GameObject _explosion;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	void OnCollisionEnter (Collision collision) {
		Instantiate(_explosion, transform.position, transform.rotation);
		Destroy (gameObject);
	}
}
