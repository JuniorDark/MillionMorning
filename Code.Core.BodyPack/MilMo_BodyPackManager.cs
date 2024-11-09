using System.Collections.Generic;
using Code.Core.BodyPack.ColorSystem;
using Code.Core.BodyPack.HairSystem;
using Code.Core.BodyPack.SkinPartSystem;
using Code.Core.EventSystem;
using Code.Core.Utility;
using Code.Core.Visual;
using UnityEngine;

namespace Code.Core.BodyPack;

public class MilMo_BodyPackManager
{
	private class ExternalQueuedApply
	{
		public readonly MilMo_Priority Priority;

		public readonly ApplyCallback Callback;

		public ExternalQueuedApply(MilMo_Priority priority, ApplyCallback callback)
		{
			Priority = priority;
			Callback = callback;
		}
	}

	private class QueuedBodyPack
	{
		public readonly MilMo_BodyPack Bodypack;

		public readonly IDictionary<string, int> ColorIndices;

		public readonly bool UnEquip;

		public QueuedBodyPack(MilMo_BodyPack bodypack, IDictionary<string, int> colorIndices, bool unEquip)
		{
			Bodypack = bodypack;
			ColorIndices = colorIndices;
			UnEquip = unEquip;
		}
	}

	public delegate void InitCallback();

	public delegate void ApplyCallback();

	private class QueuedApply
	{
		private readonly MilMo_BodyPackManager _manager;

		private readonly bool _isInit;

		private static readonly MilMo_PriorityQueue<QueuedApply> ApplyQueue = new MilMo_PriorityQueue<QueuedApply>();

		private static bool _isRunning;

		private static MilMo_GenericReaction _update;

		private QueuedApply(MilMo_BodyPackManager manager, bool isInit)
		{
			_manager = manager;
			_isInit = isInit;
		}

		public static void Queue(MilMo_BodyPackManager manager, bool init, MilMo_Priority priority)
		{
			if (_update == null)
			{
				_update = MilMo_EventSystem.RegisterUpdate(Update);
			}
			ApplyQueue.Enqueue(new QueuedApply(manager, init), priority);
		}

		public static void Done()
		{
			MilMo_EventSystem.NextFrame(delegate
			{
				_isRunning = false;
			});
		}

		private static void Update(object obj)
		{
			if (ApplyQueue.Count != 0 && !_isRunning)
			{
				_isRunning = true;
				QueuedApply queuedApply = ApplyQueue.Dequeue();
				if (queuedApply._isInit)
				{
					queuedApply._manager.InitFinish();
				}
				else
				{
					queuedApply._manager.ApplyFinish();
				}
			}
		}
	}

	private MilMo_Color _skinColor;

	private int _hairColor;

	private MilMo_Color _eyeColor;

	private MilMo_SkinPart _mouth;

	private MilMo_SkinPart _eyes;

	private MilMo_SkinPart _eyeBrows;

	private float _height = 1f;

	private bool _male;

	private Texture2D _mainAvatarTexture;

	private int _texWidth = 512;

	private int _texHeight = 512;

	private readonly List<MilMo_BodyPack> _equipped = new List<MilMo_BodyPack>();

	private readonly List<MilMo_BodyPack> _remove = new List<MilMo_BodyPack>();

	private readonly List<MilMo_BodyPack> _storedPacks = new List<MilMo_BodyPack>();

	private readonly Dictionary<string, int> _storedColorIndices = new Dictionary<string, int>();

	private readonly Dictionary<string, int> _colorIndices = new Dictionary<string, int>();

	private Dictionary<string, int> _workingColorIndices;

	private bool _highQualityShader = true;

	private int _addonLayer = 14;

	private bool _updateSkin = true;

	private bool _updateSoftMesh = true;

	private bool _updateAddons = true;

	private bool _haveHat;

	private bool _hideHair;

	private bool _updating;

	private bool _isInitialized;

	private bool _enabled = true;

	private bool _unloaded;

	private readonly Queue<ExternalQueuedApply> _applyQueue = new Queue<ExternalQueuedApply>();

	private readonly Queue<QueuedBodyPack> _equipQueue = new Queue<QueuedBodyPack>();

	private readonly Queue<KeyValuePair<string, int>> _setColorIndexQueue = new Queue<KeyValuePair<string, int>>();

	private bool _compressMainTexture = true;

	private bool _pendingApply;

	private bool _pendingInit;

	private InitCallback _initCallback;

	private readonly Queue<ApplyCallback> _applyCallbacks = new Queue<ApplyCallback>();

	private MilMo_Priority _currentInitPriority = MilMo_Priority.Normal;

	private MilMo_Priority _currentApplyPriority = MilMo_Priority.Normal;

	private readonly int _ramp = Shader.PropertyToID("_Ramp");

	private readonly int _velvetChannel = Shader.PropertyToID("_VelvetChannel");

	private readonly int _velvetColor = Shader.PropertyToID("_VelvetColor");

	private readonly int _mainColor = Shader.PropertyToID("_Color");

	private readonly int _mainTex = Shader.PropertyToID("_MainTex");

	public List<MilMo_BodyPack> Equipped => _equipped;

	public SkinnedMeshRenderer Renderer { private get; set; }

	public bool HighQualityShader
	{
		set
		{
			if (_highQualityShader != value)
			{
				_highQualityShader = value;
				Renderer.materials[0].shader = MilMo_BodyPackSystem.GetAvatarShader(_highQualityShader);
				Renderer.materials[0].SetTexture(_mainTex, _mainAvatarTexture);
				Renderer.materials[0].SetTexture(_ramp, MilMo_BodyPackSystem.ShaderRamp);
				Renderer.materials[0].SetVector(_velvetChannel, MilMo_BodyPackSystem.CharacterVelvetChannels);
				Renderer.materials[0].SetColor(_velvetColor, MilMo_BodyPackSystem.CharacterVelvetColor);
				Renderer.materials[0].SetColor(_mainColor, MilMo_BodyPackSystem.CharacterMainColor);
			}
		}
	}

	public MilMo_SoftMeshManager SoftMeshManager { get; private set; }

	public MilMo_Color SkinColor
	{
		set
		{
			_skinColor = value;
			_updateSkin = true;
		}
	}

	public int HairColor
	{
		private get
		{
			foreach (MilMo_BodyPack item in _equipped)
			{
				if (item.IsHair && _colorIndices.TryGetValue(item.Path + ":Hair", out var value))
				{
					return value;
				}
			}
			return _hairColor;
		}
		set
		{
			if (_hairColor == value)
			{
				return;
			}
			foreach (MilMo_BodyPack item in _equipped)
			{
				if (!item.IsHair)
				{
					continue;
				}
				foreach (ColorGroup colorGroup in item.ColorGroups)
				{
					if (_colorIndices.ContainsKey(item.Path + ":" + colorGroup.GroupName))
					{
						_colorIndices[item.Path + ":" + colorGroup.GroupName] = value;
					}
					else
					{
						_colorIndices.Add(item.Path + ":" + colorGroup.GroupName, value);
					}
				}
			}
			_hairColor = value;
			_updateSkin = true;
			_updateAddons = true;
		}
	}

	public MilMo_Color EyeColor
	{
		set
		{
			_eyeColor = value;
			_updateSkin = true;
		}
	}

	public MilMo_SkinPart Mouth
	{
		set
		{
			_mouth = value;
			_updateSkin = true;
		}
	}

	public MilMo_SkinPart Eyes
	{
		set
		{
			_eyes = value;
			_updateSkin = true;
		}
	}

	public MilMo_SkinPart EyeBrows
	{
		set
		{
			_eyeBrows = value;
			_updateSkin = true;
		}
	}

	public IEnumerable<MilMo_VisualRep> HairVisualReps
	{
		get
		{
			IList<MilMo_VisualRep> list = new List<MilMo_VisualRep>();
			foreach (MilMo_BodyPack item in _equipped)
			{
				if (!item.IsHair)
				{
					continue;
				}
				foreach (MilMo_Addon addon in item.Addons)
				{
					MilMo_VisualRep visualRep = addon.GetVisualRep(Renderer);
					if (visualRep != null)
					{
						list.Add(visualRep);
					}
				}
			}
			return list;
		}
	}

	public bool CompressMainTexture
	{
		set
		{
			_compressMainTexture = value;
		}
	}

	public void SetGender(bool male)
	{
		_male = male;
	}

	public bool SetBodyPackColorIndex(string groupName, int colorIndex)
	{
		if (_updating || !_isInitialized)
		{
			_setColorIndexQueue.Enqueue(new KeyValuePair<string, int>(groupName, colorIndex));
			return true;
		}
		if (_colorIndices.ContainsKey(groupName) && _colorIndices[groupName].Equals(colorIndex))
		{
			return false;
		}
		_colorIndices.Remove(groupName);
		_colorIndices.Add(groupName, colorIndex);
		_updateSkin = true;
		_updateSoftMesh = true;
		_updateAddons = true;
		return true;
	}

	public int GetBodyPackColorIndex(string key)
	{
		return _colorIndices[key];
	}

	public bool ContainsBodyPackColorIndex(string key)
	{
		return _colorIndices.ContainsKey(key);
	}

	public void SetAddonLayer(int layer)
	{
		foreach (MilMo_BodyPack item in _equipped)
		{
			foreach (MilMo_Addon addon in item.Addons)
			{
				if (addon != null)
				{
					addon.SetLayer(Renderer, layer);
					if (layer == 12 || layer == 13)
					{
						addon.EnableSilhouetteRendering(Renderer);
					}
				}
			}
		}
		_addonLayer = layer;
	}

	public void SetHairLayer(int layer)
	{
		MilMo_BodyPack milMo_BodyPack = null;
		foreach (MilMo_BodyPack item in _equipped)
		{
			if (item.IsHair)
			{
				milMo_BodyPack = item;
				break;
			}
		}
		if (milMo_BodyPack == null)
		{
			return;
		}
		foreach (MilMo_Addon addon in milMo_BodyPack.Addons)
		{
			addon.SetLayer(Renderer, layer);
		}
	}

	public void SetMainTextureSize(int size)
	{
		if (size != _texWidth || size != _texHeight)
		{
			_texHeight = size;
			_texWidth = size;
			_updateSkin = true;
		}
	}

	public void Unload()
	{
		_unloaded = true;
		if (SoftMeshManager != null)
		{
			SoftMeshManager.Destroy();
			SoftMeshManager = null;
		}
		if (_mainAvatarTexture != null)
		{
			if (!Application.isPlaying)
			{
				Object.DestroyImmediate(_mainAvatarTexture);
			}
			else
			{
				Object.Destroy(_mainAvatarTexture);
			}
			_mainAvatarTexture = null;
		}
		foreach (MilMo_BodyPack item in _equipped)
		{
			item.UnloadContent(Renderer);
		}
		_equipped.Clear();
	}

	public bool IsUnloaded()
	{
		if ((bool)Renderer)
		{
			return _unloaded;
		}
		if (!_unloaded)
		{
			Unload();
		}
		return true;
	}

	private bool IsDoneLoading()
	{
		foreach (MilMo_BodyPack item in _equipped)
		{
			if (!item.DoneLoading(Renderer))
			{
				return false;
			}
		}
		return true;
	}

	public void AsyncInit(MilMo_Priority priority, SkinnedMeshRenderer meshRenderer, bool male, string skinColor, int hairColor, string eyeColor, string mouth, string eyes, string eyebrows, float height, InitCallback callback)
	{
		if (meshRenderer == null)
		{
			Debug.LogWarning("Got null mesh renderer when initializing body pack manager. Body packs will not work!");
			callback();
			return;
		}
		_workingColorIndices = new Dictionary<string, int>(_colorIndices);
		_updating = true;
		_currentInitPriority = priority;
		Renderer = meshRenderer;
		_skinColor = MilMo_ColorSystem.GetSkinColor(skinColor);
		_hairColor = hairColor;
		_eyeColor = MilMo_ColorSystem.GetEyeColor(eyeColor);
		_male = male;
		_initCallback = callback;
		_mouth = MilMo_SkinPartSystem.GetMouth(mouth, _male);
		_eyes = MilMo_SkinPartSystem.GetEyes(eyes, _male);
		_eyeBrows = MilMo_SkinPartSystem.GetEyeBrows(eyebrows, _male);
		_height = height;
		if (!IsDoneLoading())
		{
			_pendingInit = true;
		}
		else
		{
			ContinueInit();
		}
	}

	private void ContinueInit()
	{
		_pendingInit = false;
		QueuedApply.Queue(this, init: true, _currentInitPriority);
	}

	private void InitFinish()
	{
		if (IsUnloaded())
		{
			if (_initCallback != null)
			{
				_initCallback();
			}
			QueuedApply.Done();
			return;
		}
		Renderer.materials = new Material[2];
		SoftMeshManager = new MilMo_SoftMeshManager(Renderer, _male)
		{
			ColorIndices = new Dictionary<string, int>(_workingColorIndices)
		};
		SoftMeshManager.UpdateSoftMeshes(_equipped);
		BuildMainTexture();
		RepaintAddons();
		Renderer.materials[0].shader = MilMo_BodyPackSystem.GetAvatarShader(_highQualityShader);
		Renderer.materials[0].SetTexture(_ramp, MilMo_BodyPackSystem.ShaderRamp);
		Renderer.materials[0].SetVector(_velvetChannel, MilMo_BodyPackSystem.CharacterVelvetChannels);
		Renderer.materials[0].SetColor(_velvetColor, MilMo_BodyPackSystem.CharacterVelvetColor);
		Renderer.materials[0].SetColor(_mainColor, MilMo_BodyPackSystem.CharacterMainColor);
		UpdateMainTexture();
		SoftMeshManager.ApplyChanges();
		UpdateAddons();
		_haveHat = false;
		_hideHair = false;
		MilMo_BodyPack milMo_BodyPack = null;
		foreach (MilMo_BodyPack item in _equipped)
		{
			if (item.IsHat)
			{
				_haveHat = true;
			}
			if (item.HideHair)
			{
				_hideHair = true;
			}
			if (item.IsHair)
			{
				milMo_BodyPack = item;
			}
		}
		if (milMo_BodyPack != null)
		{
			foreach (MilMo_Addon addon in milMo_BodyPack.Addons)
			{
				if (addon is MilMo_HairAddon milMo_HairAddon)
				{
					milMo_HairAddon.SetHaveHat(Renderer, _haveHat);
					milMo_HairAddon.SetHideHair(Renderer, _hideHair);
				}
			}
		}
		_updateSkin = false;
		_updateSoftMesh = false;
		_updateAddons = false;
		_updating = false;
		_isInitialized = true;
		int count = _equipQueue.Count;
		for (int i = 0; i < count; i++)
		{
			QueuedBodyPack queuedBodyPack = _equipQueue.Dequeue();
			if (queuedBodyPack.UnEquip)
			{
				Unequip(queuedBodyPack.Bodypack);
			}
			else
			{
				Equip(queuedBodyPack.Bodypack, queuedBodyPack.ColorIndices);
			}
		}
		int count2 = _setColorIndexQueue.Count;
		for (int j = 0; j < count2; j++)
		{
			KeyValuePair<string, int> keyValuePair = _setColorIndexQueue.Dequeue();
			SetBodyPackColorIndex(keyValuePair.Key, keyValuePair.Value);
		}
		AsyncApplyFromQueue();
		if (_initCallback != null)
		{
			_initCallback();
		}
		QueuedApply.Done();
	}

	public void AsyncApply(MilMo_Priority priority, ApplyCallback callback)
	{
		if (_updating || !_isInitialized)
		{
			_applyQueue.Enqueue(new ExternalQueuedApply(priority, callback));
			return;
		}
		_workingColorIndices = new Dictionary<string, int>(_colorIndices);
		SoftMeshManager.ColorIndices = new Dictionary<string, int>(_workingColorIndices);
		_updating = true;
		_currentApplyPriority = priority;
		if (callback != null)
		{
			_applyCallbacks.Enqueue(callback);
		}
		if (!IsDoneLoading())
		{
			_pendingApply = true;
		}
		else
		{
			ContinueApply();
		}
	}

	private void AsyncApplyFromQueue()
	{
		if (_updating || !_isInitialized || _applyQueue.Count <= 0 || SoftMeshManager == null)
		{
			return;
		}
		_workingColorIndices = new Dictionary<string, int>(_colorIndices);
		SoftMeshManager.ColorIndices = new Dictionary<string, int>(_workingColorIndices);
		_updating = true;
		for (int i = 0; i < _applyQueue.Count; i++)
		{
			ExternalQueuedApply externalQueuedApply = _applyQueue.Dequeue();
			if (externalQueuedApply.Callback != null)
			{
				_applyCallbacks.Enqueue(externalQueuedApply.Callback);
			}
			if (externalQueuedApply.Priority > _currentApplyPriority)
			{
				_currentApplyPriority = externalQueuedApply.Priority;
			}
		}
		if (!IsDoneLoading())
		{
			_pendingApply = true;
		}
		else
		{
			ContinueApply();
		}
	}

	private void ContinueApply()
	{
		QueuedApply.Queue(this, init: false, _currentApplyPriority);
	}

	private void ApplyFinish()
	{
		if (IsUnloaded())
		{
			QueuedApply.Done();
			return;
		}
		if (_updateSoftMesh && SoftMeshManager != null)
		{
			SoftMeshManager.UpdateSoftMeshes(_equipped);
		}
		BuildMainTexture();
		RepaintAddons();
		if (_updateSkin)
		{
			if (_mainAvatarTexture == null)
			{
				Debug.LogWarning("Trying to update main texture without generating a main color array. Use call BuildMainTexture() first.");
				return;
			}
			if (Renderer != null)
			{
				Renderer.material.SetTexture(_mainTex, _mainAvatarTexture);
			}
		}
		MilMo_EventSystem.UpdateOne(delegate
		{
			if (SoftMeshManager != null)
			{
				SoftMeshManager.ApplyChanges();
				UpdateAddons();
				_haveHat = false;
				_hideHair = false;
				MilMo_BodyPack milMo_BodyPack = null;
				foreach (MilMo_BodyPack item in _equipped)
				{
					if (item.IsHat)
					{
						_haveHat = true;
					}
					if (item.HideHair)
					{
						_hideHair = true;
					}
					if (item.IsHair)
					{
						milMo_BodyPack = item;
					}
				}
				if (milMo_BodyPack != null)
				{
					foreach (MilMo_Addon addon in milMo_BodyPack.Addons)
					{
						if (addon is MilMo_HairAddon milMo_HairAddon)
						{
							milMo_HairAddon.SetHaveHat(Renderer, _haveHat);
							milMo_HairAddon.SetHideHair(Renderer, _hideHair);
						}
					}
				}
			}
		});
		MilMo_EventSystem.UpdateOne(delegate
		{
			_updateSkin = false;
			_updateSoftMesh = false;
			_updateAddons = false;
			_remove.Clear();
			_updating = false;
			_pendingApply = false;
			int count = _applyCallbacks.Count;
			for (int i = 0; i < count; i++)
			{
				_applyCallbacks.Dequeue()();
			}
			int count2 = _equipQueue.Count;
			for (int j = 0; j < count2; j++)
			{
				QueuedBodyPack queuedBodyPack = _equipQueue.Dequeue();
				if (queuedBodyPack.UnEquip)
				{
					Unequip(queuedBodyPack.Bodypack);
				}
				else
				{
					Equip(queuedBodyPack.Bodypack, queuedBodyPack.ColorIndices);
				}
			}
			int count3 = _setColorIndexQueue.Count;
			for (int k = 0; k < count3; k++)
			{
				KeyValuePair<string, int> keyValuePair = _setColorIndexQueue.Dequeue();
				SetBodyPackColorIndex(keyValuePair.Key, keyValuePair.Value);
			}
			AsyncApplyFromQueue();
			QueuedApply.Done();
		});
	}

	public void Equip(MilMo_BodyPack bodypack, IDictionary<string, int> colorIndices)
	{
		if (bodypack == null)
		{
			return;
		}
		if (_updating)
		{
			_equipQueue.Enqueue(new QueuedBodyPack(bodypack, colorIndices, unEquip: false));
			return;
		}
		if (Renderer == null)
		{
			Debug.LogWarning("Trying to equip body packs when renderer is null. Did you call AsyncInit?");
			return;
		}
		EquipInternal(bodypack, colorIndices);
		if (!bodypack.DoneLoading(Renderer))
		{
			bodypack.AsyncLoadContent(Renderer, delegate
			{
				CheckForPendingRequests();
			});
		}
		else
		{
			CheckForPendingRequests();
		}
	}

	private void CheckForPendingRequests()
	{
		if (_pendingInit && IsDoneLoading())
		{
			ContinueInit();
		}
		else if (_pendingApply && IsDoneLoading())
		{
			ContinueApply();
		}
	}

	private void EquipInternal(MilMo_BodyPack bodypack, IDictionary<string, int> colorIndices)
	{
		UnequipByCategory(bodypack);
		foreach (KeyValuePair<string, int> colorIndex in colorIndices)
		{
			if (_colorIndices.ContainsKey(colorIndex.Key))
			{
				_colorIndices[colorIndex.Key] = colorIndex.Value;
			}
			else
			{
				_colorIndices.Add(colorIndex.Key, colorIndex.Value);
			}
		}
		if (bodypack.BlendLayers.Count > 0)
		{
			_updateSkin = true;
		}
		if (bodypack.SkinLayers.Count > 0)
		{
			_updateSkin = true;
		}
		if (bodypack.SoftMeshes.Count > 0)
		{
			_updateSoftMesh = true;
		}
		if (bodypack.Addons.Count > 0)
		{
			_updateAddons = true;
		}
		_equipped.Add(bodypack);
	}

	public void Disable()
	{
		_enabled = false;
		foreach (MilMo_BodyPack item in _equipped)
		{
			foreach (MilMo_Addon addon in item.Addons)
			{
				addon.Disable(Renderer);
			}
		}
	}

	public void Enable()
	{
		_enabled = true;
		foreach (MilMo_BodyPack item in _equipped)
		{
			foreach (MilMo_Addon addon in item.Addons)
			{
				addon.Enable(Renderer);
			}
		}
	}

	public void Update()
	{
		foreach (MilMo_BodyPack item in _equipped)
		{
			item.Update(Renderer);
		}
	}

	public void Unequip(MilMo_BodyPack bodyPack)
	{
		if (bodyPack == null)
		{
			return;
		}
		if (_updating)
		{
			_equipQueue.Enqueue(new QueuedBodyPack(bodyPack, null, unEquip: true));
		}
		else
		{
			if (!_equipped.Remove(bodyPack))
			{
				return;
			}
			if (bodyPack.BlendLayers.Count > 0)
			{
				_updateSkin = true;
			}
			if (bodyPack.SkinLayers.Count > 0)
			{
				_updateSkin = true;
			}
			if (bodyPack.SoftMeshes.Count > 0)
			{
				_updateSoftMesh = true;
			}
			if (bodyPack.Addons.Count > 0)
			{
				_updateAddons = true;
			}
			foreach (ColorGroup colorGroup in bodyPack.ColorGroups)
			{
				_colorIndices.Remove(bodyPack.Path + ":" + colorGroup.GroupName);
			}
			_remove.Add(bodyPack);
		}
	}

	private void UnequipByCategory(MilMo_BodyPack bodypack)
	{
		if (_updating)
		{
			return;
		}
		for (int num = _equipped.Count - 1; num >= 0; num--)
		{
			MilMo_BodyPack milMo_BodyPack = _equipped[num];
			bool flag = false;
			foreach (string category in milMo_BodyPack.Categories)
			{
				foreach (string category2 in bodypack.Categories)
				{
					if (!(category != category2))
					{
						Unequip(milMo_BodyPack);
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
	}

	public void UnEquipAll()
	{
		if (!_updating)
		{
			for (int num = _equipped.Count - 1; num >= 0; num--)
			{
				Unequip(_equipped[num]);
			}
		}
	}

	private void BuildMainTexture()
	{
		if (_updateSkin)
		{
			RenderTexture active = RenderTexture.active;
			RenderTexture renderTexture2 = (RenderTexture.active = new RenderTexture(_texWidth, _texHeight, 0)
			{
				isPowerOfTwo = true,
				hideFlags = HideFlags.DontSave
			});
			ApplySkinParts();
			Texture2D texture2D = new Texture2D(_texWidth, _texHeight, TextureFormat.ARGB32, mipChain: false);
			texture2D.ReadPixels(new Rect(0f, 0f, _texWidth, _texHeight), 0, 0);
			texture2D.Apply();
			ApplySkinColor(texture2D);
			ApplyHairColor(texture2D);
			ApplyEyeColor(texture2D);
			ApplyCopyOperations(texture2D);
			ApplySkinLayers(texture2D);
			Texture2D mainAvatarTexture = _mainAvatarTexture;
			_mainAvatarTexture = new Texture2D(_texWidth, _texHeight, TextureFormat.ARGB32, mipChain: false);
			_mainAvatarTexture.ReadPixels(new Rect(0f, 0f, _texWidth, _texWidth), 0, 0);
			if (_compressMainTexture)
			{
				_mainAvatarTexture.Compress(highQuality: false);
			}
			_mainAvatarTexture.Apply();
			Object.Destroy(mainAvatarTexture, 1f);
			RenderTexture.active = active;
			renderTexture2.Release();
			if (!Application.isPlaying)
			{
				Object.DestroyImmediate(renderTexture2);
				Object.DestroyImmediate(texture2D);
			}
			else
			{
				Object.Destroy(renderTexture2);
				Object.Destroy(texture2D);
			}
		}
	}

	private void RepaintAddons()
	{
		foreach (MilMo_BodyPack item in _equipped)
		{
			foreach (MilMo_Addon addon in item.Addons)
			{
				addon.CreateAddonTexture(Renderer, _workingColorIndices);
			}
		}
	}

	private void ApplySkinParts()
	{
		GL.Clear(clearDepth: true, clearColor: true, Color.clear);
		GL.PushMatrix();
		GL.LoadOrtho();
		MilMo_SkinPartSystem.GetMainPart(_male).Apply();
		if (_mouth != null)
		{
			_mouth.Apply();
		}
		if (_eyes != null)
		{
			_eyes.Apply();
		}
		if (_eyeBrows != null)
		{
			_eyeBrows.Apply();
		}
		MilMo_SkinPartSystem.GetMainEyes().Apply();
		MilMo_SkinPartSystem.GetTeeth(_male).Apply();
		GL.PopMatrix();
	}

	private void ApplySkinColor(Texture inTex)
	{
		if (_skinColor == null)
		{
			Debug.LogWarning("Skin color is null");
			return;
		}
		GL.PushMatrix();
		GL.LoadOrtho();
		foreach (Rect theSkinColorSection in MilMo_BodyPackSystem.TheSkinColorSections)
		{
			_skinColor.Apply(theSkinColorSection, inTex);
		}
		GL.PopMatrix();
	}

	private void ApplyHairColor(Texture inTex)
	{
		GL.PushMatrix();
		GL.LoadOrtho();
		MilMo_Color colorFromIndex = MilMo_BodyPackSystem.GetColorFromIndex(HairColor);
		if (colorFromIndex != null)
		{
			foreach (Rect theHairColorSection in MilMo_BodyPackSystem.TheHairColorSections)
			{
				colorFromIndex.Apply(theHairColorSection, inTex);
			}
		}
		GL.PopMatrix();
	}

	private void ApplyEyeColor(Texture inTex)
	{
		if (_eyeColor == null)
		{
			Debug.LogWarning("Eye color is null");
			return;
		}
		GL.PushMatrix();
		GL.LoadOrtho();
		foreach (Rect theEyeColorSection in MilMo_BodyPackSystem.TheEyeColorSections)
		{
			_eyeColor.Apply(theEyeColorSection, inTex, null, "EYE");
		}
		GL.PopMatrix();
	}

	private static void ApplyCopyOperations(Texture inTex)
	{
		GL.PushMatrix();
		GL.LoadOrtho();
		foreach (MilMo_CopyOperation theCopyOperation in MilMo_BodyPackSystem.TheCopyOperations)
		{
			theCopyOperation.Apply(inTex);
		}
		GL.PopMatrix();
	}

	private void ApplySkinLayers(Texture inTex)
	{
		RenderTexture active = RenderTexture.active;
		RenderTexture renderTexture2 = (RenderTexture.active = new RenderTexture(inTex.width, inTex.height, 0)
		{
			isPowerOfTwo = (MilMo_ColorShaderUtil.IsPowerOfTwo(inTex.width) && MilMo_ColorShaderUtil.IsPowerOfTwo(inTex.height)),
			hideFlags = HideFlags.DontSave
		});
		GL.Clear(clearDepth: true, clearColor: true, Color.clear);
		GL.PushMatrix();
		GL.LoadOrtho();
		List<MilMo_SkinLayer> list = new List<MilMo_SkinLayer>();
		foreach (MilMo_BodyPack item in _equipped)
		{
			list.AddRange(item.BlendLayers);
		}
		foreach (MilMo_SkinLayer item2 in list)
		{
			item2.StitchBlendLayer(_male);
		}
		GL.PopMatrix();
		RenderTexture.active = active;
		GL.PushMatrix();
		GL.LoadOrtho();
		foreach (MilMo_SkinLayer item3 in list)
		{
			if (item3.HasContent)
			{
				item3.ApplyAsBlendLayer(_male, _workingColorIndices);
			}
		}
		List<MilMo_SkinLayer> list2 = new List<MilMo_SkinLayer>();
		foreach (MilMo_BodyPack item4 in _equipped)
		{
			list2.AddRange(item4.SkinLayers);
		}
		foreach (MilMo_SkinLayer item5 in list2)
		{
			if (item5.HasContent)
			{
				item5.ApplyAsSkinLayer(_male, _workingColorIndices, renderTexture2);
			}
		}
		GL.PopMatrix();
		renderTexture2.Release();
		if (!Application.isPlaying)
		{
			Object.DestroyImmediate(renderTexture2);
		}
		else
		{
			Object.Destroy(renderTexture2);
		}
	}

	private void UpdateMainTexture()
	{
		if (_updateSkin)
		{
			if (_mainAvatarTexture == null)
			{
				Debug.LogWarning("Trying to update main texture without generating a main color array. Use call BuildMainTexture() first.");
				return;
			}
			Renderer.material.SetTexture(_mainTex, _mainAvatarTexture);
			_updateSkin = false;
		}
	}

	private void UpdateAddons()
	{
		if (!_updateAddons)
		{
			return;
		}
		foreach (MilMo_BodyPack item in _remove)
		{
			foreach (MilMo_Addon addon in item.Addons)
			{
				addon.UnEquip(Renderer);
			}
		}
		foreach (MilMo_BodyPack item2 in _equipped)
		{
			foreach (MilMo_Addon addon2 in item2.Addons)
			{
				addon2.Equip(Renderer, _male, 1f / _height);
				if (_enabled)
				{
					addon2.Enable(Renderer);
				}
				else
				{
					addon2.Disable(Renderer);
				}
			}
		}
		SetAddonLayer(_addonLayer);
	}

	public void Store()
	{
		if (_storedPacks.Count > 0)
		{
			Debug.LogWarning("Must call Restore before calling Store again in body pack manager");
			return;
		}
		_storedPacks.AddRange(_equipped);
		foreach (KeyValuePair<string, int> colorIndex in _colorIndices)
		{
			_storedColorIndices.Add(colorIndex.Key, colorIndex.Value);
		}
	}

	public void AddToStoreAndRemoveByCategory(MilMo_BodyPack bodypack, IDictionary<string, int> colorIndices)
	{
		for (int i = 0; i < _storedPacks.Count; i++)
		{
			if (_storedPacks[i].IsCategory(bodypack))
			{
				_storedPacks.RemoveAt(i);
				break;
			}
		}
		_storedPacks.Add(bodypack);
		foreach (KeyValuePair<string, int> colorIndex in colorIndices)
		{
			if (!_storedColorIndices.ContainsKey(colorIndex.Key))
			{
				_storedColorIndices.Add(colorIndex.Key, colorIndex.Value);
			}
			else
			{
				_storedColorIndices[colorIndex.Key] = colorIndex.Value;
			}
		}
	}

	public void Restore()
	{
		_remove.AddRange(_equipped);
		_equipped.Clear();
		_equipped.AddRange(_storedPacks);
		_storedPacks.Clear();
		_colorIndices.Clear();
		foreach (KeyValuePair<string, int> storedColorIndex in _storedColorIndices)
		{
			_colorIndices.Add(storedColorIndex.Key, storedColorIndex.Value);
		}
		_storedColorIndices.Clear();
		_updateSkin = true;
		_updateSoftMesh = true;
		_updateAddons = true;
		foreach (MilMo_BodyPack item in _equipped)
		{
			if (!item.HasContent(Renderer))
			{
				item.AsyncLoadContent(Renderer, delegate
				{
					CheckForPendingRequests();
				});
			}
		}
	}
}
