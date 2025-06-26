using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Testy
{
    public class Shape
    {
        public Vector3[] Vertices { get; }

        public int[] Indices { get; }

        public Vector3[] Normals { get; private set; }

        public Shape(Vector3[] vertices, int[] indeces)
        {
            Vertices = vertices;
            Indices = indeces;
        }

        public void GenerateNormals()
        {
            if (Vertices.Length == 0)
            {
                return;
            }

            Normals = new Vector3[Indices.Length];

            if (Indices.Length != 0)
            {
                
                for (var i = 0; i < Indices.Length; i += 3)
                {
                    var a = Indices[i + 0];
                    var b = Indices[i + 1];
                    var c = Indices[i + 2];
                    
                    Vector3 normal = Vector3.Cross((Vertices[b] - Vertices[a]), (Vertices[c] - Vertices[a])); 
                    
                    Normals[i] = normal;
                    Normals[i + 1] = normal;
                    Normals[i + 2] = normal;
                }
            }
            else
            {
                //TODO: 
            }

            foreach (var vector3 in Normals)
            {
                vector3.Normalize();
            }
            
        }
    }

    public class TriangleShape : Shape
    {
        public TriangleShape() : base(ShapeUtils.TriangleVertices, new int[] { 0, 1, 2 })
        {
        }
    }

    public class SquareShape : Shape
    {
        public SquareShape()
            : base(ShapeUtils.SquareVertices, new int[] { 0, 1, 3, 1, 2, 3 })
        {
        }
    }

    public class CubeShape : Shape
    {
        public CubeShape() :
            base(new Vector3[]
                {
                    new Vector3( -0.5f, -0.5f,  0.5f ),    //0
                    new Vector3( 0.5f, -0.5f,  0.5f ),     //1
                    new Vector3( -0.5f,  0.5f,  0.5f ),    //2
                    new Vector3( 0.5f,  0.5f,  0.5f ),     //3
                    new Vector3( -0.5f, -0.5f, -0.5f),     //4
                    new Vector3( 0.5f, -0.5f, -0.5f),      //5
                    new Vector3( -0.5f,  0.5f, -0.5f),     //6
                    new Vector3( 0.5f,  0.5f, -0.5f)      //7
                },
                new int[]
                {
                    //Top
                    2, 6, 7,
                    2, 3, 7,

                    //Bottom
                    0, 4, 5,
                    0, 1, 5,

                    //Left
                    0, 2, 6,
                    0, 4, 6,

                    //Right
                    1, 3, 7,
                    1, 5, 7,

                    //Front
                    0, 2, 3,
                    0, 1, 3,

                    //Back
                    4, 6, 7,
                    4, 5, 7
                }
            )
        {
            
        }
    }

    internal class ShapeUtils
    {
        public static Vector3[] TriangleVertices => new[]
        {
            new Vector3( -0.5f, -0.5f, 0.0f ), // Bottom left 
            new Vector3(  0.5f, -0.5f, 0.0f ), // Bottom right
            new Vector3(  0.0f,  0.5f, 0.0f )  // Top
        };

        public static Vector3[] SquareVertices => new[]
        {
            new Vector3( 0.5f,  0.5f, 0.0f ),  // top right
            new Vector3( 0.5f, -0.5f, 0.0f ),  // bottom right
            new Vector3( -0.5f, -0.5f, 0.0f ), // bottom left
            new Vector3( -0.5f,  0.5f, 0.0f )  // top left
        };
    }
}
