using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.BodyPack.ColorSystem;
using Code.Core.ResourceSystem;
using Code.Core.Visual;
using UnityEngine;

namespace Code.Core.BodyPack;

public class MilMo_Addon
{
	public delegate void AddonDone(bool success);

	private const string SHADER = "Shaders/skinLayerShader";

	protected readonly string BodyPackPath;

	protected readonly string VisualRepPath;

	private readonly string _node;

	private readonly bool _scale;

	private readonly IList<MilMo_TextureColorGroupPair> _addonLayers;

	protected readonly Dictionary<int, MilMo_VisualRep> PlayerAddons = new Dictionary<int, MilMo_VisualRep>();

	private bool _doneLoading;

	private readonly Vector3 _boyOffset;

	private readonly Vector3 _girlOffset;

	private static readonly int Ramp = Shader.PropertyToID("_Ramp");

	private static readonly int VelvetChannel = Shader.PropertyToID("_VelvetChannel");

	private static readonly int VelvetColor = Shader.PropertyToID("_VelvetColor");

	private static readonly int MainColor = Shader.PropertyToID("_Color");

	public bool HasContent
	{
		get
		{
			if (_addonLayers != null)
			{
				return _addonLayers.All((MilMo_TextureColorGroupPair addonLayer) => addonLayer.Texture != null);
			}
			return true;
		}
	}

	public bool DoneLoading
	{
		get
		{
			if (!_doneLoading)
			{
				return _addonLayers.Count == 0;
			}
			return true;
		}
	}

	public MilMo_Addon(string node, string visualRepPath, bool scale, string bodyPackPath, IList<MilMo_TextureColorGroupPair> addonLayers, Vector3 boyOffset, Vector3 girlOffset)
	{
		_node = node;
		_scale = scale;
		BodyPackPath = bodyPackPath;
		_addonLayers = addonLayers;
		_boyOffset = boyOffset;
		_girlOffset = girlOffset;
		VisualRepPath = "Content/Bodypacks/" + visualRepPath;
	}

	public MilMo_VisualRep GetVisualRep(SkinnedMeshRenderer renderer)
	{
		if (!PlayerAddons.TryGetValue(renderer.GetInstanceID(), out var value))
		{
			return null;
		}
		return value;
	}

	public virtual void Disable(SkinnedMeshRenderer renderer)
	{
		if (PlayerAddons.TryGetValue(renderer.GetInstanceID(), out var value) && value != null && !value.IsInvalid)
		{
			value.Renderer.enabled = false;
		}
	}

	public virtual void Enable(SkinnedMeshRenderer renderer)
	{
		if (PlayerAddons.TryGetValue(renderer.GetInstanceID(), out var value) && value != null && !value.IsInvalid)
		{
			value.Renderer.enabled = true;
		}
	}

	public virtual void SetLayer(SkinnedMeshRenderer renderer, int layer)
	{
		if (PlayerAddons.TryGetValue(renderer.GetInstanceID(), out var value) && value != null && !value.IsInvalid)
		{
			value.GameObject.layer = layer;
			value.Renderer.gameObject.layer = layer;
		}
	}

	public void EnableSilhouetteRendering(SkinnedMeshRenderer renderer)
	{
		if (!PlayerAddons.TryGetValue(renderer.GetInstanceID(), out var value) || value == null || value.IsInvalid)
		{
			return;
		}
		foreach (MilMo_Material material in value.Materials)
		{
			if (material.Material.renderQueue == 2000)
			{
				material.Material.renderQueue = 1800;
			}
		}
	}

	public bool HasContentInstantiated(SkinnedMeshRenderer renderer)
	{
		if (renderer == null)
		{
			return false;
		}
		if (PlayerAddons.ContainsKey(renderer.GetInstanceID()))
		{
			return PlayerAddons[renderer.GetInstanceID()] != null;
		}
		return false;
	}

	public bool DoneLoadingInstantiatedContent(SkinnedMeshRenderer renderer)
	{
		if ((bool)renderer)
		{
			return PlayerAddons.ContainsKey(renderer.GetInstanceID());
		}
		return false;
	}

	public virtual void CreateAddonTexture(SkinnedMeshRenderer renderer, IDictionary<string, int> colorIndices)
	{
		if (_addonLayers == null || _addonLayers.Count == 0)
		{
			return;
		}
		if (renderer == null)
		{
			Debug.LogWarning("Argument renderer was null when calling CreateAddonTexture");
			throw new ArgumentNullException();
		}
		RenderTexture active = RenderTexture.active;
		ColorGroup colorGroup = _addonLayers[0].ColorGroup;
		Texture2D texture = _addonLayers[0].Texture;
		if (texture == null)
		{
			Debug.LogWarning("Got null texture as first addon texture '" + _addonLayers[0].TextureName + "' in body pack " + BodyPackPath);
			return;
		}
		Shader shader = MilMo_ResourceManager.LoadShaderLocal("Shaders/skinLayerShader");
		if (shader == null)
		{
			Debug.LogWarning("Failed to load shader Shaders/skinLayerShader when creating addon texture for " + BodyPackPath);
			return;
		}
		Material material = new Material(shader);
		RenderTexture renderTexture = new RenderTexture(texture.width, texture.height, 0);
		renderTexture.isPowerOfTwo = MilMo_ColorShaderUtil.IsPowerOfTwo(renderTexture.width) && MilMo_ColorShaderUtil.IsPowerOfTwo(renderTexture.height);
		renderTexture.hideFlags = HideFlags.DontSave;
		if (!renderTexture.Create())
		{
			throw new NullReferenceException("Calling create on addon render texture failed for addon " + BodyPackPath + ". Width=" + renderTexture.width + ", Height=" + renderTexture.height + ", Depth=" + renderTexture.depth + ", IsPowerOfTwo=" + renderTexture.isPowerOfTwo);
		}
		RenderTexture.active = renderTexture;
		if (RenderTexture.active == null)
		{
			throw new NullReferenceException("Active render texture is null directly after being set for addon " + BodyPackPath + ". Width=" + renderTexture.width + ", Height=" + renderTexture.height + ", Depth=" + renderTexture.depth + ", IsPowerOfTwo=" + renderTexture.isPowerOfTwo);
		}
		GL.Clear(clearDepth: true, clearColor: true, Color.clear);
		GL.PushMatrix();
		GL.LoadOrtho();
		material.mainTexture = texture;
		material.SetPass(0);
		MilMo_ColorShaderUtil.DrawQuad(new Rect(0f, 0f, 1f, 1f));
		if (colorIndices != null && colorGroup != null && colorIndices.ContainsKey(BodyPackPath + ":" + colorGroup.GroupName))
		{
			try
			{
				MilMo_BodyPackSystem.GetColorFromIndex(colorIndices[BodyPackPath + ":" + colorGroup.GroupName]).Apply(new Rect(0f, 0f, 1f, 1f), texture);
			}
			catch (ArgumentOutOfRangeException)
			{
				Debug.LogWarning("Got invalid color index for Addon in BodyPack " + BodyPackPath + ".");
			}
		}
		for (int i = 1; i < _addonLayers.Count; i++)
		{
			ColorGroup colorGroup2 = _addonLayers[i].ColorGroup;
			Texture2D texture2 = _addonLayers[i].Texture;
			Vector2 uVOffset = _addonLayers[i].UVOffset;
			Rect rect = default(Rect);
			rect.x = uVOffset.x / (float)texture.width;
			rect.y = ((float)texture.height - uVOffset.y - (float)texture2.height) / (float)texture.height;
			rect.width = (float)texture2.width / (float)texture.width;
			rect.height = (float)texture2.height / (float)texture.height;
			Rect r = rect;
			if (colorIndices != null && colorGroup2 != null && colorIndices.ContainsKey(BodyPackPath + ":" + colorGroup2.GroupName))
			{
				try
				{
					MilMo_BodyPackSystem.GetColorFromIndex(colorIndices[BodyPackPath + ":" + colorGroup2.GroupName]).Apply(r, texture, texture2);
				}
				catch (ArgumentOutOfRangeException)
				{
					Debug.LogWarning("Got invalid color index for Addon in BodyPack " + BodyPackPath + ".");
				}
			}
			else
			{
				material.mainTexture = texture2;
				material.SetPass(0);
				MilMo_ColorShaderUtil.DrawQuad(r);
			}
		}
		if (PlayerAddons.TryGetValue(renderer.GetInstanceID(), out var value))
		{
			if (value == null)
			{
				Debug.LogWarning("Addon is null for instance id: " + renderer.GetInstanceID() + " and BodyPack: " + BodyPackPath);
			}
			if (value != null && value.Renderer == null)
			{
				throw new NullReferenceException("Addon renderer is null when creating addon texture");
			}
			if (value != null && !value.IsInvalid)
			{
				if (RenderTexture.active == null)
				{
					throw new NullReferenceException("Active render texture is null when creating addon texture");
				}
				int width = RenderTexture.active.width;
				int height = RenderTexture.active.height;
				if (value.Materials.Count == 0)
				{
					UnityEngine.Object.Destroy(value.Renderer.material);
					value.Materials.Add(MilMo_Material.GetMaterial("Junebug/Diffuse"));
					value.Renderer.material = value.Materials[0].Material;
					value.Renderer.material.mainTexture = new Texture2D(width, height, TextureFormat.ARGB32, mipChain: false);
					value.Renderer.material.SetTexture(Ramp, MilMo_BodyPackSystem.ShaderRamp);
					value.Renderer.material.SetVector(VelvetChannel, MilMo_BodyPackSystem.CharacterVelvetChannels);
					value.Renderer.material.SetColor(VelvetColor, MilMo_BodyPackSystem.CharacterVelvetColor);
					value.Renderer.material.SetColor(MainColor, MilMo_BodyPackSystem.CharacterMainColor);
				}
				if (value.Renderer.sharedMaterial == null)
				{
					throw new NullReferenceException("Addon shared material is null when creating addon texture");
				}
				Texture2D texture2D = (Texture2D)value.Renderer.sharedMaterial.mainTexture;
				try
				{
					texture2D.ReadPixels(new Rect(0f, 0f, width, height), 0, 0, recalculateMipMaps: true);
					texture2D.Apply();
				}
				catch (Exception ex3)
				{
					Debug.LogWarning("ReadPixels: " + ex3.Message + " for BodyPack: " + BodyPackPath);
				}
			}
		}
		if (renderTexture == null)
		{
			throw new NullReferenceException("Colored soft mesh render texture is null when creating addon texture");
		}
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

	public async void AsyncLoadContent(AddonDone callback)
	{
		if (HasContent || _addonLayers == null)
		{
			callback(success: true);
			return;
		}
		foreach (MilMo_TextureColorGroupPair addonLayer in _addonLayers.ToList())
		{
			string path = "Content/Bodypacks/" + addonLayer.TextureName;
			addonLayer.Texture = await MilMo_ResourceManager.Instance.LoadTextureAsync(path);
		}
		_doneLoading = true;
		callback(HasContent);
	}

	public void AsyncLoadVisualRep(SkinnedMeshRenderer renderer, AddonDone callback)
	{
		if (callback == null)
		{
			Debug.LogWarning("Got null callback when async loading content for addon in " + BodyPackPath);
			return;
		}
		if (!renderer)
		{
			Debug.Log("Renderer was destroyed.");
			callback(success: false);
			return;
		}
		Transform nodeTransform = FindTransform(renderer);
		if (nodeTransform == null)
		{
			OnVisualRepLoaded(renderer, null, success: false);
			callback(success: false);
			return;
		}
		if (PlayerAddons.TryGetValue(renderer.GetInstanceID(), out var value))
		{
			bool success = value != null && !value.IsInvalid;
			OnVisualRepLoaded(renderer, value, success);
			callback(success);
			return;
		}
		MilMo_VisualRepContainer.AsyncCreateVisualRep(VisualRepPath, Vector3.zero, Quaternion.identity, setDefaultMaterialTexture: false, waitForMaterial: true, delegate(MilMo_VisualRep visualRep)
		{
			if (renderer == null || nodeTransform == null)
			{
				Debug.Log("Renderer " + ((renderer == null) ? "(null) " : " ") + "or transform " + ((nodeTransform == null) ? "(null) " : " ") + "is null when loading content for " + BodyPackPath);
				MilMo_VisualRepContainer.DestroyVisualRep(visualRep);
				callback(success: false);
			}
			else if (PlayerAddons.ContainsKey(renderer.GetInstanceID()))
			{
				Debug.Log("Visual rep for addon " + BodyPackPath + " was already loaded for player " + renderer.GetInstanceID());
				MilMo_VisualRepContainer.DestroyVisualRep(visualRep);
				callback(PlayerAddons[renderer.GetInstanceID()] != null);
			}
			else
			{
				PlayerAddons.Add(renderer.GetInstanceID(), visualRep);
				if (visualRep == null || visualRep.GameObject == null || visualRep.GameObject.transform == null)
				{
					Debug.LogWarning("Failed to load visual rep for addon in body pack " + BodyPackPath);
					OnVisualRepLoaded(renderer, null, success: false);
					callback(success: false);
				}
				else
				{
					visualRep.GameObject.transform.parent = nodeTransform.transform;
					visualRep.GameObject.transform.localPosition = Vector3.zero;
					visualRep.GameObject.transform.localRotation = Quaternion.identity;
					visualRep.GameObject.SetActive(value: false);
					visualRep.SetLayerOnRenderObject(14);
					if (_addonLayers != null && _addonLayers.Count > 0 && visualRep.Renderer != null && visualRep.Renderer.sharedMaterial != null && visualRep.Renderer.sharedMaterial.mainTexture != null)
					{
						visualRep.Renderer.sharedMaterial.mainTexture = UnityEngine.Object.Instantiate(visualRep.Renderer.sharedMaterial.mainTexture) as Texture2D;
					}
					OnVisualRepLoaded(renderer, visualRep, success: true);
					callback(success: true);
				}
			}
		});
	}

	protected virtual void OnVisualRepLoaded(SkinnedMeshRenderer renderer, MilMo_VisualRep visualRep, bool success)
	{
	}

	public void PlayAnimation(SkinnedMeshRenderer renderer, string animation)
	{
		if (!(renderer == null) && PlayerAddons.TryGetValue(renderer.GetInstanceID(), out var value) && value != null && !value.IsInvalid && !(value.GameObject == null) && !(value.GameObject.GetComponent<Animation>() == null) && !(value.GameObject.GetComponent<Animation>()[animation] == null))
		{
			value.GameObject.GetComponent<Animation>()[animation].wrapMode = WrapMode.Once;
			value.GameObject.GetComponent<Animation>().CrossFade(animation);
		}
	}

	public Transform GetTransform(SkinnedMeshRenderer renderer)
	{
		if (renderer == null)
		{
			return null;
		}
		if (!PlayerAddons.TryGetValue(renderer.GetInstanceID(), out var value))
		{
			return null;
		}
		if (value == null || value.IsInvalid || value.GameObject == null)
		{
			return null;
		}
		return value.GameObject.transform;
	}

	public GameObject GetGameObject(SkinnedMeshRenderer renderer)
	{
		if (renderer == null)
		{
			return null;
		}
		if (!PlayerAddons.TryGetValue(renderer.GetInstanceID(), out var value))
		{
			return null;
		}
		if (value == null || value.IsInvalid)
		{
			return null;
		}
		return value.GameObject;
	}

	public void UnloadContent(SkinnedMeshRenderer renderer)
	{
		if (PlayerAddons.TryGetValue(renderer.GetInstanceID(), out var value))
		{
			if (value != null && !value.IsInvalid && _addonLayers != null && _addonLayers.Count > 0 && value.Renderer != null && value.Renderer.sharedMaterial != null && value.Renderer.sharedMaterial.mainTexture != null)
			{
				UnityEngine.Object.Destroy(value.Renderer.sharedMaterial.mainTexture);
			}
			MilMo_VisualRepContainer.RemoveFromUpdate(value);
			MilMo_VisualRepContainer.DestroyVisualRep(value);
			PlayerAddons.Remove(renderer.GetInstanceID());
		}
	}

	public virtual void Update(SkinnedMeshRenderer meshRenderer)
	{
	}

	public virtual void Equip(SkinnedMeshRenderer meshRenderer, bool isBoy, float height = 1f)
	{
		if (PlayerAddons.TryGetValue(meshRenderer.GetInstanceID(), out var value))
		{
			if (value == null || value.IsInvalid || value.GameObject == null)
			{
				Debug.LogWarning("Addon or addon game object is null for " + VisualRepPath);
				return;
			}
			value.GameObject.transform.localScale = (_scale ? new Vector3(height, height, height) : new Vector3(1f, 1f, 1f));
			value.GameObject.transform.localPosition = (isBoy ? _boyOffset : _girlOffset);
			value.Enable();
			value.GameObject.SetActive(value: true);
		}
		else
		{
			Debug.LogWarning("Failed to equip addon " + VisualRepPath + ". Not instantiated for this player.");
		}
	}

	public virtual void UnEquip(SkinnedMeshRenderer meshRenderer)
	{
		if (PlayerAddons.TryGetValue(meshRenderer.GetInstanceID(), out var value) && value != null && !value.IsInvalid)
		{
			value.Disable();
			value.GameObject.SetActive(value: false);
		}
	}

	protected virtual void Unload(SkinnedMeshRenderer meshRenderer)
	{
		if (!PlayerAddons.TryGetValue(meshRenderer.GetInstanceID(), out var value))
		{
			return;
		}
		if (value != null)
		{
			if (!value.IsInvalid && _addonLayers != null && _addonLayers.Count > 0 && value.Renderer != null && value.Renderer.sharedMaterial != null && value.Renderer.sharedMaterial.mainTexture != null)
			{
				UnityEngine.Object.Destroy(value.Renderer.sharedMaterial.mainTexture);
			}
			MilMo_VisualRepContainer.RemoveFromUpdate(value);
			MilMo_VisualRepContainer.DestroyVisualRep(value);
		}
		PlayerAddons.Remove(meshRenderer.GetInstanceID());
	}

	private Transform FindTransform(SkinnedMeshRenderer meshRenderer)
	{
		string boneName = MilMo_BodyPackSystem.TheAddonNodes[_node];
		return (from bone in meshRenderer.bones
			where bone.name == boneName
			select bone.Find(_node)).FirstOrDefault();
	}
}
