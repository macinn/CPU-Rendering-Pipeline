﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Deprecated;
using Utils;


namespace Objects
{
    public abstract class IRenderable
    {
        public Vertex[] vertices;
        public int[][] indices;
        public Matrix4x4 ModelMatrix = Matrix4x4.Identity;
        public static bool _drawLines = true;
        public Action<long> updateModelFun = (dt) => { };

        public void Render(Matrix4x4 ViewMatrix, Matrix4x4 ProjectionMatrix, long dt)
        {
            //updateModelMatrix();
            updateModelFun(dt);
            Vertex[] projectedPoints = projectPoints(ViewMatrix, ProjectionMatrix);

            if (_drawLines)
                drawLines(projectedPoints);
            else
                drawMesh(projectedPoints);
        }

        private void updateModelMatrix() { }

        private Vertex[] projectPoints(Matrix4x4 ViewMatrix, Matrix4x4 ProjectionMatrix)
        {
            return vertices.ToList().Select(
                v =>
                {
                    Vector3 normal = Vector3.Transform(v.Normal, ModelMatrix);
                    normal = Vector3.Transform(normal, ViewMatrix);

                    Vector4 projectedPosition = Vector4.Transform(v.Position, ModelMatrix);
                    projectedPosition = Vector4.Transform(projectedPosition, ViewMatrix);
                    float fogFacotr = RenderStream.FogFactor(projectedPosition);
                    projectedPosition = Vector4.Transform(projectedPosition, ProjectionMatrix);
                    projectedPosition /= projectedPosition.W;


                    return new Vertex()
                    {
                        Position = projectedPosition,
                        Normal = normal,
                        Color = v.Color * fogFacotr
                    };
                }
                ).ToArray();
        }

        private void drawMesh(Vertex[] projectedPoints)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                RenderStream.drawTriangle(new[] {
                    projectedPoints[indices[i][1]],
                    projectedPoints[indices[i][0]],
                    projectedPoints[indices[i][2]] });
            }
        }

        private void drawLines(Vertex[] projectedPoints)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                if (RenderStream.backFace(projectedPoints[indices[i][0]],
                    projectedPoints[indices[i][1]],
                    projectedPoints[indices[i][2]]))
                    continue;
                RenderStream.drawLine(projectedPoints[indices[i][0]], projectedPoints[indices[i][1]]);
                RenderStream.drawLine(projectedPoints[indices[i][1]], projectedPoints[indices[i][2]]);
                RenderStream.drawLine(projectedPoints[indices[i][0]], projectedPoints[indices[i][2]]);
            }
        }

    }
    public class Cube : IRenderable
    {
        public Cube(float a, Vector3 color)
        {
            vertices = CreateCubeVertices(a, color).ToArray();
            indices = CreateIndices();

            ModelMatrix = Matrix4x4.CreateTranslation(new Vector3(-a / 2, -a / 2, -a / 2));
        }
        static Vertex[] CreateCubeVertices(float sideLength, Vector3 color)
        {
            Vector4 LeftUpBack = new Vector4(0, sideLength, 0, 1);
            Vector4 RightUpBack = new Vector4(sideLength, sideLength, 0, 1);
            Vector4 RightDownBack = new Vector4(sideLength, 0, 0, 1);
            Vector4 LeftDownBack = new Vector4(0, 0, 0, 1);
            Vector4 LeftUpFront = new Vector4(0, sideLength, sideLength, 1);
            Vector4 RightUpFront = new Vector4(sideLength, sideLength, sideLength, 1);
            Vector4 RightDownFront = new Vector4(sideLength, 0, sideLength, 1);
            Vector4 LeftDownFront = new Vector4(0, 0, sideLength, 1);

            Vector3 frontN = new Vector3(0, 0, 1);
            Vector3 backN = new Vector3(0, 0, -1);

            Vector3 leftN = new Vector3(-1, 0, 0);
            Vector3 rightN = new Vector3(1, 0, 0);

            Vector3 upN = new Vector3(0, 1, 0);
            Vector3 downN = new Vector3(0, -1, 0);

            Vector3 leftColor = new Vector3(1, 0, 1);
            Vector3 rightColor = color;

            Vector3 upColor = color;
            Vector3 downColor = color;

            Vector3 frontColor = color;
            Vector3 backColor = color;


            Vertex[] vertices = new Vertex[24]
            {
                // front             
                    new Vertex
                    {
                        Position = LeftDownFront,
                        Normal = frontN,
                        Color = frontColor
                    },
                    new Vertex
                    {
                        Position = LeftUpFront,
                        Normal = frontN,
                        Color = frontColor
                    },
                    new Vertex
                    {
                        Position = RightUpFront,
                        Normal = frontN,
                        Color = frontColor
                    },
                    new Vertex
                    {
                        Position = RightDownFront,
                        Normal = frontN,
                        Color = frontColor
                    },
                // back
                    new Vertex
                    {
                        Position = LeftUpBack,
                        Normal = backN,
                        Color = backColor
                    },
                    new Vertex
                    {
                        Position = LeftDownBack,
                        Normal = backN,
                        Color = backColor
                    },
                    new Vertex
                    {
                        Position = RightDownBack,
                        Normal = backN,
                        Color = backColor
                    },
                    new Vertex
                    {
                        Position = RightUpBack,
                        Normal = backN,
                        Color = backColor
                    },
                // left
                    new Vertex
                    {
                        Position = LeftUpBack,
                        Normal = leftN,
                        Color = leftColor
                    },
                    new Vertex
                    {
                        Position = LeftDownBack,
                        Normal = leftN,
                        Color = leftColor
                    },
                    new Vertex
                    {
                        Position = LeftDownFront,
                        Normal = leftN,
                        Color = leftColor
                    },
                    new Vertex
                    {
                        Position = LeftUpFront,
                        Normal = leftN,
                        Color = leftColor
                    },
                // right
                    new Vertex
                    {
                        Position = RightUpBack,
                        Normal = rightN,
                        Color = rightColor
                    },
                    new Vertex
                    {
                        Position = RightDownBack,
                        Normal = rightN,
                        Color = rightColor
                    },
                    new Vertex
                    {
                        Position = RightDownFront,
                        Normal = rightN,
                        Color = rightColor
                    },
                    new Vertex
                    {
                        Position = RightUpFront,
                        Normal = rightN,
                        Color = rightColor
                    },
                // up
                    new Vertex
                    {
                        Position = LeftUpBack,
                        Normal = upN,
                        Color = upColor
                    },
                    new Vertex
                    {
                        Position = RightUpBack,
                        Normal = upN,
                        Color = upColor
                    },
                    new Vertex
                    {
                        Position = RightUpFront,
                        Normal = upN,
                        Color = upColor
                    },
                    new Vertex
                    {
                        Position = LeftUpFront,
                        Normal = upN,
                        Color = upColor
                    },
                // down
                    new Vertex
                    {
                        Position = LeftDownBack,
                        Normal = downN,
                        Color = downColor
                    },
                    new Vertex
                    {
                        Position = RightDownBack,
                        Normal = downN,
                        Color = downColor
                    },
                    new Vertex
                    {
                        Position = RightDownFront,
                        Normal = downN,
                        Color = downColor
                    },
                    new Vertex
                    {
                        Position = LeftDownFront,
                        Normal = downN,
                        Color = downColor
                    },
            };

            return vertices;
        }

        static int[][] CreateIndices()
        {
            int[][] indices =
            [
                // front
                [0, 1, 2],
                [0, 2, 3],
                //back
                [4, 5, 6],
                [4, 6, 7],
                //left
                [8, 9, 10],
                [8, 10, 11],
                // right          
                [12, 13, 14],
                [12, 14, 15],
                // up
                [16, 17, 18],
                [16, 18, 19],
                // down
                [20, 21, 22],
                [20, 22, 23]
            ];
            return indices;
        }
    }

    public class Sphere : IRenderable
    {
        public Sphere(float radius, int meridians, int parallels, Vector3 color)
        {
            vertices = CreateSphereVertices(radius, meridians, parallels, color).ToArray();
            indices = CreateIndices(meridians, parallels);
        }

        static Vertex[] CreateSphereVertices(float radius, int meridians, int parallels, Vector3 color)
        {
            List<Vertex> vertices = new List<Vertex>();
            float meridianAngle = 2 * MathF.PI / meridians;
            float parallelAngle = MathF.PI / parallels;

            for (int i = 0; i < parallels; i++)
            {
                float parallel = i * parallelAngle;
                for (int j = 0; j < meridians; j++)
                {
                    float meridian = j * meridianAngle;
                    float x = radius * MathF.Sin(parallel) * MathF.Cos(meridian);
                    float y = radius * MathF.Cos(parallel);
                    float z = radius * MathF.Sin(parallel) * MathF.Sin(meridian);

                    Vector4 position = new Vector4(x, y, z, 1);
                    Vector3 normal = new Vector3(x,y,z);
                    normal = Vector3.Normalize(normal);
                    Vector3 color1 = color;

                    vertices.Add(new Vertex()
                    {
                        Position = position,
                        Normal = normal,
                        Color = color1
                    });
                }
            }
            return vertices.ToArray();
        }

        int[][] CreateIndices(int meridians, int parallels)
        {
            List<int[]> indices = new List<int[]>();
            for (int i = 0; i < parallels - 1; i++)
            {
                for (int j = 0; j < meridians; j++)
                {
                    int[] indices1 = new int[3];
                    indices1[0] = (i * meridians + j) % (parallels * meridians);
                    indices1[1] = (i * meridians + j + 1) % (parallels * meridians);
                    indices1[2] = ((i + 1) * meridians + j) % (parallels * meridians);
                    indices.Add(indices1);

                    int[] indices2 = new int[3];
                    indices2[0] = (i * meridians + j + 1) % (parallels * meridians);
                    indices2[1] = ((i + 1) * meridians + j + 1) % (parallels * meridians);
                    indices2[2] = ((i + 1) * meridians + j) % (parallels * meridians);
                    indices.Add(indices2);
                }
            }

            return indices.ToArray();
        }
    }

}
