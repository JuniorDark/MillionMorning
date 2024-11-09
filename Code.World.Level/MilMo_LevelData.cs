using Code.Core.ResourceSystem;
using Code.World.GUI.LoadingScreen;
using Code.World.Level.LevelInfo;
using UnityEngine;

namespace Code.World.Level;

public class MilMo_LevelData
{
	public delegate void LevelDataDone(bool success);

	private readonly string _worldContentName;

	private readonly string _levelContentName;

	private readonly string _resourcePath;

	private readonly string _levelIconPath;

	private int _assetCounter;

	private const int ASSET_COUNTER_ALL = 12;

	public string VerboseName => _worldContentName + ":" + _levelContentName;

	public TerrainData TerrainData { get; private set; }

	public MilMo_SFFile Environment { get; private set; }

	public MilMo_SFFile PreStream { get; private set; }

	public MilMo_SFFile GroundMaterials { get; private set; }

	public MilMo_SFFile Music { get; private set; }

	public MilMo_SFFile MusicAreas { get; private set; }

	public MilMo_SFFile ClimbingSurfaces { get; private set; }

	public MilMo_SFFile Props { get; private set; }

	public MilMo_SFFile TutorialAreas { get; private set; }

	public Texture2D GroundMaterialPrimary { get; private set; }

	public Texture2D GroundMaterialSecondary { get; private set; }

	public Texture2D Icon { get; private set; }

	public static string GetWorldContentName(string world)
	{
		if (string.IsNullOrEmpty(world) || world.Length <= 4)
		{
			return "";
		}
		return "W" + world.Substring(world.Length - 2);
	}

	public static string GetLevelContentName(string level)
	{
		if (string.IsNullOrEmpty(level) || level.Length <= 4)
		{
			return "";
		}
		return "L" + level.Substring(level.Length - 2);
	}

	public static string GetLevelIconPath(string world, string level)
	{
		if (string.IsNullOrEmpty(world) || string.IsNullOrEmpty(level))
		{
			return "";
		}
		string worldContentName = GetWorldContentName(world);
		string levelContentName = GetLevelContentName(level);
		if (string.IsNullOrEmpty(worldContentName) || string.IsNullOrEmpty(levelContentName))
		{
			return "MISSING_LEVEL_ICON";
		}
		return "Content/Worlds/" + worldContentName + "/LevelIcons/LevelIcon" + worldContentName + levelContentName;
	}

	public static string GetLevelIconAddressableKey(string world, string level)
	{
		if (string.IsNullOrEmpty(world) || string.IsNullOrEmpty(level))
		{
			return "";
		}
		string worldContentName = GetWorldContentName(world);
		string levelContentName = GetLevelContentName(level);
		if (string.IsNullOrEmpty(worldContentName) || string.IsNullOrEmpty(levelContentName))
		{
			return "MISSING_LEVEL_ICON";
		}
		return "LevelIcon" + worldContentName + levelContentName;
	}

	public static void LoadAndSetLevelIcon(string world, string level)
	{
		string levelIconPath = GetLevelIconPath(world, level);
		if (!string.IsNullOrEmpty(levelIconPath))
		{
			MilMo_LevelInfoData levelInfoData = MilMo_LevelInfo.GetLevelInfoData(world + ":" + level);
			if (levelInfoData != null)
			{
				MilMo_LoadingScreen.Instance.SetPremiumIcon(levelInfoData.PremiumToken);
				LoadAndSetIconAsync(levelIconPath);
			}
		}
	}

	private static async void LoadAndSetIconAsync(string iconPath)
	{
		Texture2D icon = await MilMo_ResourceManager.Instance.LoadTextureAsync(iconPath, "LevelIcon");
		MilMo_LoadingScreen.Instance.LevelIconLoaded(icon);
	}

	public MilMo_LevelData(string worldContentName, string levelContentName)
	{
		_worldContentName = worldContentName;
		_levelContentName = levelContentName;
		_levelIconPath = "Content/Worlds/" + _worldContentName + "/LevelIcons/LevelIcon" + _worldContentName + _levelContentName;
		_resourcePath = "Content/Worlds/" + _worldContentName + "/Levels/" + _levelContentName + "/";
	}

	public void Unload()
	{
		TerrainData = null;
		GroundMaterialPrimary = null;
		GroundMaterialSecondary = null;
	}

	public bool LoadLocal()
	{
		TerrainData = MilMo_ResourceManager.Instance.LoadTerrainLocal(_resourcePath + "TerrainData");
		Environment = MilMo_SimpleFormat.LoadLocal(_resourcePath + "Environment");
		PreStream = MilMo_SimpleFormat.LoadLocal(_resourcePath + "PreStream");
		GroundMaterials = MilMo_SimpleFormat.LoadLocal(_resourcePath + "GroundMaterials");
		Music = MilMo_SimpleFormat.LoadLocal(_resourcePath + "Music");
		MusicAreas = MilMo_SimpleFormat.LoadLocal(_resourcePath + "MusicAreas");
		TutorialAreas = MilMo_SimpleFormat.LoadLocal(_resourcePath + "TutorialAreas");
		Props = MilMo_SimpleFormat.LoadLocal(_resourcePath + "Props");
		GroundMaterialPrimary = MilMo_ResourceManager.Instance.LoadTextureLocal(_resourcePath + "GroundMaterial01");
		GroundMaterialSecondary = MilMo_ResourceManager.Instance.LoadTextureLocal(_resourcePath + "GroundMaterial02");
		return Success();
	}

	public async void AsyncLoad(LevelDataDone callback)
	{
		_assetCounter = 0;
		Icon = await MilMo_ResourceManager.Instance.LoadTextureAsync(_levelIconPath, "LevelIcon");
		AssetArrived(callback);
		TerrainData = await MilMo_ResourceManager.Instance.LoadTerrainDataAsync(_resourcePath + "TerrainData", "Level");
		MilMo_LoadingScreen.Instance.TerrainDataLoaded();
		AssetArrived(callback);
		MilMo_SimpleFormat.AsyncLoad(_resourcePath + "GroundMaterials", "Level", delegate(MilMo_SFFile file)
		{
			GroundMaterials = file;
			MilMo_LoadingScreen.Instance.GroundMaterialsFileLoaded();
			AssetArrived(callback);
		});
		GroundMaterialPrimary = await MilMo_ResourceManager.Instance.LoadTextureAsync(_resourcePath + "GroundMaterial01", "Level");
		MilMo_LoadingScreen.Instance.GroundMaterials01Loaded();
		AssetArrived(callback);
		GroundMaterialSecondary = await MilMo_ResourceManager.Instance.LoadTextureAsync(_resourcePath + "GroundMaterial02", "Level");
		MilMo_LoadingScreen.Instance.GroundMaterials02Loaded();
		AssetArrived(callback);
		MilMo_SimpleFormat.AsyncLoad(_resourcePath + "Environment", "Level", delegate(MilMo_SFFile file)
		{
			Environment = file;
			MilMo_LoadingScreen.Instance.EnvironmentLoaded();
			AssetArrived(callback);
		});
		MilMo_SimpleFormat.AsyncLoad(_resourcePath + "Music", "Level", delegate(MilMo_SFFile file)
		{
			Music = file;
			MilMo_LoadingScreen.Instance.MusicLoaded();
			AssetArrived(callback);
		});
		MilMo_SimpleFormat.AsyncLoad(_resourcePath + "MusicAreas", "Level", delegate(MilMo_SFFile file)
		{
			MusicAreas = file;
			MilMo_LoadingScreen.Instance.MusicAreasLoaded();
			AssetArrived(callback);
		});
		MilMo_SimpleFormat.AsyncLoad(_resourcePath + "TutorialAreas", "Level", delegate(MilMo_SFFile file)
		{
			TutorialAreas = file;
			AssetArrived(callback);
		});
		MilMo_SimpleFormat.AsyncLoad(_resourcePath + "ClimbingSurfaces", "Level", delegate(MilMo_SFFile file)
		{
			ClimbingSurfaces = file;
			MilMo_LoadingScreen.Instance.ClimbingSurfacesLoaded();
			AssetArrived(callback);
		});
		MilMo_SimpleFormat.AsyncLoad(_resourcePath + "PreStream", "Level", delegate(MilMo_SFFile file)
		{
			PreStream = file;
			MilMo_LoadingScreen.Instance.PreStreamListLoaded();
			AssetArrived(callback);
		});
		MilMo_SimpleFormat.AsyncLoad(_resourcePath + "Props", "Level", delegate(MilMo_SFFile file)
		{
			Props = file;
			MilMo_LoadingScreen.Instance.PropsFileLoaded(file);
			AssetArrived(callback);
		});
	}

	private void AssetArrived(LevelDataDone callback)
	{
		_assetCounter++;
		if (_assetCounter >= 12)
		{
			callback(Success());
		}
	}

	private bool Success()
	{
		if (!TerrainData)
		{
			Debug.LogWarning("Terrain data is null when async loading " + VerboseName);
		}
		if (Props == null)
		{
			Debug.LogWarning("Props is null when async loading " + VerboseName);
		}
		if (Environment == null)
		{
			Debug.LogWarning("Environment is null when async loading " + VerboseName);
		}
		if ((bool)TerrainData && Props != null)
		{
			return Environment != null;
		}
		return false;
	}
}
