﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IslandGenerator : Generator {

	public int _islandSizeX = 100;
	public int _islandSizeY = 100;
  public int _maxHeight = 100;

  private Texture2D _texture;
  private Queue<WorldPos> _mapQueue = new Queue<WorldPos>();
  public bool _DisplayMap = true;

  void OnGUI() {
    if (_DisplayMap) {
      if (_mapQueue.Count > 0) {
        if (!_texture) {
          _texture = new Texture2D(_islandSizeX * _chunkSize/2, _islandSizeY * _chunkSize/2);
        }

        WorldPos chunkPos = _mapQueue.Dequeue();
        ChunkData chunkData;
        if (chunkDatas.TryGetValue(chunkPos, out chunkData))
        {
          for (int xi = 0; xi < 10; xi++)
          {
            for (int yi = 0; yi < 10; yi++)
            {
              float color = chunkData._heightMap[(xi / 10) * _chunkSize, (yi / 10) * _chunkSize]/_maxHeight;
              _texture.SetPixel(chunkPos.x * 10 + xi, chunkPos.z * 10 + yi, new Color(color, color, color, color < 0.2f ? 0.0f : 1.0f));
            }
          }

          _texture.Apply();
        }
      }

      GUI.DrawTexture(new Rect(0,0,200,200), _texture, ScaleMode.ScaleToFit);
    }
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

    _mapQueue.Enqueue(chunkPos);
		return chunkData;
	}
}
