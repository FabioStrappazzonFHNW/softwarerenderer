using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace triangles
{
    class Triangle
    {
        public Vertex A { get;}
        public Vertex B { get;}
        public Vertex C { get;}
        public bool IsFacingCamera
        {
            get
            {

                return Vector3.Cross(new Vector3(AB, 0), new Vector3(AC, 0)).Z>=0;
            }
        }

        public Vector2 AB
        {
            get
            {
                return new Vector2((float)B.Position.X - (float)A.Position.X, (float)B.Position.Y - (float)A.Position.Y);
            }
        }
        public Vector2 AC
        {
            get
            {
                return new Vector2((float)C.Position.X - (float)A.Position.X, (float)C.Position.Y - (float)A.Position.Y);
            }
        }

        public float Determinant
        {
            get
            {
                return 1 / (AB.X * AC.Y - AC.X * AB.Y);
            }
        }

        public Vector3 getColor(float u, float v)
        {
            var w = Texture.wall;
            var pos = (A.HTextureUv + u * (B.HTextureUv - A.HTextureUv) + v * (C.HTextureUv - A.HTextureUv));
            pos /= pos.Z;
            return w.getColor(pos.X, pos.Y);


            var col = (A.HColor + u * (B.HColor - A.HColor) + v * (C.HColor - A.HColor));
            col /= col.W;
            return new Vector3(col.X, col.Y, col.Z);
        }

        public Triangle(Vertex a, Vertex b, Vertex c)
        {
            A = a;
            B = b;
            C = c;
        }
    }
}
