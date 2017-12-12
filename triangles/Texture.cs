using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace triangles
{
    class Texture
    {
        public static Texture wall = new Texture("./stones.bmp");
        private int w;
        private int h;

        private Vector3[] imageData;
        private Texture(string path)
        {
            var image = new Bitmap(path);
            w = image.Width;
            h = image.Height;
            imageData = new Vector3[w*h];
            BitmapData data = image.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* p = (byte*)data.Scan0;
                for (int i = 0; i < w * h; i++)
                {
                    var r = *(p++);
                    var g = *(p++);
                    var b = *(p++);

                    imageData[i] = new Vector3(r , g , b )/255;
                }

                image.UnlockBits(data);
            }
        }
        public Vector3 getColor(double u, double v)
        {
            var actualPos = new Vector2((float)u * w, (float)v * h);

            var left = (int)actualPos.X;
            var right = Math.Min((int)actualPos.X + 1, w-1);
            var bottom = Math.Min((int)actualPos.Y + 1, w - 1);
            var top = (int)actualPos.Y;

            var topLeft = GetPixel(left, top);
            var bottomLeft = GetPixel(left, bottom);
            var topRight = GetPixel(right, top);
            var bottomRight = GetPixel(right, bottom);

            var leftToRight = actualPos.X - left;
            var topToBottom = actualPos.Y - top;

            var leftColor = Vector3.Lerp(topLeft, bottomLeft, topToBottom);
            var rightColor = Vector3.Lerp(topRight, bottomRight, topToBottom);
            var centerColor = Vector3.Lerp(leftColor, rightColor, leftToRight);
            return centerColor ;
        }

        private Vector3 GetPixel(int x, int y)
        {
            return imageData[Math.Min(y * w + x, (w*h)-1)];
        }
    }
}
