using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	[Range(0.0f, 99.0f)] public float _explosionRadius = 10.0f;
	[Range(0.0f, 999.0f)] public float _explosionForce = 10.0f;
	[Range(0.0f, 100.0f)] public float _explosionDelay = 10.0f;
	[Range(0.0f, 100.0f)] public float _upwardsModifier = 3.0f;
	public ParticleSystem _mainParticle;

	private float _time = 0.0f;
	private bool _boomed = false;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		_time += Time.deltaTime;
		if (_time > _explosionDelay && !_boomed) {
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, _explosionRadius);
			foreach (Collider collider in hitColliders) {
				GameObject obj = collider.gameObject;
				Rigidbody rb = collider.attachedRigidbody;

				if (rb != null) {
					//print (collider.name);
					rb.AddExplosionForce (_explosionForce*1000.0f, transform.position, _explosionRadius, _upwardsModifier);
				}
			}
			_boomed = true;
			Destroy (gameObject, _mainParticle.duration);
		}
	}
}
