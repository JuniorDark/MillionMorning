using System;
using System.Collections.Generic;
using Code.Core.BodyPack;
using Code.Core.BodyPack.ColorSystem;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.HomePack;

public abstract class MilMo_HomePainter
{
	protected abstract IList<MilMo_TextureColorGroupPair> SkinLayers { get; }

	protected abstract string Path { get; }

	protected abstract string ShaderName { get; }

	protected abstract void ApplyTexture(GameObject gameObject);

	public void CreateTexture(GameObject gameObject, IDictionary<string, int> colorIndices)
	{
		if (SkinLayers == null || SkinLayers.Count == 0)
		{
			return;
		}
		RenderTexture active = RenderTexture.active;
		ColorGroup colorGroup = SkinLayers[0].ColorGroup;
		Texture2D texture = SkinLayers[0].Texture;
		if (texture == null)
		{
			Debug.LogWarning("Got null texture as first addon texture '" + SkinLayers[0].TextureName + "' in home pack " + Path);
			return;
		}
		Shader shader = MilMo_ResourceManager.LoadShaderLocal(ShaderName);
		if (shader == null)
		{
			Debug.LogWarning("Failed to load shader " + ShaderName + " when creating addon texture for " + Path);
			return;
		}
		Material material = new Material(shader);
		RenderTexture renderTexture = new RenderTexture(texture.width, texture.height, 0);
		renderTexture.isPowerOfTwo = MilMo_ColorShaderUtil.IsPowerOfTwo(renderTexture.width) && MilMo_ColorShaderUtil.IsPowerOfTwo(renderTexture.height);
		renderTexture.hideFlags = HideFlags.DontSave;
		RenderTexture.active = renderTexture;
		GL.Clear(clearDepth: true, clearColor: true, Color.clear);
		GL.PushMatrix();
		GL.LoadOrtho();
		material.mainTexture = texture;
		material.SetPass(0);
		MilMo_ColorShaderUtil.DrawQuad(new Rect(0f, 0f, 1f, 1f));
		if (colorIndices != null && colorGroup != null && colorIndices.ContainsKey(Path + ":" + colorGroup.GroupName))
		{
			try
			{
				MilMo_BodyPackSystem.GetColorFromIndex(colorIndices[Path + ":" + colorGroup.GroupName]).Apply(new Rect(0f, 0f, 1f, 1f), texture);
			}
			catch (ArgumentOutOfRangeException)
			{
				Debug.LogWarning("Got invalid color index for SkinLayer in HomePackSurface " + Path + ".");
			}
		}
		for (int i = 1; i < SkinLayers.Count; i++)
		{
			ColorGroup colorGroup2 = SkinLayers[i].ColorGroup;
			Texture2D texture2 = SkinLayers[i].Texture;
			Vector2 uVOffset = SkinLayers[i].UVOffset;
			Rect r = default(Rect);
			r.x = uVOffset.x / (float)texture.width;
			r.y = ((float)texture.height - uVOffset.y - (float)texture2.height) / (float)texture.height;
			r.width = (float)texture2.width / (float)texture.width;
			r.height = (float)texture2.height / (float)texture.height;
			if (colorIndices != null && colorGroup2 != null && colorIndices.ContainsKey(Path + ":" + colorGroup2.GroupName))
			{
				try
				{
					MilMo_BodyPackSystem.GetColorFromIndex(colorIndices[Path + ":" + colorGroup2.GroupName]).Apply(r, texture, texture2);
				}
				catch (ArgumentOutOfRangeException)
				{
					Debug.LogWarning("Got invalid color index for Addon in HomePack " + Path + ".");
				}
			}
			else
			{
				material.mainTexture = texture2;
				material.SetPass(0);
				MilMo_ColorShaderUtil.DrawQuad(r);
			}
		}
		ApplyTexture(gameObject);
		GL.PopMatrix();
		RenderTexture.active = active;
		renderTexture.Release();
		if (!Application.isPlaying)
		{
			UnityEngine.Object.DestroyImmediate(renderTexture);
			UnityEngine.Object.DestroyImmediate(material);
		}
		else
		{
			UnityEngine.Object.Destroy(renderTexture);
			UnityEngine.Object.Destroy(material);
		}
	}
}
