using UnityEngine;
using System.Collections;

public class camera01 : MonoBehaviour {

	public float height = 2;
	public float distance = 5;

	private Vector3 _velocity = Vector3.zero;
	private Rigidbody _parentBody;

	// Use this for initialization
	void Start () {
		_parentBody = transform.parent.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate()
	{
		Vector3 target = transform.parent.position + transform.parent.up * height - transform.parent.GetComponent<Rigidbody> ().velocity.normalized * distance;
		if (_parentBody.velocity.magnitude < 0.01 && _parentBody.velocity.magnitude > -0.01) {
			target = transform.parent.position + transform.parent.up * height - transform.parent.forward * distance;
		}
		transform.position = Vector3.SmoothDamp(transform.position, target, ref _velocity, 0.1f);
		transform.LookAt (transform.parent);
	}
}
