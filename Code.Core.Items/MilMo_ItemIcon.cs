using System.Collections.Generic;
using Code.Core.BodyPack;
using Code.Core.BodyPack.ColorSystem;
using UnityEngine;

namespace Code.Core.Items;

public class MilMo_ItemIcon
{
	protected const int ICON_SIZE = 128;

	private const string CHANNEL_SHADER_R_PATH = "Shaders/IconBuilder/ChannelMaskerR";

	private const string CHANNEL_SHADER_G_PATH = "Shaders/IconBuilder/ChannelMaskerG";

	private const string CHANNEL_SHADER_B_PATH = "Shaders/IconBuilder/ChannelMaskerB";

	private const string CHANNEL_COMBINE_SHADER_PATH = "Shaders/IconBuilder/MaskCombine";

	private static readonly int ChannelTexR = Shader.PropertyToID("_ChannelTexR");

	private static readonly int ChannelTexG = Shader.PropertyToID("_ChannelTexG");

	private static readonly int ChannelTexB = Shader.PropertyToID("_ChannelTexB");

	private static bool _initializedShaders;

	private static Shader _channelShaderR;

	private static Shader _channelShaderG;

	private static Shader _channelShaderB;

	private static Shader _channelCombineShader;

	private static void InitializeShaders()
	{
		if (!_initializedShaders)
		{
			_channelShaderR = Resources.Load<Shader>("Shaders/IconBuilder/ChannelMaskerR");
			_channelShaderG = Resources.Load<Shader>("Shaders/IconBuilder/ChannelMaskerG");
			_channelShaderB = Resources.Load<Shader>("Shaders/IconBuilder/ChannelMaskerB");
			_channelCombineShader = Resources.Load<Shader>("Shaders/IconBuilder/MaskCombine");
			_initializedShaders = true;
		}
	}

	public static void GenerateColoredIcon(Texture2D baseIcon, IDictionary<string, int> colorIndices, string colorIndicesKey, List<ColorGroup> colorGroups, Texture2D renderTarget)
	{
		InitializeShaders();
		RenderTexture active = RenderTexture.active;
		RenderTexture renderTexture = null;
		if (colorGroups[0].ColorIndices.Count > 0 && colorIndices.TryGetValue(colorIndicesKey + ":" + colorGroups[0].GroupName, out var value))
		{
			MilMo_Color color = MilMo_BodyPackSystem.GetColorFromIndex(value) ?? MilMo_BodyPackSystem.GetColorFromIndex(0);
			renderTexture = GetTemporaryRenderTexture();
			ApplyColorWithChannelMasker(renderTexture, baseIcon, color, _channelShaderR);
		}
		RenderTexture renderTexture2 = null;
		if (colorGroups.Count > 1 && colorGroups[1].ColorIndices.Count > 0 && colorIndices.TryGetValue(colorIndicesKey + ":" + colorGroups[1].GroupName, out var value2))
		{
			MilMo_Color color2 = MilMo_BodyPackSystem.GetColorFromIndex(value2) ?? MilMo_BodyPackSystem.GetColorFromIndex(0);
			renderTexture2 = GetTemporaryRenderTexture();
			ApplyColorWithChannelMasker(renderTexture2, baseIcon, color2, _channelShaderG);
		}
		RenderTexture renderTexture3 = null;
		if (colorGroups.Count > 2 && colorGroups[2].ColorIndices.Count > 0 && colorIndices.TryGetValue(colorIndicesKey + ":" + colorGroups[2].GroupName, out var value3))
		{
			MilMo_Color color3 = MilMo_BodyPackSystem.GetColorFromIndex(value3) ?? MilMo_BodyPackSystem.GetColorFromIndex(0);
			renderTexture3 = GetTemporaryRenderTexture();
			ApplyColorWithChannelMasker(renderTexture3, baseIcon, color3, _channelShaderB);
		}
		RenderTexture temporary = RenderTexture.GetTemporary(128, 128, 0);
		temporary.isPowerOfTwo = true;
		RenderTexture.active = temporary;
		Material material = new Material(_channelCombineShader)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		GL.Clear(clearDepth: true, clearColor: true, Color.clear);
		GL.PushMatrix();
		GL.LoadOrtho();
		material.mainTexture = baseIcon;
		if (renderTexture != null)
		{
			material.SetTexture(ChannelTexR, renderTexture);
		}
		if (renderTexture2 != null)
		{
			material.SetTexture(ChannelTexG, renderTexture2);
		}
		if (renderTexture3 != null)
		{
			material.SetTexture(ChannelTexB, renderTexture3);
		}
		material.SetPass(0);
		MilMo_ColorShaderUtil.DrawQuad(new Rect(0f, 0f, 1f, 1f));
		renderTarget.ReadPixels(new Rect(0f, 0f, 128f, 128f), 0, 0, recalculateMipMaps: false);
		renderTarget.Apply();
		GL.PopMatrix();
		if (!Application.isPlaying)
		{
			Object.DestroyImmediate(material);
		}
		else
		{
			Object.Destroy(material);
		}
		RenderTexture.active = null;
		if (renderTexture != null)
		{
			RenderTexture.ReleaseTemporary(renderTexture);
		}
		if (renderTexture2 != null)
		{
			RenderTexture.ReleaseTemporary(renderTexture2);
		}
		if (renderTexture3 != null)
		{
			RenderTexture.ReleaseTemporary(renderTexture3);
		}
		if (temporary != null)
		{
			RenderTexture.ReleaseTemporary(temporary);
		}
		RenderTexture.active = active;
	}

	private static RenderTexture GetTemporaryRenderTexture()
	{
		RenderTexture temporary = RenderTexture.GetTemporary(128, 128, 0);
		temporary.isPowerOfTwo = true;
		return temporary;
	}

	private static void ApplyColorWithChannelMasker(RenderTexture renderTexture, Texture2D baseIcon, MilMo_Color color, Shader shader)
	{
		RenderTexture.active = renderTexture;
		Material material = new Material(shader)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		GL.Clear(clearDepth: true, clearColor: true, Color.clear);
		GL.PushMatrix();
		GL.LoadOrtho();
		material.mainTexture = baseIcon;
		material.SetPass(0);
		MilMo_ColorShaderUtil.DrawQuad(new Rect(0f, 0f, 1f, 1f));
		Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, mipChain: false);
		texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
		texture2D.Apply();
		color.Apply(new Rect(0f, 0f, 1f, 1f), texture2D);
		GL.PopMatrix();
		if (!Application.isPlaying)
		{
			Object.DestroyImmediate(texture2D);
			Object.DestroyImmediate(material);
		}
		else
		{
			Object.Destroy(texture2D);
			Object.Destroy(material);
		}
	}
}
