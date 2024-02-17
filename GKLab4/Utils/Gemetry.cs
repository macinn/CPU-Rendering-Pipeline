using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public struct Vertex
    {
        public Vector4 Position;
        public Vector4 Normal;
        public Vector4 Color;
    }

    public struct Traiangle
    {
        public Vertex[] Vertices;
        public Traiangle(Vertex v1, Vertex v2, Vertex v3)
        {
            Vertices = new Vertex[3];
            Vertices[0] = v1;
            Vertices[1] = v2;
            Vertices[2] = v3;
        }

        public Traiangle(Vertex[] vertices)
        {
            if (vertices.Length != 3)
                throw new ArgumentException("Triangle must have 3 vertices");
            Vertices = vertices;
        }
    }
}
