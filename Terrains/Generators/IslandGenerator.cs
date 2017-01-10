using UnityEngine;
using System.Collections;

public class IslandGenerator : Generator {

	public int _islandSizeX = 10;
	public int _islandSizeY = 10;

	public IslandGenerator() {
		Debug.Log ("Init island generator");
	}

	public override ChunkData Generate(WorldPos pos) {
		ChunkData chunkData;
		if (!chunkDatas.TryGetValue(pos, out chunkData))
		{
			chunkData = new ChunkData(_chunkSize);

			for (int xi = 0; xi < _chunkSize; xi++)
			{
				for (int zi = 0; zi < _chunkSize; zi++)
				{
					chunkData._heightMap[xi, zi] = Mathf.PerlinNoise((pos.x*_chunkSize + xi)/60.0f, (pos.z*_chunkSize + zi)/60.0f)*20.0f;
					chunkData._heightMap [xi, zi] += 800 - Mathf.Sqrt (_islandSizeX * _islandSizeX + _islandSizeY * _islandSizeY);
				}
			}
		}

		return chunkData;
	}
}
