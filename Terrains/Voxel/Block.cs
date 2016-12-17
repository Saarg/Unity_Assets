using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class Block {
  public enum Direction { north, east, south, west, up, down };
  public struct Tile { public int x; public int y;}

  public bool changed = true;
  private const float tileSize = 0.25f;

  [NonSerialized]
  private Vector3[] topVertices = new Vector3[4] {
    new Vector3(- 0.5f, + 0.5f, + 0.5f),
    new Vector3(+ 0.5f, + 0.5f, + 0.5f),
    new Vector3(+ 0.5f, + 0.5f, - 0.5f),
    new Vector3(- 0.5f, + 0.5f, - 0.5f)
  };
  [NonSerialized]
  private Vector3[] bottomVertices = new Vector3[4] {
    new Vector3(- 0.5f, - 0.5f, - 0.5f),
    new Vector3(+ 0.5f, - 0.5f, - 0.5f),
    new Vector3(+ 0.5f, - 0.5f, + 0.5f),
    new Vector3(- 0.5f, - 0.5f, + 0.5f)
  };
  [NonSerialized]
  private Vector3 pos;

  [NonSerialized]
  private bool smoothEdges = true;

  //Base block constructor
  public Block(){
  }

  public virtual MeshData Blockdata
  (Chunk chunk, int x, int y, int z, MeshData meshData)
  {
    pos = new Vector3(x, y, z);

    topVertices = new Vector3[4] {
      pos + new Vector3(- 0.5f, + 0.5f, + 0.5f),
      pos + new Vector3(+ 0.5f, + 0.5f, + 0.5f),
      pos + new Vector3(+ 0.5f, + 0.5f, - 0.5f),
      pos + new Vector3(- 0.5f, + 0.5f, - 0.5f)
    };

    bottomVertices = new Vector3[4] {
      pos + new Vector3(- 0.5f, - 0.5f, - 0.5f),
      pos + new Vector3(+ 0.5f, - 0.5f, - 0.5f),
      pos + new Vector3(+ 0.5f, - 0.5f, + 0.5f),
      pos + new Vector3(- 0.5f, - 0.5f, + 0.5f)
    };

    meshData.useRenderDataForCol = true;

    if (!chunk.GetBlock(x, y + 1, z).IsSolid(Direction.down))
    {
      meshData = FaceDataUp(chunk, x, y, z, meshData);
    }

    if (!chunk.GetBlock(x, y - 1, z).IsSolid(Direction.up))
    {
      meshData = FaceDataDown(chunk, x, y, z, meshData);
    }

    if (!chunk.GetBlock(x, y, z + 1).IsSolid(Direction.south))
    {
      meshData = FaceDataNorth(chunk, x, y, z, meshData);
    }

    if (!chunk.GetBlock(x, y, z - 1).IsSolid(Direction.north))
    {
      meshData = FaceDataSouth(chunk, x, y, z, meshData);
    }

    if (!chunk.GetBlock(x + 1, y, z).IsSolid(Direction.west))
    {
      meshData = FaceDataEast(chunk, x, y, z, meshData);
    }

    if (!chunk.GetBlock(x - 1, y, z).IsSolid(Direction.east))
    {
      meshData = FaceDataWest(chunk, x, y, z, meshData);
    }

    return meshData;

  }

  protected virtual MeshData FaceDataUp
  (Chunk chunk, int x, int y, int z, MeshData meshData)
  {
    if(smoothEdges) {
      if(!chunk.GetBlock(x, y, z + 1).IsSticky(Direction.south)) {
        topVertices[0].y -= 0.25f;
        topVertices[1].y -= 0.25f;
      } else if(chunk.GetBlock(x, y + 1, z + 1).IsSticky(Direction.south)) {
        topVertices[0].y += 0.25f;
        topVertices[1].y += 0.25f;
      }

      if(!chunk.GetBlock(x, y, z - 1).IsSticky(Direction.north)) {
        topVertices[2].y -= 0.25f;
        topVertices[3].y -= 0.25f;
      } else if(chunk.GetBlock(x, y + 1, z - 1).IsSticky(Direction.north)) {
        topVertices[2].y += 0.25f;
        topVertices[3].y += 0.25f;
      }

      if(!chunk.GetBlock(x + 1, y, z).IsSticky(Direction.east)) {
        topVertices[1].y -= 0.25f;
        topVertices[2].y -= 0.25f;
      } else if(chunk.GetBlock(x + 1, y + 1, z).IsSticky(Direction.east)) {
        topVertices[1].y += 0.25f;
        topVertices[2].y += 0.25f;
      }

      if(!chunk.GetBlock(x - 1, y, z).IsSticky(Direction.west)) {
        topVertices[0].y -= 0.25f;
        topVertices[3].y -= 0.25f;
      } else if(chunk.GetBlock(x - 1, y + 1, z).IsSticky(Direction.west)) {
        topVertices[0].y += 0.25f;
        topVertices[3].y += 0.25f;
      }

      if(!chunk.GetBlock(x - 1, y, z + 1).IsSticky(Direction.west)) {
        topVertices[0].y -= 0.25f;
      } else if(chunk.GetBlock(x - 1, y + 1, z + 1).IsSticky(Direction.west)) {
        topVertices[0].y += 0.25f;
      }

      if(!chunk.GetBlock(x + 1, y, z + 1).IsSticky(Direction.west)) {
        topVertices[1].y -= 0.25f;
      } else if(chunk.GetBlock(x + 1, y + 1, z + 1).IsSticky(Direction.west)) {
        topVertices[1].y += 0.25f;
      }

      if(!chunk.GetBlock(x + 1, y, z - 1).IsSticky(Direction.west)) {
        topVertices[2].y -= 0.25f;
      } else if(chunk.GetBlock(x + 1, y + 1, z - 1).IsSticky(Direction.west)) {
        topVertices[2].y += 0.25f;
      }

      if(!chunk.GetBlock(x - 1, y, z - 1).IsSticky(Direction.west)) {
        topVertices[3].y -= 0.25f;
      } else if(chunk.GetBlock(x - 1, y + 1, z - 1).IsSticky(Direction.west)) {
        topVertices[3].y += 0.25f;
      }

      topVertices[0].y = Mathf.Clamp(topVertices[0].y, bottomVertices[3].y, pos.y + 1.25f);
      topVertices[1].y = Mathf.Clamp(topVertices[1].y, bottomVertices[2].y, pos.y + 1.25f);
      topVertices[2].y = Mathf.Clamp(topVertices[2].y, bottomVertices[1].y, pos.y + 1.25f);
      topVertices[3].y = Mathf.Clamp(topVertices[3].y, bottomVertices[0].y, pos.y + 1.25f);
    }

    meshData.AddVertex(topVertices[0]);
    meshData.AddVertex(topVertices[1]);
    meshData.AddVertex(topVertices[2]);
    meshData.AddVertex(topVertices[3]);

    meshData.AddQuadTriangles();

    meshData.uv.AddRange(FaceUVs(Direction.up));
    return meshData;
  }

  protected virtual MeshData FaceDataDown
  (Chunk chunk, int x, int y, int z, MeshData meshData)
  {
    if(smoothEdges) {
      if(!chunk.GetBlock(x, y, z + 1).IsSticky(Direction.south)) {
        bottomVertices[2].y += 0.25f;
        bottomVertices[3].y += 0.25f;
      } else if(chunk.GetBlock(x, y, z + 1).IsSticky(Direction.south)) {
        bottomVertices[2].y -= 0.25f;
        bottomVertices[3].y -= 0.25f;
      }

      if(!chunk.GetBlock(x, y, z - 1).IsSticky(Direction.north)) {
        bottomVertices[0].y += 0.25f;
        bottomVertices[1].y += 0.25f;
      } else if(chunk.GetBlock(x, y, z - 1).IsSticky(Direction.north)) {
        bottomVertices[0].y -= 0.25f;
        bottomVertices[1].y -= 0.25f;
      }

      if(!chunk.GetBlock(x + 1, y, z).IsSticky(Direction.east)) {
        bottomVertices[1].y += 0.25f;
        bottomVertices[2].y += 0.25f;
      } else if(chunk.GetBlock(x + 1, y, z).IsSticky(Direction.east)) {
        bottomVertices[1].y -= 0.25f;
        bottomVertices[2].y -= 0.25f;
      }

      if(!chunk.GetBlock(x - 1, y, z).IsSticky(Direction.west)) {
        bottomVertices[0].y += 0.25f;
        bottomVertices[3].y += 0.25f;
      } else if(chunk.GetBlock(x - 1, y, z).IsSticky(Direction.west)) {
        bottomVertices[0].y -= 0.25f;
        bottomVertices[3].y -= 0.25f;
      }

      if(!chunk.GetBlock(x - 1, y, z - 1).IsSticky(Direction.west)) {
        bottomVertices[0].y += 0.25f;
      } else if(chunk.GetBlock(x - 1, y, z - 1).IsSticky(Direction.west)) {
        bottomVertices[0].y -= 0.25f;
      }

      if(!chunk.GetBlock(x + 1, y, z - 1).IsSticky(Direction.west)) {
        bottomVertices[1].y += 0.25f;
      } else if(chunk.GetBlock(x + 1, y, z - 1).IsSticky(Direction.west)) {
        bottomVertices[1].y -= 0.25f;
      }

      if(!chunk.GetBlock(x + 1, y, z + 1).IsSticky(Direction.west)) {
        bottomVertices[2].y += 0.25f;
      } else if(chunk.GetBlock(x + 1, y, z + 1).IsSticky(Direction.west)) {
        bottomVertices[2].y -= 0.25f;
      }

      if(!chunk.GetBlock(x - 1, y, z + 1).IsSticky(Direction.west)) {
        bottomVertices[3].y += 0.25f;
      } else if(chunk.GetBlock(x - 1, y, z + 1).IsSticky(Direction.west)) {
        bottomVertices[3].y -= 0.25f;
      }

      bottomVertices[0].y = Mathf.Clamp(bottomVertices[0].y, pos.y - 1.25f, topVertices[3].y);
      bottomVertices[1].y = Mathf.Clamp(bottomVertices[1].y, pos.y - 1.25f, topVertices[2].y);
      bottomVertices[2].y = Mathf.Clamp(bottomVertices[2].y, pos.y - 1.25f, topVertices[1].y);
      bottomVertices[3].y = Mathf.Clamp(bottomVertices[3].y, pos.y - 1.25f, topVertices[0].y);
    }

    meshData.AddVertex(bottomVertices[0]);
    meshData.AddVertex(bottomVertices[1]);
    meshData.AddVertex(bottomVertices[2]);
    meshData.AddVertex(bottomVertices[3]);

    meshData.AddQuadTriangles();

    meshData.uv.AddRange(FaceUVs(Direction.down));
    return meshData;
  }

  protected virtual MeshData FaceDataNorth
  (Chunk chunk, int x, int y, int z, MeshData meshData)
  {
    meshData.AddVertex(bottomVertices[2]);
    meshData.AddVertex(topVertices[1]);
    meshData.AddVertex(topVertices[0]);
    meshData.AddVertex(bottomVertices[3]);

    meshData.AddQuadTriangles();

    meshData.uv.AddRange(FaceUVs(Direction.north));
    return meshData;
  }

  protected virtual MeshData FaceDataEast
  (Chunk chunk, int x, int y, int z, MeshData meshData)
  {
    meshData.AddVertex(bottomVertices[1]);
    meshData.AddVertex(topVertices[2]);
    meshData.AddVertex(topVertices[1]);
    meshData.AddVertex(bottomVertices[2]);

    meshData.AddQuadTriangles();

    meshData.uv.AddRange(FaceUVs(Direction.east));
    return meshData;
  }

  protected virtual MeshData FaceDataSouth
  (Chunk chunk, int x, int y, int z, MeshData meshData)
  {
    Vector3 pos = new Vector3(x, y, z);

    meshData.AddVertex(bottomVertices[0]);
    meshData.AddVertex(topVertices[3]);
    meshData.AddVertex(topVertices[2]);
    meshData.AddVertex(bottomVertices[1]);

    meshData.AddQuadTriangles();

    meshData.uv.AddRange(FaceUVs(Direction.south));
    return meshData;
  }

  protected virtual MeshData FaceDataWest
  (Chunk chunk, int x, int y, int z, MeshData meshData)
  {
    meshData.AddVertex(bottomVertices[3]);
    meshData.AddVertex(topVertices[0]);
    meshData.AddVertex(topVertices[3]);
    meshData.AddVertex(bottomVertices[0]);

    meshData.AddQuadTriangles();

    meshData.uv.AddRange(FaceUVs(Direction.west));
    return meshData;
  }

  public virtual bool IsSolid(Direction direction)
  {
    switch(direction){
      case Direction.north:
      return (topVertices[2].y>=0.5f && topVertices[3].y>=0.5f);
      case Direction.east:
      return (topVertices[0].y>=0.5f && topVertices[3].y>=0.5f);
      case Direction.south:
      return (topVertices[0].y>=0.5f && topVertices[1].y>=0.5f);
      case Direction.west:
      return (topVertices[1].y>=0.5f && topVertices[2].y>=0.5f);
      case Direction.up:
      return true;
      case Direction.down:
      return true;
    }
    return false;
  }

  public virtual bool IsSticky(Direction direction)
  {
    switch(direction){
      case Direction.north:
      return true;
      case Direction.east:
      return true;
      case Direction.south:
      return true;
      case Direction.west:
      return true;
      case Direction.up:
      return true;
      case Direction.down:
      return true;
    }
    return false;
  }

  public virtual Tile TexturePosition(Direction direction)
  {
    Tile tile = new Tile();
    tile.x = 0;
    tile.y = 0;
    return tile;
  }

  public virtual Vector2[] FaceUVs(Direction direction)
  {
    Vector2[] UVs = new Vector2[4];
    Tile tilePos = TexturePosition(direction);
    UVs[0] = new Vector2(tileSize * tilePos.x + tileSize,
    tileSize * tilePos.y);
    UVs[1] = new Vector2(tileSize * tilePos.x + tileSize,
    tileSize * tilePos.y + tileSize);
    UVs[2] = new Vector2(tileSize * tilePos.x,
    tileSize * tilePos.y + tileSize);
    UVs[3] = new Vector2(tileSize * tilePos.x,
    tileSize * tilePos.y);
    return UVs;
  }
}
