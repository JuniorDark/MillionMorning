using System.Collections.Generic;
using Code.Core.Global;
using Code.Core.ResourceSystem;
using Code.World.Environment;
using UnityEngine;
using UnityEngine.Rendering;

namespace Code.World;

public static class MilMo_Terrain
{
	private static bool _lowEndTerrain;

	private const float DETAIL_OBJECT_DISTANCE_FACTOR_HE = 1f;

	private const float BASE_MAP_DISTANCE_FACTOR_HE = 1f;

	private const float TREE_BILLBOARD_DISTANCE_FACTOR_HE = 1f;

	private const float TREE_CROSS_FADE_LENGTH_FACTOR_HE = 1f;

	private const float TREE_MAXIMUM_FULL_LOD_COUNT_FACTOR_HE = 1f;

	private const float DETAIL_OBJECT_DISTANCE_FACTOR_LE = 0.5f;

	private const float BASE_MAP_DISTANCE_FACTOR_LE = 0.5f;

	private const float TREE_BILLBOARD_DISTANCE_FACTOR_LE = 0.5f;

	private const float TREE_CROSS_FADE_LENGTH_FACTOR_LE = 0.5f;

	private const float TREE_MAXIMUM_FULL_LOD_COUNT_FACTOR_LE = 0.5f;

	private static float _detailObjectDistance;

	private static float _baseMapDistance;

	private static float _treeBillboardDistance;

	private static float _treeCrossFadeLength;

	private static float _treeMaximumFullLodCount;

	private static Material _material;

	private static readonly List<GameObject> TreeColliders = new List<GameObject>();

	private static bool _editorMode;

	private static Material MaterialTemplate
	{
		get
		{
			if (!_material)
			{
				_material = new Material(Shader.Find("Nature/Terrain/Diffuse"));
			}
			return _material;
		}
	}

	public static GameObject GameObject { get; private set; }

	private static float DetailObjectDistance
	{
		set
		{
			_detailObjectDistance = value;
			Terrain.activeTerrain.detailObjectDistance = _detailObjectDistance * (_lowEndTerrain ? 0.5f : 1f);
		}
	}

	private static float BaseMapDistance
	{
		set
		{
			_baseMapDistance = value;
			Terrain.activeTerrain.basemapDistance = _baseMapDistance * (_lowEndTerrain ? 0.5f : 1f);
		}
	}

	private static float TreeBillboardDistance
	{
		set
		{
			_treeBillboardDistance = value;
			Terrain.activeTerrain.treeBillboardDistance = _treeBillboardDistance * (_lowEndTerrain ? 0.5f : 1f);
		}
	}

	private static float TreeDistance
	{
		set
		{
			Terrain.activeTerrain.treeDistance = value;
		}
	}

	private static int HeightmapMaximumLOD
	{
		set
		{
			Terrain.activeTerrain.heightmapMaximumLOD = value;
		}
	}

	private static float HeightMapPixelError
	{
		set
		{
			Terrain.activeTerrain.heightmapPixelError = value;
		}
	}

	private static float TreeCrossFadeLength
	{
		set
		{
			_treeCrossFadeLength = value;
			Terrain.activeTerrain.treeCrossFadeLength = _treeCrossFadeLength * (_lowEndTerrain ? 0.5f : 1f);
		}
	}

	private static int TreeMaximumFullLODCount
	{
		set
		{
			_treeMaximumFullLodCount = value;
			Terrain.activeTerrain.treeMaximumFullLODCount = (int)(_treeMaximumFullLodCount * (_lowEndTerrain ? 0.5f : 1f));
		}
	}

	public static void Load(TerrainData terrainData)
	{
		if (!terrainData)
		{
			Debug.LogWarning("Can't load terrain with null data.");
			return;
		}
		if ((bool)GameObject)
		{
			Debug.LogWarning("Loading terrain without unloading the previous loaded terrain.");
			Unload();
		}
		GameObject = Terrain.CreateTerrainGameObject(terrainData);
		if (!GameObject)
		{
			Debug.LogWarning("Failed to create terrain game object.");
			return;
		}
		Terrain activeTerrain = Terrain.activeTerrain;
		activeTerrain.materialTemplate = MaterialTemplate;
		GameObject.layer = 18;
		activeTerrain.shadowCastingMode = ShadowCastingMode.Off;
		float num = terrainData.size.x / 2f;
		float num2 = terrainData.size.z / 2f;
		Vector3 position = GameObject.transform.position;
		position.x -= num;
		position.z -= num2;
		GameObject.transform.position = position;
	}

	private static void ShouldCastShadows(bool enable)
	{
		Terrain.activeTerrain.shadowCastingMode = (enable ? ShadowCastingMode.On : ShadowCastingMode.Off);
	}

	private static void SetTerrainLayers(IEnumerable<MilMo_EnvironmentSettings.TerrainLayer> terrainLayers)
	{
		foreach (MilMo_EnvironmentSettings.TerrainLayer terrainLayer in terrainLayers)
		{
			SetLayer(terrainLayer.Index, terrainLayer.Path);
		}
	}

	private static async void SetLayer(int index, string path)
	{
		TerrainLayer terrainLayer = ((!_editorMode) ? (await MilMo_ResourceManager.Instance.LoadTerrainLayerAsync(path)) : MilMo_ResourceManager.Instance.LoadTerrainLayerLocal(path));
		TerrainLayer terrainLayer2 = terrainLayer;
		if (!terrainLayer2)
		{
			Debug.LogWarning("Got invalid terrain layer '" + path + "'");
			return;
		}
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain == null || activeTerrain.terrainData == null)
		{
			Debug.LogWarning("Terrain/TerrainData is null while changing layer!");
			return;
		}
		TerrainLayer[] terrainLayers = activeTerrain.terrainData.terrainLayers;
		TerrainLayer[] array = new TerrainLayer[terrainLayers.Length];
		for (int i = 0; i < terrainLayers.Length; i++)
		{
			array[i] = terrainLayers[i];
			if (i == index)
			{
				array[i] = terrainLayer2;
			}
		}
		activeTerrain.terrainData.terrainLayers = array;
	}

	private static void SetDetailSprite(IEnumerable<MilMo_EnvironmentSettings.DetailSprite> detailSprites)
	{
		foreach (MilMo_EnvironmentSettings.DetailSprite detailSprite in detailSprites)
		{
			SetDetailSprite(detailSprite.Index, detailSprite.Path, detailSprite.DryColor, detailSprite.HealthyColor);
		}
	}

	private static async void SetDetailSprite(int index, string path, Color dryColor, Color healthyColor)
	{
		Texture2D texture2D = ((!_editorMode) ? (await MilMo_ResourceManager.Instance.LoadTextureAsync(path)) : MilMo_ResourceManager.Instance.LoadTextureLocal(path));
		Texture2D texture2D2 = texture2D;
		if (texture2D2 == null)
		{
			Debug.LogWarning("Got invalid terrain detail sprite '" + path + "'");
			return;
		}
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain == null || activeTerrain.terrainData == null)
		{
			Debug.LogWarning("Terrain/TerrainData is null while changing splat!");
			return;
		}
		DetailPrototype[] detailPrototypes = activeTerrain.terrainData.detailPrototypes;
		DetailPrototype[] array = new DetailPrototype[detailPrototypes.Length];
		for (int i = 0; i < detailPrototypes.Length; i++)
		{
			DetailPrototype detailPrototype = detailPrototypes[i];
			DetailPrototype detailPrototype2 = (array[i] = new DetailPrototype());
			detailPrototype2.prototypeTexture = detailPrototype.prototypeTexture;
			detailPrototype2.minWidth = detailPrototype.minWidth;
			detailPrototype2.maxWidth = detailPrototype.maxWidth;
			detailPrototype2.minHeight = detailPrototype.minHeight;
			detailPrototype2.maxHeight = detailPrototype.maxHeight;
			detailPrototype2.noiseSpread = detailPrototype.noiseSpread;
			detailPrototype2.healthyColor = detailPrototype.healthyColor;
			detailPrototype2.dryColor = detailPrototype.dryColor;
			if (i == index)
			{
				detailPrototype2.prototypeTexture = texture2D2;
				detailPrototype2.dryColor = dryColor;
				detailPrototype2.healthyColor = healthyColor;
			}
		}
		activeTerrain.terrainData.detailPrototypes = array;
	}

	private static void SetTreeObjects(IEnumerable<MilMo_EnvironmentSettings.TreeObject> treeObjects)
	{
		foreach (MilMo_EnvironmentSettings.TreeObject treeObject in treeObjects)
		{
			SetTree(treeObject.Index, treeObject.Path);
		}
	}

	private static async void SetTree(int index, string path)
	{
		GameObject gameObject = ((!_editorMode) ? (await MilMo_ResourceManager.Instance.LoadPrefabAsync(path)) : MilMo_ResourceManager.Instance.LoadGameObjectLocal(path));
		GameObject gameObject2 = gameObject;
		if (gameObject2 == null)
		{
			Debug.LogWarning("Got invalid terrain tree prefab '" + path + "'");
			return;
		}
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain == null || activeTerrain.terrainData == null)
		{
			Debug.LogWarning("Terrain/TerrainData is null while changing tree!");
			return;
		}
		TreePrototype[] treePrototypes = activeTerrain.terrainData.treePrototypes;
		TreePrototype[] array = new TreePrototype[treePrototypes.Length];
		for (int i = 0; i < treePrototypes.Length; i++)
		{
			TreePrototype treePrototype = treePrototypes[i];
			TreePrototype treePrototype2 = (array[i] = new TreePrototype());
			treePrototype2.bendFactor = treePrototype.bendFactor;
			treePrototype2.prefab = treePrototype.prefab;
			if (i == index)
			{
				treePrototype2.prefab = gameObject2;
			}
		}
		activeTerrain.terrainData.treePrototypes = array;
	}

	public static void InstantiateTreeColliders()
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		if (!activeTerrain || !activeTerrain.terrainData)
		{
			Debug.LogWarning("Terrain/TerrainData is null while instantiating tree colliders!");
			return;
		}
		TreeInstance[] treeInstances = activeTerrain.terrainData.treeInstances;
		for (int i = 0; i < treeInstances.Length; i++)
		{
			TreeInstance treeInstance = treeInstances[i];
			int prototypeIndex = treeInstance.prototypeIndex;
			if (activeTerrain.terrainData.treePrototypes.Length - 1 < prototypeIndex || activeTerrain.terrainData.treePrototypes[prototypeIndex] == null)
			{
				Debug.LogWarning("TreePrototype (" + prototypeIndex + ") is null!");
				continue;
			}
			GameObject prefab = activeTerrain.terrainData.treePrototypes[prototypeIndex].prefab;
			if (!prefab)
			{
				continue;
			}
			Collider[] componentsInChildren = prefab.transform.GetComponentsInChildren<Collider>();
			if (componentsInChildren.Length == 0)
			{
				continue;
			}
			GameObject gameObject = new GameObject
			{
				name = "TreeCollider"
			};
			Vector3 position = Vector3.Scale(treeInstance.position, activeTerrain.terrainData.size) + activeTerrain.transform.position;
			Vector3 localScale = prefab.transform.localScale;
			Vector3 localScale2 = new Vector3(treeInstance.widthScale * localScale.x, treeInstance.heightScale * localScale.y, treeInstance.widthScale * localScale.z);
			gameObject.transform.localScale = localScale2;
			gameObject.transform.position = position;
			Collider[] array = componentsInChildren;
			foreach (Collider obj in array)
			{
				MeshCollider meshCollider = obj as MeshCollider;
				if (meshCollider != null)
				{
					MeshCollider meshCollider2 = gameObject.AddComponent<MeshCollider>();
					meshCollider2.convex = meshCollider.convex;
					meshCollider2.cookingOptions = meshCollider.cookingOptions;
					meshCollider2.sharedMesh = meshCollider.sharedMesh;
				}
				CapsuleCollider capsuleCollider = obj as CapsuleCollider;
				if (capsuleCollider != null)
				{
					CapsuleCollider capsuleCollider2 = gameObject.AddComponent<CapsuleCollider>();
					capsuleCollider2.center = capsuleCollider.center;
					capsuleCollider2.direction = capsuleCollider.direction;
					capsuleCollider2.height = capsuleCollider.height;
					capsuleCollider2.radius = capsuleCollider.radius;
				}
			}
			TreeColliders.Add(gameObject);
		}
	}

	private static void UnloadTreeColliders()
	{
		foreach (GameObject treeCollider in TreeColliders)
		{
			MilMo_Global.Destroy(treeCollider);
		}
		TreeColliders.Clear();
	}

	public static void Unload()
	{
		UnloadTreeColliders();
		if ((bool)GameObject)
		{
			MilMo_Global.Destroy(GameObject, forceImmediate: true);
			GameObject = null;
		}
	}

	public static void ApplySettingsAsync(MilMo_EnvironmentSettings environmentSettings)
	{
		SetTerrainLayers(environmentSettings.TerrainLayers);
		SetDetailSprite(environmentSettings.DetailSprites);
		SetTreeObjects(environmentSettings.TreeObjects);
		TreeBillboardDistance = environmentSettings.TreeBillboardDistance;
		TreeCrossFadeLength = environmentSettings.TreeCrossFadeLength;
		TreeDistance = environmentSettings.TreeDistance;
		TreeMaximumFullLODCount = environmentSettings.TreeMaximumFullLODCount;
		HeightmapMaximumLOD = environmentSettings.HeightmapMaximumLOD;
		HeightMapPixelError = environmentSettings.HeightmapPixelError;
		BaseMapDistance = environmentSettings.BasemapDistance;
		DetailObjectDistance = environmentSettings.DetailObjectDistance;
		ShouldCastShadows(environmentSettings.TerrainCastShadows);
		if ((bool)GameObject)
		{
			GameObject.transform.position = environmentSettings.TerrainPosition;
		}
	}

	public static void UseLowEnd()
	{
		if (!_lowEndTerrain)
		{
			_lowEndTerrain = true;
			if ((bool)Terrain.activeTerrain)
			{
				DetailObjectDistance = _detailObjectDistance;
				BaseMapDistance = _baseMapDistance;
				TreeBillboardDistance = _treeBillboardDistance;
				TreeCrossFadeLength = _treeCrossFadeLength;
				TreeMaximumFullLODCount = (int)_treeMaximumFullLodCount;
				Terrain.activeTerrain.materialTemplate = MaterialTemplate;
			}
		}
	}

	public static void UseHighEnd()
	{
		if (_lowEndTerrain)
		{
			_lowEndTerrain = false;
			if ((bool)Terrain.activeTerrain)
			{
				DetailObjectDistance = _detailObjectDistance;
				BaseMapDistance = _baseMapDistance;
				TreeBillboardDistance = _treeBillboardDistance;
				TreeCrossFadeLength = _treeCrossFadeLength;
				TreeMaximumFullLODCount = (int)_treeMaximumFullLodCount;
				Terrain.activeTerrain.materialTemplate = MaterialTemplate;
			}
		}
	}

	public static void SetLevelEditorMode(bool editorMode)
	{
		_editorMode = editorMode;
	}
}
