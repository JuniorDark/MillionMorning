using System;
using Code.Core.Camera;
using Code.Core.ResourceSystem;
using Code.World.GroundMaterials;
using UnityEngine;

namespace Code.World.Level;

public class MilMo_ImportedLevel : MilMo_Level
{
	public bool LoadInEditorMode(string world, string level)
	{
		base.World = world;
		base.Name = level;
		base.VerboseName = base.World + ":" + base.Name;
		ChannelName = base.World + ":" + base.Name;
		base.WorldContentName = MilMo_LevelData.GetWorldContentName(base.World);
		base.LevelContentName = MilMo_LevelData.GetLevelContentName(base.Name);
		Unload();
		_data = new MilMo_LevelData(base.WorldContentName, base.LevelContentName);
		if (!_data.LoadLocal())
		{
			return false;
		}
		MilMo_Terrain.SetLevelEditorMode(editorMode: true);
		MilMo_Terrain.Load(_data.TerrainData);
		LoadGroundMaterials();
		Environment.Load(this, async: false, _data.Environment);
		LoadProps(async: false, null);
		return true;
	}

	protected override void LoadGroundMaterials()
	{
		MilMo_GroundMaterials.LoadInEditorMode(base.WorldContentName);
		base.GroundMaterialManager = new MilMo_GroundMaterialManager();
		if (!MilMo_Terrain.GameObject)
		{
			throw new NullReferenceException("Terrain game object is null when loading materials manager");
		}
		if (_data == null)
		{
			throw new NullReferenceException("Level data is null when loading materials manager");
		}
		base.GroundMaterialManager.Load(_data, MilMo_Terrain.GameObject.transform.position, _data.TerrainData.size.x, _data.TerrainData.size.z);
	}

	public override void Unload()
	{
		MilMo_Instance.UnloadChatRooms();
		MilMo_Instance.CurrentInstance = null;
		Debug.Log("Unloading climbing surfaces for level " + base.VerboseName);
		UnloadClimbingSurfaces();
		Debug.Log("Unloading environment for level " + base.VerboseName);
		Environment.UnloadEnvironment(editorMode: true);
		Debug.Log("Unloading props for level " + base.VerboseName);
		UnloadProps();
		Debug.Log("Unloading rooms for level " + base.VerboseName);
		UnloadRoom();
		Debug.Log("Unloading gameplay objects for level " + base.VerboseName);
		UnloadGameplayObjects();
		Debug.Log("Unloading terrain for level " + base.VerboseName);
		MilMo_Terrain.Unload();
		Debug.Log("Unloading NPCs for level " + base.VerboseName);
		UnloadNpCs();
		Debug.Log("Unloading creatures for level " + base.VerboseName);
		UnloadCreatures();
		Debug.Log("Unloading items for level " + base.VerboseName);
		UnloadItems();
		Debug.Log("Unloading exploration tokens for level " + base.VerboseName);
		UnloadExplorationTokens();
		Debug.Log("Unloading coin tokens for level " + base.VerboseName);
		UnloadCoinTokens();
		Debug.Log("Unloading static gems for level " + base.VerboseName);
		UnloadStaticGems();
		Debug.Log("Unloading premium token for level " + base.VerboseName);
		UnloadPremiumToken();
		Debug.Log("Unloading camera magnets for level " + base.VerboseName);
		MilMo_CameraController.UnloadCameraMagnets();
		UnloadLevelData();
		Debug.Log("Unloading assets with tag 'Level' for level " + base.VerboseName);
		MilMo_ResourceManager.Instance.UnloadAllByTag("Level");
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}
}
