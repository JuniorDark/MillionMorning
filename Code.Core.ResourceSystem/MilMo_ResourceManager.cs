using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Core.Command;
using Code.Core.Config;
using UnityEngine;

namespace Code.Core.ResourceSystem;

[ExecuteInEditMode]
public class MilMo_ResourceManager : MonoBehaviour
{
	private delegate void TextureDone(Texture2D texture);

	private delegate void TextDone(TextAsset text);

	private delegate void TerrainDataDone(TerrainData terrainData);

	private delegate void GameObjectDone(GameObject gameObject);

	private delegate void AudioDone(AudioClip audioClip);

	private delegate void FlareDone(Flare flare);

	public enum Priority
	{
		High,
		Medium,
		Low
	}

	public delegate string GetTexturePathCallback(string textureName);

	public delegate string GetAudioClipPathCallback(AudioClip clip);

	public delegate void AssetDoneCallback(MilMo_Asset asset);

	public delegate void AssetBundleDoneCallback(MilMo_AssetBundle assetBundle);

	public delegate void AssetsDoneCallback(IList<MilMo_Asset> assets);

	private bool _debug;

	private static MilMo_ResourceManager _instance;

	private AssetLoader _theAssetLoader;

	private AssetBundleLoader _theAssetBundleLoader;

	private readonly Dictionary<string, MilMo_AssetBundle> _loadedAssetBundles = new Dictionary<string, MilMo_AssetBundle>();

	private readonly Dictionary<string, MilMo_Asset> _loadedAssets = new Dictionary<string, MilMo_Asset>();

	public readonly Dictionary<string, List<MilMo_Asset>> LoadedAssetsByTag = new Dictionary<string, List<MilMo_Asset>>();

	private GetTexturePathCallback _textureResolver;

	private GetAudioClipPathCallback _audioClipResolver;

	public string dataUrl = "";

	public bool profileStreaming;

	public static MilMo_ResourceManager Instance
	{
		get
		{
			if ((bool)_instance)
			{
				return _instance;
			}
			_instance = UnityEngine.Object.FindObjectOfType<MilMo_ResourceManager>();
			if ((bool)_instance)
			{
				return _instance;
			}
			_instance = new GameObject("ResourceManager").AddComponent<MilMo_ResourceManager>();
			return _instance;
		}
	}

	public int NumberOfLoadingAssets => _theAssetLoader.NumberOfLoadingAssets;

	public bool SmoothLoading
	{
		set
		{
			_theAssetLoader.SmoothLoading = value;
			Application.backgroundLoadingPriority = ((!value) ? ThreadPriority.High : ThreadPriority.Low);
		}
	}

	public bool Paused
	{
		set
		{
			_theAssetLoader.Paused = value;
		}
	}

	public async Task<Texture2D> LoadTextureAsync(string path, string assetTag = "Generic", Priority priority = Priority.High, bool pauseMode = false)
	{
		TaskCompletionSource<Texture2D> tcs = new TaskCompletionSource<Texture2D>();
		LoadTexture(path, assetTag, priority, pauseMode, delegate(Texture2D file)
		{
			if (file == null)
			{
				Debug.LogWarning("Failed to load Texture at: " + path);
			}
			tcs.TrySetResult(file);
		});
		return await tcs.Task;
	}

	private void LoadTexture(string path, string assetTag, Priority priority, bool pauseMode, TextureDone callback)
	{
		path += "_tex";
		LoadAsset(path, assetTag, priority, pauseMode, delegate(MilMo_Asset loadedAsset)
		{
			callback?.Invoke(loadedAsset?.Asset as Texture2D);
		});
	}

	public async Task<TextAsset> LoadTextAsync(string path, string assetTag = "Generic", Priority priority = Priority.High)
	{
		TaskCompletionSource<TextAsset> tcs = new TaskCompletionSource<TextAsset>();
		LoadText(path, assetTag, priority, delegate(TextAsset file)
		{
			if (file == null)
			{
				Debug.LogWarning("Failed to load Text at: " + path);
			}
			tcs.TrySetResult(file);
		});
		return await tcs.Task;
	}

	private void LoadText(string path, string assetTag, Priority priority, TextDone callback)
	{
		path += "_sf";
		LoadAsset(path, assetTag, priority, delegate(MilMo_Asset loadedAsset)
		{
			callback?.Invoke(loadedAsset?.Asset as TextAsset);
		});
	}

	public async Task<TerrainData> LoadTerrainDataAsync(string path, string assetTag = "Generic", Priority priority = Priority.High)
	{
		TaskCompletionSource<TerrainData> tcs = new TaskCompletionSource<TerrainData>();
		LoadTerrainData(path, assetTag, priority, delegate(TerrainData file)
		{
			if (file == null)
			{
				Debug.LogWarning("Failed to load TerrainData at: " + path);
			}
			tcs.TrySetResult(file);
		});
		return await tcs.Task;
	}

	private void LoadTerrainData(string path, string assetTag, Priority priority, TerrainDataDone callback)
	{
		LoadAsset(path, assetTag, priority, delegate(MilMo_Asset loadedAsset)
		{
			callback?.Invoke(loadedAsset?.Asset as TerrainData);
		});
	}

	public async Task<GameObject> LoadMeshAsync(string path, string assetTag = "Generic", Priority priority = Priority.High)
	{
		TaskCompletionSource<GameObject> tcs = new TaskCompletionSource<GameObject>();
		LoadGameObject(path, assetTag, priority, "_mesh", delegate(GameObject file)
		{
			if (file == null)
			{
				Debug.LogWarning("Failed to load mesh at: " + path);
			}
			tcs.TrySetResult(file);
		});
		return await tcs.Task;
	}

	public async Task<GameObject> LoadPrefabAsync(string path, string assetTag = "Generic", Priority priority = Priority.High)
	{
		TaskCompletionSource<GameObject> tcs = new TaskCompletionSource<GameObject>();
		LoadGameObject(path, assetTag, priority, "", delegate(GameObject file)
		{
			if (file == null)
			{
				Debug.LogWarning("Failed to load prefab at: " + path);
			}
			tcs.TrySetResult(file);
		});
		return await tcs.Task;
	}

	private void LoadGameObject(string path, string assetTag, Priority priority, string suffix, GameObjectDone callback)
	{
		path += suffix;
		LoadAsset(path, assetTag, priority, delegate(MilMo_Asset loadedAsset)
		{
			callback?.Invoke(loadedAsset?.Asset as GameObject);
		});
	}

	public async Task<TerrainLayer> LoadTerrainLayerAsync(string path, string assetTag = "Environment", Priority priority = Priority.High)
	{
		TaskCompletionSource<TerrainLayer> tcs = new TaskCompletionSource<TerrainLayer>();
		path += "_layer";
		TerrainLayer terrainLayer;
		LoadAsset(path, assetTag, priority, delegate(MilMo_Asset loadedAsset)
		{
			terrainLayer = loadedAsset?.Asset as TerrainLayer;
			if (terrainLayer == null)
			{
				Debug.LogWarning("Failed to load Layer at: " + path);
			}
			tcs.TrySetResult(terrainLayer);
		});
		return await tcs.Task;
	}

	public async Task<AudioClip> LoadAudioAsync(string path, string assetTag = "Generic", Priority priority = Priority.High)
	{
		TaskCompletionSource<AudioClip> tcs = new TaskCompletionSource<AudioClip>();
		LoadAudio(path, assetTag, priority, delegate(AudioClip file)
		{
			if (file == null)
			{
				Debug.LogWarning("Failed to load Audio at: " + path);
			}
			tcs.TrySetResult(file);
		});
		return await tcs.Task;
	}

	private void LoadAudio(string path, string assetTag, Priority priority, AudioDone callback)
	{
		LoadAsset(path, assetTag, priority, delegate(MilMo_Asset loadedAsset)
		{
			callback?.Invoke(loadedAsset?.Asset as AudioClip);
		});
	}

	public async Task<Flare> LoadFlareAsync(string path)
	{
		TaskCompletionSource<Flare> tcs = new TaskCompletionSource<Flare>();
		LoadFlare(path, "Generic", Priority.High, delegate(Flare file)
		{
			if (file == null)
			{
				Debug.LogWarning("Failed to load Flare at: " + path);
			}
			tcs.TrySetResult(file);
		});
		return await tcs.Task;
	}

	private void LoadFlare(string path, string assetTag, Priority priority, FlareDone callback)
	{
		LoadAsset(path, assetTag, priority, delegate(MilMo_Asset loadedAsset)
		{
			callback?.Invoke(loadedAsset?.Asset as Flare);
		});
	}

	public void Stop()
	{
		_theAssetLoader.Stopped = true;
	}

	public void Resume()
	{
		_theAssetLoader.Stopped = false;
	}

	public void Awake()
	{
		if (_instance != null)
		{
			_instance.UnloadAll();
			UnityEngine.Object.Destroy(_instance.gameObject);
		}
		Create();
	}

	private void UnloadAll()
	{
		foreach (KeyValuePair<string, MilMo_Asset> item in _loadedAssets.ToList())
		{
			UnloadAsset(item.Key);
		}
	}

	private void Create()
	{
		_debug = MilMo_Config.Instance.IsTrue("Debug.ResourceManager", defaultValue: false);
		profileStreaming = MilMo_Config.Instance.IsTrue("Streaming.Profile", defaultValue: false);
		if (_debug)
		{
			Debug.Log("MilMo_ResourceManager: Creating ResourceManager");
		}
		if (Instance == null)
		{
			Debug.LogWarning("Failed to async create resource manager");
			return;
		}
		dataUrl = Application.streamingAssetsPath + "/";
		MilMo_Command.Instance.RegisterCommand("Streaming.PrintLoadedAssets", Debug_PrintLoadedAssets);
		MilMo_Command.Instance.RegisterCommand("Streaming.AssetsStats", Debug_AssetsStats);
		MilMo_Command.Instance.RegisterCommand("Streaming.Profile", Debug_ToggleProfileStreaming);
		_theAssetLoader = base.gameObject.AddComponent<AssetLoader>();
		_theAssetBundleLoader = base.gameObject.AddComponent<AssetBundleLoader>();
		if (_debug)
		{
			Debug.Log("MilMo_ResourceManager: ResourceManager ready");
		}
	}

	public async Task<bool> ValidateContentServerAsync()
	{
		if (_debug)
		{
			Debug.Log("MilMo_ResourceManager: Requesting content status from " + dataUrl);
		}
		try
		{
			if (await MilMo_SimpleFormat.RealAsyncLoad("Content/Status/ContentServerStatus") == null)
			{
				Debug.LogWarning("MilMo_ResourceManager: Failed to fetch content status file from " + dataUrl);
				return false;
			}
		}
		catch (Exception value)
		{
			Console.WriteLine(value);
			Debug.LogWarning("MilMo_ResourceManager: Failed to fetch content status file from " + dataUrl);
			return false;
		}
		if (_debug)
		{
			Debug.Log("MilMo_ResourceManager: Content status OK");
		}
		return true;
	}

	public void RemoveLoadedAsset(string path)
	{
		_loadedAssets.Remove(path);
	}

	public void AddLoadedAsset(MilMo_Asset asset)
	{
		_loadedAssets.Add(asset.Path, asset);
	}

	public MilMo_Asset GetLoadedAsset(string path)
	{
		_loadedAssets.TryGetValue(path, out var value);
		return value;
	}

	public void RemoveLoadedAssetBundle(string path)
	{
		_loadedAssetBundles.Remove(path);
	}

	public void AddLoadedAssetBundle(MilMo_AssetBundle assetBundle)
	{
		_loadedAssetBundles.Add(assetBundle.Path, assetBundle);
	}

	public MilMo_AssetBundle GetLoadedAssetBundle(string path)
	{
		_loadedAssetBundles.TryGetValue(path, out var value);
		return value;
	}

	public void RegisterTexturePathCallback(GetTexturePathCallback callback)
	{
		_textureResolver = callback;
	}

	public string ResolveTexturePath(string textureName)
	{
		if (_textureResolver == null)
		{
			return textureName;
		}
		return _textureResolver(textureName);
	}

	public void RegisterAudioClipPathCallback(GetAudioClipPathCallback callback)
	{
		_audioClipResolver = callback;
	}

	public string ResolveAudioClipPath(AudioClip clip)
	{
		if (clip == null)
		{
			return null;
		}
		if (_audioClipResolver != null)
		{
			return _audioClipResolver(clip);
		}
		Debug.LogWarning("Attempting to resolve audio clip path when audio clip resolver is null");
		return clip.name;
	}

	public void PreloadAsset(string path, string assetTag, AssetDoneCallback callback = null, Priority priority = Priority.High)
	{
		if (Instance._loadedAssets.TryGetValue(path, out var value))
		{
			callback?.Invoke(value);
		}
		else
		{
			LoadAsset(path, assetTag, priority, callback ?? ((AssetDoneCallback)delegate
			{
			}));
		}
	}

	public void PreloadAssets(string[] paths, string assetTag, AssetsDoneCallback callback, Priority priority = Priority.High)
	{
		int assetsLoaded = 0;
		IList<MilMo_Asset> assets = new List<MilMo_Asset>();
		if (paths == null || paths.Length == 0)
		{
			callback?.Invoke(assets);
			return;
		}
		string[] array = paths;
		foreach (string path in array)
		{
			PreloadAsset(path, assetTag, delegate(MilMo_Asset asset)
			{
				assetsLoaded++;
				if (asset != null)
				{
					assets.Add(asset);
				}
				if (assetsLoaded >= paths.Length - 1)
				{
					callback?.Invoke(assets);
				}
			}, priority);
		}
	}

	private void LoadAsset(string path, string assetTag, Priority priority, AssetDoneCallback callback)
	{
		LoadAsset(path, assetTag, priority, pause: false, callback);
	}

	private void LoadAsset(string path, string assetTag, Priority priority, bool pause, AssetDoneCallback callback)
	{
		MilMo_Asset value;
		if (path.Length < 2)
		{
			callback(null);
		}
		else if (Instance._loadedAssets.TryGetValue(path, out value))
		{
			if (Instance.profileStreaming)
			{
				Debug.Log("Already loaded Callback for: " + path);
			}
			value.AssetDoneCallbacks.Add(callback);
			value.Ready();
		}
		else
		{
			if (Instance.profileStreaming)
			{
				Debug.Log("Loading started for: " + path);
			}
			_theAssetLoader.Load(path, assetTag, priority, pause, callback);
		}
	}

	public void LoadAssetBundle(string path, AssetBundleDoneCallback callback)
	{
		_theAssetBundleLoader.Load(path, callback);
	}

	public void UnloadAllByTag(string assetTag)
	{
		if (Instance.LoadedAssetsByTag.TryGetValue(assetTag, out var value))
		{
			for (int num = value.Count - 1; num >= 0; num--)
			{
				value[num].DecrRef();
			}
		}
	}

	public void UnloadAsset(string path)
	{
		if (Instance._loadedAssets.TryGetValue(path, out var value))
		{
			value.DecrRef();
		}
	}

	private string Debug_PrintLoadedAssets(string[] args)
	{
		Debug.Log(_loadedAssets.Values.Aggregate("Loaded assets:\n<ASSETS>\n", (string current, MilMo_Asset asset) => current + "\t " + asset.Path + " <" + asset.Tag + ">\n") + "</ASSETS>");
		return _loadedAssets.Count + " assets printed to log";
	}

	private string Debug_AssetsStats(string[] args)
	{
		return $"{_loadedAssets.Count} assets loaded";
	}

	private string Debug_ToggleProfileStreaming(string[] args)
	{
		profileStreaming = !profileStreaming;
		return profileStreaming.ToString();
	}

	public TextAsset LoadTextLocal(string path)
	{
		return Resources.Load<TextAsset>(path);
	}

	public TextAsset[] LoadAllLocal(string path)
	{
		return Resources.LoadAll<TextAsset>(path);
	}

	public Texture2D LoadTextureLocal(string path)
	{
		return Resources.Load<Texture2D>(path);
	}

	public static Shader LoadShaderLocal(string path)
	{
		return Resources.Load<Shader>(path);
	}

	public TerrainData LoadTerrainLocal(string path)
	{
		return Resources.Load<TerrainData>(path);
	}

	public TerrainLayer LoadTerrainLayerLocal(string path)
	{
		return Resources.Load<TerrainLayer>(path);
	}

	public Mesh LoadMeshLocal(string path)
	{
		return Resources.Load<Mesh>(path);
	}

	public Flare LoadFlareLocal(string path)
	{
		return Resources.Load<Flare>(path);
	}

	public GameObject LoadGameObjectLocal(string path)
	{
		return Resources.Load<GameObject>(path);
	}
}
