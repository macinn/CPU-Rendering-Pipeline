using System.Collections.Concurrent;
using System.Numerics;
using Utils;

namespace CPU_Rendering
{
    public interface IShader<T>
    {
        abstract public static T Process(T state);
    }

    static class Pipeline
    {
        public static Matrix4x4 viewMatrix;
        public static Matrix4x4 projMatrix;

        public static int canvasW, canvasH;
        static double[,] ZBuffer;

        static Bitmap bitmap;
        private static Graphics currentGraphics;
        static Graphics g { get { return currentGraphics; } }

        public static float fogFactor = 0.1f;
        static Vector3 constantColor;

        private static readonly ProcessingChain<IRenderable, Traiangle> shaders;

        static Pipeline()
        {
            // TODO: init viewMatrix
            // TODO: init projMatrix
            
            shaders = new ProcessingChain<IRenderable, Traiangle>(
                VertexModule.Process,
                [
                FragmentModule.Process,
                DrawModule.Process
                ]);
        }


        static public void StartProcessing(IList<IRenderable> objects)
        {
            Parallel.ForEach(objects, obj =>
                {
                    List<Traiangle> traiangles = VertexModule.Process(obj);
                    Parallel.ForEach(traiangles, traiangle =>
                    {
                        shaders.Execute(traiangle);
                        DrawModule.CalcaultePosition(ref traiangle);
                    });
                    
                });
        }

        class VertexModule
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
           
            public static List<Traiangle> Process(IRenderable renderable)
            {
                Vertex[] projectedPoints = VertexModule.UpdateVerticies(renderable);

                return renderable.indices.Select((int[] indicies) 
                    => new Traiangle(
                            projectedPoints[indicies[0]],
                            projectedPoints[indicies[1]],
                            projectedPoints[indicies[2]])).ToList();
            }
        }

        class FragmentModule : IShader<Traiangle>
        {
            public static Traiangle Process(Traiangle state)
            {
                throw new NotImplementedException();
            }
        }

        class DrawModule : IShader<Traiangle>
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

            public static Traiangle Process(Traiangle state)
            {
                throw new NotImplementedException();
            }
        }

    }
}
