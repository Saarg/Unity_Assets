using UnityEngine;
using System.Collections;

public class IslandGenerator : Generator {

	public int _islandSizeX = 100;
	public int _islandSizeY = 100;
  public int _maxHeight = 100;

  private Texture2D _texture;

  void OnGUI() {
    if (!_texture) {
      _texture = new Texture2D(_islandSizeX * _chunkSize, _islandSizeY * _chunkSize);
    }

    foreach(var chunkData in chunkDatas)
    {
      for (int xi = 0; xi < _chunkSize; xi++)
			{
				for (int yi = 0; yi < _chunkSize; yi++)
				{
          _texture.SetPixel(chunkData.Key.x * _chunkSize + xi, chunkData.Key.z * _chunkSize + yi, new Color(chunkData.Value._heightMap[xi, yi]/_maxHeight, chunkData.Value._heightMap[xi, yi]/_maxHeight, chunkData.Value._heightMap[xi, yi]/_maxHeight, chunkData.Value._heightMap[xi, yi]/_maxHeight < 0.2f ? 0.0f : 1.0f));
        }
      }
    }

    _texture.Apply();
    GUI.DrawTexture(new Rect(0,10,200,200), _texture, ScaleMode.ScaleToFit);
  }

	public override ChunkData Generate(WorldPos chunkPos) {
		ChunkData chunkData;
		if (!chunkDatas.TryGetValue(chunkPos, out chunkData))
		{
			chunkData = new ChunkData(_chunkSize);

			for (int xi = 0; xi < _chunkSize; xi++)
			{
				for (int zi = 0; zi < _chunkSize; zi++)
				{
          Vector2 pos = new Vector2(_chunkSize * chunkPos.x + xi, _chunkSize * chunkPos.z + zi);
          float multiplier = 1-(Vector2.Distance(pos, new Vector2(_islandSizeX/2 * _chunkSize, _islandSizeY/2 * _chunkSize)) /
                            Vector2.Distance(new Vector2(0, _islandSizeY/2 * _chunkSize), new Vector2(_islandSizeX/2 * _chunkSize, _islandSizeY/2 * _chunkSize)));
          float color = Mathf.PerlinNoise(pos.x/(float)(_islandSizeX), pos.y/(float)(_islandSizeY)) * multiplier;

          chunkData._heightMap[xi, zi] = color * _maxHeight;
				}
			}
		}

		return chunkData;
	}
}
