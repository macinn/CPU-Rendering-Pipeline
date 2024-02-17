using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Objects;
using Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Deprecated
{
    public enum ShadingType
    {
        Constant,
        Gouraud,
        Phong
    }

    public static class RenderStream
    {
        static PictureBox canvas;
        static int W { get { return canvas.Width; } }
        static int H { get { return canvas.Height; } }

        static double[,] ZBuffer;
        static Bitmap bitmap;
        public static List<Light> lights = new List<Light>();

        static List<Camera> cameras = new List<Camera>();
        static int index = 0;
        public static Camera currentCam;
        public static bool backFaceCulling = true;
        public static float fogFactor = 0.1f;
        static Vector3 constantColor;

        public static ShadingType shadingType = ShadingType.Phong;
        public static void AddCamera(Camera camera)
        {
            cameras.Add(camera);
            if (currentCam == null)
            {
                currentCam = camera;
            }
        }
        public static void setUp(PictureBox canvas)
        {
            RenderStream.canvas = canvas;
            ZBuffer = new double[W, H];
        }
        public static void addLight(Light light)
        {

            lights.Add(light);
        }

        private static Graphics currentG;
        static Graphics g { get { return currentG; } }
        public static void startDraw()
        {
            bitmap = new Bitmap(W, H);
            currentG = Graphics.FromImage(bitmap);



            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    ZBuffer[i, j] = double.PositiveInfinity;
                }
            }
        }
        public static void flush()
        {
            canvas.Image = bitmap;
            g.Dispose();
        }
        private static void putLine(Vertex p1, Vertex p2)
        {
            if (!float.IsFinite(p1.Position.X) || !float.IsFinite(p1.Position.Y))
                return;
            if (!float.IsFinite(p2.Position.X) || !float.IsFinite(p2.Position.Y))
                return;

            //https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
            int DY = (int)(p2.Position.Y - p1.Position.Y);
            int DX = (int)(p2.Position.X - p1.Position.X);
            if (Math.Abs(DY) < Math.Abs(DX))
            {
                if (p1.Position.X > p2.Position.X)
                    plotLineLow(p2, p1);
                else
                    plotLineLow(p1, p2);
            }
            else
            {
                if (p1.Position.Y > p2.Position.Y)
                    plotLineHigh(p2, p1);
                else
                    plotLineHigh(p1, p2);
            }

            void plotLineHigh(Vertex point1, Vertex point2)
            {
                int dx = (int)(point2.Position.X - point1.Position.X);
                int dy = (int)(point2.Position.Y - point1.Position.Y);
                int xi = 1;
                if (dx < 0)
                {
                    xi = -1;
                    dx = -dx;
                }
                int D = 2 * dx - dy;
                int x1 = (int)point1.Position.X;
                for (int y1 = (int)point1.Position.Y; y1 <= point2.Position.Y; y1++)
                {
                    putPixel(Lerp(x1, y1, point1, point2));

                    if (D > 0)
                    {
                        x1 += xi;
                        D -= 2 * dy;
                    }
                    D += 2 * dx;
                }
            }
            void plotLineLow(Vertex point0, Vertex point1)
            {
                int dx = (int)(point1.Position.X - point0.Position.X);
                int dy = (int)(point1.Position.Y - point0.Position.Y);
                int yi = 1;
                if (dy < 0)
                {
                    yi = -1;
                    dy = -dy;
                }
                int D = 2 * dy - dx;
                int y1 = (int)point0.Position.Y;
                for (int x1 = (int)point0.Position.X; x1 <= point1.Position.X; x1++)
                {
                    putPixel(Lerp(x1, y1, point0, point1));

                    if (D >= 0)
                    {
                        y1 += yi;
                        D -= 2 * dx;
                    }
                    D += 2 * dy;
                }
            }
        }
        private static void putPixel(Vertex vertex)
        {
            int X = (int)vertex.Position.X;
            int Y = (int)vertex.Position.Y;

            if (X < 0 || X >= W || Y < 0 || Y >= H)
                return;
            if (-Math.Log2(vertex.Position.Z) < ZBuffer[X, Y])
            {
                ZBuffer[X, Y] = -Math.Log2(vertex.Position.Z);
                //g.DrawEllipse(new Pen(getColor(vertex)), X, Y, 1, 1);
                if (shadingType == ShadingType.Phong)
                {
                    updateColor(ref vertex);
                }
                g.DrawEllipse(new Pen(getColorVector(vertex.Color)), X, Y, 1, 1);
            }

            Color getColorVector(Vector4 color)
            {
                color = Vector4.Clamp(color, Vector4.Zero, Vector4.One);
                return Color.FromArgb(255, (int)(color.X * 255f), (int)(color.Y * 255f), (int)(color.Z * 255f));
            }
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
                Color = Vector4.Lerp(v1.Color, v2.Color, amount),
                Normal = Vector4.Lerp(v1.Normal, v2.Normal, amount)
            };
            if (validXY)
            {
                v.Position.X = X;
                v.Position.Y = Y;
            }
            return v;
        }
        public static void drawTriangle(Vertex[] Points)
        {
            // TODO: Clipping
            {
                if (Math.Abs(Points[0].Position.Y) > 1 && Math.Abs(Points[1].Position.Y) > 1 && Math.Abs(Points[2].Position.Y) > 1)
                {
                    return;
                }
                if (Math.Abs(Points[0].Position.X) > 1 && Math.Abs(Points[1].Position.X) > 1 && Math.Abs(Points[2].Position.X) > 1)
                {
                    return;
                }
            }
            if (backFace(Points[0], Points[1], Points[2]))
                return;

            if (shadingType == ShadingType.Gouraud)
            {
                updateColor(ref Points[0]);
                updateColor(ref Points[1]);
                updateColor(ref Points[2]);
            }
            Vertex P1 = Points[0];
            Vertex P2 = Points[1];
            Vertex P3 = Points[2];
            Vector4 constantNormal = (P1.Normal + P2.Normal + P3.Normal) / 3;

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

            P1.Position.X = (P1.Position.X + 1) * W / 2;
            P1.Position.Y = (P1.Position.Y + 1) * H / 2;

            P2.Position.X = (P2.Position.X + 1) * W / 2;
            P2.Position.Y = (P2.Position.Y + 1) * H / 2;

            P3.Position.X = (P3.Position.X + 1) * W / 2;
            P3.Position.Y = (P3.Position.Y + 1) * H / 2;


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
                if (shadingType == ShadingType.Constant)
                {
                    V1.Normal = constantNormal;
                    V2.Normal = constantNormal;
                }

                putLine(V1, V2);
            }
        }
        private static Vertex interpolate(int X, int Y, Vertex vA, Vertex vB, Vertex vC)
        {
            // Compute barycentric coordinates
            Vector4 A = vA.Position;
            Vector4 B = vB.Position;
            Vector4 C = vC.Position;

            float alpha = ((B.Y - C.Y) * (X - C.X) + (C.X - B.X) * (Y - C.Y)) /
                          ((B.Y - C.Y) * (A.X - C.X) + (C.X - B.X) * (A.Y - C.Y));

            float beta = ((C.Y - A.Y) * (X - C.X) + (A.X - C.X) * (Y - C.Y)) /
                         ((B.Y - C.Y) * (A.X - C.X) + (C.X - B.X) * (A.Y - C.Y));

            float gamma = 1 - alpha - beta;

            return new Vertex() { Position = alpha * A + beta * B + gamma * C };
        }
        public static void drawLine(Vertex p1, Vertex p2)
        {
            p1.Position.X = (p1.Position.X + 1) * W / 2;
            p1.Position.Y = (p1.Position.Y + 1) * H / 2;
            p2.Position.X = (p2.Position.X + 1) * W / 2;
            p2.Position.Y = (p2.Position.Y + 1) * H / 2;
            putLine(p1, p2);
        }
        private static bool backVeretx(Vertex vertex)
        {
            Vector3 V = Vector3.UnitZ;
            return backFaceCulling && Vector3.Dot(new Vector3(vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z),
                 V) > 0;
        }
        public static bool backFace(Vertex v1, Vertex v2, Vertex v3)
        {
            return backVeretx(v1) && backVeretx(v2) && backVeretx(v3);
        }
        private static void updateColor(ref Vertex vertex)
        {
            Vector4 I = new Vector4(0f);
            foreach (Light light in lights)
            {
                I += light.getIntensity(vertex, currentCam.Position);
            }
            vertex.Color = new Vector4(I.X * vertex.Color.X, I.Y * vertex.Color.Y, I.Z * vertex.Color.Z, 1);
        }
        internal static float FogFactor(Vector4 z)
        {
            Vector3 L = currentCam.Position - new Vector3(z.X, z.Y, z.Z);
            return (float)Math.Exp(-fogFactor / 10 * L.Length());
        }

        internal static void changeCamera()
        {
            currentCam = cameras[++index % cameras.Count];
        }
    }
}
