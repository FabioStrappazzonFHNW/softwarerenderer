using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Numerics;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        Vector3[] normalBuffer = new Vector3[w * h];
        Vector3[] positionBuffer = new Vector3[w * h];
        Vector3[] colorBuffer = new Vector3[w * h];
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

            var allTransforms = rotate * moveBack;
            frame++;
            List<Triangle> triangles = new List<Triangle>();
            float zPlane = 3.7f;

            foreach (var index in TriangleIdx)
            {

                var a = Points[(int)index.X].Project(allTransforms);
                var b = Points[(int)index.Y].Project(allTransforms);
                var c = Points[(int)index.Z].Project(allTransforms);
                List<Vertex> verts = new List<Vertex>();
                if (a.Position.Z > zPlane)
                {
                    verts.Add(a);
                }
                intersectPlane(a, b, zPlane, verts);
                if (b.Position.Z > zPlane)
                {
                    verts.Add(b);
                }
                intersectPlane(b, c, zPlane, verts);
                if (c.Position.Z > zPlane)
                {
                    verts.Add(c);
                }
                intersectPlane(c, a, zPlane, verts);

                if (verts.Count == 3)
                {
                    triangles.Add(new Triangle(verts[0], verts[1], verts[2]));
                }else if(verts.Count == 4)
                {
                    triangles.Add(new Triangle(verts[0], verts[1], verts[2]));
                    triangles.Add(new Triangle(verts[0], verts[2], verts[3]));

                }
            }

            foreach(Triangle tr in triangles) {

                var t = tr.Project(project).PerspectiveDivision();
                
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
                                var pos = tr.getPosition(u, v);
                                float depth = pos.Z;
                                var bufferIndex = w * y + x;
                                if (depthBuffer[bufferIndex] > depth)
                                {
                                    depthBuffer[bufferIndex] = depth;
                                    normalBuffer[bufferIndex] = tr.getNormal(u, v);
                                    positionBuffer[bufferIndex] = pos;
                                    colorBuffer[bufferIndex] = tr.getColor(u, v);

                                }
                            }

                        }
                    }
                }
            }
            Parallel.For(0, w * h, i =>
              {
                if(depthBuffer[i] != float.MaxValue)
                {
                      var normal = normalBuffer[i];
                      var position = positionBuffer[i];

                      var light = new Vector3(-1, 0, -5);
                      var toLight = Vector3.Normalize(light - position);
                      var diffuse =  Math.Max((Vector3.Dot(normal, toLight)), 0);

                      var eye = new Vector3(0, 0, 0);
                      var viewDir = Vector3.Normalize(position-eye);
                      var specularDir = normal * (2 * Vector3.Dot(normal, toLight)) - toLight;
                      specularDir = Vector3.Normalize(specularDir);
                      var specular = new Vector3(0.8f, 0.8f, 0.8f) * (float)Math.Pow(Math.Max(0.0, -Vector3.Dot(specularDir, viewDir)), 50);
                      var col = colorBuffer[i];
                      var c = new Vector3(0.1f, 0.1f, 0.1f)*col+diffuse*col+specular;
                      c = new Vector3(0.1f, 0.1f, 0.1f) * col;
                      DrawPixel(i, Color.FromScRgb(1, c.X, c.Y, c.Z));
                  }

                
              });
            
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

        private void DrawPixel(int i, Color color)
        {
            pixels[3*i] = color.R;
            pixels[3*i+1] = color.G;
            pixels[3*i+2] = color.B;
        }

        private void intersectPlane(Vertex a, Vertex b, float zPlane, List<Vertex> verts)
        {
            var dir = b.Position - a.Position;
            var planeN = new Vector3(0, 0, 1);
            if (Vector3.Dot(dir, planeN) != 0)
            {
                var planeP = new Vector3(0, 0, zPlane);
                var distance = Vector3.Dot((planeP - a.Position), planeN) / Vector3.Dot(dir, planeN);
                if (distance > 0 && distance < 1)
                {
                    var point = Vertex.Lerp(a, b, distance);
                    verts.Add(point);
                }
            }
        }
    }
}
