using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Numerics;
namespace triangles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Vertex[] Points = {
            //top
            new Vertex(new Vector3(-1, -1, -1), new Vector3(0, 1, 1), new Vector2(0,0), -Vector3.UnitZ),//0
            new Vertex(new Vector3(+1, -1, -1), new Vector3(1, 0, 1), new Vector2(1,0), -Vector3.UnitZ),//1
            new Vertex(new Vector3(+1, +1, -1), new Vector3(1, 1, 0), new Vector2(1,1), -Vector3.UnitZ),//2
            new Vertex(new Vector3(-1, +1, -1), new Vector3(0, 0, 1), new Vector2(0,1), -Vector3.UnitZ),//3
            //bottom
            new Vertex(new Vector3(-1, -1, +1), new Vector3(0, 1, 0), new Vector2(0,0), Vector3.UnitZ),//4
            new Vertex(new Vector3(+1, -1, +1), new Vector3(1, 0, 0), new Vector2(0,1), Vector3.UnitZ),//5
            new Vertex(new Vector3(+1, +1, +1), new Vector3(0, 0, 0), new Vector2(1,1), Vector3.UnitZ),//6
            new Vertex(new Vector3(-1, +1, +1), new Vector3(1, 1, 1), new Vector2(1,0), Vector3.UnitZ),//7
            //left
            new Vertex(new Vector3(-1, -1, -1), new Vector3(0, 1, 1), new Vector2(0,0), -Vector3.UnitX),//8
            new Vertex(new Vector3(-1, +1, -1), new Vector3(0, 0, 1), new Vector2(0,1), -Vector3.UnitX),//9
            new Vertex(new Vector3(-1, +1, +1), new Vector3(1, 1, 1), new Vector2(1,1), -Vector3.UnitX),//10
            new Vertex(new Vector3(-1, -1, +1), new Vector3(0, 1, 0), new Vector2(1,0), -Vector3.UnitX),//11
            //right
            new Vertex(new Vector3(+1, +1, -1), new Vector3(1, 1, 0), new Vector2(1,1), Vector3.UnitX),//12
            new Vertex(new Vector3(+1, -1, -1), new Vector3(1, 0, 1), new Vector2(1,0), Vector3.UnitX),//13
            new Vertex(new Vector3(+1, -1, +1), new Vector3(1, 0, 0), new Vector2(0,0), Vector3.UnitX),//14
            new Vertex(new Vector3(+1, +1, +1), new Vector3(0, 0, 0), new Vector2(0,1), Vector3.UnitX),//15
            //front
            new Vertex(new Vector3(-1, +1, -1), new Vector3(0, 0, 1), new Vector2(0,0), Vector3.UnitY),//16
            new Vertex(new Vector3(+1, +1, -1), new Vector3(1, 1, 0), new Vector2(0,1), Vector3.UnitY),//17
            new Vertex(new Vector3(+1, +1, +1), new Vector3(0, 0, 0), new Vector2(1,1), Vector3.UnitY),//18
            new Vertex(new Vector3(-1, +1, +1), new Vector3(1, 1, 1), new Vector2(1,0), Vector3.UnitY),//19
            //back
            new Vertex(new Vector3(+1, -1, -1), new Vector3(1, 0, 1), new Vector2(0,0), -Vector3.UnitY),//20
            new Vertex(new Vector3(-1, -1, -1), new Vector3(0, 1, 1), new Vector2(0,1), -Vector3.UnitY),//21
            new Vertex(new Vector3(-1, -1, +1), new Vector3(0, 1, 0), new Vector2(1,1), -Vector3.UnitY),//22
            new Vertex(new Vector3(+1, -1, +1), new Vector3(1, 0, 0), new Vector2(1,0), -Vector3.UnitY),//23



            };


        private Vector3[] TriangleIdx =
        {
            new Vector3(0, 1, 2),//top
            new Vector3(0, 2, 3),
            new Vector3(7, 6, 5),//bottom
            new Vector3(7, 5, 4),
            new Vector3(8, 9, 10),//left
            new Vector3(8, 10, 11),
            new Vector3(12, 13, 14),//right
            new Vector3(12, 14, 15),
            new Vector3(16, 17, 18),//front
            new Vector3(16, 18, 19),
            new Vector3(20, 21, 22),//back
            new Vector3(20, 22, 23)
        };

        private int frame = 0;
        const int w = 400;
        const int h = 400;
        private WriteableBitmap bitmap = new WriteableBitmap(w, h, w, h, PixelFormats.Rgb24, null);
        byte[] pixels = new byte[w*h*3];
        float[] depthBuffer = new float[w * h];
        public MainWindow()
        {
            InitializeComponent();
            CompositionTarget.Rendering += Render;
            image.Source = bitmap;
        }
        
        private void Render(object sender, EventArgs e)
        {
            for(int i = 0; i < w*h*3; i++)
            {
                pixels[i] = 0xff;
            }
            for (int i = 0; i < w * h ; i++)
            {
                depthBuffer[i] = float.MaxValue;
            }
            var moveBack = Matrix4x4.CreateTranslation(0, 0, 5);
            var rotate = Matrix4x4.CreateFromYawPitchRoll((float)(frame / 100.0), (float)(frame / 50.0), (float)(frame / 75.0));
            var project = Matrix4x4.Transpose(new Matrix4x4(
                w, 0, w/2, 0,
                0, w, h/2, 0,
                0, 0, 0, 0,
                0, 0, 1, 0));

            var allTransforms = rotate * moveBack * project;
            frame++;
            canvas.Children.Clear();
            colorMod = 0;

            foreach (var index in TriangleIdx)
            {
                
                Triangle t = new Triangle(
                    Points[(int)index.X].Project(allTransforms),
                    Points[(int)index.Y].Project(allTransforms),
                    Points[(int)index.Z].Project(allTransforms));
                if (t.IsFacingCamera)
                {
                    var minX = Math.Min(Math.Min(t.A.Position.X, t.B.Position.X), t.C.Position.X);
                    minX = Math.Max(0, minX);
                    var maxX = Math.Max(Math.Max(t.A.Position.X, t.B.Position.X), t.C.Position.X);
                    maxX = Math.Min(w, maxX);
                    var minY = Math.Min(Math.Min(t.A.Position.Y, t.B.Position.Y), t.C.Position.Y);
                    minY = Math.Max(0, minY);
                    var maxY = Math.Max(Math.Max(t.A.Position.Y, t.B.Position.Y), t.C.Position.Y);
                    maxY = Math.Min(h, maxY);

                    for (int y = (int)minY; y < maxY; y++)
                    {
                        var determinant = t.Determinant;
                        for (int x = (int)minX; x < maxX; x++)
                        {

                            Vector2 ap = new Vector2(x - (float)t.A.Position.X, y - (float)t.A.Position.Y);
                            var u = determinant * (t.AC.Y * ap.X - t.AC.X * ap.Y);
                            var v = determinant * (-t.AB.Y * ap.X + t.AB.X * ap.Y);
                            if (u >= 0 && v >= 0 && (u + v) < 1)
                            {
                                float depth = t.A.Position.Z + u * (t.B.Position.Z - t.A.Position.Z) + v * (t.C.Position.Z - t.A.Position.Z);
                                if(depth>3.6 && depthBuffer[w * y + x] > depth)
                                {
                                    depthBuffer[w * y + x] = depth;
                                }
                            }
                        }
                    }
                }
            }


            foreach (var index in TriangleIdx)
            {

                Triangle t = new Triangle(
                    Points[(int)index.X].Project(allTransforms),
                    Points[(int)index.Y].Project(allTransforms),
                    Points[(int)index.Z].Project(allTransforms));
                if (t.IsFacingCamera)
                {
                    var minX = Math.Min(Math.Min(t.A.Position.X, t.B.Position.X), t.C.Position.X);
                    minX = Math.Max(0, minX);
                    var maxX = Math.Max(Math.Max(t.A.Position.X, t.B.Position.X), t.C.Position.X);
                    maxX = Math.Min(w, maxX);
                    var minY = Math.Min(Math.Min(t.A.Position.Y, t.B.Position.Y), t.C.Position.Y);
                    minY = Math.Max(0, minY);
                    var maxY = Math.Max(Math.Max(t.A.Position.Y, t.B.Position.Y), t.C.Position.Y);
                    maxY = Math.Min(h, maxY);

                    for (int y = (int)minY; y < maxY; y++)
                    {
                        var determinant = t.Determinant;
                        for (int x = (int)minX; x < maxX; x++)
                        {

                            Vector2 ap = new Vector2(x - (float)t.A.Position.X, y - (float)t.A.Position.Y);
                            var u = determinant * (t.AC.Y * ap.X - t.AC.X * ap.Y);
                            var v = determinant * (-t.AB.Y * ap.X + t.AB.X * ap.Y);
                            if (u >= 0 && v >= 0 && (u + v) < 1)
                            {
                                float depth = t.A.Position.Z + u * (t.B.Position.Z - t.A.Position.Z) + v * (t.C.Position.Z - t.A.Position.Z);
                                if (depth > 3.6 && depthBuffer[w * y + x] == depth)
                                {
                                    Vector3 c = t.getColor(u, v);
                                    DrawPixel(x, y, Color.FromScRgb(1, c.X, c.Y, c.Z));
                                }
                            }
                        }
                    }
                }
            }

            bitmap.Lock();
            bitmap.WritePixels(new Int32Rect(0, 0, w, h), pixels, w*3, 0);
            bitmap.Unlock();

        }

        private static int colorMod = 0;

        private static Vector3 getRandomColor()
        {
            colorMod++;

            return new Vector3((colorMod / 1) % 2, (colorMod / 2) % 2, (colorMod / 4) % 2);
        }

        private Vertex Project1(Vertex v, Matrix4x4 m)
        {
            var p = v.Position;
            var res = Vector4.Transform(p, m);
            var pos = new Vector3(res.X / res.W, res.Y / res.W, res.W);
            return new Vertex(pos, v.Color, v.TextureUv, v.Normal);
        }

        private void DrawPixel(int x, int y, Color color)
        {
            pixels[3*(w * y + x)] = color.R;
            pixels[3*(w * y + x )+1] = color.G;
            pixels[3*(w * y + x )+2] = color.B;
        }
    }
}
