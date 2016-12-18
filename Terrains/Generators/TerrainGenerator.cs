using System;
using System.Collections;
using System.Collections.Generic;
#if (!UNITY_EDITOR && !UNITY_STANDALONE)
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
#endif

[Serializable]
public struct TerrainPos
{
  public int x, y;

  public TerrainPos(int x, int y)
  {
    this.x = x;
    this.y = y;
  }
}

[Serializable]
public struct TerrainDataBlock
{
  public byte[, ] data;
  public static int width = 512;
  public static int height = 512;

  public TerrainDataBlock(TerrainPos center)
  {
    data = new byte[width, height];

    for (int x = center.x-width/2; x < center.x+width/2; x++) {
      for (int y = center.y-height/2; y < center.y+height/2; y++) {
        byte value = (byte)(2+Math.Sqrt(x*x + y*y)/10);

        data[x-center.x+width/2, y-center.y+height/2] = value;
      }
    }
  }

  public byte GetHeight(int x, int y) {
    return data[x, y];
  }
}

public class TerrainGenerator {
  public Dictionary<TerrainPos, TerrainDataBlock> _TerrainData = new Dictionary<TerrainPos, TerrainDataBlock>();

  public TerrainGenerator() {
    TerrainPos pos = new TerrainPos(0, 0);
    _TerrainData.Add(pos, new TerrainDataBlock(pos));
  }

#if (!UNITY_EDITOR && !UNITY_STANDALONE)
  public Bitmap ToBitmap() {
    TerrainDataBlock data;
    _TerrainData.TryGetValue(new TerrainPos(0, 0), out data);

    int width = TerrainDataBlock.width;
    int height = TerrainDataBlock.height;

    Bitmap b = new Bitmap(width, height);
    Graphics g = Graphics.FromImage(b);

    for (int x = 0; x < width; x++) {
      for (int y = 0; y < height; y++) {
        int value = (int)(data.GetHeight(x, y));

        Color color = Color.FromArgb(255, value, value, value);

        g.FillRectangle(new SolidBrush(color), x, y, 1, 1);
      }
    }

    return b;
  }
}

namespace TerrainGeneratorTester
{
  class Tester
  {
    static void Main()
    {
      Console.WriteLine("Generating Terrain");
      TerrainGenerator terrain = new TerrainGenerator();

      Bitmap b = terrain.ToBitmap();
      b.Save(@"output.png", ImageFormat.Png);

      Console.WriteLine("Terrain Generated");
    }
  }
  #endif
}
