using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace triangles
{
    class Vertex
    {
        public Vector3 Position;
        public Vector3 Color;
        public Vector3 Normal;
        public Vector4 HColor
        {
            get
            {
                return new Vector4(Color / Position.Z, 1 / Position.Z);
            }
        }
        public Vector2 TextureUv;
        public Vector3 HTextureUv
        {
            get
            {
                return new Vector3(TextureUv / Position.Z, 1 / Position.Z);
            }
        }

        public Vertex(Vector3 position, Vector3 color, Vector2 textureUv, Vector3 normal)
        {
            Position = position;
            Color = color;
            TextureUv = textureUv;
            Normal = normal;
        }

        public Vertex Project(Matrix4x4 m)
        {
            Matrix4x4.Invert(m, out var mat);
            mat = Matrix4x4.Transpose(mat);
            
            var res = Vector4.Transform(Position, m);
            var pos = new Vector3(res.X / res.W, res.Y / res.W, res.W);

            return new Vertex(pos, Color, TextureUv, Vector3.TransformNormal(Normal, mat));
        }
    }
}
