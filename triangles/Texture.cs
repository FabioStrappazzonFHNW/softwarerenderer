using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace triangles
{
    class Texture
    {
        public static Texture wall = new Texture("./stones.bmp");
        private Bitmap image;
        private Texture(string path)
        {
            image = new Bitmap(path);
        }
        public Vector3 getColor(double u, double v)
        {
            var actualPos = new Vector2((float)u * (image.Width), (float)v * image.Height);

            var left = (int)actualPos.X;
            var right = Math.Min((int)actualPos.X + 1, image.Width-1);
            var bottom = Math.Min((int)actualPos.Y + 1, image.Height - 1);
            var top = (int)actualPos.Y;

            var topLeft = image.GetPixel(left, top);
            var bottomLeft = image.GetPixel(left, bottom);
            var topRight = image.GetPixel(right, top);
            var bottomRight = image.GetPixel(right, bottom);

            var leftToRight = actualPos.X - left;
            var topToBottom = actualPos.Y - top;

            var leftColor = Lerp(topLeft, bottomLeft, topToBottom);
            var rightColor = Lerp(topRight, bottomRight, topToBottom);
            var centerColor = Vector3.Lerp(leftColor, rightColor, leftToRight);
            return centerColor / 255;
        }

        private Vector3 Lerp(Color a, Color b, float val)
        {
            return Vector3.Lerp(new Vector3(a.R, a.G, a.B), new Vector3(b.R, b.G, b.B), val);
        }
    }
}
