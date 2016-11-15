using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour {

  private Camera _Camera;

  public Transform _Target;

  [Range(0.0f, 0.5f)] public float _AngularSpeed = 0.1f;
  [Range(0.0f, 0.5f)] public float _LinearSpeed = 0.1f;

  [Range(0.0f, 5.0f)] public float _YStabilisation = 3.0f;

  public Vector3 _Offset;

  // Use this for initialization
	void Start () {
    _Camera = GetComponent<Camera> ();

    if (_Camera == null) {
      Debug.LogWarning("No camera found on " + name + " destroying script...");
      Destroy(this);
    }

    if (_Target == null) {
      Debug.LogWarning("No target found on " + name + " destroying script...");
      Destroy(this);
    }
  }

  // Update is called once per frame
	void Update () {

  }

  void FixedUpdate ()
	{
    Vector3 decal = _Offset.x * _Target.forward + _Offset.z * _Target.right + _Offset.y * Vector3.up;

    if(Mathf.Abs(_Camera.transform.position.y - (_Target.position.y + decal.y)) > 1*_YStabilisation-_LinearSpeed*_YStabilisation)
      decal.y = Mathf.Abs(decal.y);
    else {
      decal.y = _Camera.transform.position.y - _Target.position.y;
    }

    _Camera.transform.position = Vector3.Lerp(_Camera.transform.position, _Target.position + decal, _LinearSpeed);

    Vector3 forward = _Target.position - _Camera.transform.position;
    _Camera.transform.rotation = Quaternion.Lerp(_Camera.transform.rotation, Quaternion.LookRotation(forward), _AngularSpeed);
  }

}
