using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ShootingRangeGame.Rendering
{
    public class SelectionRenderFeature : ScriptableRendererFeature
    {
        [SerializeField] private Material material;

        private Pass pass;

        private HashSet<Renderer> renderers = new();

        private static List<SelectionRenderFeature> selectionRenderFeatures = new();

        public override void Create()
        {
            pass = new Pass(material);

            if (!selectionRenderFeatures.Contains(this)) selectionRenderFeatures.Add(this);
        }

        private void OnDestroy() => selectionRenderFeatures.Remove(this);
        private void OnDisable() => selectionRenderFeatures.Remove(this);

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var outlines = FindObjectsOfType<SelectionOutline>();
            foreach (var outline in outlines)
            {
                var rl = outline.GetComponentsInChildren<Renderer>();
                foreach (var r in rl) renderers.Add(r);
            }
            
            pass.SetRenderers(renderers);
            renderers.Clear();

            renderer.EnqueuePass(pass);
        }

        public static void Add(GameObject gameObject) => Operation(gameObject, (feature, renderer) =>
        {
            if (!feature.renderers.Contains(renderer))
            {
                feature.renderers.Add(renderer);
            }
        });

        private static void Operation(GameObject gameObject, Action<SelectionRenderFeature, Renderer> callback)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>();

            foreach (var feature in selectionRenderFeatures)
            {
                foreach (var renderer in renderers)
                {
                    callback(feature, renderer);
                }
            }
        }

        public class Pass : ScriptableRenderPass
        {
            private Material material;
            private HashSet<Renderer> renderers;

            public Pass(Material material)
            {
                this.material = material;

                renderers = new HashSet<Renderer>();
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (!material) return;
                if (renderers == null || renderers.Count == 0) return;

                var cmd = CommandBufferPool.Get("Selection Render");
                cmd.Clear();

                foreach (var renderer in renderers)
                {
                    cmd.DrawRenderer(renderer, material);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public void SetRenderers(IEnumerable<Renderer> renderers)
            {
                this.renderers.Clear();
                foreach (var renderer in renderers) this.renderers.Add(renderer);
            }
        }
    }
}