using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[ExecuteAlways]
public class Test : MonoBehaviour
{
    public Toggle toggleNone;
    public Toggle toggleDepth;
    public Toggle toggleNormal;
    public Toggle toggleRaw;
    public Volume volume;

	void Start()
    {
		BindToggle(toggleNone, Ebitender.TestPostProcessRenderFeature.ModeType.None);
		BindToggle(toggleDepth, Ebitender.TestPostProcessRenderFeature.ModeType.Depth);
		BindToggle(toggleNormal, Ebitender.TestPostProcessRenderFeature.ModeType.Normal);
		BindToggle(toggleRaw, Ebitender.TestPostProcessRenderFeature.ModeType.Raw);
	}

    private void BindToggle(Toggle toggle, Ebitender.TestPostProcessRenderFeature.ModeType mode)
    {
        toggle.isOn = IsVolumeMode(mode);
        toggle.onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                SetVolumeMode(mode);
            }
        });
    }

    void SetVolumeMode(Ebitender.TestPostProcessRenderFeature.ModeType mode)
    {
        if (volume.profile.TryGet<Ebitender.TestPostProcessVolume>(out var vol))
        {
            vol.mode.value = mode;
        }
    }

    bool IsVolumeMode(Ebitender.TestPostProcessRenderFeature.ModeType mode)
    {
        if (volume.profile.TryGet<Ebitender.TestPostProcessVolume>(out var vol))
        {
            return vol.mode.value == mode;
        }
        return false;
    }
}
