using Objects;
using System.Numerics;
using Utils;

namespace CPU_Rendering;

public interface IShader<T>
    where T : struct
{
    public static abstract T? Process(T state);
}

public interface ITransformShader<Q, T>
    where T : struct
{
    public static abstract IEnumerable<T> Process(Q state);
}

internal partial class Pipeline
{
    static public Matrix4x4 ViewMatrix => currentCam.GetViewMatrix();
    static public Matrix4x4 ProjMatrix;

    // Global variables
    static List<Light> lights;
    static Vector3 backgroundColor;
    static Camera currentCam;

    private readonly ProcessingChain<IRenderable, Traiangle> geometryShaders;
    private readonly ProcessingChain<Traiangle, Pixel> fragmentShaders;
    private readonly DrawModule drawModule;

    public Pipeline(PictureBox Canvas)
    {
        Camera cam = new(
                new Vector3(3, 3, 30),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0));

        currentCam = cam;

        ProjMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
            MathF.PI / 4,
            Canvas.Width / Canvas.Height,
            0.1f,
            10000f);

        geometryShaders = new ProcessingChain<IRenderable, Traiangle>(
            VertexShader.Process,
            [
                // VertexLighting.Process,
                BackfaceCulling.Process,
                t => Clipping.Process(t, Canvas),
                Projection.Process,
            ]);

        fragmentShaders = new ProcessingChain<Traiangle, Pixel>(
            Rasterization.Process,
            [
                FragmentLighting.Process,
                Fog.Process,
            ]);

        lights = [];

        drawModule = new DrawModule(Canvas);
    }

    public void StartProcessing(ICollection<IRenderable> objects)
    {
        drawModule.Process(objects
            .AsParallel()
            .SelectMany(geometryShaders.Execute)
            .SelectMany(fragmentShaders.Execute));
    }
}

