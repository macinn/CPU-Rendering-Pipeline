using Objects;
using System.Numerics;
using Utils;

namespace CPU_Rendering;

public interface IShader<T>
{
    public static abstract T? Process(T state);
}

public interface ITransformShader<Q, T>
{
    public static abstract IEnumerable<T> Process(Q state);
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
    
    // Global variables
    static List<Light> lights;
    static Camera currentCam;

    public static float fogFactor = 0.1f;
    static Vector3 constantColor;

    private static readonly ProcessingChain<IRenderable, Traiangle> geometryShaders;
    private static readonly ProcessingChain<Traiangle, Pixel> fragmentShaders;

    static Pipeline()
    {
        Camera movingCam = new(
                new Vector3(3, 3, 30),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0));

        Pipeline.viewMatrix = movingCam.GetViewMatrix();

        geometryShaders = new ProcessingChain<IRenderable, Traiangle>(
            VertexShader.Process,
            [        
                BackfaceCulling.Process,
                Clipping.Process,
                Projection.Process,
            ]);

        fragmentShaders = new ProcessingChain<Traiangle, Pixel>(
            Rasterization.Process,
            []
            );
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
            (IRenderable obj) => geometryShaders.Execute(obj));
    }
}

