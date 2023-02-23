using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Scripts.C_
{
    public class CellRenderer : MonoBehaviour
    {
        private static readonly int PosDirPropertyId = Shader.PropertyToID("posDirBuffer");

        public Material material;
        public Sprite defaultSprite;
        public Sprite[] sprites;
        public string renderLayer = "Default";

        private const int batchSize = 7;
        private readonly Vector4[] posDirBuffer = new Vector4[batchSize];
        private readonly float[] textureIndexBuffer = new float[batchSize];

        private MaterialPropertyBlock _propertyBlock;
        private int _renderLayer;
        private RenderInfo _renderInfo;

        private void Start() {
            _propertyBlock = new MaterialPropertyBlock();
            _renderLayer = LayerMask.NameToLayer(renderLayer);
            _renderInfo = RenderInfo.FromSprite(material, defaultSprite);
            //Set Texture2DArray in material to sprites
            // Create Texture2DArray
            Texture2DArray texture2DArray = new
                Texture2DArray(sprites[0].texture.width, sprites[0].texture.height, sprites.Length, TextureFormat.RGBA32, false);
            // Apply settings
            texture2DArray.filterMode = FilterMode.Bilinear;
            texture2DArray.wrapMode = TextureWrapMode.Repeat;
            // Loop through ordinary textures and copy pixels to the
            // Texture2DArray
            for (int i = 0; i < sprites.Length; i++)
            {
                texture2DArray.SetPixels(
                    sprites[i].texture.GetPixels(), i);
            }
            // Apply our changes
            texture2DArray.Apply();

            _renderInfo.mat.SetTexture("_CellTextures", texture2DArray);
            material.SetTexture("_CellTextures", texture2DArray);

            Cell cell = new Cell(0, 0, 2, 0);
            Cell cell2 = new Cell(2, 2, 1, 3);
            Cell cell3 = new Cell(5, 3, 3, 2);

            Camera.onPreCull += Render;
        }

        private void OnDestroy()
        {
            Camera.onPreCull -= Render;
        }

        private void Render(Camera cam)
        {
            if(!Application.isPlaying) return;

            for (int i = 0; i < Cell.Cells.Count; i+= batchSize)
            {
                var batchCount = 0;
                for (int batchIndex = 0; batchIndex < batchSize; batchIndex++)
                {
                    if (i + batchIndex >= Cell.Cells.Count) break;
                    batchCount++;

                    var cell = Cell.Cells[i + batchIndex];

                    posDirBuffer[batchIndex] = new Vector4(cell.X, cell.Y,
                        Mathf.Cos(cell.Direction * 90 * Mathf.Deg2Rad),
                        Mathf.Sin(cell.Direction * 90 * Mathf.Deg2Rad));

                    //Convert cell texture index to slice index
                    textureIndexBuffer[batchIndex] = cell.Texture + 0.5f;
                }

                _propertyBlock.SetVectorArray(PosDirPropertyId, posDirBuffer);
                _propertyBlock.SetFloatArray("textureIndex", textureIndexBuffer);
                CallRender(cam, batchCount);
            }
        }

        private void CallRender(Camera c, int count) {
            Graphics.DrawMeshInstancedProcedural(_renderInfo.mesh, 0, _renderInfo.mat,
                bounds: new Bounds(Vector3.zero, Vector3.one * 1000f),
                count: count,
                properties: _propertyBlock,
                castShadows: ShadowCastingMode.Off,
                receiveShadows: false,
                layer: _renderLayer,
                camera: c);
        }
    }
}