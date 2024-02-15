using System.Collections.Concurrent;
using System.Numerics;
using Utils;

namespace CPU_Rendering
{
    public interface IShader
    {
        public static void Process(object? state) { }
    }

    static class Pipeline
    {
        public static BlockingCollection<Traiangle> traiangles = [];
        public static BlockingCollection<Vertex> pixels = [];

        public static Matrix4x4 viewMatrix;
        public static Matrix4x4 projMatrix;

        public static int canvasW, canvasH;
        static double[,] ZBuffer;

        static Bitmap bitmap;
        private static Graphics currentGraphics;
        static Graphics g { get { return currentGraphics; } }

        public static float fogFactor = 0.1f;
        static Vector3 constantColor;

        static private Chain<IShader> shaders;

        static Pipeline()
        {
            // TODO: init viewMatrix
            // TODO: init projMatrix
            shaders = [new VertexModule(), new FragmentModule(), new DrawModule()];
        }


        static public void StartProcessing(List<IRenderable> objects)
        {
            foreach (IRenderable renderable in objects)
                ThreadPool.QueueUserWorkItem(VertexModule.Process, renderable);
        }

        class VertexModule : IShader
        {
            private static Vertex[] UpdateVerticies(IRenderable renderable)
            {
                Vertex[] newVerticies = new Vertex[renderable.vertices.Length];
                Array.Copy(renderable.vertices, newVerticies, newVerticies.Length);
                Matrix4x4 MV = renderable.ModelMatrix * Pipeline.viewMatrix;
                Matrix4x4 MVP = MV * Pipeline.projMatrix;
                for (int i = 0; i < newVerticies.Length; i++)
                {
                    // TODO: fix normal transformation
                    // TODO: add fog
                    newVerticies[i].Normal = Vector4.Transform(newVerticies[i].Normal, MV);

                    newVerticies[i].Position = Vector4.Transform(newVerticies[i].Position, MVP);
                    newVerticies[i].Position /= newVerticies[i].Position.W;
                };

                return newVerticies;
            }

            public static void Process(IList<IRenderable> renderables)
            {
                foreach (IRenderable renderable in renderables)
                {
                    ThreadPool.QueueUserWorkItem(VertexModule.Process, renderable);
                }
            }
           
            public static void Process(object? state)
            {
                if(state != null)
                {
                    VertexModule.Process((IRenderable)state);
                }
            }

            public static void Process(IRenderable renderable)
            {
                Vertex[] projectedPoints = VertexModule.UpdateVerticies(renderable);

                foreach (int[] indicies in renderable.indices)
                {
                    ThreadPool.QueueUserWorkItem(FragmentModule.Process,
                        new Traiangle(
                            projectedPoints[indicies[0]],
                            projectedPoints[indicies[1]],
                            projectedPoints[indicies[2]]));
                }
            }
        }

        class FragmentModule : IShader
        {
            public static void Process(object? state)
            {

            }
        }

        class DrawModule : IShader
        {
            public static void CalcaultePosition(ref Traiangle traiangle)
            {
                traiangle.Vertices[0].Position.X = (traiangle.Vertices[0].Position.X + 1) * Pipeline.canvasW;
                traiangle.Vertices[0].Position.Y = (traiangle.Vertices[0].Position.Y + 1) * Pipeline.canvasH;

                traiangle.Vertices[1].Position.X = (traiangle.Vertices[1].Position.X + 1) * Pipeline.canvasW;
                traiangle.Vertices[1].Position.Y = (traiangle.Vertices[1].Position.Y + 1) * Pipeline.canvasH;

                traiangle.Vertices[2].Position.X = (traiangle.Vertices[2].Position.X + 1) * Pipeline.canvasW;
                traiangle.Vertices[2].Position.Y = (traiangle.Vertices[2].Position.Y + 1) * Pipeline.canvasH;
            }
        }

    }
}
