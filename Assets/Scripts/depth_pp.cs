using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Custom/DepthExample")]
public sealed class DepthExample : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter depthDistance = new ClampedFloatParameter(1f, 0f, 4f);
    public ClampedFloatParameter enabled = new ClampedFloatParameter(1f, 0f, 1f);


    Material m_Material;

    public bool IsActive() => m_Material != null;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/DepthExample") != null)
            m_Material = new Material(Shader.Find("Hidden/Shader/DepthExample"));
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;

        if (enabled == 0.0f)
            return;

        // Packing multiple float paramters into one float4 uniform
        //Vector4 parameters = new Vector4(depthDistance.value, depthDistance.value, depthDistance.value, depthDistance.value);
        //m_Material.SetVector("_Params", parameters);
        m_Material.SetFloat("_DepthDistance", depthDistance.value);
        m_Material.SetTexture("_InputTexture", source);

        HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }

    public override void Cleanup() => CoreUtils.Destroy(m_Material);
}