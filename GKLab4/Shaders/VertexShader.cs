using System.Numerics;
using Objects;
using Utils;

namespace CPU_Rendering;

internal partial class Pipeline
{
    class VertexShader : IVetexShader<IRenderable, Traiangle>
    {
        private static Vertex[] UpdateVerticies(IRenderable renderable)
        {
            Vertex[] newVerticies = new Vertex[renderable.vertices.Length];
            Array.Copy(renderable.vertices, newVerticies, newVerticies.Length);

            Matrix4x4 MV = renderable.ModelMatrix * Pipeline.viewMatrix;

            for (int i = 0; i < newVerticies.Length; i++)
            {
                // TODO: fix normal transformation
                // TODO: add fog
                newVerticies[i].Normal
                    = Vector4.Transform(newVerticies[i].Normal, MV);
                newVerticies[i].Position
                    = Vector4.Transform(newVerticies[i].Position, MV);

                newVerticies[i].Position
                    /= newVerticies[i].Position.W;
            };

            return newVerticies;
        }
        public static ICollection<Traiangle> Process(IRenderable renderable)
        {
            Vertex[] updatedVertivies  = UpdateVerticies(renderable);
            return
            [
                .. renderable.indices.Select((int[] indicies)
                                    => new Traiangle(
                                            updatedVertivies[indicies[0]],
                                            updatedVertivies[indicies[1]],
                                            updatedVertivies[indicies[2]])).AsParallel(),
            ];
        }
    }
}
