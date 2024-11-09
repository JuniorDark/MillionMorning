using System;
using System.Collections.Generic;
using Code.Core.BodyPack.ColorSystem;
using Code.Core.BodyPack.SkinPartSystem;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.Core.Visual;
using Core;
using Core.Avatar;
using UnityEngine;

namespace Code.Core.BodyPack;

public static class MilMo_BodyPackSystem
{
	public const int THE_DEFAULT_TEXTURE_SIZE = 512;

	private static Shader _theAvatarHighQualityShader;

	private static Shader _theAvatarLowQualityShader;

	public static Texture2D ShaderRamp;

	public static Vector4 CharacterVelvetChannels = new Vector4(0.8f, 1f, 0f, 0f);

	public static Color CharacterVelvetColor = new Color(62f / 85f, 0.81960785f, 1f, 1f);

	public static Color CharacterMainColor = new Color(0.6f, 0.6f, 0.6f, 1f);

	public static GameObject MaleGameObject;

	public static GameObject FemaleGameObject;

	public static MilMo_BlendMesh MaleBlendMesh;

	public static MilMo_BlendMesh FemaleBlendMesh;

	private static IList<MilMo_Color> _theBlendColors;

	private static IDictionary<string, IList<int>> _theColorTemplates;

	public static int MaxAtlasSize = 512;

	public static readonly SkinLayerSections SkinLayerSections = new SkinLayerSections();

	public static readonly Dictionary<string, string> TheAddonNodes = new Dictionary<string, string>();

	private static readonly Dictionary<string, List<string>> CategoryAnimations = new Dictionary<string, List<string>>();

	public static readonly List<Rect> TheSkinColorSections = new List<Rect>();

	public static readonly List<Rect> TheHairColorSections = new List<Rect>();

	public static readonly List<Rect> TheEyeColorSections = new List<Rect>();

	public static readonly List<MilMo_CopyOperation> TheCopyOperations = new List<MilMo_CopyOperation>();

	public const string THE_BODYPACKS_PATH = "Content/Bodypacks/";

	private const string THE_COLOR_TEMPLATE_PATH = "Bodypacks/ColorPalettes/";

	private static bool _boyMeshDone;

	private static bool _girlMeshDone;

	private static bool _boyMainDone;

	private static bool _girlMainDone;

	private static bool _boyTeethDone;

	private static bool _girlTeethDone;

	private static bool _genericDone;

	private static bool _genericLoading;

	public static string ErrorMessage { get; set; }

	private static bool BoyDone
	{
		get
		{
			if (_genericDone && _boyMainDone && _boyMeshDone && _boyTeethDone)
			{
				return MilMo_SkinPartSystem.BoyDone;
			}
			return false;
		}
	}

	private static bool GirlDone
	{
		get
		{
			if (_genericDone && _girlMainDone && _girlMeshDone && _girlTeethDone)
			{
				return MilMo_SkinPartSystem.GirlDone;
			}
			return false;
		}
	}

	public static bool AllDone
	{
		get
		{
			if (BoyDone)
			{
				return GirlDone;
			}
			return false;
		}
	}

	public static Shader GetAvatarShader(bool highQuality)
	{
		if (!highQuality)
		{
			return _theAvatarLowQualityShader;
		}
		return _theAvatarHighQualityShader;
	}

	public static MilMo_Color GetColorFromIndex(int index)
	{
		if (index >= _theBlendColors.Count)
		{
			return null;
		}
		return _theBlendColors[index];
	}

	public static IList<int> GetTemplateColorIndices(string template)
	{
		if (!_theColorTemplates.ContainsKey(template))
		{
			return null;
		}
		return _theColorTemplates[template];
	}

	public static void CreateForBoy()
	{
		MilMo_SkinPartSystem.CreateBoyMainPart(Resources.Load<Texture2D>("AvatarContent/Boy/Main"));
		_boyMainDone = true;
		MilMo_SkinPartSystem.CreateBoyTeeth(Resources.Load<Texture2D>("AvatarContent/Boy/Teeth"));
		_boyTeethDone = true;
		MaleGameObject = Resources.Load<GameObject>("AvatarContent/Boy/Mesh/Boy");
		LoadBoyBlendMesh();
		_boyMeshDone = true;
		MilMo_SkinPartSystem.CreateForBoy();
	}

	public static void CreateForGirl()
	{
		MilMo_SkinPartSystem.CreateGirlMainPart(Resources.Load<Texture2D>("AvatarContent/Girl/Main"));
		_girlMainDone = true;
		MilMo_SkinPartSystem.CreateGirlTeeth(Resources.Load<Texture2D>("AvatarContent/Girl/Teeth"));
		_girlTeethDone = true;
		FemaleGameObject = Resources.Load<GameObject>("AvatarContent/Girl/Mesh/Girl");
		LoadGirlBlendMesh();
		_girlMeshDone = true;
		MilMo_SkinPartSystem.CreateForGirl();
	}

	public static void CreateGeneric()
	{
		if (!_genericDone && !_genericLoading)
		{
			_genericLoading = true;
			_theAvatarHighQualityShader = Resources.Load<Shader>("Shaders/Velvet");
			_theAvatarLowQualityShader = Resources.Load<Shader>("Shaders/VelvetLow");
			ShaderRamp = MilMo_Material.RampTexture;
			if (!LoadColorList())
			{
				ErrorMessage = "Failed to load blend colors";
			}
			if (!LoadColorTemplates())
			{
				ErrorMessage = "Failed to load color templates";
			}
			if (!LoadConf())
			{
				ErrorMessage = "Failed to load body pack configuration file";
			}
			if (!MilMo_ColorSystem.Create())
			{
				ErrorMessage = "Failed to create color system";
			}
			MilMo_SkinPartSystem.CreateEyes(Resources.Load<Texture2D>("AvatarContent/Eyes"));
			_genericLoading = false;
			_genericDone = true;
		}
	}

	private static bool LoadColorList()
	{
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadLocal("Bodypacks/ColorList");
		if (milMo_SFFile == null)
		{
			return false;
		}
		_theBlendColors = new List<MilMo_Color>();
		MilMo_Color milMo_Color = new MilMo_Color();
		while (milMo_Color.Read(milMo_SFFile))
		{
			_theBlendColors.Add(milMo_Color);
			milMo_Color = new MilMo_Color();
		}
		return true;
	}

	private static bool LoadColorTemplates()
	{
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadLocal("Bodypacks/ColorTemplates");
		if (milMo_SFFile == null)
		{
			return false;
		}
		_theColorTemplates = new Dictionary<string, IList<int>>();
		while (milMo_SFFile.NextRow())
		{
			while (milMo_SFFile.HasMoreTokens())
			{
				string text = "Bodypacks/ColorPalettes/" + milMo_SFFile.GetString();
				MilMo_SFFile milMo_SFFile2 = MilMo_SimpleFormat.LoadLocal(text);
				if (milMo_SFFile2 == null)
				{
					Debug.LogWarning("Missing Color Template File " + text);
					continue;
				}
				IList<int> list = new List<int>();
				while (milMo_SFFile2.NextRow())
				{
					while (milMo_SFFile2.HasMoreTokens())
					{
						int @int = milMo_SFFile2.GetInt();
						if (_theBlendColors != null && @int < _theBlendColors.Count)
						{
							list.Add(@int);
						}
						else
						{
							Debug.LogWarning("Trying to add non-existing color index " + @int + " in template " + milMo_SFFile2.Name);
						}
					}
				}
				_theColorTemplates.Add(milMo_SFFile2.Name, list);
			}
		}
		return true;
	}

	private static void LoadBoyBlendMesh()
	{
		MaleBlendMesh = new MilMo_BlendMesh();
		GameObject gameObject = UnityEngine.Object.Instantiate(MaleGameObject);
		SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponentInChildren(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
		if (skinnedMeshRenderer != null)
		{
			Mesh sharedMesh = skinnedMeshRenderer.sharedMesh;
			MaleBlendMesh.Vertices = sharedMesh.vertices;
			MaleBlendMesh.Normals = sharedMesh.normals;
			MaleBlendMesh.Triangles = sharedMesh.triangles;
			MaleBlendMesh.Colors = sharedMesh.colors;
			MaleBlendMesh.UV = sharedMesh.uv;
			MaleBlendMesh.BindPoses = sharedMesh.bindposes;
			MaleBlendMesh.BoneWeights = sharedMesh.boneWeights;
		}
		UnityEngine.Object.Destroy(gameObject);
	}

	private static void LoadGirlBlendMesh()
	{
		FemaleBlendMesh = new MilMo_BlendMesh();
		GameObject gameObject = UnityEngine.Object.Instantiate(FemaleGameObject);
		SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponentInChildren(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
		if (skinnedMeshRenderer != null)
		{
			Mesh sharedMesh = skinnedMeshRenderer.sharedMesh;
			FemaleBlendMesh.Vertices = sharedMesh.vertices;
			FemaleBlendMesh.Normals = sharedMesh.normals;
			FemaleBlendMesh.Triangles = sharedMesh.triangles;
			FemaleBlendMesh.Colors = sharedMesh.colors;
			FemaleBlendMesh.UV = sharedMesh.uv;
			FemaleBlendMesh.BindPoses = sharedMesh.bindposes;
			FemaleBlendMesh.BoneWeights = sharedMesh.boneWeights;
		}
		UnityEngine.Object.Destroy(gameObject);
	}

	public static MilMo_Template BodyPackCreator(string category, string path, string filePath)
	{
		return new MilMo_BodyPack(category, path, filePath);
	}

	public static MilMo_BodyPack GetBodyPackByName(string name)
	{
		MilMo_Template template = Singleton<MilMo_TemplateContainer>.Instance.GetTemplate("BodyPack", "Bodypacks." + name);
		if (template == null)
		{
			Debug.LogWarning("Trying to fetch non existing body pack template " + name);
			return null;
		}
		try
		{
			return (MilMo_BodyPack)template;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Trying to fetch a body pack with wrong template type " + name + "\n" + ex);
			return null;
		}
	}

	private static bool LoadConf()
	{
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadLocal("Bodypacks/Configuration");
		if (milMo_SFFile == null)
		{
			return false;
		}
		while (milMo_SFFile.NextRow())
		{
			switch (milMo_SFFile.GetString())
			{
			case "SkinColorSection":
			{
				int @int = milMo_SFFile.GetInt();
				int int2 = milMo_SFFile.GetInt();
				int int3 = milMo_SFFile.GetInt();
				int int4 = milMo_SFFile.GetInt();
				TheSkinColorSections.Add(NormalizeRect(new Rect(@int, 1024 - int4, int3 - @int, int4 - int2)));
				break;
			}
			case "HairColorSection":
			{
				int int5 = milMo_SFFile.GetInt();
				int int6 = milMo_SFFile.GetInt();
				int int7 = milMo_SFFile.GetInt();
				int int8 = milMo_SFFile.GetInt();
				TheHairColorSections.Add(NormalizeRect(new Rect(int5, 1024 - int8, int7 - int5, int8 - int6)));
				break;
			}
			case "EyeColorSection":
			{
				int int9 = milMo_SFFile.GetInt();
				int int10 = milMo_SFFile.GetInt();
				int int11 = milMo_SFFile.GetInt();
				int int12 = milMo_SFFile.GetInt();
				TheEyeColorSections.Add(NormalizeRect(new Rect(int9, 1024 - int12, int11 - int9, int12 - int10)));
				break;
			}
			case "CopyOperation":
			{
				MilMo_CopyOperation milMo_CopyOperation = new MilMo_CopyOperation();
				if (!milMo_CopyOperation.Load(milMo_SFFile))
				{
					Debug.LogWarning("Failed to load copy operation for body pack system at line " + milMo_SFFile.GetLineNumber());
				}
				else
				{
					TheCopyOperations.Add(milMo_CopyOperation);
				}
				break;
			}
			case "MaxAltasSize":
				MaxAtlasSize = milMo_SFFile.GetInt();
				break;
			case "AddonNode":
			{
				string string3 = milMo_SFFile.GetString();
				string string4 = milMo_SFFile.GetString();
				if (!TheAddonNodes.ContainsKey(string4))
				{
					TheAddonNodes.Add(string4, string3);
				}
				break;
			}
			case "CategoryAnimation":
			{
				string @string = milMo_SFFile.GetString();
				string string2 = milMo_SFFile.GetString();
				if (!CategoryAnimations.TryGetValue(@string, out var value))
				{
					value = new List<string>();
					CategoryAnimations.Add(@string, value);
				}
				value.Add(string2);
				break;
			}
			}
		}
		return true;
	}

	private static Rect NormalizeRect(Rect r)
	{
		return new Rect(r.xMin / 1024f, r.yMin / 1024f, r.width / 1024f, r.height / 1024f);
	}
}
