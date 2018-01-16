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
        public Vector4 HPosition
        {
            get
            {
                return new Vector4(Position / Position.Z, 1/ Position.Z);
            }
        }
        public Vector3 Color;
        public Vector3 Normal;
        public Vector4 HNormal
        {
            get
            {
                return new Vector4(Normal / Position.Z, 1 / Position.Z);
            }
        }
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
        private float W;

        public Vertex(Vector3 position, Vector3 color, Vector2 textureUv, Vector3 normal)
        {
            Position = position;
            Color = color;
            TextureUv = textureUv;
            Normal = normal;
            W = 1;
        }

        public Vertex Project(Matrix4x4 m)
        {
            Matrix4x4.Invert(m, out var mat);
            mat = Matrix4x4.Transpose(mat);
            var pos = Vector4.Transform(Position, m);

            var vert = new Vertex(new Vector3(pos.X, pos.Y, pos.Z), Color, TextureUv, Vector3.Normalize(Vector3.TransformNormal(Normal, mat)));
            vert.W = pos.W;
            return vert;
        }

        public static Vertex Lerp(Vertex a, Vertex b, float amount)
        {

            var pos = Vector4.Lerp(a.HPosition, b.HPosition, amount);
            var col = Vector4.Lerp(a.HColor, b.HColor, amount);
            var uv = Vector3.Lerp(a.HTextureUv, b.HTextureUv, amount);
            var norm = Vector4.Lerp(a.HNormal, b.HNormal, amount);

            return new Vertex(
                new Vector3(pos.X, pos.Y, pos.Z) /pos.W,
                new Vector3(col.X, col.Y, col.Z) / col.W,
                new Vector2(uv.X, uv.Y) / uv.Z,
                new Vector3(norm.X, norm.Y, norm.Z) / norm.W
                );
        }

        public Vertex PerspectiveDivision()
        {
            return new Vertex(Position / W, Color, TextureUv, Normal);
        }
    }
}
