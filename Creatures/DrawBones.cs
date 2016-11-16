using UnityEngine;

public class DrawBones : MonoBehaviour
{
  private SkinnedMeshRenderer m_Renderer;

  public Color _Color = Color.green;

  void LateUpdate()
  {
    draw(transform);
  }

  void draw (Transform t) {
    foreach (Transform child in t)
    {
      Debug.DrawLine(child.parent.position, child.position, _Color);
      draw(child);
    }
  }
}
