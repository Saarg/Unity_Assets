using UnityEngine;
using System.Collections;

public class Modify : MonoBehaviour
{
  private MultiOSControls _controls;

  private Vector2 rot;

  void Start() {
    _controls = GameObject.Find ("Scripts").GetComponent<MultiOSControls> ();
  }

  void Update() {
    if (_controls.getValue("Destroy1") != 0.0f)
    {
      RaycastHit hit;
      if (Physics.Raycast(transform.position, transform.forward,out hit, 100 ))
      {
        Terrain.SetBlock(hit, new BlockAir());
      }
    }

    if (_controls.getValue("Add1") != 0.0f)
    {
      RaycastHit hit;
      if (Physics.Raycast(transform.position, transform.forward,out hit, 100 ))
      {
        Terrain.SetBlock(hit, new Block(), true);
      }
    }

    rot= new Vector2(
      rot.x + _controls.getValue("Horizontal1") * 60 * Time.deltaTime,
      rot.y + _controls.getValue("Vertical1") * 40 * Time.deltaTime
    );

    transform.parent.localRotation = Quaternion.AngleAxis(rot.x, Vector3.up);
    transform.localRotation = Quaternion.AngleAxis(rot.y, Vector3.left);

    transform.parent.position += transform.parent.forward * -_controls.getValue("Forward1") * 8 * Time.deltaTime;
    transform.parent.position += transform.parent.right * _controls.getValue("Sideway1") * 8 * Time.deltaTime;
  }
}
