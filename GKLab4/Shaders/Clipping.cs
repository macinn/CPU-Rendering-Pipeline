using Utils;

namespace CPU_Rendering;



internal partial class Pipeline
{
    class Clipping
    {
        public static Traiangle? Process(Traiangle state, PictureBox? canvas = null)
        {
            // TODO: Implement clipping
            bool isInside = isBelow(state.Vertices[0]) 
                && isBelow(state.Vertices[1]) 
                && isBelow(state.Vertices[2]);

            return isInside ? state : null;

            static bool isBelow(Vertex v)
            {
                return v.Position.X < 0 && v.Position.Y < 0;
            }
        }
    }
}
