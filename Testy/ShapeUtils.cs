using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testy
{
    public class Shape
    {
        public float[] Vertices => m_vertices;
        public uint[] Indices => m_indeces;

        private float[] m_vertices;
        private uint[] m_indeces;

        public Shape(float[] vertices, uint[] indeces)
        {
            m_vertices = vertices;
            m_indeces = indeces;
        }
    }

    public class TriangleShape : Shape
    {
        public TriangleShape() : base(ShapeUtils.TriangleVertices, new uint[] { 0, 1, 2 })
        {
        }
    }

    public class SquareShape : Shape
    {
        public SquareShape()
            : base(ShapeUtils.SquareVertices, new uint[] { 0, 1, 3, 1, 2, 3 })
        {
        }
    }

    internal class ShapeUtils
    {
        public static float[] TriangleVertices => new[]
        {
            -0.5f, -0.5f, 0.0f, // Bottom left
             0.5f, -0.5f, 0.0f, // Bottom right
             0.0f,  0.5f, 0.0f, // Top
        };

        public static float[] SquareVertices => new[]
        {
            0.5f, 0.5f, 0.0f, // top right
            0.5f, -0.5f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, // bottom left
            -0.5f, 0.5f, 0.0f // top left
        };
    }
}
