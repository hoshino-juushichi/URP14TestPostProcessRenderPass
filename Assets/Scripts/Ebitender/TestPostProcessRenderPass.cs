using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

#nullable enable

namespace Ebitender
{
    public class TestPostProcessRenderPass : ScriptableRenderPass
    {
        private const string ProfilerTag = nameof(TestPostProcessRenderPass);
        private readonly ProfilingSampler _profilingSampler = new ProfilingSampler(ProfilerTag);

        private Material? _material;
        private RTHandle? _cameraColorTarget;
		private TestPostProcessRenderFeature.ModeType _mode = TestPostProcessRenderFeature.ModeType.None;

		public TestPostProcessRenderPass(Material material)
        {
            _material = material;
		}

		public void SetTarget(RTHandle colorHandle)
        {
            _cameraColorTarget = colorHandle;
        }

		public void SetMode(TestPostProcessRenderFeature.ModeType mode)
		{
			_mode = mode;
		}

		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			switch (_mode)
			{
				case TestPostProcessRenderFeature.ModeType.ShowDepth:
					ConfigureInput(ScriptableRenderPassInput.Depth);
					break;
				case TestPostProcessRenderFeature.ModeType.ShowNormal:
					ConfigureInput(ScriptableRenderPassInput.Normal);
					break;
			}
			ConfigureTarget(_cameraColorTarget);
		}

		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_material == null || _cameraColorTarget == null)
            {
                return;
            }

			var cmd = CommandBufferPool.Get(ProfilerTag);
            using (new ProfilingScope(cmd, _profilingSampler))
            {
				Blitter.BlitCameraTexture(cmd, _cameraColorTarget, _cameraColorTarget, _material, GetPassIndex());
			}
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

		private int GetPassIndex()
		{
			switch (_mode)
			{
				case TestPostProcessRenderFeature.ModeType.ShowDepth:
					return 0;
				case TestPostProcessRenderFeature.ModeType.ShowNormal:
					return 1;
			}
			return 0;
		}
	}
}