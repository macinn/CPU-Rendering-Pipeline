using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Utils;

namespace CPU_Rendering;

public interface IShader<T>
{
    public static abstract T Process(T state);
}

public interface IVetexShader<Q, T>
{
    public static abstract ICollection<T> Process(Q state);
}

internal class Pipeline
{
    public static Matrix4x4 viewMatrix;
    public static Matrix4x4 projMatrix;

    // Drawing
    static double[,] ZBuffer;
    static Bitmap bitmap;
    static Graphics currentGraphics;
    static PictureBox canvas;
    static Graphics g { get { return currentGraphics; } }
    static PictureBox Canvas { get { return canvas; } 
        set { canvas = value; ZBuffer = new double[Canvas.Width, Canvas.Height]; } }


    public static float fogFactor = 0.1f;
    static Vector3 constantColor;

    private static readonly ProcessingChain<IRenderable, Traiangle> shaders;

    static Pipeline()
    {
        Camera movingCam = new(
                new Vector3(3, 3, 30),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0));

        Pipeline.viewMatrix = movingCam.GetViewMatrix();
        shaders = new ProcessingChain<IRenderable, Traiangle>(
            VertexShader.Process,
            [
                ModelAndCameraTransofrmation.Process,
                Lighting.Process,
                Projection.Process,
                Clipping.Process,
                WindowViewpointTransformation.Process,
                DrawModule.Process,
            ]);
    }
    public Pipeline(PictureBox Canvas)
    {
        setProjectionMatrix(
            MathF.PI / 4, 
            Canvas.Width / Canvas.Height, 
            0.1f, 
            10000f);
    }
    public static void setProjectionMatrix(float fov, float aspectRatio, float nearPlane, float farPlane)
    {
        projMatrix = Matrix4x4.CreatePerspectiveFieldOfView(fov, aspectRatio, nearPlane, farPlane);
    }
    public void StartProcessing(ICollection<IRenderable> objects)
    {
        Parallel.ForEach(objects, 
            (IRenderable obj) => shaders.Execute(obj));
    }


    public class ModelAndCameraTransofrmation : IShader<Traiangle>
    {
        public static Traiangle Process(Traiangle state)
        {
            throw new NotImplementedException();
        }
    }
    class Lighting : IShader<Traiangle>
    {
        public static Traiangle Process(Traiangle state)
        {
            throw new NotImplementedException();
        }
    }

    class Projection : IShader<Traiangle>
    {
        public static Traiangle Process(Traiangle state)
        {
            throw new NotImplementedException();
        }
    }

    class Clipping : IShader<Traiangle>
    {
        public static Traiangle Process(Traiangle state)
        {
            throw new NotImplementedException();
        }
    }

    class WindowViewpointTransformation : IShader<Traiangle>
    {
        public static Traiangle Process(Traiangle state)
        {
            throw new NotImplementedException();
        }
    }
    class VertexShader : IVetexShader<IRenderable, Traiangle>
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
                newVerticies[i].Normal 
                    = Vector4.Transform(newVerticies[i].Normal, MV);
                newVerticies[i].Position 
                    = Vector4.Transform(newVerticies[i].Position, MVP);
                newVerticies[i].Position 
                    /= newVerticies[i].Position.W;
            };

            return newVerticies;
        }

        public static ICollection<Traiangle> Process(IRenderable renderable)
        {
            Vertex[] projectedPoints = VertexShader.UpdateVerticies(renderable);

            return
            [
                .. renderable.indices.Select((int[] indicies)
                                    => new Traiangle(
                                            projectedPoints[indicies[0]],
                                            projectedPoints[indicies[1]],
                                            projectedPoints[indicies[2]])).AsParallel(),
            ];
        }
    }
    class FragmentShader : IShader<Traiangle>
    {
        public static Traiangle Process(Traiangle state)
        {
            throw new NotImplementedException();
        }
    }
    class DrawModule
    {
        public void CalcaultePosition(ref Traiangle traiangle)
        {
            traiangle.Vertices[0].Position.X = calculateX(traiangle.Vertices[0].Position.X);
            traiangle.Vertices[0].Position.Y = calculateY(traiangle.Vertices[0].Position.Y);

            traiangle.Vertices[1].Position.X = calculateX(traiangle.Vertices[1].Position.X);
            traiangle.Vertices[1].Position.Y = calculateY(traiangle.Vertices[1].Position.Y);

            traiangle.Vertices[2].Position.X = calculateX(traiangle.Vertices[2].Position.X);
            traiangle.Vertices[2].Position.Y = calculateY(traiangle.Vertices[3].Position.Y);

            float calculateX(float x) => (x + 1) * Pipeline.Canvas.Width;
            float calculateY(float y) => (y + 1) * Pipeline.Canvas.Height;    
        }
        public static Traiangle Process(Traiangle state)
        {
            throw new NotImplementedException();
        }
    }
}

