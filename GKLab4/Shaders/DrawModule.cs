using Utils;

namespace CPU_Rendering;



internal partial class Pipeline
{
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
