using System;
using System.Collections.Generic;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using Code.Core.Visual.Materials;
using UnityEngine;

namespace Code.Core.Visual;

public abstract class MilMo_Material
{
	public delegate void MaterialTextureDone(Texture2D texture);

	private const string SHADER_PATH = "Shaders/";

	private const string RAMP_TEXTURE_PATH = "Shaders/VelvetRamp";

	protected const string RAMP_TEXTURE_NAME = "VelvetRamp";

	private static Texture2D _rampTexture;

	private readonly Dictionary<string, MilMo_MaterialTexture> _textures = new Dictionary<string, MilMo_MaterialTexture>();

	private int _texturesLoaded;

	protected int TextureCountToLoad;

	private bool _texturesFinishedLoading;

	protected string MainColorName = "_Color";

	private string _path;

	private string _name;

	private bool _async;

	private string _assetTag;

	private MilMo_ResourceManager.Priority _loadPriority;

	private bool _pauseMode;

	private static readonly int Ramp = Shader.PropertyToID("_Ramp");

	public static Texture2D RampTexture
	{
		get
		{
			if (_rampTexture != null)
			{
				return _rampTexture;
			}
			if (!LoadRampTexture())
			{
				Debug.LogWarning("Failed to load ramp texture 'Shaders/VelvetRamp' for velvet shading.");
			}
			return _rampTexture;
		}
	}

	public Material Material { get; private set; }

	public Color MainColor { get; protected set; }

	protected abstract string Name { get; }

	private static bool LoadRampTexture()
	{
		_rampTexture = MilMo_ResourceManager.Instance.LoadTextureLocal("Shaders/VelvetRamp");
		if (_rampTexture != null)
		{
			_rampTexture.wrapMode = TextureWrapMode.Clamp;
		}
		return _rampTexture != null;
	}

	public static async void AsyncLoadTexture(string texture, string path, string name, string assetTag, MilMo_ResourceManager.Priority priority, bool pauseMode, MaterialTextureDone callback)
	{
		if (texture != null)
		{
			bool flag = texture.Contains("/");
			callback(await MilMo_ResourceManager.Instance.LoadTextureAsync(flag ? texture : (path + texture), assetTag, priority, pauseMode));
		}
	}

	public static Texture2D LoadTexture(string texture, string path, string name)
	{
		Texture2D texture2D = MilMo_ResourceManager.Instance.LoadTextureLocal(texture);
		if (!texture2D)
		{
			Debug.LogWarning("Bad texture path: Failed to load texture '" + texture + "' trying with '" + path + texture + "'");
			texture2D = MilMo_ResourceManager.Instance.LoadTextureLocal(path + texture);
		}
		if (!texture2D)
		{
			Debug.LogWarning("Bad texture path: Failed to load texture '" + path + texture + "' trying with '" + path + name + "' (second try)");
			texture2D = MilMo_ResourceManager.Instance.LoadTextureLocal(path + name);
		}
		return texture2D;
	}

	public static MilMo_Material GetMaterial(string name)
	{
		if (name.Equals("Junebug/PropsStandard", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_PropsStandard();
		}
		if (name.Equals("Junebug/Props2UV", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_Props2UV();
		}
		if (name.Equals("Junebug/Diffuse", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_Diffuse();
		}
		if (name.Equals("Junebug/DiffuseTrans", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_NormalDiffuseTransparent();
		}
		if (name.Equals("Junebug/UnlitDiffuse", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_UnlitDiffuse();
		}
		if (name.Equals("Junebug/UnlitDiffuseTrans", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_UnlitDiffuseTrans();
		}
		if (name.Equals("Junebug/DiffuseAmbient", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_DiffuseAmbient();
		}
		if (name.Equals("Junebug/SkyLayer", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_SkyLayer();
		}
		if (name.Equals("Junebug/SkyDome", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_SkyDome();
		}
		if (name.Equals("Junebug/TerrainSplat", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_TerrainSplat();
		}
		if (name.Equals("Junebug/Transparent/Cutout/Diffuse", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_TransparentCutoutDiffuse();
		}
		if (name.Equals("Junebug/Transparent/Bumped-Specular", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_TransparentBumpSpec();
		}
		if (name.Equals("Junebug/Particles/AlphaBlended", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_ParticlesAlphaBlend();
		}
		if (name.Equals("Junebug/Particles/AdditiveBlended", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_ParticleAdditiveBlend();
		}
		if (name.Equals("Junebug/Particles/AdditiveBlendedZOffset", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_ParticleAdditiveBlendZOffset();
		}
		if (name.Equals("Junebug/StarStoneShader", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_StarStoneMaterial();
		}
		if (name.Equals("Junebug/CutOut", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_CutOut();
		}
		if (name.Equals("Junebug/Door", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_DoorMaterial();
		}
		if (name.Equals("Junebug/FurnitureDiffuse", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_FurnitureDiffuse();
		}
		if (name.Equals("Junebug/FurnitureTransparent", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_FurnitureTransparent();
		}
		if (name.Equals("Junebug/TempMenuShader", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_TempMenuShader();
		}
		Debug.Log("Using non supported material: " + name);
		return null;
	}

	public static void WriteMaterial(Material material, MilMo_SFFile file, int index = -1, MilMo_Material template = null)
	{
		string name = material.shader.name;
		if (name.Equals("Junebug/PropsStandard", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_PropsStandard.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/Props2UV", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_Props2UV.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/Diffuse", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_Diffuse.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/UnlitDiffuse", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_UnlitDiffuse.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/UnlitDiffuseTrans", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_UnlitDiffuseTrans.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/DiffuseTrans", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_NormalDiffuseTransparent.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/DiffuseAmbient", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_DiffuseAmbient.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/SkyLayer", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_SkyLayer.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/SkyDome", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_SkyDome.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/TerrainSplat", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_TerrainSplat.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/Transparent/Cutout/Diffuse", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_TransparentCutoutDiffuse.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/Transparent/Bumped-Specular", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_TransparentBumpSpec.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/Particles/AlphaBlended", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_ParticlesAlphaBlend.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/Particles/AdditiveBlended", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_ParticleAdditiveBlend.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/Particles/AdditiveBlendedZOffset", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_ParticleAdditiveBlendZOffset.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/StarStoneShader", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_StarStoneMaterial.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/CutOut", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_CutOut.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/Door", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_DoorMaterial.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/FurnitureDiffuse", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_FurnitureDiffuse.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/FurnitureTransparent", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_FurnitureTransparent.Write(material, file, index, template);
		}
		else if (name.Equals("Junebug/TempMenuShader", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_TempMenuShader.Write(material, file, index, template);
		}
		else
		{
			Debug.LogWarning("Trying to write non supported material " + name);
		}
	}

	protected static void WriteTexture(Material material, MilMo_SFFile file, MilMo_Material template, string property, string tag)
	{
		if (!material.HasProperty(property))
		{
			return;
		}
		Texture texture = material.GetTexture(property);
		if (texture == null)
		{
			return;
		}
		Vector2 textureScale = material.GetTextureScale(property);
		Vector2 textureOffset = material.GetTextureOffset(property);
		bool flag = false;
		string text = MilMo_ResourceManager.Instance.ResolveTexturePath(texture.name);
		MilMo_MaterialTexture value = null;
		if (template != null)
		{
			template._textures.TryGetValue(property, out value);
			if (value != null && (!MilMo_Utility.Equals(textureScale, value.UVTiling) || !MilMo_Utility.Equals(textureOffset, value.UVOffset)))
			{
				flag = true;
			}
		}
		if (value == null || !(text == value.Path) || flag)
		{
			file.AddRow();
			file.Write(tag);
			file.Write(text);
			if (value == null || !MilMo_Utility.Equals(textureScale, value.UVTiling))
			{
				file.Write(textureScale);
			}
			if (value == null || !MilMo_Utility.Equals(textureOffset, value.UVOffset))
			{
				file.Write(textureOffset);
			}
		}
	}

	protected MilMo_Material(string shaderName)
	{
		MainColor = Color.white;
		Shader shader = MilMo_ResourceManager.LoadShaderLocal("Shaders/" + shaderName);
		if (shader == null)
		{
			Debug.LogWarning("Failed to load shader " + shaderName);
		}
		else
		{
			Material = new Material(shader);
		}
	}

	public abstract void Load(MilMo_SFFile file);

	protected abstract bool CreateInternal();

	public virtual bool IsDone()
	{
		return _texturesFinishedLoading;
	}

	public void SetColor(Color color)
	{
		if ((bool)Material && Material.HasProperty(MainColorName))
		{
			Material.SetColor(MainColorName, color);
		}
	}

	public void ResetColor()
	{
		if ((bool)Material && Material.HasProperty(MainColorName))
		{
			Material.SetColor(MainColorName, MainColor);
		}
	}

	public void SetColor(string property, Color color)
	{
		if ((bool)Material && Material.HasProperty(property))
		{
			Material.SetColor(property, color);
		}
	}

	public void Create(string path, string name, string assetTag = "Generic", MilMo_ResourceManager.Priority priority = MilMo_ResourceManager.Priority.High, bool pauseMode = false, bool async = false)
	{
		_path = path;
		_name = name;
		_async = async;
		_assetTag = assetTag;
		_loadPriority = priority;
		_pauseMode = pauseMode;
		CreateInternal();
	}

	public void Create(string path, string name, string assetTag, bool async)
	{
		Create(path, name, assetTag, MilMo_ResourceManager.Priority.High, pauseMode: false, async);
	}

	private void LoadVelvetRamp(string texturePath)
	{
		if (!Material.HasProperty("_Ramp"))
		{
			TextureLoaded();
		}
		else if (texturePath.Length > 0)
		{
			LoadTexture(texturePath, "_Ramp", delegate
			{
				TextureLoaded();
			});
		}
		else
		{
			Material.SetTexture(Ramp, RampTexture);
			TextureLoaded();
		}
	}

	protected void LoadTexture(string texture, string property, Vector2 offset, Vector2 tiling, MaterialTextureDone callback)
	{
		if (!_async)
		{
			Texture2D texture2D = LoadTexture(texture, _path, _name);
			if (texture2D == null || Material == null)
			{
				Debug.LogWarning("Failed to load texture " + texture + " @ " + _path + _name);
				callback(null);
			}
			else if (!Material.HasProperty(property))
			{
				Debug.LogWarning("Material " + Material.shader?.ToString() + " does not have property " + property + " @ " + _path + _name);
				callback(null);
			}
			else
			{
				Material.SetTexture(property, texture2D);
				Material.SetTextureOffset(property, offset);
				Material.SetTextureScale(property, tiling);
				callback(texture2D);
			}
			return;
		}
		AsyncLoadTexture(texture, _path, _name, _assetTag, _loadPriority, _pauseMode, delegate(Texture2D tex)
		{
			if (tex == null || Material == null || !Material.HasProperty(property))
			{
				Debug.LogWarning("Failed to async load texture " + texture + "@" + _path);
				callback(null);
			}
			else
			{
				Material.SetTexture(property, tex);
				Material.SetTextureOffset(property, offset);
				Material.SetTextureScale(property, tiling);
				callback(tex);
			}
		});
	}

	protected void LoadTexture(string texture, string property, MaterialTextureDone callback)
	{
		LoadTexture(texture, property, new Vector2(0f, 0f), new Vector2(1f, 1f), callback);
	}

	protected void LoadTexture(MilMo_MaterialTexture texture, MaterialTextureDone callback)
	{
		if (texture == null)
		{
			callback(null);
		}
		else
		{
			LoadTexture(texture.Path, texture.Property, texture.UVOffset, texture.UVTiling, callback);
		}
	}

	protected void ReadTexture(MilMo_SFFile file, string property)
	{
		if (!_textures.TryGetValue(property, out var value))
		{
			value = new MilMo_MaterialTexture(property);
			_textures.Add(property, value);
		}
		value.Read(file);
	}

	protected void LoadAllTextures(string velvetRampPath = null)
	{
		TextureCountToLoad = _textures.Count;
		if (velvetRampPath != null)
		{
			TextureCountToLoad++;
		}
		if (TextureCountToLoad == 0)
		{
			_texturesFinishedLoading = true;
			return;
		}
		if (velvetRampPath != null)
		{
			LoadVelvetRamp(velvetRampPath);
		}
		foreach (MilMo_MaterialTexture value in _textures.Values)
		{
			LoadTexture(value, delegate
			{
				TextureLoaded();
			});
		}
	}

	protected void TextureLoaded()
	{
		_texturesLoaded++;
		if (_texturesLoaded == TextureCountToLoad)
		{
			_texturesFinishedLoading = true;
		}
	}

	public static bool LoadMaterial(List<MilMo_Material> materials, MilMo_SFFile file, int currentMaterialIndex)
	{
		file.NextRow();
		int num = -1;
		if (file.IsNext("Index"))
		{
			num = file.GetInt();
			file.NextRow();
		}
		if (num == -1)
		{
			num = currentMaterialIndex;
		}
		string @string = file.GetString();
		MilMo_Material milMo_Material = ((num < materials.Count && materials[num] != null && materials[num].Name == @string) ? materials[num] : GetMaterial(@string));
		if (milMo_Material != null)
		{
			milMo_Material.Load(file);
			if (num < materials.Count)
			{
				materials[num] = milMo_Material;
			}
			else
			{
				materials.Add(milMo_Material);
			}
			return true;
		}
		Debug.LogWarning("Got null material in " + file.Path + " at " + file.GetLineNumber() + ".");
		return false;
	}
}
