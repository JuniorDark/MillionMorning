using System;
using System.Collections.Generic;
using Code.Core.BodyPack.ColorSystem;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.BodyPack;

public class MilMo_SkinLayer
{
	public delegate void SkinLayerContentDone(bool success);

	private readonly string _skinSection;

	private readonly string _texturePath;

	private readonly string _fullPath;

	private readonly ColorGroup _colorGroup;

	private readonly Material _material;

	private readonly string _bodyPackPath;

	private readonly Vector2 _uvOffset;

	private readonly bool _haveUVOffset;

	private static readonly int Stitch = Shader.PropertyToID("_Stitch");

	private static readonly int BlendTex1 = Shader.PropertyToID("_BlendTex1");

	private const string SHADER = "Shaders/skinLayerShader";

	private Texture Texture { get; set; }

	public bool HasContent { get; private set; }

	public bool DoneLoading { get; private set; }

	public MilMo_SkinLayer(string skinSection, string texturePath, ColorGroup colorGroup, string bodyPackPath, Vector2 uvOffset)
	{
		DoneLoading = false;
		_skinSection = skinSection;
		_texturePath = texturePath;
		_colorGroup = colorGroup;
		_bodyPackPath = bodyPackPath;
		_uvOffset = uvOffset;
		_haveUVOffset = !MilMo_Utility.Equals(_uvOffset, Vector2.zero, 0.0005f);
		Texture = null;
		HasContent = false;
		Shader shader = MilMo_ResourceManager.LoadShaderLocal("Shaders/skinLayerShader");
		_material = new Material(shader)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		_fullPath = "Content/Bodypacks/" + _texturePath;
	}

	public void StitchBlendLayer(bool male)
	{
		if (Texture == null)
		{
			Debug.LogWarning("Skin layer have null texture when stitching blend layer in body pack " + _bodyPackPath);
			return;
		}
		Material material = new Material(MilMo_ResourceManager.LoadShaderLocal("Shaders/BodyBuilder/MilMoColor"));
		material.SetTexture(BlendTex1, Texture);
		material.EnableKeyword("STITCHING");
		material.SetPass(0);
		MilMo_ColorShaderUtil.DrawQuad(MilMo_BodyPackSystem.SkinLayerSections.GetSkinLayerSection(male, _skinSection));
		if (!Application.isPlaying)
		{
			UnityEngine.Object.DestroyImmediate(material);
		}
		else
		{
			UnityEngine.Object.Destroy(material);
		}
	}

	public void ApplyAsBlendLayer(bool male, IDictionary<string, int> colorIndices)
	{
		if (Texture == null)
		{
			Debug.LogWarning("Skin layer '" + _texturePath + "' has null texture");
			return;
		}
		RenderTexture renderTexture = null;
		if (_colorGroup != null && colorIndices != null && colorIndices.ContainsKey(_bodyPackPath + ":" + _colorGroup.GroupName))
		{
			RenderTexture active = RenderTexture.active;
			renderTexture = new RenderTexture(MilMo_ColorShaderUtil.NearestBiggerPowerOfTwo(Texture.width), MilMo_ColorShaderUtil.NearestBiggerPowerOfTwo(Texture.height), 0)
			{
				hideFlags = HideFlags.DontSave
			};
			renderTexture.isPowerOfTwo = MilMo_ColorShaderUtil.IsPowerOfTwo(renderTexture.width) && MilMo_ColorShaderUtil.IsPowerOfTwo(renderTexture.height);
			RenderTexture.active = renderTexture;
			GL.Clear(clearDepth: true, clearColor: true, Color.clear);
			GL.PushMatrix();
			GL.LoadOrtho();
			_material.mainTexture = Texture;
			_material.SetPass(0);
			MilMo_ColorShaderUtil.DrawQuad(new Rect(0f, 0f, 1f, 1f));
			try
			{
				MilMo_BodyPackSystem.GetColorFromIndex(colorIndices[_bodyPackPath + ":" + _colorGroup.GroupName]).Apply(new Rect(0f, 0f, 1f, 1f), Texture);
			}
			catch (ArgumentOutOfRangeException)
			{
				Debug.LogWarning("Got invalid color index for BlendLayer in BodyPack " + _bodyPackPath + ".");
			}
			GL.PopMatrix();
			RenderTexture.active = active;
			_material.mainTexture = renderTexture;
		}
		else
		{
			_material.mainTexture = Texture;
		}
		Rect skinLayerSection = MilMo_BodyPackSystem.SkinLayerSections.GetSkinLayerSection(male, _skinSection);
		_material.SetPass(0);
		MilMo_ColorShaderUtil.DrawQuad(skinLayerSection);
		if (!(renderTexture == null))
		{
			renderTexture.Release();
			if (!Application.isPlaying)
			{
				UnityEngine.Object.DestroyImmediate(renderTexture);
			}
			else
			{
				UnityEngine.Object.Destroy(renderTexture);
			}
		}
	}

	public void ApplyAsSkinLayer(bool male, IDictionary<string, int> colorIndices, Texture blendLayerTexture)
	{
		if (Texture == null)
		{
			Debug.LogWarning("Skin layer '" + _texturePath + "' has null texture in body pack " + _bodyPackPath);
			return;
		}
		Rect skinLayerSection = MilMo_BodyPackSystem.SkinLayerSections.GetSkinLayerSection(male, _skinSection);
		Rect r = ((!_haveUVOffset) ? skinLayerSection : new Rect(skinLayerSection.x + _uvOffset.x, skinLayerSection.y + skinLayerSection.height - _uvOffset.y - (float)Texture.height / 1024f, (float)Texture.width / 1024f, (float)Texture.height / 1024f));
		if (_colorGroup != null && colorIndices != null && colorIndices.ContainsKey(_bodyPackPath + ":" + _colorGroup.GroupName))
		{
			try
			{
				MilMo_BodyPackSystem.GetColorFromIndex(colorIndices[_bodyPackPath + ":" + _colorGroup.GroupName]).Apply(r, blendLayerTexture, Texture);
				return;
			}
			catch (ArgumentOutOfRangeException)
			{
				Debug.LogWarning("Got invalid color index for SkinLayer in BodyPack " + _bodyPackPath + ".");
				return;
			}
		}
		Material material = new Material(MilMo_ResourceManager.LoadShaderLocal("Shaders/BodyBuilder/MilMoColor"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		material.SetTexture(Stitch, blendLayerTexture);
		material.SetTexture(BlendTex1, Texture);
		material.EnableKeyword("NOBLEND");
		material.SetPass(0);
		MilMo_ColorShaderUtil.DrawQuad(r);
		if (!Application.isPlaying)
		{
			UnityEngine.Object.DestroyImmediate(material);
		}
		else
		{
			UnityEngine.Object.Destroy(material);
		}
	}

	public async void AsyncLoadContent(SkinLayerContentDone callback)
	{
		if (HasContent)
		{
			callback(success: true);
			return;
		}
		Texture2D texture2D = await MilMo_ResourceManager.Instance.LoadTextureAsync(_fullPath);
		if (texture2D == null)
		{
			DoneLoading = true;
			HasContent = false;
			callback(success: false);
		}
		else
		{
			Texture = texture2D;
			_material.mainTexture = Texture;
			DoneLoading = true;
			HasContent = true;
			callback(success: true);
		}
	}
}
