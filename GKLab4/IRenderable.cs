using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace GKLab4
{
    public struct Vertex
    {
        public Vector4 Position;
        public Vector4 Normal;
        public Vector4 Color;
    }
    public abstract class IRenderable
    {
        public Vertex[] vertices;
        public int[][] indices;
        public Matrix4x4 ModelMatrix = Matrix4x4.Identity;

        public void Render(Matrix4x4 ViewMatrix, Matrix4x4 ProjectionMatrix)
        {
            updateModelMatrix();
            Vertex[] projectedPoints = projectPoints(ViewMatrix, ProjectionMatrix);
            //drawMesh(projectedPoints);
            drawLines(projectedPoints);
        }

        private void updateModelMatrix() { }

        private Vertex[] projectPoints(Matrix4x4 ViewMatrix, Matrix4x4 ProjectionMatrix)
        {
            return vertices.ToList().Select(
                v =>
                {
                    Vector4 projectedPosition = Vector4.Transform(v.Position, ModelMatrix);
                    projectedPosition = Vector4.Transform(projectedPosition, ViewMatrix);
                    float fogFacotr = RenderStream.FogFactor(projectedPosition);
                    projectedPosition = Vector4.Transform(projectedPosition, ProjectionMatrix);
                    projectedPosition /= projectedPosition.W;

                    Vector4 normal = Vector4.Transform(v.Normal, ModelMatrix);
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
                RenderStream.drawLine(projectedPoints[indices[i][0]], projectedPoints[indices[i][1]]);
                RenderStream.drawLine(projectedPoints[indices[i][1]], projectedPoints[indices[i][2]]);
                RenderStream.drawLine(projectedPoints[indices[i][0]], projectedPoints[indices[i][2]]);
            }
        }

    }
    public class Cube : IRenderable
    {
        public Cube(float a, Vector4 color)
        {
            this.vertices = CreateCubeVertices(a, color).ToArray();
            this.indices = CreateIndices();

            this.ModelMatrix = Matrix4x4.CreateTranslation(new Vector3(-a / 2, -a / 2, -a / 2));
        }
        static Vertex[] CreateCubeVertices(float sideLength, Vector4 color)
        {
            Vector4 LeftUpFront = new Vector4(0, sideLength, 0, 1);
            Vector4 RightUpFront = new Vector4(sideLength, sideLength, 0, 1);
            Vector4 RightDownFront = new Vector4(sideLength, 0, 0, 1);
            Vector4 LeftDownFront = new Vector4(0, 0, 0, 1);
            Vector4 LeftUpBack = new Vector4(0, sideLength, sideLength, 1);
            Vector4 RightUpBack = new Vector4(sideLength, sideLength, sideLength, 1);
            Vector4 RightDownBack = new Vector4(sideLength, 0, sideLength, 1);
            Vector4 LeftDownBack = new Vector4(0, 0, sideLength, 1);

            Vector4 frontN = new Vector4(0, 0, 1, 0);
            Vector4 backN = new Vector4(0, 0, -1, 0);

            Vector4 leftN = new Vector4(-1, 0, 0, 0);
            Vector4 rightN = new Vector4(1, 0, 0, 0);

            Vector4 upN = new Vector4(0, 1, 0, 0);
            Vector4 downN = new Vector4(0, -1, 0, 0);

            // TODO: fix backface
            //backN *= -1;
            //frontN *= -1;

            Vector4 leftColor = new Vector4(1, 0, 1, 1);
            Vector4 rightColor = color;

            Vector4 upColor = color;
            Vector4 downColor = color;

            Vector4 frontColor = color;
            Vector4 backColor = color;


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
}
