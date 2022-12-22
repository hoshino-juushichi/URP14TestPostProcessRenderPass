using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#if UNITY_EDITOR
using UnityEditor;
#endif

#nullable enable

namespace Ebitender
{
	[Serializable]
	public class TestPostProcessRenderFeature : ScriptableRendererFeature
	{
		[SerializeField] private Shader? _shader = null;
		[SerializeField] private RenderPassEvent _renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

		private Material? _material = null!;
		private TestPostProcessRenderPass? _renderPass = null!;
        private TestPostProcessVolume? _volume;

        public enum ModeType
		{
			None = 0,
			Depth,
			Normal,
            Raw,
        };

		public override void Create()
		{
#if UNITY_EDITOR
			if (_shader == null)
			{
				_shader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/Scripts/Ebitender/TestPostProcess.shader");
			}
#endif
            if (_shader == null)
			{
				return;
			}
			_material = new Material(_shader);
			_renderPass = new TestPostProcessRenderPass(_material);
		}

		protected override void Dispose(bool disposing)
		{
			CoreUtils.Destroy(_material);
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
            _volume = VolumeManager.instance.stack.GetComponent<TestPostProcessVolume>();
            if (!ShouldRender(renderingData))
				return;

			renderer.EnqueuePass(_renderPass);
		}

		public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
		{
			if (!ShouldRender(renderingData))
				return;

			if (_renderPass == null || _volume == null
                )
            {
				throw new InvalidOperationException($"{this.name} null");
			}
			_renderPass.renderPassEvent = _renderPassEvent;
			_renderPass.SetTarget(renderer.cameraColorTargetHandle);
            _renderPass.SetMode(_volume.mode.value);
#if UNITY_EDITOR
            CheckRenderModeIfChangedThenUpdateGameView();
#endif
		}

		public bool ShouldRender(in RenderingData renderingData)
		{
            if (!renderingData.cameraData.postProcessEnabled ||
				renderingData.cameraData.cameraType != CameraType.Game)
			{
				return false;
			}
			if (_renderPass == null ||
                _volume == null ||
				!_volume.active ||
				_volume.mode.value == ModeType.None
                )
            {
				return false;
			}
			return true;
		}

#if UNITY_EDITOR
		private ModeType _modeCache;
		private void CheckRenderModeIfChangedThenUpdateGameView()
		{
            if (_volume != null && _modeCache != _volume.mode.value)
			{
				_modeCache = _volume.mode.value;

                // RepaintGameView
                System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
				Type gameView = assembly.GetType("UnityEditor.PlayModeView");
				EditorWindow editorWindow = EditorWindow.GetWindow(gameView);
				editorWindow.Repaint();
			}
		}
#endif
	}
}