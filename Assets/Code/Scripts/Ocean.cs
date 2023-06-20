using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace ShootingRangeGame
{
    public class Ocean : ScriptableRendererFeature
    {
        [SerializeField] private Material oceanMaterial;
        [SerializeField] private float oceanSize = 1000.0f;
        [SerializeField] private float height;

        private OceanPass pass;

        public override void Create()
        {
            pass = new OceanPass();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            pass.material = oceanMaterial;
            pass.size = oceanSize;
            pass.height = height;
            renderer.EnqueuePass(pass);
        }
    }

    public sealed class OceanPass : ScriptableRenderPass
    {
        private readonly Mesh mesh;

        public Material material;
        public float size;
        public float height;

        public OceanPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            
            mesh = new Mesh();
            mesh.name = "Ocean Mesh [PROCEDURAL]";

            mesh.SetVertices(new Vector3[]
            {
                new(1.0f, 0.0f, 1.0f),
                new(-1.0f, 0.0f, 1.0f),
                new(-1.0f, 0.0f, -1.0f),
                new(1.0f, 0.0f, -1.0f),
            });
            mesh.SetTriangles(new[] { 0, 1, 2, 2, 3, 0 }, 0);
            mesh.SetNormals(new[]
            {
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
            });
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!material) return;
            
            var cmd = CommandBufferPool.Get("Ocean");
            cmd.Clear();

            var position = renderingData.cameraData.camera.transform.position;
            position.y = height;
            
            var matrix = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one * size);
            cmd.DrawMesh(mesh, matrix, material, 0);
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}