using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class BlockAir : Block
{
  public BlockAir() : base() {
  }

  public override MeshData Blockdata (Chunk chunk, int x, int y, int z, MeshData meshData) {
    return meshData;
  }

  public override bool IsSolid(Block.Direction direction) {
    return false;
  }

  public override bool IsSticky(Block.Direction direction) {
    return false;
  }
}
