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
        public static Texture wall = new Texture("E:\\projects\\VisualStudio\\triangles\\triangles\\stones.bmp");
        private Bitmap image;
        private Texture(string path)
        {
            image = new Bitmap(path);
        }
        public Vector3 getColor(double u, double v)
        {
            var c = image.GetPixel((int)(u * image.Width), (int)(v * image.Height));
            return new Vector3(c.R/255f, c.G / 255f, c.B / 255f);
        }
    }
}
