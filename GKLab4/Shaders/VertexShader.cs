using System.Numerics;
using Objects;
using Utils;

namespace CPU_Rendering;

internal partial class Pipeline
{
    class VertexShader : ITransformShader<IRenderable, Traiangle>
    {
        private static Vertex[] UpdateVerticies(IRenderable renderable)
        {
            Vertex[] newVerticies = new Vertex[renderable.vertices.Length];
            Array.Copy(renderable.vertices, newVerticies, newVerticies.Length);

            Matrix4x4 MV = renderable.ModelMatrix * Pipeline.viewMatrix;

            for (int i = 0; i < newVerticies.Length; i++)
            {
                // TODO: fix normal transformation
                newVerticies[i].Normal
                    = Vector3.Transform(newVerticies[i].Normal, MV);

                newVerticies[i].Position
                    = Vector4.Transform(newVerticies[i].Position, MV);
            };

            return newVerticies;
        }
        public static IEnumerable<Traiangle> Process(IRenderable renderable)
        {
            Vertex[] updatedVertivies  = UpdateVerticies(renderable);
            return renderable.indices.Select((int[] indicies)
                                    => new Traiangle(
                                            updatedVertivies[indicies[0]],
                                            updatedVertivies[indicies[1]],
                                            updatedVertivies[indicies[2]]));
        }
    }
}
