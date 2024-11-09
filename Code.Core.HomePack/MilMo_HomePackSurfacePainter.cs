using System.Collections.Generic;
using Code.Core.BodyPack;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.HomePack;

public class MilMo_HomePackSurfacePainter : MilMo_HomePainter
{
	private bool m_IsLoaded;

	private IList<MilMo_HomePackSurface.HomePackSurfaceLoaded> m_Callbacks = new List<MilMo_HomePackSurface.HomePackSurfaceLoaded>();

	private IDictionary<GameObject, Texture2D> m_Textures = new Dictionary<GameObject, Texture2D>();

	private const string m_Shader = "Shaders/skinLayerShader";

	private string m_Path;

	private IList<MilMo_TextureColorGroupPair> m_SkinLayers = new List<MilMo_TextureColorGroupPair>();

	public bool IsLoaded => m_IsLoaded;

	protected override IList<MilMo_TextureColorGroupPair> SkinLayers => m_SkinLayers;

	protected override string Path => m_Path;

	protected override string ShaderName => "Shaders/skinLayerShader";

	public MilMo_HomePackSurfacePainter(string path)
	{
		m_Path = path;
	}

	public Texture2D GetTexture(GameObject gameObject)
	{
		if (!m_Textures.ContainsKey(gameObject))
		{
			return null;
		}
		return m_Textures[gameObject];
	}

	public void AddSkinLayer(MilMo_TextureColorGroupPair skinLayer)
	{
		m_SkinLayers.Add(skinLayer);
	}

	public async void AsyncLoadContent(MilMo_HomePackSurface.HomePackSurfaceLoaded callback)
	{
		if (m_IsLoaded)
		{
			callback();
			return;
		}
		m_Callbacks.Add(callback);
		foreach (MilMo_TextureColorGroupPair pair in m_SkinLayers)
		{
			Texture2D texture2D = await MilMo_ResourceManager.Instance.LoadTextureAsync(pair.TextureName);
			if (texture2D == null)
			{
				Debug.LogWarning("Failed to load texture " + pair.TextureName + " for home pack surface " + m_Path);
			}
			pair.Texture = texture2D;
		}
		foreach (MilMo_HomePackSurface.HomePackSurfaceLoaded callback2 in m_Callbacks)
		{
			callback2();
		}
		m_Callbacks.Clear();
		m_IsLoaded = true;
	}

	public void UnloadContent(GameObject gameObject)
	{
		if (m_Textures.TryGetValue(gameObject, out var value))
		{
			m_Textures.Remove(gameObject);
			if (!Application.isPlaying)
			{
				Object.DestroyImmediate(value);
			}
			else
			{
				Object.Destroy(value);
			}
		}
	}

	public void UnloadContent()
	{
		foreach (Texture2D value in m_Textures.Values)
		{
			if (!Application.isPlaying)
			{
				Object.DestroyImmediate(value);
			}
			else
			{
				Object.Destroy(value);
			}
		}
		m_Textures.Clear();
	}

	protected override void ApplyTexture(GameObject gameObject)
	{
		int width = RenderTexture.active.width;
		int height = RenderTexture.active.height;
		if (!m_Textures.TryGetValue(gameObject, out var value))
		{
			value = new Texture2D(width, height, TextureFormat.ARGB32, mipChain: false);
		}
		value.ReadPixels(new Rect(0f, 0f, width, height), 0, 0, recalculateMipMaps: true);
		value.Apply();
		if (!m_Textures.ContainsKey(gameObject))
		{
			m_Textures.Add(gameObject, value);
		}
	}
}
