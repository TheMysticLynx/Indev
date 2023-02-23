using System.Linq;
using UnityEngine;

namespace Scripts.C_
{
    public readonly struct RenderInfo {
        private static readonly int MainTexPropertyId = Shader.PropertyToID("_MainTex");
        private static readonly int TexturePropertyId = Shader.PropertyToID("_Texture");

        public readonly Mesh mesh;
        public readonly Material mat;

        public RenderInfo(Mesh m, Material material) {
            mesh = m;
            mat = material;
        }

        public static RenderInfo FromSprite(Material baseMaterial, Sprite s) {
            var renderMaterial = UnityEngine.Object.Instantiate(baseMaterial);

            renderMaterial.enableInstancing = true;
            renderMaterial.SetTexture(MainTexPropertyId, s.texture);
            var m = new Mesh {
                vertices = s.vertices.Select(v => (Vector3)v).ToArray(),
                triangles = s.triangles.Select(t => (int)t).ToArray(),
                uv = s.uv
            };

            return new RenderInfo(m, renderMaterial);
        }
    }
}