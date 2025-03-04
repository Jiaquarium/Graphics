#if ENABLE_VR && ENABLE_XR_MODULE

namespace UnityEngine.Rendering.Universal
{
    /// <summary>
    /// Draw the XR occlusion mesh into the current depth buffer when XR is enabled.
    /// </summary>
    public class XROcclusionMeshPass : ScriptableRenderPass
    {
        public XROcclusionMeshPass(RenderPassEvent evt)
        {
            base.profilingSampler = new ProfilingSampler(nameof(XROcclusionMeshPass));
            renderPassEvent = evt;
        }

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.xr.hasValidOcclusionMesh)
            {
                CommandBuffer cmd = renderingData.commandBuffer;
                renderingData.cameraData.xr.RenderOcclusionMesh(cmd);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
            }
        }
    }
}

#endif
