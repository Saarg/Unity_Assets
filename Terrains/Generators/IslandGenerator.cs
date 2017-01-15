using UnityEngine;
using System.Collections;

public class IslandGenerator : Generator {

	public int _islandSizeX = 100;
	public int _islandSizeY = 100;
  public int _maxHeight = 100;

  public Center[, ] _centers;
  public Corner[, ] _corners;

  private Texture2D _texture;

  public void Init() {
    _centers = new Center[_islandSizeX, _islandSizeY];
    _corners = new Corner[_islandSizeX+1, _islandSizeY+1];

    _texture = new Texture2D(_islandSizeX * _chunkSize, _islandSizeY * _chunkSize);

    for (int xi = 0; xi < _islandSizeX+1; xi++) {
      _corners[xi, 0] = new Corner();
    }
    for (int yi = 0; yi < _islandSizeY+1; yi++) {
      _corners[0, yi] = new Corner();
    }

    for (int xi = 0; xi < _islandSizeX; xi++) {
      for (int yi = 0; yi < _islandSizeY; yi++) {
        _centers[xi, yi] = new Center();
        _centers[xi, yi].point = new Vector2(xi * _chunkSize, yi * _chunkSize);

        _corners[xi+1, yi+1] = new Corner();

        // Setup neighbors
        if(xi > 0)              _centers[xi, yi].neighbors.Add(_centers[xi-1, yi]);
        if(xi < _islandSizeX-1) _centers[xi, yi].neighbors.Add(_centers[xi+1, yi]);

        if(yi > 0)              _centers[xi, yi].neighbors.Add(_centers[xi, yi-1]);
        if(yi < _islandSizeY-1) _centers[xi, yi].neighbors.Add(_centers[xi, yi+1]);

        // Setup corners
        _corners[xi, yi].point = new Vector2(xi * _chunkSize, yi * _chunkSize);
        _centers[xi, yi].corners.Add(_corners[xi, yi]);

        _corners[xi+1, yi].point = new Vector2((xi+1) * _chunkSize, yi * _chunkSize);
        _centers[xi, yi].corners.Add(_corners[xi+1, yi]);

        _corners[xi+1, yi+1].point = new Vector2((xi+1) * _chunkSize, (yi+1) * _chunkSize);
        _centers[xi, yi].corners.Add(_corners[xi+1, yi+1]);

        _corners[xi+1, yi].point = new Vector2((xi+1) * _chunkSize, yi * _chunkSize);
        _centers[xi, yi].corners.Add(_corners[xi+1, yi]);

        _corners[xi, yi].adjacent.Add(_corners[xi+1, yi]);
        _corners[xi, yi].adjacent.Add(_corners[xi, yi+1]);

        _corners[xi+1, yi].adjacent.Add(_corners[xi, yi]);
        _corners[xi+1, yi].adjacent.Add(_corners[xi+1, yi+1]);

        _corners[xi+1, yi+1].adjacent.Add(_corners[xi+1, yi]);
        _corners[xi+1, yi+1].adjacent.Add(_corners[xi, yi+1]);

        _corners[xi, yi+1].adjacent.Add(_corners[xi, yi]);
        _corners[xi, yi+1].adjacent.Add(_corners[xi+1, yi+1]);

      }
    }

    GenerateHeight();
  }

  private void GenerateHeight() {
    for (int xi = 0; xi < _islandSizeX; xi++) {
      for (int yi = 0; yi < _islandSizeY; yi++) {

        for (int xj = 0; xj < Chunk.chunkSize; xj++) {
          for (int yj = 0; yj < Chunk.chunkSize; yj++) {
            float color = 0.0f;

            Vector2 pos = new Vector2(_chunkSize * xi + xj, _chunkSize * yi + yj);
            float multiplier = 1-(Vector2.Distance(pos, new Vector2(_islandSizeX/2 * _chunkSize, _islandSizeY/2 * _chunkSize)) /
                              Vector2.Distance(new Vector2(0, _islandSizeY/2 * _chunkSize), new Vector2(_islandSizeX/2 * _chunkSize, _islandSizeY/2 * _chunkSize)));
            color = Mathf.PerlinNoise(pos.x/(float)(_islandSizeX), pos.y/(float)(_islandSizeY)) * multiplier;

            _texture.SetPixel(xi * _chunkSize + xj, yi * _chunkSize + yj, new Color(color, color, color, color < 0.2f ? 0.0f : 1.0f));
          }
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
    GUI.DrawTexture(new Rect(0,0,200,200), _texture, ScaleMode.ScaleToFit);
  }

	public override ChunkData Generate(WorldPos pos) {
    if(_centers == null) {
      Init();
    }
    _continueTh = false;

		ChunkData chunkData;
		if (!chunkDatas.TryGetValue(pos, out chunkData))
		{
			chunkData = new ChunkData(_chunkSize);

			for (int xi = 0; xi < _chunkSize; xi++)
			{
				for (int zi = 0; zi < _chunkSize; zi++)
				{
          chunkData._heightMap[xi, zi] = _texture.GetPixel(pos.x*_chunkSize + xi, pos.z*_chunkSize + zi).grayscale * _maxHeight;
				}
			}
		}

		return chunkData;
	}
}
