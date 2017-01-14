using UnityEngine;
using System.Collections;

public class IslandGenerator : Generator {

	public static int _islandSizeX = 100;
	public static int _islandSizeY = 100;

  public Center[, ] _centers;
  public Corner[, ] _corners;

  private Texture2D _texture;

  public void Init() {
    _centers = new Center[_islandSizeX, _islandSizeY];
    _corners = new Corner[_islandSizeX+1, _islandSizeY+1];

    _texture = new Texture2D(_islandSizeX, _islandSizeY);

    for (int xi = 0; xi < _islandSizeX+1; xi++) {
      _corners[xi, 0] = new Corner();
    }
    for (int yi = 0; yi < _islandSizeY+1; yi++) {
      _corners[0, yi] = new Corner();
    }

    for (int xi = 0; xi < _islandSizeX; xi++) {
      for (int yi = 0; yi < _islandSizeY; yi++) {
        _centers[xi, yi] = new Center();
        _centers[xi, yi].point = new Vector2(xi, yi);

        _corners[xi+1, yi+1] = new Corner();

        // Setup neighbors
        if(xi > 0)              _centers[xi, yi].neighbors.Add(_centers[xi-1, yi]);
        if(xi < _islandSizeX-1) _centers[xi, yi].neighbors.Add(_centers[xi+1, yi]);

        if(yi > 0)              _centers[xi, yi].neighbors.Add(_centers[xi, yi-1]);
        if(yi < _islandSizeY-1) _centers[xi, yi].neighbors.Add(_centers[xi, yi+1]);

        // Setup corners
        _corners[xi, yi].point = new Vector2(xi, yi);
        _centers[xi, yi].corners.Add(_corners[xi, yi]);

        _corners[xi, yi].point = new Vector2(xi+1, yi);
        _centers[xi, yi].corners.Add(_corners[xi+1, yi]);

        _corners[xi, yi].point = new Vector2(xi+1, yi+1);
        _centers[xi, yi].corners.Add(_corners[xi+1, yi+1]);

        _corners[xi, yi].point = new Vector2(xi+1, yi);
        _centers[xi, yi].corners.Add(_corners[xi+1, yi]);

        _corners[xi, yi].adjacent.Add(_corners[xi+1, yi]);
        _corners[xi, yi].adjacent.Add(_corners[xi, yi+1]);

        _corners[xi+1, yi].adjacent.Add(_corners[xi, yi]);
        _corners[xi+1, yi].adjacent.Add(_corners[xi+1, yi+1]);

        _corners[xi+1, yi+1].adjacent.Add(_corners[xi+1, yi]);
        _corners[xi+1, yi+1].adjacent.Add(_corners[xi, yi+1]);

        _corners[xi, yi+1].adjacent.Add(_corners[xi, yi]);
        _corners[xi, yi+1].adjacent.Add(_corners[xi+1, yi+1]);

        float multiplier = 1-(Vector2.Distance(_centers[xi, yi].point, new Vector2(_islandSizeX/2, _islandSizeY/2)) /
                           Vector2.Distance(new Vector2(0, _islandSizeY/2), new Vector2(_islandSizeX/2, _islandSizeY/2)));
        _centers[xi, yi].elevation = Mathf.PerlinNoise(10 * (xi/(float)(_islandSizeX)), 10 * (yi/(float)(_islandSizeY))) * multiplier;

        if(_centers[xi, yi].elevation < 0.1f) {
          _centers[xi, yi].water = true;
          _texture.SetPixel(xi, yi, Color.blue);
        } else {
          _centers[xi, yi].water = false;
          _texture.SetPixel(xi, yi, new Color(_centers[xi, yi].elevation, _centers[xi, yi].elevation, _centers[xi, yi].elevation, 1));
        }

      }
    }
  }

  void OnGUI() {
    if (!_texture) {
      Debug.LogError("Assign a Texture in the inspector.");
      return;
    }
    _texture.Apply();
    GUI.DrawTexture(new Rect(0,0,_islandSizeX,_islandSizeY), _texture);
  }

	public override ChunkData Generate(WorldPos pos) {
    if(_centers == null) {
      Init();
    }

		ChunkData chunkData;
		if (!chunkDatas.TryGetValue(pos, out chunkData))
		{
			chunkData = new ChunkData(_chunkSize);

			for (int xi = 0; xi < _chunkSize; xi++)
			{
				for (int zi = 0; zi < _chunkSize; zi++)
				{
					chunkData._heightMap[xi, zi] = Mathf.PerlinNoise((pos.x*_chunkSize + xi)/60.0f, (pos.z*_chunkSize + zi)/60.0f)*20.0f;
				}
			}
		}

		return chunkData;
	}
}
