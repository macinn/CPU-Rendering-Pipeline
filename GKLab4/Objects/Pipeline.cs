using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Objects;
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
internal partial class Pipeline
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
                CameraTransofrmation.Process,
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

}

