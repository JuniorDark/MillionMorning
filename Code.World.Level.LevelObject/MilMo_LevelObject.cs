using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Core.Global;
using Code.Core.Network.types;
using Code.Core.ObjectEffectSystem;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.Core.Utility;
using Code.Core.Visual;
using UnityEngine;

namespace Code.World.Level.LevelObject;

public abstract class MilMo_LevelObject
{
	public delegate void OnReadDone(bool success, MilMo_LevelObject obj);

	private delegate void LevelObjectDone(bool success);

	public delegate void IconDone(Texture2D icon);

	private const float SQRD_LOW_LOAD_PRIORITY_DISTANCE = 3600f;

	private const float SQRD_MEDIUM_LOAD_PRIORITY_DISTANCE = 1600f;

	protected string BasePath;

	protected string FullPath;

	protected string VisualRepName;

	public Vector3 SpawnPosition;

	protected Vector3 SpawnRotation;

	protected Vector3 Scale = Vector3.one;

	protected readonly List<string> SpawnEffectNames = new List<string>();

	protected readonly List<string> RemovalEffectNames = new List<string>();

	private readonly List<MilMo_ObjectEffect> _removalEffects = new List<MilMo_ObjectEffect>();

	private readonly List<MilMo_ObjectEffect> _inactiveRemovalEffects = new List<MilMo_ObjectEffect>();

	protected bool IsSpawning;

	protected bool Paused;

	private float _creationTime;

	private float _totalLifeSpan;

	protected bool WaitForMaterial;

	protected OnReadDone ReadDoneCallback;

	protected bool UseSpawnEffect = true;

	protected bool IsReady;

	private readonly Dictionary<string, Texture2D> _icons = new Dictionary<string, Texture2D>();

	private readonly List<IconDone> _iconCallbacks = new List<IconDone>();

	public readonly List<MilMo_ObjectEffect> SpawnEffects = new List<MilMo_ObjectEffect>();

	public readonly List<MilMo_ObjectEffect> ObjectEffects = new List<MilMo_ObjectEffect>();

	public int Id { get; protected set; }

	public string FullLevelName { get; protected set; }

	public Vector3 Position
	{
		get
		{
			if (!(GameObject != null))
			{
				return SpawnPosition;
			}
			return GameObject.transform.position;
		}
	}

	public GameObject GameObject { get; protected set; }

	public MilMo_VisualRep VisualRep { get; private set; }

	protected event Action DoneSpawning = delegate
	{
	};

	private MilMo_ResourceManager.Priority GetLoadPriority()
	{
		float sqrMagnitude = (MilMo_Global.Camera.transform.position - SpawnPosition).sqrMagnitude;
		if (sqrMagnitude > 3600f)
		{
			return MilMo_ResourceManager.Priority.Low;
		}
		if (sqrMagnitude > 1600f)
		{
			return MilMo_ResourceManager.Priority.Medium;
		}
		return MilMo_ResourceManager.Priority.High;
	}

	protected MilMo_LevelObject(string path, bool useSpawnEffect)
	{
		UseSpawnEffect = useSpawnEffect;
		BasePath = path;
		_creationTime = Time.time;
	}

	protected MilMo_LevelObject(Vector3 position, Vector3 rotation, Vector3 scale, string fullPath)
	{
		FullPath = fullPath;
		BasePath = MilMo_Utility.RemoveFileNameFromFullPath(fullPath);
		VisualRepName = MilMo_Utility.ExtractNameFromPath(fullPath);
		SpawnPosition = position;
		SpawnRotation = rotation;
		Scale = scale;
		_creationTime = Time.time;
	}

	public async Task<AudioClip> LoadVoiceAsync(string voice)
	{
		string path = "Content/Sounds/Batch01/Creatures/NPCs/" + voice;
		return await MilMo_ResourceManager.Instance.LoadAudioAsync(path);
	}

	public async Task<Texture2D> LoadIconAsync(string appendix)
	{
		TaskCompletionSource<Texture2D> tcs = new TaskCompletionSource<Texture2D>();
		AsyncGetIcon(appendix, delegate(Texture2D file)
		{
			tcs.TrySetResult(file);
		});
		return await tcs.Task;
	}

	public void AsyncGetIcon(string appendix, IconDone callback)
	{
		AsyncGetIcon(appendix, MilMo_ResourceManager.Priority.High, callback);
	}

	protected void AsyncGetIcon(string appendix, MilMo_ResourceManager.Priority priority, IconDone callback)
	{
		string text = MilMo_Utility.RemoveFileNameFromFullPath(VisualRepName);
		string text2 = MilMo_Utility.ExtractNameFromPath(VisualRepName);
		string text3 = BasePath + text + "Icon" + text2 + appendix;
		if (_icons.ContainsKey(text3))
		{
			callback?.Invoke(_icons[text3]);
			return;
		}
		if (callback != null)
		{
			_iconCallbacks.Add(callback);
		}
		LoadAndSetLevelIconAsync(priority, text3);
	}

	private async void LoadAndSetLevelIconAsync(MilMo_ResourceManager.Priority priority, string iconPath)
	{
		Texture2D texture2D = await MilMo_ResourceManager.Instance.LoadTextureAsync(iconPath, "LevelIcon", priority);
		if (texture2D != null && !_icons.ContainsKey(iconPath))
		{
			_icons.Add(iconPath, texture2D);
		}
		foreach (IconDone iconCallback in _iconCallbacks)
		{
			iconCallback(texture2D);
		}
		_iconCallbacks.Clear();
	}

	private void AsyncLoad(LevelObjectDone callback)
	{
		if (string.IsNullOrEmpty(VisualRepName))
		{
			VisualRep = null;
			bool success = FinishLoad();
			callback(success);
			return;
		}
		FullPath = BasePath + VisualRepName;
		MilMo_VisualRepContainer.AsyncCreateVisualRep(FullPath, null, SpawnPosition, Quaternion.Euler(SpawnRotation), new Vector3(1f, 1f, 1f), "Level", setDefaultMaterialTexture: true, WaitForMaterial, pauseModeOnMaterial: false, GetLoadPriority(), delegate(MilMo_VisualRep visualRep)
		{
			if (visualRep == null)
			{
				Debug.LogWarning("Failed to load visual rep for level object " + FullPath);
			}
			VisualRep = visualRep;
			bool success2 = FinishLoad();
			callback(success2);
		});
	}

	protected virtual bool FinishLoad()
	{
		if (VisualRep == null)
		{
			return false;
		}
		GameObject = VisualRep.GameObject;
		if (GameObject == null)
		{
			return false;
		}
		VisualRep.SetLayerOnRenderObject(10);
		Vector3 localScale = GameObject.transform.localScale;
		localScale.Scale(Scale);
		GameObject.transform.localScale = localScale;
		if (UseSpawnEffect)
		{
			CreateSpawnEffects();
		}
		CreateObjectEffects();
		CreateRemovalEffects();
		return true;
	}

	public virtual void Unload()
	{
		foreach (MilMo_ObjectEffect spawnEffect in SpawnEffects)
		{
			spawnEffect.Destroy();
		}
		SpawnEffects.Clear();
		foreach (MilMo_ObjectEffect objectEffect in ObjectEffects)
		{
			objectEffect.Destroy();
		}
		ObjectEffects.Clear();
		foreach (MilMo_ObjectEffect removalEffect in _removalEffects)
		{
			removalEffect.Destroy();
		}
		_removalEffects.Clear();
		foreach (MilMo_ObjectEffect inactiveRemovalEffect in _inactiveRemovalEffects)
		{
			inactiveRemovalEffect.Destroy();
		}
		_inactiveRemovalEffects.Clear();
		if (VisualRep != null)
		{
			MilMo_VisualRepContainer.RemoveFromUpdate(VisualRep);
			MilMo_VisualRepContainer.DestroyVisualRep(VisualRep);
			VisualRep = null;
		}
	}

	public virtual void Update()
	{
		if (Paused)
		{
			return;
		}
		UpdateSpawnEffects();
		UpdateObjectEffects();
		float num = Time.time - _creationTime;
		for (int num2 = _inactiveRemovalEffects.Count - 1; num2 >= 0; num2--)
		{
			if (num >= _totalLifeSpan - _inactiveRemovalEffects[num2].Duration)
			{
				_removalEffects.Add(_inactiveRemovalEffects[num2]);
				_inactiveRemovalEffects.RemoveAt(num2);
			}
		}
		for (int num3 = _removalEffects.Count - 1; num3 >= 0; num3--)
		{
			if (!_removalEffects[num3].Update())
			{
				_removalEffects.RemoveAt(num3);
			}
		}
		VisualRep?.Update();
	}

	public virtual void FixedUpdate()
	{
		if (Paused)
		{
			return;
		}
		if (IsSpawning)
		{
			foreach (MilMo_ObjectEffect spawnEffect in SpawnEffects)
			{
				spawnEffect.FixedUpdate();
			}
		}
		foreach (MilMo_ObjectEffect removalEffect in _removalEffects)
		{
			removalEffect.FixedUpdate();
		}
	}

	public virtual void LateUpdate()
	{
	}

	public virtual void Read(Code.Core.Network.types.LevelObject levelObject, OnReadDone callback)
	{
		ReadDoneCallback = callback;
		Id = levelObject.GetId();
		FullLevelName = levelObject.GetFullLevelName();
		SpawnPosition = new Vector3(levelObject.GetPosition().GetX(), levelObject.GetPosition().GetY(), levelObject.GetPosition().GetZ());
		SpawnRotation = new Vector3(levelObject.GetRotation().GetX(), levelObject.GetRotation().GetY(), levelObject.GetRotation().GetZ());
		SpawnEffectNames.AddRange(levelObject.GetSpawnTypes());
		RemovalEffectNames.AddRange(levelObject.GetRemovalEffects());
		_totalLifeSpan = levelObject.GetLifespan();
	}

	protected void UpdateSpawnEffects()
	{
		if (!IsSpawning)
		{
			return;
		}
		for (int num = SpawnEffects.Count - 1; num >= 0; num--)
		{
			if (!SpawnEffects[num].Update())
			{
				SpawnEffects.RemoveAt(num);
				if (SpawnEffects.Count == 0)
				{
					IsSpawning = false;
					this.DoneSpawning?.Invoke();
				}
			}
		}
	}

	protected void UpdateObjectEffects()
	{
		for (int num = ObjectEffects.Count - 1; num >= 0; num--)
		{
			if (!ObjectEffects[num].Update())
			{
				ObjectEffects.RemoveAt(num);
			}
		}
	}

	protected virtual void FinishRead(MilMo_Template template, bool timeOut)
	{
		if (timeOut)
		{
			return;
		}
		AsyncLoad(delegate(bool success)
		{
			if (!success)
			{
				Debug.LogWarning("Failed to load level object " + FullPath);
			}
			ReadDoneCallback(success, this);
		});
	}

	protected virtual void CreateSpawnEffects()
	{
		if (!MilMo_World.Instance.enabled)
		{
			return;
		}
		bool flag = false;
		foreach (string spawnEffectName in SpawnEffectNames)
		{
			MilMo_ObjectEffect objectEffect = MilMo_ObjectEffectSystem.GetObjectEffect(GameObject, spawnEffectName);
			if (objectEffect != null)
			{
				SpawnEffects.Add(objectEffect);
				if (objectEffect is MilMo_SoundEffect)
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			MilMo_ObjectEffect objectEffect2 = MilMo_ObjectEffectSystem.GetObjectEffect(GameObject, "GenericSpawnSound");
			if (objectEffect2 != null)
			{
				SpawnEffects.Add(objectEffect2);
			}
		}
		if (SpawnEffects.Count > 0)
		{
			IsSpawning = true;
		}
	}

	protected virtual void CreateObjectEffects()
	{
		foreach (string objectEffectName in VisualRep.ObjectEffectNames)
		{
			MilMo_ObjectEffect objectEffect = MilMo_ObjectEffectSystem.GetObjectEffect(GameObject, objectEffectName);
			if (objectEffect != null)
			{
				ObjectEffects.Add(objectEffect);
			}
		}
	}

	private void CreateRemovalEffects()
	{
		foreach (MilMo_ObjectEffect item in from removalEffectName in RemovalEffectNames
			select MilMo_ObjectEffectSystem.GetObjectEffect(GameObject, removalEffectName) into effect
			where effect != null
			select effect)
		{
			_inactiveRemovalEffects.Add(item);
		}
	}

	public void SetCreationTime()
	{
		_creationTime = Time.time;
	}

	public void Pause()
	{
		if (GameObject != null)
		{
			GameObject.SetActive(value: false);
		}
		IsSpawning = false;
		Paused = true;
	}

	public void Unpause()
	{
		VisualRep?.ActivateCurrentLOD();
		if (SpawnEffects.Count > 0)
		{
			IsSpawning = true;
		}
		Paused = false;
	}
}
