using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Map3dConstructor
{
  public enum ElementType
  {
    Wall,
    Road
  }

  public struct Triangle
  {
    public Vector3 A;
    public Vector3 B;
    public Vector3 C;
    public Vector2 At;
    public Vector2 Bt;
    public Vector2 Ct;
  }
  public class Map3DConstructor
  {
    private readonly ElementType[] _map;

    private readonly int _width;

    private readonly int _height;
    public Map3DConstructor(ElementType[] map, int width, int height)
    {
      _map = map;
      _height = height;
      _width = width;
    }
    public static IEnumerable<Triangle> GenerateWall(float x1, float x2, float y1, float y2, float wallHeight)
    {
      yield return new Triangle
      {
        A = { X = x1, Y = y1, Z = 0 }         , At = { X = 0, Y = 1},
        B = { X = x1, Y = y1, Z = wallHeight }, Bt = { X = 0, Y = 0},
        C = { X = x2, Y = y2, Z = wallHeight }, Ct = { X = 1, Y = 0},
      };
      yield return new Triangle
      {
        A = { X = x1, Y = y1, Z = 0 }         , At = { X = 0, Y = 1},
        B = { X = x2, Y = y2, Z = wallHeight }, Bt = { X = 1, Y = 0},
        C = { X = x2, Y = y2, Z = 0 }         , Ct = { X = 1, Y = 1},
      };
    }
    private static IEnumerable<Triangle> GenerateFloor(float x1, float x2, float y1, float y2)
    {
      yield return new Triangle
      {
        A = { X = x1, Y = y1, Z = 0 }, At = { X = 0, Y = 1},
        B = { X = x1, Y = y2, Z = 0 }, Bt = { X = 0, Y = 0},
        C = { X = x2, Y = y2, Z = 0 }, Ct = { X = 1, Y = 0},
      };
      yield return new Triangle
      {
        A = { X = x1, Y = y1, Z = 0 }, At = { X = 0, Y = 1},
        B = { X = x2, Y = y2, Z = 0 }, Bt = { X = 1, Y = 0},
        C = { X = x2, Y = y1, Z = 0 }, Ct = { X = 1, Y = 1},
      };
    }

    public void Generate(List<Triangle> walls, List<Triangle> floor, float size)
    {
      for (int i = 0; i < _width; i++)
      {
        for (int j = 0; j < _height; j++)
        {
          float xl = i * size;
          float yt = j * size;
          float xr = (i + 1) * size;
          float yb = (j + 1) * size;

          var current = _map[i * _height + j];
          if (i == 0 || current != _map[(i - 1) * _height + j])
          {// left
            walls.AddRange(GenerateWall(xl, xl, yt, yb, size));
          }

          if (i == _width - 1 || current != _map[(i + 1) * _height + j])
          {// right
            walls.AddRange(GenerateWall(xr, xr, yt, yb, size));
          }

          if (j == 0 || current != _map[i * _height + j - 1])
          {// top
            walls.AddRange(GenerateWall(xl, xr, yt, yt, size));
          }

          if (j == _height - 1 || current != _map[i * _height + j + 1])
          {// bottom
            walls.AddRange(GenerateWall(xl, xr, yb, yb, size));
          }
          if (current == ElementType.Road)
          {
            floor.AddRange(GenerateFloor(xl, xr, yt, yb));
          }
        }
      }
    }
  }
}
