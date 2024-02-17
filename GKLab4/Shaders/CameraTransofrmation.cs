using System.Numerics;
using Utils;

namespace CPU_Rendering;

internal partial class Pipeline
{
    class CameraTransofrmation : IShader<Traiangle>
    {
        public static Traiangle Process(Traiangle state)
        {
            updateVertex(ref state.Vertices[0]);
            updateVertex(ref state.Vertices[1]);
            updateVertex(ref state.Vertices[2]);

            return state;

            void updateVector(ref Vector4 vector) 
                => vector = Vector4.Transform(vector, viewMatrix);

            void updateVertex(ref Vertex vertex)
            {
                updateVector(ref vertex.Position);
                updateVector(ref vertex.Position);
            }
        }
    }
}
