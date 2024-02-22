using Objects;
using System.Numerics;
using Utils;

namespace CPU_Rendering;



internal partial class Pipeline
{
    class VertexLighting : IShader<Traiangle>
    {
        public static Traiangle Process(Traiangle state)
        {
            Vector3[] I = new Vector3[3];
            foreach (Light light in lights)
            {
                I[0] += light.getIntensity(state.Vertices[0], Pipeline.currentCam.Position);
                I[1] += light.getIntensity(state.Vertices[1], Pipeline.currentCam.Position);
                I[2] += light.getIntensity(state.Vertices[2], Pipeline.currentCam.Position);
            }

            state.Vertices[0].Color = Vector3.Multiply(state.Vertices[0].Color, I[0]);
            state.Vertices[1].Color = Vector3.Multiply(state.Vertices[1].Color, I[1]);
            state.Vertices[2].Color = Vector3.Multiply(state.Vertices[2].Color, I[2]);

            return state;
        }
    }
}
