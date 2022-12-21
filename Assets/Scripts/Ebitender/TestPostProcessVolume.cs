using System;
using UnityEngine;
using UnityEngine.Rendering;

#nullable enable

namespace Ebitender
{
	public class TestPostProcessVolume : VolumeComponent
	{
		[Serializable]
		public sealed class DebugModeParameter : VolumeParameter<TestPostProcessRenderFeature.ModeType>
		{
			public DebugModeParameter(TestPostProcessRenderFeature.ModeType value, bool overriderState = false) : base(value, overriderState) { }
		}

		public DebugModeParameter mode = new DebugModeParameter(TestPostProcessRenderFeature.ModeType.None);
	}
}