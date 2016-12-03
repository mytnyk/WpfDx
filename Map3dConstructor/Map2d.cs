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
    }
    public class Map2d
    {
        private ElementType[] m_map;

        private int m_width;

        private int m_height;
        public Map2d(ElementType[] map, int width, int height)
        {
            m_map = map;
            m_height = height;
            m_width = width;
        }
        private IEnumerable<Triangle> GenerateWall(float x1, float x2, float y1, float y2, float size)
        {
            yield return new Triangle
            {
                A = { X = x1, Y = y1, Z = 0 },
                B = { X = x1, Y = y1, Z = size },
                C = { X = x2, Y = y2, Z = size }
            };
            yield return new Triangle
            {
                A = { X = x1, Y = y1, Z = 0 },
                B = { X = x2, Y = y2, Z = size },
                C = { X = x2, Y = y2, Z = 0 }
            };
        }
        private IEnumerable<Triangle> GenerateFloor(float x1, float x2, float y1, float y2)
        {
            yield return new Triangle
            {
                A = { X = x1, Y = y1, Z = 0 },
                B = { X = x1, Y = y2, Z = 0 },
                C = { X = x2, Y = y2, Z = 0 }
            };
            yield return new Triangle
            {
                A = { X = x1, Y = y1, Z = 0 },
                B = { X = x2, Y = y2, Z = 0 },
                C = { X = x2, Y = y1, Z = 0 }
            };
        }

        public void Generate(List<Triangle> walls, List<Triangle> floor, float size)
        {
            for (int i = 0; i < m_width; i++)
            {
                for (int j = 0; j < m_height; j++)
                {
                    float xl = i * size;
                    float yt = j * size;
                    float xr = (i + 1) * size;
                    float yb = (j + 1) * size;

                    var current = m_map[i * m_height + j];
                    if (i == 0 || current != m_map[(i - 1) * m_height + j])
                    {// left
                        walls.AddRange(GenerateWall(xl, xl, yt, yb, size));
                    }

                    if (i == m_width - 1 || current != m_map[(i + 1) * m_height + j])
                    {// right
                        walls.AddRange(GenerateWall(xr, xr, yt, yb, size));
                    }

                    if (j == 0 || current != m_map[i * m_height + j - 1])
                    {// top
                        walls.AddRange(GenerateWall(xl, xr, yt, yt, size));
                    }

                    if (j == m_height - 1 || current != m_map[i * m_height + j + 1])
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
