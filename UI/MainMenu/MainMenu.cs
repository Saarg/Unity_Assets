using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public struct MenuScene {
	public string name;
	public string path;
  [HideInInspector]public GameObject button;
  public void StartScene () {
    Application.LoadLevel(path);
  }
}

public class MainMenu : MonoBehaviour {

  public GameObject _Canvas;
  public GameObject _SceneButton;
  public MenuScene[] _Scenes;

	// Use this for initialization
	void Start () {
    for (int i = 0; i < _Scenes.Length; i++) {
      _Scenes[i].button = Instantiate(_SceneButton) as GameObject;
      _Scenes[i].button.transform.SetParent(_Canvas.transform, false);

      _Scenes[i].button.name = _Scenes[i].name;
      foreach (Transform child in _Scenes[i].button.transform) {
        if (child.name == "Text"){
          child.GetComponent<Text>().text = _Scenes[i].name;
        }
      }
      _Scenes[i].button.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(100, 30*(_Scenes.Length-1) + 15 - 30*i, 0);

      _Scenes[i].button.GetComponent<Button>().onClick.AddListener(_Scenes[i].StartScene);
    }
	}

	// Update is called once per frame
	void Update () {

	}

  public void StartOptions () {

  }

  public void Quit () {
    Application.Quit();
  }
}
