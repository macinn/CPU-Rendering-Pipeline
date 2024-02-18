using System.Numerics;
using Utils;

namespace CPU_Rendering
{
    internal class Rasterization : ITransformShader<Traiangle, Pixel>
    {
        public static IEnumerable<Pixel> Process(Traiangle state)
        {
            Vertex P1 = state.Vertices[0];
            Vertex P2 = state.Vertices[1];
            Vertex P3 = state.Vertices[2];


            if (P1.Position.Y > P2.Position.Y)
            {
                (P2, P1) = (P1, P2);
            }
            if (P2.Position.Y > P3.Position.Y)
            {
                (P2, P3) = (P3, P2);
            }
            if (P1.Position.Y > P2.Position.Y)
            {
                (P2, P1) = (P1, P2);
            }

            for (int y = (int)P1.Position.Y; y <= (int)P3.Position.Y; y++)
            {
                Vertex V1, V2;
                if (y < (int)P2.Position.Y)
                {
                    // P1 - P2, P1 - P3
                    V1 = Lerp(-1, y, P1, P2);
                    V2 = Lerp(-1, y, P1, P3);
                }
                else
                {
                    // P1 - P3, P2 - P3
                    V1 = Lerp(-1, y, P1, P3);
                    V2 = Lerp(-1, y, P2, P3);
                }

                foreach (Pixel item in ComputeLineBresenham(V1, V2))
                    yield return item;
            }

            static IEnumerable<Pixel> ComputeLineBresenham(Vertex p1, Vertex p2)
            {
                int DY = (int)(p2.Position.Y - p1.Position.Y);
                int DX = (int)(p2.Position.X - p1.Position.X);

                if (Math.Abs(DY) < Math.Abs(DX))
                {
                    if (p1.Position.X > p2.Position.X)
                        foreach (Pixel item in compLineLow(p2, p1))
                            yield return item;
                    else
                        foreach (Pixel item in compLineLow(p1, p2))
                            yield return item;
                }
                else
                {
                    if (p1.Position.Y > p2.Position.Y)
                        foreach (Pixel item in compLineHigh(p2, p1))
                            yield return item;
                    else
                        foreach (Pixel item in compLineHigh(p1, p2))
                            yield return item;
                }

                yield break;

                IEnumerable<Pixel> compLineHigh(Vertex vertex0, Vertex vertex1)
                {
                    int dx = (int)(vertex1.Position.X - vertex0.Position.X);
                    int dy = (int)(vertex1.Position.Y - vertex0.Position.Y);
                    int xi = 1;
                    if (dx < 0)
                    {
                        xi = -1;
                        dx = -dx;
                    }
                    int D = 2 * dx - dy;
                    int x1 = (int)vertex0.Position.X;
                    for (int y1 = (int)vertex0.Position.Y; y1 <= vertex1.Position.Y; y1++)
                    {
                        yield return CreatePixel(x1, y1, vertex0, vertex1);

                        if (D > 0)
                        {
                            x1 += xi;
                            D -= 2 * dy;
                        }
                        D += 2 * dx;
                    }
                }

                IEnumerable<Pixel> compLineLow(Vertex vertex0, Vertex vertex1)
                {
                    int dx = (int)(vertex1.Position.X - vertex0.Position.X);
                    int dy = (int)(vertex1.Position.Y - vertex0.Position.Y);
                    int yi = 1;
                    if (dy < 0)
                    {
                        yi = -1;
                        dy = -dy;
                    }
                    int D = 2 * dy - dx;
                    int y1 = (int)vertex0.Position.Y;
                    for (int x1 = (int)vertex0.Position.X; x1 <= vertex1.Position.X; x1++)
                    {
                        yield return CreatePixel(x1, y1, vertex0, vertex1);

                        if (D >= 0)
                        {
                            y1 += yi;
                            D -= 2 * dx;
                        }
                        D += 2 * dy;
                    }
                }
            }
        }

        private static Pixel CreatePixel(int X, int Y, Vertex v1, Vertex v2)
        {
            float xDiff = v2.Position.X - v1.Position.X;
            float yDiff = v2.Position.Y - v1.Position.Y;
            float amount;

            if (xDiff > yDiff)
            {
                amount = (X - v1.Position.X) / xDiff;
            }
            else
            {
                amount = (Y - v1.Position.Y) / yDiff;
            }
            amount = Math.Clamp(amount, 0, 1);

            return new Pixel(
                X,
                Y,
                amount * v1.Position.Z + (1 - amount) * v2.Position.Z
                , Vector3.Clamp(
                    Vector3.Lerp(v1.Color, v2.Color, amount),
                    Vector3.Zero, Vector3.One));
        }
        private static Vertex Lerp(int X, int Y, Vertex v1, Vertex v2)
        {
            float xDiff = v2.Position.X - v1.Position.X;
            float yDiff = v2.Position.Y - v1.Position.Y;
            float amount;
            bool validXY = X != -1 && Y != -1;
            if (xDiff > yDiff && X != -1)
            {
                amount = (X - v1.Position.X) / xDiff;
            }
            else
            {
                amount = (Y - v1.Position.Y) / yDiff;
            }
            amount = Math.Clamp(amount, 0, 1);

            if (yDiff == 0 && xDiff == 0)
            {
                amount = 1;
            }
            Vertex v = new Vertex
            {
                Position = Vector4.Lerp(v1.Position, v2.Position, amount),
                Color = Vector3.Lerp(v1.Color, v2.Color, amount),
                Normal = Vector3.Lerp(v1.Normal, v2.Normal, amount)
            };
            if (validXY)
            {
                v.Position.X = X;
                v.Position.Y = Y;
            }
            return v;
        }
    }
}