using System.Collections.Generic;
using Code.Core.BodyPack;
using Code.Core.ResourceSystem;
using Code.Core.Visual;
using UnityEngine;

namespace Code.Core.HomePack;

public class MilMo_HomeAddon : MilMo_HomePainter
{
	private enum LoadingState
	{
		NOT_LOADED,
		LOADING,
		LOADED
	}

	public delegate void HomeAddonLoaded(bool success);

	private const string m_Shader = "Shaders/skinLayerShader";

	private string m_HomePackPath;

	private string m_VisualRepPath;

	private LoadingState m_LoadingState;

	private IList<KeyValuePair<HomeAddonLoaded, GameObject>> m_Callbacks = new List<KeyValuePair<HomeAddonLoaded, GameObject>>();

	private IList<MilMo_TextureColorGroupPair> m_AddonLayers = new List<MilMo_TextureColorGroupPair>();

	private IDictionary<GameObject, MilMo_VisualRep> m_VisualReps = new Dictionary<GameObject, MilMo_VisualRep>();

	protected override IList<MilMo_TextureColorGroupPair> SkinLayers => m_AddonLayers;

	protected override string Path => m_HomePackPath;

	protected override string ShaderName => "Shaders/skinLayerShader";

	public MilMo_HomeAddon(string visualRepPath, string homePackPath, IList<MilMo_TextureColorGroupPair> addonLayers)
	{
		m_HomePackPath = homePackPath;
		m_AddonLayers = addonLayers;
		m_VisualRepPath = visualRepPath;
	}

	protected override void ApplyTexture(GameObject gameObject)
	{
		if (!m_VisualReps.TryGetValue(gameObject, out var value))
		{
			return;
		}
		int width = RenderTexture.active.width;
		int height = RenderTexture.active.height;
		if (value.Materials.Count == 0)
		{
			Object.Destroy(value.Renderer.material);
			value.Materials.Add(MilMo_Material.GetMaterial("Junebug/FurnitureDiffuse"));
			value.Renderer.material = value.Materials[0].Material;
			value.Renderer.material.SetTexture("_Ramp", MilMo_BodyPackSystem.ShaderRamp);
			value.Renderer.material.SetVector("_VelvetChannel", MilMo_BodyPackSystem.CharacterVelvetChannels);
			value.Renderer.material.SetColor("_VelvetColor", MilMo_BodyPackSystem.CharacterVelvetColor);
			value.Renderer.material.SetColor("_Color", MilMo_BodyPackSystem.CharacterMainColor);
		}
		if (value.Renderer.material.mainTexture == null)
		{
			value.Renderer.material.mainTexture = new Texture2D(width, height, TextureFormat.ARGB32, mipChain: false);
		}
		Texture2D obj = (Texture2D)value.Renderer.sharedMaterial.mainTexture;
		obj.ReadPixels(new Rect(0f, 0f, width, height), 0, 0, recalculateMipMaps: true);
		obj.Apply();
		MilMo_MeshHeld meshHeld = value.MeshHeld;
		if (meshHeld != null)
		{
			if (meshHeld.Materials.Count == 0)
			{
				Object.Destroy(meshHeld.Renderer.material);
				meshHeld.Materials.Add(MilMo_Material.GetMaterial("Junebug/FurnitureDiffuse"));
				meshHeld.Renderer.material = meshHeld.Materials[0].Material;
				meshHeld.Renderer.material.SetTexture("_Ramp", MilMo_BodyPackSystem.ShaderRamp);
				meshHeld.Renderer.material.SetVector("_VelvetChannel", MilMo_BodyPackSystem.CharacterVelvetChannels);
				meshHeld.Renderer.material.SetColor("_VelvetColor", MilMo_BodyPackSystem.CharacterVelvetColor);
				meshHeld.Renderer.material.SetColor("_Color", MilMo_BodyPackSystem.CharacterMainColor);
			}
			if (meshHeld.Renderer.material.mainTexture == null)
			{
				meshHeld.Renderer.material.mainTexture = new Texture2D(width, height, TextureFormat.ARGB32, mipChain: false);
			}
			Texture2D obj2 = (Texture2D)meshHeld.Renderer.sharedMaterial.mainTexture;
			obj2.ReadPixels(new Rect(0f, 0f, width, height), 0, 0, recalculateMipMaps: true);
			obj2.Apply();
		}
	}

	public GameObject GetGameObject(GameObject parent)
	{
		return m_VisualReps[parent].GameObject;
	}

	public async void AsyncLoadContent(GameObject gameObject, HomeAddonLoaded callback)
	{
		switch (m_LoadingState)
		{
		case LoadingState.LOADED:
			AsyncLoadVisualRep(callback, gameObject);
			break;
		case LoadingState.LOADING:
			m_Callbacks.Add(new KeyValuePair<HomeAddonLoaded, GameObject>(callback, gameObject));
			break;
		case LoadingState.NOT_LOADED:
			m_LoadingState = LoadingState.LOADING;
			m_Callbacks.Add(new KeyValuePair<HomeAddonLoaded, GameObject>(callback, gameObject));
			foreach (MilMo_TextureColorGroupPair pair in m_AddonLayers)
			{
				pair.Texture = await MilMo_ResourceManager.Instance.LoadTextureAsync(pair.TextureName);
			}
			m_LoadingState = LoadingState.LOADED;
			AsyncLoadVisualRep();
			break;
		}
	}

	private void AsyncLoadVisualRep()
	{
		foreach (KeyValuePair<HomeAddonLoaded, GameObject> callback in m_Callbacks)
		{
			AsyncLoadVisualRep(callback.Key, callback.Value);
		}
		m_Callbacks.Clear();
	}

	private void AsyncLoadVisualRep(HomeAddonLoaded callback, GameObject gameObject)
	{
		if (m_VisualReps.TryGetValue(gameObject, out var value))
		{
			callback(value != null);
			return;
		}
		MilMo_VisualRepContainer.AsyncCreateVisualRep(m_VisualRepPath, Vector3.zero, Quaternion.identity, setDefaultMaterialTexture: false, waitForMaterial: true, delegate(MilMo_VisualRep visualRep)
		{
			if (!m_VisualReps.ContainsKey(gameObject))
			{
				m_VisualReps.Add(gameObject, visualRep);
				if (SkinLayers.Count > 0 && visualRep != null && visualRep.Renderer != null && visualRep.Renderer.sharedMaterial.mainTexture != null)
				{
					visualRep.Renderer.sharedMaterial.mainTexture = Object.Instantiate(visualRep.Renderer.sharedMaterial.mainTexture) as Texture2D;
				}
				if (SkinLayers.Count > 0 && visualRep != null && visualRep.MeshHeld != null && visualRep.MeshHeld.Renderer != null && visualRep.MeshHeld.Renderer.sharedMaterial != null && visualRep.MeshHeld.Renderer.sharedMaterial.mainTexture != null)
				{
					visualRep.MeshHeld.Renderer.sharedMaterial.mainTexture = Object.Instantiate(visualRep.MeshHeld.Renderer.sharedMaterial.mainTexture) as Texture2D;
				}
			}
			callback(visualRep != null);
		});
	}

	public void UnloadContent()
	{
	}

	public void UnloadContent(GameObject gameObject)
	{
		if (m_LoadingState == LoadingState.LOADING)
		{
			for (int num = m_Callbacks.Count - 1; num >= 0; num--)
			{
				if (m_Callbacks[num].Value == gameObject)
				{
					m_Callbacks.RemoveAt(num);
					break;
				}
			}
		}
		else if (m_LoadingState == LoadingState.LOADED && m_VisualReps.ContainsKey(gameObject))
		{
			MilMo_VisualRepContainer.RemoveFromUpdate(m_VisualReps[gameObject]);
			MilMo_VisualRepContainer.DestroyVisualRep(m_VisualReps[gameObject]);
			m_VisualReps.Remove(gameObject);
		}
	}
}
