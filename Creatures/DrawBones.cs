using UnityEngine;

public class DrawBones : MonoBehaviour
{
  private SkinnedMeshRenderer m_Renderer;

  void Start()
  {

  }

  void LateUpdate()
  {
    draw(transform);
  }

  void draw (Transform t) {
    foreach (Transform child in t)
    {
      Debug.Log(child.name);
      //child is your child transform
      Debug.DrawLine(child.parent.position, child.position, Color.red);
      draw(child);
    }
  }
}
