using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System;
using Utils;
using System.Drawing;

namespace CPU_Rendering;

internal partial class Pipeline
{
    class DrawModule
    {
        // Drawing
        readonly double[,] zBuffer;
        readonly object[,] locks;
        readonly PictureBox canvas;
        readonly Rectangle rect;


        public DrawModule(PictureBox canvas)
        {
            this.canvas = canvas;
            // Math.Log2
            zBuffer = new double[canvas.Width, canvas.Height];
            locks = new object[canvas.Width, canvas.Height];
            rect = new Rectangle(0, 0, canvas.Width, canvas.Height);
        }

        // TODO: Implement drawing

        public void Process(IEnumerable<Pixel> pixels)
        {
            // TODO: Add 2 bitmaps switching
            Bitmap bitmap = new(
                canvas.Width, 
                canvas.Height);
            BitmapData data = bitmap.LockBits(
                rect, 
                ImageLockMode.ReadWrite,
                bitmap.PixelFormat);

            int depth = Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;

            byte[] buffer = new byte[canvas.Width * canvas.Height * depth];

            Parallel.ForEach(pixels,
                pixel => DrawPixel(ref pixel));

            Marshal.Copy(buffer, 0, data.Scan0, buffer.Length);

            bitmap.UnlockBits(data);
            canvas.Image = bitmap;


            void DrawPixel(ref Pixel pixel)
            {
                lock (locks[pixel.X, pixel.Y])
                {
                    double log2z = Math.Log2(pixel.Z);
                    if (log2z >= zBuffer[pixel.X, pixel.Y])
                        return;

                    zBuffer[pixel.X, pixel.Y] = log2z;

                    int offset = ((pixel.Y * canvas.Width) + pixel.X) * depth;
                    buffer[offset + 3] = 255;
                    buffer[offset + 2] = (byte)(pixel.Color.X * 255);
                    buffer[offset + 1] = (byte)(pixel.Color.Y * 255);
                    buffer[offset + 0] = (byte)(pixel.Color.Z * 255);
                }
            }
        }
    }
}
