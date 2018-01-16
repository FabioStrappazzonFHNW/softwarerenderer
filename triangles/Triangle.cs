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
                return new Vector2((float)B.PosProject.X - (float)A.PosProject.X, (float)B.PosProject.Y - (float)A.PosProject.Y);
            }
        }
        public Vector2 AC
        {
            get
            {
                return new Vector2((float)C.PosProject.X - (float)A.PosProject.X, (float)C.PosProject.Y - (float)A.PosProject.Y);
            }
        }

        public float Determinant
        {
            get
            {
                return 1 / (AB.X * AC.Y - AC.X * AB.Y);
            }
        }

        public Vector3 getNormal(float u, float v)
        {
            var normal = (A.HNormal + u * (B.HNormal - A.HNormal) + v * (C.HNormal - A.HNormal));
            normal /= normal.W;
            return new Vector3(normal.X, normal.Y, normal.Z);
        }

        public Vector3 getPosition(float u, float v)
        {
            var pos = (A.HPosition + u * (B.HPosition - A.HPosition) + v * (C.HPosition - A.HPosition));
            pos /= pos.W;
            return new Vector3(pos.X, pos.Y, pos.Z);
        }

        public Vector3 getPosP(float u, float v)
        {
            var pos = (A.HPosProject + u * (B.HPosProject - A.HPosProject) + v * (C.HPosProject - A.HPosProject));
            pos /= pos.W;
            return new Vector3(pos.X, pos.Y, pos.Z);
        }

        public Vector3 getColor(float u, float v)
        {
            
            var w = Texture.wall;
            var uvPos = (A.HTextureUv + u * (B.HTextureUv - A.HTextureUv) + v * (C.HTextureUv - A.HTextureUv));
            uvPos /= uvPos.Z;
            var c =  w.getColor(uvPos.X, uvPos.Y);


            var col = (A.HColor + u * (B.HColor - A.HColor) + v * (C.HColor - A.HColor));
            col /= col.W;
            return new Vector3(col.X, col.Y, col.Z) * c;
        }

        public Triangle(Vertex a, Vertex b, Vertex c)
        {
            A = a;
            B = b;
            C = c;
        }
    }
}
