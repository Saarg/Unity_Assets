using UnityEngine;
using System.Collections;

public class Modify : MonoBehaviour
{

  Vector2 rot;

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      RaycastHit hit;
      if (Physics.Raycast(transform.position, transform.forward,out hit, 100 ))
      {
        Terrain.SetBlock(hit, new BlockAir());
      }
    }

    rot= new Vector2(
      rot.x + Input.GetAxis("mouse x")/3,
      rot.y + Input.GetAxis("mouse y")/3);

    transform.localRotation = Quaternion.AngleAxis(rot.x, Vector3.up);
    transform.localRotation *= Quaternion.AngleAxis(rot.y, Vector3.left);

    transform.position += transform.forward * (Input.GetKey("z") ? 1 : 0);
    transform.position += transform.right * (Input.GetKey("q") ? 1 : 0);
    transform.position += transform.forward * (Input.GetKey("s") ? -1 : 0);
    transform.position += transform.right * (Input.GetKey("d") ? -1 : 0);
  }
}
