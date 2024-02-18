using System.Numerics;
using Utils;

namespace CPU_Rendering;



internal partial class Pipeline
{
    class Projection : IShader<Traiangle>
    {
        public static Traiangle Process(Traiangle state)
        {
            updateVertex(ref state.Vertices[0]);
            updateVertex(ref state.Vertices[1]);
            updateVertex(ref state.Vertices[2]);

            return state;

            void updateVertex(ref Vertex vertex)
            {
                vertex.Position = Vector4.Transform(vertex.Position, Pipeline.projMatrix);
                vertex.Position /= vertex.Position.W;
            }
        }
    }
}
