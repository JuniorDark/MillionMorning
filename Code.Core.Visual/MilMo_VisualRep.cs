using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Code.Core.Camera;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Utility;
using Code.Core.Visual.Audio;
using Code.Core.Visual.Effect;
using UnityEngine;
using UnityEngine.Rendering;

namespace Code.Core.Visual;

public class MilMo_VisualRep : IMilMoNoPlayerCollisionArea
{
	public class AnimationConfig
	{
		public MilMo_AnimationSound Sound;

		public float Speed = 1f;
	}

	private enum MilMo_CollisionType
	{
		None,
		Sphere,
		Capsule
	}

	public delegate void VisualRepLoadDone(MilMo_VisualRep visualRep);

	public delegate void MaterialsDone(MilMo_VisualRep visualRep);

	public const int RENDER_QUEUE_GEOMETRY = 2000;

	private const int RENDER_QUEUE_SILHOUETTE_OBJECT = 1600;

	public const int RENDER_QUEUE_AVATAR = 1800;

	private const float DEFAULT_LOD_STEP = 48f;

	private const string DEFAULT_SHADER = "Shaders/Junebug/Diffuse";

	private static readonly int FogColor = UnityEngine.Shader.PropertyToID("_FogColor");

	private static readonly int Ramp = UnityEngine.Shader.PropertyToID("_Ramp");

	private bool _isDestroyed;

	private int _layer;

	private string _mesh;

	private bool _meshNone;

	private readonly List<string> _particleEffectNames = new List<string>();

	private readonly List<Vector3> _particleEffectOffsets = new List<Vector3>();

	private readonly List<MilMo_Effect> _particleEffects = new List<MilMo_Effect>();

	private int _nextParticleEffect;

	private string _idleAnimation = "";

	private bool _receiveShadows = true;

	private bool _castShadows = true;

	private bool _animateAndRemove;

	protected readonly Dictionary<string, AnimationConfig> AnimationConfigs = new Dictionary<string, AnimationConfig>();

	private MilMo_AnimationSound _currentAnimationSound;

	private readonly Dictionary<string, List<Color>> _randomColors = new Dictionary<string, List<Color>>();

	private MilMo_CollisionType _collisionType;

	private float _collisionRadius = 1f;

	private float _collisionHeight = 1f;

	private bool _colliderCenterZero;

	private bool _isWalkable;

	private string _walkableName = "";

	private int _walkableResolution = 32;

	private bool _isCritterBlocker;

	private string _groundMaterial;

	private bool _silhouette;

	private AudioSourceWrapper _audioSource;

	private SkinQuality _skinQuality = SkinQuality.Bone2;

	private bool _skinNormals;

	private MaterialsDone _materialsDoneCallback;

	private Vector3 _spin;

	private bool _noCameraCollision;

	private bool _treatAsTerrainForJump;

	public float _noPlayerCollisionRadius;

	private float _noPlayerCollisionSqrRadius;

	private int _fogColorIndex = -1;

	private MilMo_BlobShadow _blobShadow;

	private bool _useBlobShadow;

	private float _blobShadowFieldOfView = 20f;

	private string _blobShadowMaterial = "ShadowMaterial";

	private Vector3 _blobShadowOffset = Vector3.zero;

	private bool _blobShadowAutoFarPlane = true;

	private float _blobShadowFarPlane = 10f;

	private bool _uvAnimation;

	private Vector2 _uvAnimationSpeed = Vector2.zero;

	private Vector2 _currentUVAnimation = Vector2.zero;

	private bool _setTextureInDefaultMaterial;

	private MilMo_ResourceManager.Priority _loadPriority;

	private bool _haveLoadPriority;

	private bool _pauseModeOnMaterial;

	private Animation _animation;

	private MilMo_VisualRepData _data;

	private bool _enabled = true;

	private MeshCollider _meshCollider;

	public GameObject GameObject { get; private set; }

	public string Name { get; private set; }

	public string Path { get; private set; }

	public string FullPath { get; private set; }

	public string AssetTag { get; private set; }

	public Vector3 Scale { get; set; } = new Vector3(1f, 1f, 1f);


	public string Shader { get; set; } = "Shaders/Junebug/Diffuse";


	public bool HasDefaultColor { get; private set; }

	public string TextureName { get; set; }

	public Renderer Renderer { get; private set; }

	public List<MilMo_Material> Materials { get; } = new List<MilMo_Material>();


	public bool MaterialsFinished { get; private set; }

	public List<MilMo_Lod> Lods { get; } = new List<MilMo_Lod>();


	public MilMo_Lod CurrentLod { get; private set; }

	public bool UseMeshHeld { get; set; }

	public bool UpdateLods { get; set; } = true;


	public MilMo_MeshHeld MeshHeld { get; private set; }

	private MilMo_Audio AudioSourceData { get; set; }

	public List<MilMo_Gib> Gibs { get; } = new List<MilMo_Gib>();


	public bool HaveSpin => _spin != Vector3.zero;

	public Vector3 Spin => _spin;

	public bool Billboard { get; set; }

	public bool OnlyLodsUseBillboard { get; set; }

	public bool Blocker { get; set; }

	private bool UpdateWhenOffscreen { get; set; }

	public string EventTag { get; set; } = "";


	public List<string> ObjectEffectNames { get; set; } = new List<string>();


	public bool IsInvalid
	{
		get
		{
			if (!_isDestroyed && !(GameObject == null))
			{
				return Renderer == null;
			}
			return true;
		}
	}

	public Color DefaultColor { get; private set; } = Color.white;


	bool IMilMoNoPlayerCollisionArea.Enabled
	{
		get
		{
			if (GameObject != null)
			{
				return _noPlayerCollisionRadius > 0f;
			}
			return false;
		}
	}

	float IMilMoNoPlayerCollisionArea.SqrRadius => _noPlayerCollisionSqrRadius;

	Vector3 IMilMoNoPlayerCollisionArea.Position => GameObject.transform.position;

	public void Init(string fullPath, string assetTag = "Generic", bool setTextureInDefaultMaterial = true, bool pauseModeOnMaterial = false, MilMo_ResourceManager.Priority priority = MilMo_ResourceManager.Priority.High)
	{
		FullPath = fullPath;
		Path = MilMo_Utility.RemoveFileNameFromFullPath(fullPath);
		Name = MilMo_Utility.ExtractNameFromPath(fullPath);
		AssetTag = assetTag;
		_pauseModeOnMaterial = pauseModeOnMaterial;
		_loadPriority = priority;
		_mesh = Name;
		TextureName = Name;
		_setTextureInDefaultMaterial = setTextureInDefaultMaterial;
	}

	public void Disable()
	{
		if (!Renderer)
		{
			return;
		}
		Renderer.enabled = false;
		foreach (MilMo_Lod item in Lods.Where((MilMo_Lod lod) => lod != null && lod.GameObjectRef != null))
		{
			item.GameObjectRef.SetActive(value: false);
		}
		if (MeshHeld != null && (bool)MeshHeld.GameObjectRef)
		{
			MeshHeld.GameObjectRef.SetActive(value: false);
		}
		_enabled = false;
	}

	public void Enable()
	{
		if (!Renderer)
		{
			return;
		}
		Renderer.enabled = true;
		foreach (MilMo_Lod item in Lods.Where((MilMo_Lod lod) => lod != null && lod.GameObjectRef != null))
		{
			item.GameObjectRef.SetActive(value: true);
		}
		if (MeshHeld != null && (bool)MeshHeld.GameObjectRef)
		{
			MeshHeld.GameObjectRef.SetActive(value: true);
		}
		_enabled = true;
	}

	public void RegisterMaterialsDoneCallback(MaterialsDone callback)
	{
		_materialsDoneCallback = callback;
		if (MaterialsFinished)
		{
			callback?.Invoke(this);
		}
	}

	public async Task<MilMo_VisualRep> InstantiateAsync(Vector3 position, Quaternion rotation, Vector3 scale)
	{
		if (!(await InitializeGameObjectAsync(position, rotation, scale)))
		{
			return null;
		}
		if (_meshNone || Blocker)
		{
			MaterialsFinished = true;
			_materialsDoneCallback?.Invoke(this);
			if (_meshNone)
			{
				InitializeParticleEffects();
			}
		}
		else
		{
			InitializeMaterials(async: true);
			InitializeAnimations();
			InitializeParticleEffects();
		}
		return this;
	}

	public bool Instantiate(Vector3 position, Quaternion rotation)
	{
		if (!InitializeGameObject(position, rotation))
		{
			return false;
		}
		InitializeMaterials(async: false);
		InitializeAnimations();
		InitializeParticleEffects();
		return true;
	}

	public void Update()
	{
		if (Blocker)
		{
			return;
		}
		if (_animateAndRemove && !_animation.isPlaying)
		{
			MilMo_VisualRepContainer.RemoveFromUpdate(this);
			MilMo_VisualRepContainer.DestroyVisualRep(this);
			return;
		}
		if (!MaterialsFinished && Materials.All((MilMo_Material mat) => mat.IsDone()))
		{
			FinishMaterials();
			MaterialsFinished = true;
		}
		_spin = _data.spin;
		if (HaveSpin && (bool)GameObject)
		{
			GameObject.transform.Rotate(_spin.x * Time.deltaTime, _spin.y * Time.deltaTime, _spin.z * Time.deltaTime);
		}
		Billboard = _data.billboard;
		if (Billboard && (bool)GameObject && (bool)MilMo_Global.MainCamera)
		{
			GameObject.transform.LookAt(MilMo_Global.MainCamera.transform.position);
		}
		if (_uvAnimation)
		{
			_currentUVAnimation.x += _uvAnimationSpeed.x * Time.deltaTime;
			_currentUVAnimation.y += _uvAnimationSpeed.y * Time.deltaTime;
			Renderer.sharedMaterial.mainTextureOffset = _currentUVAnimation;
		}
		if (_fogColorIndex > -1)
		{
			Materials[_fogColorIndex].Material.SetColor(FogColor, RenderSettings.fogColor);
		}
		foreach (MilMo_Effect particleEffect in _particleEffects)
		{
			particleEffect.Update();
		}
		foreach (MilMo_Lod lod in Lods)
		{
			lod.Update();
		}
		foreach (MilMo_Gib gib in Gibs)
		{
			gib.Update();
		}
		MeshHeld?.Update();
		if (!_enabled || !UpdateLods)
		{
			return;
		}
		if (Lods.Count > 1 && (bool)MilMo_Global.MainCamera && CurrentLod != null && (!UseMeshHeld || MeshHeld == null || !MeshHeld.GameObjectRef))
		{
			for (int num = Lods.Count - 1; num >= 0; num--)
			{
				MilMo_Lod milMo_Lod = Lods[num];
				if ((bool)milMo_Lod.GameObjectRef && milMo_Lod.ShouldUse())
				{
					if (CurrentLod != milMo_Lod)
					{
						ChangeCurrentLod(milMo_Lod);
					}
					break;
				}
			}
		}
		if (ShouldSwitchToMeshHeld())
		{
			ChangeCurrentLod(MeshHeld);
		}
	}

	private bool ShouldSwitchToMeshHeld()
	{
		if (!UseMeshHeld)
		{
			return false;
		}
		if (MeshHeld == null || !MeshHeld.GameObjectRef)
		{
			return false;
		}
		if (CurrentLod == null || !CurrentLod.GameObjectRef)
		{
			return false;
		}
		return CurrentLod != MeshHeld;
	}

	private void ChangeCurrentLod(MilMo_Lod newLodToUse)
	{
		if ((bool)CurrentLod.Renderer)
		{
			CurrentLod.Renderer.enabled = false;
		}
		CurrentLod = newLodToUse;
		if ((bool)CurrentLod.Renderer)
		{
			CurrentLod.Renderer.enabled = true;
		}
	}

	private bool IsPlayerOutside()
	{
		Transform transform = MilMo_Camera.Instance.transform;
		Transform transform2 = GameObject.transform;
		if (!transform || !transform2)
		{
			return false;
		}
		Vector3 vector = transform2.InverseTransformPoint(transform.position);
		Vector3 localScale = transform2.localScale;
		float num = localScale.x / localScale.z;
		return Mathf.Sqrt(Mathf.Pow(vector.z * num, 2f) + Mathf.Pow(vector.x, 2f)) > 1f;
	}

	public void PlayAnimation(string animation, WrapMode wrapMode)
	{
		PlayAnimation(animation, 0.3f, wrapMode);
	}

	public void PlayAnimation(string animation, float crossFade, WrapMode wrapMode)
	{
		float speed = 1f;
		if (AnimationConfigs.ContainsKey(animation))
		{
			speed = AnimationConfigs[animation].Speed;
		}
		PlayAnimation(animation, speed, crossFade, wrapMode);
	}

	private void PlayAnimation(string animation, float speed, float crossFade, WrapMode wrapMode)
	{
		if (!GameObject)
		{
			return;
		}
		Animation component = GameObject.GetComponent<Animation>();
		if (!component)
		{
			return;
		}
		AnimationState animationState = component[animation];
		if (!animationState)
		{
			return;
		}
		if (_currentAnimationSound != null && (bool)_audioSource)
		{
			_currentAnimationSound.Stop(_audioSource);
		}
		animationState.wrapMode = wrapMode;
		animationState.speed = speed;
		component.CrossFade(animation, crossFade);
		if (AnimationConfigs.ContainsKey(animation) && AnimationConfigs[animation].Sound != null)
		{
			_currentAnimationSound = AnimationConfigs[animation].Sound;
			if ((bool)_audioSource)
			{
				_currentAnimationSound.Play(_audioSource, component);
			}
		}
		else
		{
			_currentAnimationSound = null;
		}
	}

	public void Destroy()
	{
		MilMo_VisualRepContainer.RemoveFromList(this);
		if (_isDestroyed)
		{
			return;
		}
		if (_noPlayerCollisionRadius > 0f)
		{
			MilMo_EventSystem.Instance.PostEvent("no_playercollision_area_destroyed", this);
		}
		if ((bool)_blobShadow)
		{
			_blobShadow.Destroy();
		}
		foreach (MilMo_Material material in Materials)
		{
			MilMo_Global.Destroy(material.Material);
		}
		Materials.Clear();
		MilMo_Global.Destroy(GameObject);
		_isDestroyed = true;
	}

	private async Task<bool> InitializeGameObjectAsync(Vector3 position, Quaternion rotation, Vector3 scale)
	{
		GameObject gameObject = await MilMo_ResourceManager.Instance.LoadMeshAsync(Path + _mesh, AssetTag, _loadPriority);
		if (gameObject == null)
		{
			Debug.LogWarning("Failed to async load game object for visual rep " + Path + _mesh);
			return false;
		}
		if (!FinishGameObject(gameObject, position, rotation, scale))
		{
			Debug.LogWarning("Failed to finish game object for visual rep " + Path + _mesh);
			return false;
		}
		return true;
	}

	private bool InitializeGameObject(Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = MilMo_ResourceManager.Instance.LoadGameObjectLocal(Path + _mesh);
		if (!gameObject)
		{
			Debug.LogWarning("Failed to load game object for visual rep " + Path + _mesh);
			return false;
		}
		if (!FinishGameObject(gameObject, position, rotation, new Vector3(1f, 1f, 1f)))
		{
			Debug.LogWarning("Failed to finish game object for visual rep " + Path + _mesh);
			return false;
		}
		return true;
	}

	private bool FinishGameObject(GameObject loadedGameObject, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		GameObject = UnityEngine.Object.Instantiate(loadedGameObject);
		GameObject.name = Name;
		GameObject.transform.position = position;
		GameObject.transform.rotation = rotation;
		GameObject.transform.localScale = Vector3.Scale(scale, Scale);
		if (_isWalkable)
		{
			GameObject.layer = 19;
		}
		else if (_isCritterBlocker)
		{
			GameObject.layer = 20;
		}
		else if (_noCameraCollision)
		{
			GameObject.layer = 25;
		}
		InitializeBlobShadow();
		MilMo_VisualRepComponent milMo_VisualRepComponent = GameObject.AddComponent<MilMo_VisualRepComponent>();
		milMo_VisualRepComponent.SetVisualRep(this);
		MilMo_VisualRepData milMo_VisualRepData = new MilMo_VisualRepData();
		milMo_VisualRepComponent.SetData(milMo_VisualRepData);
		milMo_VisualRepData.defaultColor = DefaultColor;
		milMo_VisualRepData.groundMaterial = _groundMaterial;
		milMo_VisualRepData.spin = _spin;
		milMo_VisualRepData.billboard = Billboard;
		milMo_VisualRepData.onlyLodsUseBillboard = OnlyLodsUseBillboard;
		milMo_VisualRepData.blocker = Blocker;
		milMo_VisualRepData.silhouette = _silhouette;
		milMo_VisualRepData.walkable = _isWalkable;
		milMo_VisualRepData.walkableName = _walkableName;
		milMo_VisualRepData.walkableResolution = _walkableResolution;
		milMo_VisualRepData.critterBlocker = _isCritterBlocker;
		milMo_VisualRepData.uvAnimation = _uvAnimationSpeed;
		milMo_VisualRepData.particleEffects = _particleEffectNames.ToArray();
		milMo_VisualRepData.particleOffsets = _particleEffectOffsets.ToArray();
		milMo_VisualRepData.treatAsTerrainForJump = _treatAsTerrainForJump;
		milMo_VisualRepData.noCameraCollision = _noCameraCollision;
		milMo_VisualRepData.noPlayerCollisionRadius = _noPlayerCollisionRadius;
		milMo_VisualRepData.receiveShadows = _receiveShadows;
		milMo_VisualRepData.castShadows = _castShadows;
		milMo_VisualRepData.eventTag = EventTag;
		milMo_VisualRepData.SetupAnimations(AnimationConfigs);
		if (!IsActiveForCurrentEvent())
		{
			GameObject.SetActive(value: false);
		}
		if (_haveLoadPriority)
		{
			milMo_VisualRepData.SetPriority(_loadPriority);
		}
		else
		{
			milMo_VisualRepData.SetAutoPriority();
		}
		switch (_collisionType)
		{
		case MilMo_CollisionType.Sphere:
		{
			SphereCollider sphereCollider = GameObject.AddComponent<SphereCollider>();
			if ((bool)sphereCollider)
			{
				sphereCollider.radius = _collisionRadius;
			}
			break;
		}
		case MilMo_CollisionType.Capsule:
		{
			CapsuleCollider capsuleCollider = GameObject.AddComponent<CapsuleCollider>();
			if ((bool)capsuleCollider)
			{
				capsuleCollider.radius = _collisionRadius;
				capsuleCollider.height = _collisionHeight;
				float y = _collisionHeight * 0.5f;
				if (_colliderCenterZero)
				{
					y = 0f;
				}
				capsuleCollider.center = new Vector3(0f, y, 0f);
			}
			break;
		}
		}
		Transform[] componentsInChildren = GameObject.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if ((bool)transform && (bool)transform.gameObject && (bool)transform.parent && !(transform.parent != GameObject.transform))
			{
				string name = transform.gameObject.name;
				if (name == "Camera" || name == "Directional Light" || transform.gameObject.name.StartsWith("TextureLocators"))
				{
					MilMo_Global.Destroy(transform.gameObject);
				}
			}
		}
		Renderer[] componentsInChildren2 = GameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren2)
		{
			string name2 = renderer.gameObject.name;
			if (name2.StartsWith("Lod", StringComparison.InvariantCultureIgnoreCase))
			{
				AddLOD(renderer);
			}
			else if (name2.StartsWith("Gib", StringComparison.InvariantCultureIgnoreCase))
			{
				AddGib(renderer);
			}
			else if (name2.StartsWith("Col", StringComparison.InvariantCultureIgnoreCase))
			{
				AddMeshCollider(renderer);
			}
			else if (name2.StartsWith("CamCol", StringComparison.InvariantCultureIgnoreCase))
			{
				AddCameraCollider(renderer);
			}
			else if (name2.Equals("MeshHeld"))
			{
				AddMeshHeld(renderer);
			}
			else if (name2.StartsWith("Mesh", StringComparison.InvariantCultureIgnoreCase))
			{
				AddMainRenderer(renderer);
			}
			else if (Renderer == null)
			{
				AddMainRenderer(renderer);
			}
		}
		if (Renderer != null)
		{
			Lods.Insert(0, new MilMo_Lod(0, this, Renderer.gameObject, 0f));
			CurrentLod = Lods[0];
			CurrentLod.SetSkinQuality(_skinQuality, _skinNormals);
			if (Blocker)
			{
				Renderer.enabled = false;
			}
		}
		else
		{
			Debug.LogWarning("Got visual rep without a main renderer " + FullPath);
		}
		CleanupLODsAndGibs();
		if (Lods.Count > 0)
		{
			milMo_VisualRepData.lodDistances = new float[Lods.Count];
			int num = 0;
			foreach (MilMo_Lod lod in Lods)
			{
				milMo_VisualRepData.lodDistances[num++] = Mathf.Sqrt(lod.DistanceSquared);
			}
		}
		_data = milMo_VisualRepData;
		if (AudioSourceData != null)
		{
			_audioSource = AudioSourceData.Create(GameObject, _loadPriority);
		}
		FixUnity2Hierarchy();
		SetLayerOnRenderObject(_layer);
		componentsInChildren2 = GameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer obj in componentsInChildren2)
		{
			obj.receiveShadows = _receiveShadows;
			obj.shadowCastingMode = (_castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off);
		}
		return true;
	}

	private void AddLOD(Renderer source)
	{
		string name = source.gameObject.name;
		string text = name.Substring(3, name.Length - 3);
		if (text.IndexOf('.') > 0)
		{
			text = text.Substring(0, text.IndexOf('.'));
		}
		int index = int.Parse(text, NumberStyles.Integer);
		if (index + 10 < 0)
		{
			Debug.LogWarning(GameObject.name + ": LOD (" + name + ") has invalid index: " + text);
			return;
		}
		Renderer renderer = AddRenderer(source, $"LOD{index}", GameObject);
		if (renderer == null)
		{
			Debug.LogWarning(GameObject.name + ": Failed to create renderer for LOD (" + name + ")");
			return;
		}
		GameObject gameObject = renderer.gameObject;
		gameObject.layer = _layer;
		MilMo_Lod milMo_Lod = Lods.FirstOrDefault((MilMo_Lod l) => l.Index == index);
		if (milMo_Lod != null)
		{
			milMo_Lod.GameObjectRef = gameObject;
		}
		else
		{
			MilMo_Lod item = new MilMo_Lod(index, this, gameObject, 48f * (float)index);
			Lods.Add(item);
		}
		renderer.enabled = false;
	}

	private void AddGib(Renderer source)
	{
		string name = source.gameObject.name;
		string text = name.Substring(3, name.Length - 3);
		if (text.IndexOf('.') > 0)
		{
			text = text.Substring(0, text.IndexOf('.'));
		}
		int index = int.Parse(text, NumberStyles.Integer);
		if (index + 10 < 0)
		{
			Debug.LogWarning(GameObject.name + ": Gib (" + name + ") has invalid index: " + text);
			return;
		}
		GameObject gameObject = AddRenderer(source, $"GIB{index}", GameObject).gameObject;
		gameObject.layer = _layer;
		MilMo_Gib milMo_Gib = Gibs.FirstOrDefault((MilMo_Gib gib) => gib.Index == index);
		if (milMo_Gib != null)
		{
			milMo_Gib.GameObjectRef = gameObject;
		}
		else
		{
			MilMo_Gib item = new MilMo_Gib(index, this, gameObject, useParentMaterial: false);
			Gibs.Add(item);
		}
		gameObject.SetActive(value: false);
	}

	private void AddMeshCollider(Renderer source)
	{
		Mesh sharedMeshFromRenderer = GetSharedMeshFromRenderer(source);
		if ((bool)sharedMeshFromRenderer)
		{
			if (!_meshCollider)
			{
				_meshCollider = GameObject.AddComponent<MeshCollider>();
			}
			_meshCollider.sharedMesh = sharedMeshFromRenderer;
			source.gameObject.SetActive(value: false);
		}
	}

	private void AddCameraCollider(Renderer source)
	{
		Mesh sharedMeshFromRenderer = GetSharedMeshFromRenderer(source);
		if ((bool)sharedMeshFromRenderer)
		{
			GameObject gameObject = new GameObject("CameraEdgeDetectionCollision");
			gameObject.AddComponent<MeshCollider>().sharedMesh = sharedMeshFromRenderer;
			gameObject.transform.parent = GameObject.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			gameObject.layer = 22;
			if (_isWalkable)
			{
				GameObject.layer = 23;
			}
			else if (!_isCritterBlocker)
			{
				GameObject.layer = 24;
			}
			source.gameObject.SetActive(value: false);
		}
	}

	private void AddMeshHeld(Renderer source)
	{
		if (MeshHeld != null)
		{
			MeshHeld.GameObjectRef = source.gameObject;
			return;
		}
		Renderer renderer = AddRenderer(source, "MeshHeld", GameObject);
		if (renderer == null)
		{
			Debug.LogWarning(source.gameObject.name + ": MeshHeld renderer could not be created.");
		}
		else
		{
			MeshHeld = new MilMo_MeshHeld(this, renderer.gameObject);
		}
	}

	private void AddMainRenderer(Renderer source)
	{
		if (Renderer != null)
		{
			Debug.LogWarning(GameObject.name + ": Already have main renderer");
		}
		else
		{
			Renderer = AddRenderer(source, "Mesh", GameObject);
		}
	}

	private void CleanupLODsAndGibs()
	{
		for (int num = Gibs.Count - 1; num >= 0; num--)
		{
			MilMo_Gib milMo_Gib = Gibs[num];
			if (milMo_Gib == null || !milMo_Gib.IsValid)
			{
				Debug.Log($"GIB {num} with index " + (milMo_Gib?.Index.ToString() ?? "null") + "@" + FullPath + " has no valid game object or culling-mode. It will be removed.");
				Gibs.RemoveAt(num);
			}
		}
		for (int num2 = Lods.Count - 1; num2 >= 0; num2--)
		{
			MilMo_Lod milMo_Lod = Lods[num2];
			if (milMo_Lod == null || !milMo_Lod.IsValid)
			{
				Debug.Log($"LOD {num2} with index " + (milMo_Lod?.Index.ToString() ?? "null") + "@" + FullPath + " has no valid game object or culling-mode. It will be removed.");
				Lods.RemoveAt(num2);
			}
		}
		if (MeshHeld != null && !MeshHeld.IsValid)
		{
			Debug.Log("MeshHeld@" + FullPath + " has no valid game object or culling-mode. It will be removed.");
			MeshHeld = null;
		}
	}

	private Renderer AddRenderer(Renderer source, string name, GameObject target)
	{
		if (!(source is MeshRenderer source2))
		{
			if (source is SkinnedMeshRenderer source3)
			{
				return AddSkinnedMeshRenderer(source3, name, target);
			}
			return null;
		}
		return AddMeshRenderer(source2, name, target);
	}

	private MeshRenderer AddMeshRenderer(MeshRenderer source, string name, GameObject target)
	{
		MeshFilter component = source.gameObject.GetComponent<MeshFilter>();
		if (component == null || component.sharedMesh == null)
		{
			Debug.LogWarning(source.gameObject.name + ": Has no shared mesh. Renderer will be ignored.");
			return null;
		}
		if (source.gameObject != GameObject)
		{
			source.gameObject.name = name;
		}
		return source;
	}

	private SkinnedMeshRenderer AddSkinnedMeshRenderer(SkinnedMeshRenderer source, string name, GameObject target)
	{
		if (source.sharedMesh == null)
		{
			Debug.LogWarning(source.gameObject.name + ": Has no shared mesh. Renderer will be ignored.");
			return null;
		}
		if (source.gameObject != GameObject)
		{
			source.gameObject.name = name;
		}
		return source;
	}

	private Mesh GetSharedMeshFromRenderer(Renderer source)
	{
		if (!(source is MeshRenderer))
		{
			if (source is SkinnedMeshRenderer skinnedMeshRenderer)
			{
				return skinnedMeshRenderer.sharedMesh;
			}
			return null;
		}
		MeshFilter component = source.gameObject.GetComponent<MeshFilter>();
		if (!component)
		{
			return null;
		}
		return component.sharedMesh;
	}

	private void InitializeBlobShadow()
	{
		if (_useBlobShadow)
		{
			_blobShadow = GameObject.AddComponent<MilMo_BlobShadow>();
			if ((bool)_blobShadow)
			{
				MilMo_BlobShadowData data = new MilMo_BlobShadowData(_blobShadowMaterial, "", _blobShadowFieldOfView, 0f, 4f, orthographic: false, 0f, _blobShadowOffset, _blobShadowAutoFarPlane, _blobShadowFarPlane);
				_blobShadow.AsyncLoad(data);
				_blobShadow.IgnoreLayer(_layer);
				_blobShadow.Enabled = true;
			}
		}
	}

	public bool IsActiveForCurrentEvent()
	{
		if (string.IsNullOrEmpty(EventTag))
		{
			return true;
		}
		string[] array = EventTag.Split(' ');
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.StartsWith("!"))
			{
				if (!MilMo_Global.EventTags.Contains(text.Substring(1)))
				{
					continue;
				}
				flag = true;
			}
			flag2 = true;
			if (MilMo_Global.EventTags.Contains(text))
			{
				flag3 = true;
			}
		}
		if (!flag)
		{
			return !flag2 || flag3;
		}
		return false;
	}

	private void FixUnity2Hierarchy()
	{
		if (!GameObject)
		{
			return;
		}
		foreach (MilMo_Lod item in Lods.Where((MilMo_Lod lod) => lod.GameObjectRef != null))
		{
			item.GameObjectRef.transform.parent = GameObject.transform;
		}
		foreach (MilMo_Gib item2 in Gibs.Where((MilMo_Gib gib) => gib.GameObjectRef != null))
		{
			item2.GameObjectRef.transform.parent = GameObject.transform;
		}
	}

	private void InitializeMaterials(bool async)
	{
		if (!GameObject)
		{
			return;
		}
		Renderer[] componentsInChildren = GameObject.GetComponentsInChildren<Renderer>(includeInactive: true);
		foreach (Renderer renderer in componentsInChildren)
		{
			if (!renderer)
			{
				Debug.LogWarning("Trying to initialize material without a renderer. " + Path);
			}
			else
			{
				if (renderer.gameObject.name.ToUpper().Contains("LOD") || renderer.gameObject.name.ToUpper().Contains("GIB"))
				{
					continue;
				}
				if (Materials.Count == 0)
				{
					renderer.material = LoadDefaultMaterial(async);
					continue;
				}
				Material[] array = new Material[Materials.Count];
				int num = 0;
				foreach (MilMo_Material material in Materials)
				{
					material.Create(Path, Name, AssetTag, _loadPriority, _pauseModeOnMaterial, async);
					if (async)
					{
						material.SetColor(DefaultColor);
					}
					if (material.Material.HasProperty("_FogColor"))
					{
						_fogColorIndex = num;
					}
					if (_silhouette && material.Material.renderQueue == 2000)
					{
						material.Material.renderQueue = 1600;
					}
					array[num] = material.Material;
					num++;
				}
				renderer.materials = array;
			}
		}
		foreach (MilMo_Gib gib in Gibs)
		{
			gib.InitializeMaterials(_loadPriority, _pauseModeOnMaterial, async);
		}
		foreach (MilMo_Lod lod in Lods)
		{
			lod.InitializeMaterials(_loadPriority, _pauseModeOnMaterial, async);
		}
		if (MeshHeld != null)
		{
			MeshHeld.InitializeMaterials(_loadPriority, _pauseModeOnMaterial, async);
		}
	}

	private Material LoadDefaultMaterial(bool async)
	{
		Shader shader = MilMo_ResourceManager.LoadShaderLocal(Shader);
		if (!shader)
		{
			return null;
		}
		if (!async)
		{
			Material material = new Material(shader);
			if (_setTextureInDefaultMaterial)
			{
				material.mainTexture = MilMo_Material.LoadTexture(TextureName, Path, Name);
			}
			if (material.HasProperty(Ramp) && (bool)MilMo_Material.RampTexture)
			{
				material.SetTexture(Ramp, MilMo_Material.RampTexture);
			}
			return material;
		}
		Material mat = new Material(shader);
		MilMo_Material.AsyncLoadTexture(TextureName, Path, Name, AssetTag, _loadPriority, _pauseModeOnMaterial, delegate(Texture2D texture)
		{
			if (!(texture == null))
			{
				if (_setTextureInDefaultMaterial)
				{
					Debug.Log("Visualrep is setting texture " + texture.name);
					mat.mainTexture = texture;
				}
				if (mat.HasProperty(Ramp) && MilMo_Material.RampTexture != null)
				{
					mat.SetTexture(Ramp, MilMo_Material.RampTexture);
				}
			}
		});
		return mat;
	}

	private void FinishMaterials()
	{
		foreach (MilMo_Material material in Materials)
		{
			material.ResetColor();
		}
		if (_randomColors.Count > 0)
		{
			int num = int.MaxValue;
			Dictionary<string, List<Color>>.Enumerator enumerator2 = _randomColors.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				if (enumerator2.Current.Value.Count < num)
				{
					num = enumerator2.Current.Value.Count;
				}
			}
			enumerator2.Dispose();
			int index = UnityEngine.Random.Range(0, num);
			enumerator2 = _randomColors.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				foreach (MilMo_Material material2 in Materials)
				{
					material2.SetColor(enumerator2.Current.Key, enumerator2.Current.Value[index]);
				}
			}
			enumerator2.Dispose();
		}
		if (_materialsDoneCallback != null)
		{
			_materialsDoneCallback(this);
		}
	}

	private void InitializeAnimations()
	{
		if (!GameObject)
		{
			return;
		}
		_animation = GameObject.GetComponent<Animation>();
		if ((bool)_animation && _idleAnimation.Length > 0 && _animation[_idleAnimation] != null)
		{
			try
			{
				if (_animateAndRemove)
				{
					PlayAnimation(_idleAnimation, 0f, WrapMode.Once);
				}
				else
				{
					PlayAnimation(_idleAnimation, WrapMode.Loop);
				}
				if (!_animateAndRemove)
				{
					_animation[_idleAnimation].time = _animation[_idleAnimation].length * MilMo_Utility.Random();
				}
				_animation.clip = _animation[_idleAnimation].clip;
				_animation.playAutomatically = true;
				return;
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Failed to play animation " + _idleAnimation + " for visual rep " + FullPath + ". " + ex.Message);
				return;
			}
		}
		if ((bool)_animation)
		{
			_animation.playAutomatically = false;
			_animation.Stop();
		}
	}

	private void InitializeParticleEffects()
	{
		int num = 0;
		foreach (string particleEffectName in _particleEffectNames)
		{
			MilMo_Effect effect = MilMo_EffectContainer.GetEffect(particleEffectName, GameObject);
			if (effect != null)
			{
				effect.SetOffset(_particleEffectOffsets[num]);
				if (MilMo_ParticleContainer.DevMode)
				{
					Debug.Log("InitializeParticleEffects: " + particleEffectName + ", Attach to: " + GameObject);
				}
				_particleEffects.Add(effect);
				if (!Application.isPlaying)
				{
					effect.Update();
				}
			}
			num++;
		}
	}

	public void AddParticleEffect(MilMo_Effect effect, Vector3 offset)
	{
		effect.SetOffset(offset);
		_particleEffectOffsets.Add(offset);
		_particleEffects.Add(effect);
	}

	public GameObject PlayNextParticleEffect()
	{
		for (int i = 0; i < 100; i++)
		{
			if (_particleEffects.Count <= _nextParticleEffect)
			{
				_nextParticleEffect = 0;
				return null;
			}
			if (_particleEffects[_nextParticleEffect].TriggerNextAction())
			{
				return _particleEffects[_nextParticleEffect].GetCurrentGameObject();
			}
			_nextParticleEffect++;
		}
		return null;
	}

	public void DestroyAllParticleEffects()
	{
		foreach (MilMo_Effect particleEffect in _particleEffects)
		{
			particleEffect.Destroy();
		}
		_nextParticleEffect = 0;
	}

	public void RestoreLayer()
	{
		Renderer[] componentsInChildren = GameObject.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = _layer;
		}
	}

	public void SetTemporaryLayer(int layer)
	{
		Renderer[] componentsInChildren = GameObject.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = layer;
		}
	}

	public void SetLayerOnRenderObject(int layer)
	{
		if ((bool)GameObject)
		{
			Renderer[] componentsInChildren = GameObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.layer = layer;
			}
		}
	}

	public void ActivateCurrentLOD()
	{
		if ((bool)GameObject)
		{
			GameObject.SetActive(value: true);
		}
		foreach (MilMo_Lod item in Lods.Where((MilMo_Lod lod) => lod.GameObjectRef != null))
		{
			item.GameObjectRef.SetActive(item == CurrentLod);
		}
	}

	public async Task LoadAsync()
	{
		MilMo_SFFile milMo_SFFile = await MilMo_SimpleFormat.RealAsyncLoad(FullPath, AssetTag, _loadPriority);
		if (milMo_SFFile != null && milMo_SFFile.NumberOfRows > 0)
		{
			ParseFile(milMo_SFFile);
		}
	}

	public async void Load(VisualRepLoadDone callback)
	{
		MilMo_SFFile milMo_SFFile = await MilMo_SimpleFormat.RealAsyncLoad(FullPath, AssetTag, _loadPriority);
		if (milMo_SFFile != null && milMo_SFFile.NumberOfRows > 0)
		{
			ParseFile(milMo_SFFile);
			callback(this);
		}
		else
		{
			callback(this);
		}
	}

	public bool Load()
	{
		MilMo_SFFile file = MilMo_SimpleFormat.LoadLocal(FullPath);
		return ParseFile(file);
	}

	public bool ParseFile(MilMo_SFFile file)
	{
		bool flag = _particleEffectNames.Count > 0;
		int num = 0;
		if (file != null)
		{
			try
			{
				while (file.NextRow() && !file.IsNext("</VISUALREP>"))
				{
					if (file.IsNext("<VISUALREP>"))
					{
						continue;
					}
					if (file.IsNext("<MAT>"))
					{
						if (MilMo_Material.LoadMaterial(Materials, file, num))
						{
							num++;
						}
					}
					else if (file.IsNext("<AudioSource>"))
					{
						if (AudioSourceData == null)
						{
							AudioSourceData = new MilMo_Audio();
						}
						AudioSourceData.Load(file);
					}
					else if (file.IsNext("<LOD>"))
					{
						file.NextRow();
						if (!file.IsNext("Index"))
						{
							Debug.Log("Got LOD without index in visual rep " + FullPath);
							while (file.NextRow() && !file.IsNext("</LOD>"))
							{
							}
							continue;
						}
						int index = file.GetInt();
						file.NextRow();
						if (file.IsNext("</LOD>"))
						{
							continue;
						}
						file.PrevRow();
						MilMo_Lod milMo_Lod = Lods.FirstOrDefault((MilMo_Lod lod) => lod.Index == index);
						if (milMo_Lod != null)
						{
							milMo_Lod.Load(file);
							continue;
						}
						MilMo_Lod milMo_Lod2 = new MilMo_Lod(index, this);
						if (milMo_Lod2.Load(file))
						{
							Lods.Add(milMo_Lod2);
						}
					}
					else if (file.IsNext("<MESHHELD>"))
					{
						if (MeshHeld == null)
						{
							MeshHeld = new MilMo_MeshHeld(this);
						}
						MeshHeld.Load(file);
					}
					else if (file.IsNext("<GIB>"))
					{
						file.NextRow();
						if (!file.IsNext("Index"))
						{
							Debug.Log("Got GIB without index in visual rep " + FullPath);
							while (file.NextRow() && !file.IsNext("</GIB>"))
							{
							}
							continue;
						}
						int index = file.GetInt();
						file.NextRow();
						if (file.IsNext("</GIB>"))
						{
							continue;
						}
						file.PrevRow();
						MilMo_Gib milMo_Gib = Gibs.FirstOrDefault((MilMo_Gib gib) => gib.Index == index);
						if (milMo_Gib != null)
						{
							milMo_Gib.Load(file);
							continue;
						}
						MilMo_Gib milMo_Gib2 = new MilMo_Gib(index, this);
						if (milMo_Gib2.Load(file))
						{
							Gibs.Add(milMo_Gib2);
						}
					}
					else if (file.IsNext("Scale"))
					{
						float @float = file.GetFloat();
						Vector3 scale = new Vector3(@float, @float, @float);
						if (file.HasMoreTokens())
						{
							scale.y = file.GetFloat();
						}
						if (file.HasMoreTokens())
						{
							scale.z = file.GetFloat();
						}
						Scale = scale;
					}
					else if (file.IsNext("Mesh"))
					{
						_mesh = file.GetString();
					}
					else if (file.IsNext("Texture"))
					{
						TextureName = file.GetString();
					}
					else if (file.IsNext("Shader"))
					{
						Shader = "Shaders/" + file.GetString();
					}
					else if (file.IsNext("DefaultColor"))
					{
						HasDefaultColor = true;
						DefaultColor = file.GetColor();
					}
					else if (file.IsNext("Silhouette"))
					{
						_silhouette = true;
					}
					else if (file.IsNext("ReceiveShadows"))
					{
						_receiveShadows = file.GetBool();
					}
					else if (file.IsNext("CastShadows"))
					{
						_castShadows = file.GetBool();
					}
					else if (file.IsNext("IdleAnimation"))
					{
						_idleAnimation = file.GetString();
					}
					else if (file.IsNext("AnimateAndRemove"))
					{
						_animateAndRemove = file.GetBool();
					}
					else if (file.IsNext("AnimationSpeed"))
					{
						string @string = file.GetString();
						if (file.HasMoreTokens())
						{
							float float2 = file.GetFloat();
							if (!AnimationConfigs.ContainsKey(@string))
							{
								AnimationConfigs.Add(@string, new AnimationConfig());
							}
							AnimationConfigs[@string].Speed = float2;
						}
					}
					else if (file.IsNext("AnimationSound"))
					{
						MilMo_AnimationSound milMo_AnimationSound = new MilMo_AnimationSound();
						milMo_AnimationSound.Read(file);
						if (!AnimationConfigs.ContainsKey(milMo_AnimationSound.Animation))
						{
							AnimationConfigs.Add(milMo_AnimationSound.Animation, new AnimationConfig());
						}
						AnimationConfigs[milMo_AnimationSound.Animation].Sound = milMo_AnimationSound;
					}
					else if (file.IsNext("SkinQuality"))
					{
						int @int = file.GetInt();
						if (@int >= 4)
						{
							_skinQuality = SkinQuality.Bone4;
						}
						else if (@int >= 2)
						{
							_skinQuality = SkinQuality.Bone2;
						}
						else
						{
							_skinQuality = SkinQuality.Bone1;
						}
					}
					else if (file.IsNext("SkinNormals"))
					{
						_skinNormals = true;
					}
					else if (file.IsNext("LoadPriority"))
					{
						string string2 = file.GetString();
						if (string2.Equals("High", StringComparison.InvariantCultureIgnoreCase))
						{
							_loadPriority = MilMo_ResourceManager.Priority.High;
						}
						else if (string2.Equals("Medium", StringComparison.InvariantCultureIgnoreCase))
						{
							_loadPriority = MilMo_ResourceManager.Priority.Medium;
						}
						else if (string2.Equals("Low", StringComparison.InvariantCultureIgnoreCase))
						{
							_loadPriority = MilMo_ResourceManager.Priority.Low;
						}
						else
						{
							Debug.LogWarning("Got unknown priority '" + string2 + "' for LoadPriority in visual rep " + FullPath);
						}
						_haveLoadPriority = true;
					}
					else if (file.IsNext("CollisionType"))
					{
						string string3 = file.GetString();
						if (string3.Equals("Sphere", StringComparison.InvariantCultureIgnoreCase))
						{
							_collisionType = MilMo_CollisionType.Sphere;
						}
						else if (string3.Equals("Capsule", StringComparison.InvariantCultureIgnoreCase))
						{
							_collisionType = MilMo_CollisionType.Capsule;
						}
					}
					else if (file.IsNext("CollisionRadius"))
					{
						_collisionRadius = file.GetFloat();
					}
					else if (file.IsNext("CollisionHeight"))
					{
						_collisionHeight = file.GetFloat();
					}
					else if (file.IsNext("ColliderCenterZero"))
					{
						_colliderCenterZero = true;
					}
					else if (file.IsNext("NoPlayerCollisionRadius"))
					{
						_noPlayerCollisionRadius = file.GetFloat();
						_noPlayerCollisionSqrRadius = _noPlayerCollisionRadius * _noPlayerCollisionRadius;
					}
					else if (file.IsNext("Walkable"))
					{
						_isWalkable = true;
					}
					else if (file.IsNext("WalkableName"))
					{
						_walkableName = file.GetString();
					}
					else if (file.IsNext("WalkableResolution"))
					{
						_walkableResolution = file.GetInt();
					}
					else if (file.IsNext("CritterBlocker"))
					{
						_isCritterBlocker = true;
					}
					else if (file.IsNext("GroundMaterial"))
					{
						_groundMaterial = file.GetString();
					}
					else if (file.IsNext("Layer"))
					{
						_layer = file.GetInt();
					}
					else if (file.IsNext("Spin"))
					{
						_spin = file.GetVector3();
					}
					else if (file.IsNext("Billboard"))
					{
						Billboard = file.GetBool();
					}
					else if (file.IsNext("OnlyLodsUseBillboard"))
					{
						OnlyLodsUseBillboard = file.GetBool();
					}
					else if (file.IsNext("Blocker"))
					{
						Blocker = file.GetBool();
					}
					else if (file.IsNext("UVAnimation"))
					{
						_uvAnimation = true;
						_uvAnimationSpeed = file.GetVector2();
					}
					else if (file.IsNext("ParticleEffect"))
					{
						if (flag)
						{
							_particleEffectNames.Clear();
							_particleEffectOffsets.Clear();
							flag = false;
						}
						_particleEffectNames.Add(file.GetString());
						_particleEffectOffsets.Add(file.HasMoreTokens() ? file.GetVector3() : new Vector3(0f, 0f, 0f));
					}
					else if (file.IsNext("BlobShadow"))
					{
						_useBlobShadow = true;
					}
					else if (file.IsNext("BlobShadowFieldOfView"))
					{
						_blobShadowFieldOfView = file.GetFloat();
					}
					else if (file.IsNext("BlobShadowMaterial"))
					{
						_blobShadowMaterial = file.GetString();
					}
					else if (file.IsNext("BlobShadowOffset"))
					{
						_blobShadowOffset = file.GetVector3();
					}
					else if (file.IsNext("BlobShadowFarPlane"))
					{
						_blobShadowFarPlane = file.GetFloat();
						_blobShadowAutoFarPlane = false;
					}
					else if (file.IsNext("RandomColor"))
					{
						string string4 = file.GetString();
						Color color = file.GetColor();
						if (!_randomColors.ContainsKey(string4))
						{
							_randomColors.Add(string4, new List<Color>());
						}
						_randomColors[string4].Add(color);
					}
					else if (file.IsNext("TreatAsTerrainForJump"))
					{
						_treatAsTerrainForJump = file.GetBool();
					}
					else if (file.IsNext("NoCameraCollision"))
					{
						_noCameraCollision = file.GetBool();
					}
					else if (file.IsNext("UpdateWhenOffscreen"))
					{
						UpdateWhenOffscreen = file.GetBool();
					}
					else if (file.IsNext("EventTag"))
					{
						EventTag = file.GetString();
					}
					else if (file.IsNext("ObjectEffect"))
					{
						string string5 = file.GetString();
						ObjectEffectNames.Add(string5);
					}
					else
					{
						Debug.LogWarning("Got unknown command " + file.GetString() + " when loading visual rep " + FullPath);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Failed to load visual rep " + FullPath + " at line " + file.GetLineNumber());
				Debug.LogWarning(ex.ToString());
			}
		}
		else
		{
			_mesh = Name;
			TextureName = Name;
			Shader = "Shaders/Junebug/Diffuse";
		}
		if (_mesh.StartsWith("Blocker"))
		{
			Blocker = true;
		}
		if (_mesh.StartsWith("None"))
		{
			_meshNone = true;
		}
		foreach (MilMo_Lod lod in Lods)
		{
			lod.FinishLoading();
		}
		foreach (MilMo_Gib gib in Gibs)
		{
			gib.FinishLoading();
		}
		if (MeshHeld != null)
		{
			MeshHeld.FinishLoading();
		}
		return true;
	}

	public void Write(MilMo_SFFile file, GameObject prop, bool isTemplate)
	{
		Renderer componentInChildren = prop.GetComponentInChildren<Renderer>();
		if (componentInChildren == null)
		{
			Debug.LogWarning("Failed to write visual rep " + prop.name + ". No valid renderer exist.");
			return;
		}
		file.AddRow();
		file.Write("<VISUALREP>");
		if (isTemplate)
		{
			Material[] sharedMaterials = componentInChildren.sharedMaterials;
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				MilMo_Material.WriteMaterial(sharedMaterials[i], file);
			}
		}
		else
		{
			int num = 0;
			Material[] sharedMaterials = componentInChildren.sharedMaterials;
			foreach (Material material in sharedMaterials)
			{
				if (num < Materials.Count)
				{
					MilMo_Material.WriteMaterial(material, file, num, Materials[num]);
				}
				else
				{
					MilMo_Material.WriteMaterial(material, file);
				}
				num++;
			}
		}
		MilMo_VisualRepComponent componentInChildren2 = prop.transform.root.GetComponentInChildren<MilMo_VisualRepComponent>();
		MilMo_VisualRepData milMo_VisualRepData = ((componentInChildren2 != null) ? componentInChildren2.GetData() : null);
		Renderer[] componentsInChildren = prop.GetComponentsInChildren<Renderer>(includeInactive: true);
		int num2 = 0;
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			string name = renderer.gameObject.name;
			if (name.StartsWith("Lod", StringComparison.InvariantCultureIgnoreCase))
			{
				string text = renderer.gameObject.name.Substring(3, renderer.transform.name.Length - 3);
				if (text.IndexOf('.') > 0)
				{
					text = text.Substring(0, text.IndexOf('.'));
				}
				int index = int.Parse(text, NumberStyles.Integer);
				if (index + 10 < 0)
				{
					Debug.LogWarning(prop.name + ": Lod has invalid index: " + index);
					continue;
				}
				float distance = ((milMo_VisualRepData != null && milMo_VisualRepData.lodDistances != null && index < milMo_VisualRepData.lodDistances.Length) ? milMo_VisualRepData.lodDistances[index] : (48f * (float)index));
				MilMo_Lod milMo_Lod = new MilMo_Lod(index, this, renderer.gameObject, distance);
				num2++;
				MilMo_Lod template = Lods.FirstOrDefault((MilMo_Lod l) => l.Index == index);
				milMo_Lod.Write(file, template);
			}
			else if (name.StartsWith("Gib", StringComparison.InvariantCultureIgnoreCase))
			{
				string text2 = renderer.gameObject.name.Substring(3, renderer.transform.name.Length - 3);
				if (text2.IndexOf('.') > 0)
				{
					text2 = text2.Substring(0, text2.IndexOf('.'));
				}
				int index = int.Parse(text2, NumberStyles.Integer);
				if (index + 10 < 0)
				{
					Debug.LogWarning(prop.name + ": Gib has invalid index: " + index);
					continue;
				}
				MilMo_Gib milMo_Gib = new MilMo_Gib(index, this, renderer.gameObject, useParentMaterial: false);
				MilMo_Gib template2 = Gibs.FirstOrDefault((MilMo_Gib g) => g.Index == index);
				milMo_Gib.Write(file, template2);
			}
			else if (name.Equals("MeshHeld", StringComparison.InvariantCultureIgnoreCase))
			{
				new MilMo_MeshHeld(this, renderer.gameObject).Write(file, MeshHeld);
			}
		}
		if (_mesh != Name)
		{
			file.AddRow();
			file.Write("Mesh");
			file.Write(_mesh);
		}
		if (TextureName != Name)
		{
			file.AddRow();
			file.Write("Texture");
			file.Write(TextureName);
		}
		if (Shader != "Shaders/Junebug/Diffuse")
		{
			file.AddRow();
			file.Write("Shader");
			file.Write(Shader);
		}
		if (milMo_VisualRepData != null && (isTemplate || (!MilMo_Utility.Equals(DefaultColor, milMo_VisualRepData.defaultColor) && !MilMo_Utility.Equals(milMo_VisualRepData.defaultColor, Color.white))))
		{
			file.AddRow();
			file.Write("DefaultColor");
			file.Write(milMo_VisualRepData.defaultColor);
		}
		if (_idleAnimation.Length > 0)
		{
			file.AddRow();
			file.Write("IdleAnimation");
			file.Write(_idleAnimation);
		}
		if (_animateAndRemove)
		{
			file.AddRow();
			file.Write("AnimateAndRemove");
			file.Write(_animateAndRemove);
		}
		if (isTemplate && milMo_VisualRepData != null)
		{
			for (int j = 0; j < milMo_VisualRepData.animations.Length; j++)
			{
				if (j < milMo_VisualRepData.animationSpeeds.Length && (double)Math.Abs(milMo_VisualRepData.animationSpeeds[j] - 1f) > 0.0)
				{
					file.AddRow();
					file.Write("AnimationSpeed");
					file.Write(milMo_VisualRepData.animations[j]);
					file.Write(milMo_VisualRepData.animationSpeeds[j]);
				}
				if (j < milMo_VisualRepData.animationSounds.Length && !string.IsNullOrEmpty(milMo_VisualRepData.animationSounds[j]))
				{
					file.AddRow();
					file.Write("AnimationSound");
					file.Write(milMo_VisualRepData.animations[j]);
					file.Write(milMo_VisualRepData.animationSounds[j]);
					if (j < milMo_VisualRepData.animationSoundTilings.Length && (double)Math.Abs(milMo_VisualRepData.animationSoundTilings[j] - 1f) > 0.0)
					{
						file.Write(milMo_VisualRepData.animationSoundTilings[j]);
					}
				}
			}
		}
		else if (milMo_VisualRepData != null)
		{
			for (int k = 0; k < milMo_VisualRepData.animations.Length; k++)
			{
				string key = milMo_VisualRepData.animations[k];
				if (k < milMo_VisualRepData.animationSpeeds.Length && (!AnimationConfigs.ContainsKey(key) || (double)Math.Abs(AnimationConfigs[key].Speed - milMo_VisualRepData.animationSpeeds[k]) > 0.0))
				{
					file.AddRow();
					file.Write("AnimationSpeed");
					file.Write(milMo_VisualRepData.animations[k]);
					file.Write(milMo_VisualRepData.animationSpeeds[k]);
				}
				if (k < milMo_VisualRepData.animationSounds.Length && (!AnimationConfigs.ContainsKey(key) || (AnimationConfigs[key].Sound == null && !string.IsNullOrEmpty(milMo_VisualRepData.animationSounds[k])) || (AnimationConfigs[key].Sound != null && (AnimationConfigs[key].Sound.Path != milMo_VisualRepData.animationSounds[k] || (k < milMo_VisualRepData.animationSoundTilings.Length && (double)Math.Abs(AnimationConfigs[key].Sound.Tiling - milMo_VisualRepData.animationSoundTilings[k]) > 0.0)))))
				{
					file.AddRow();
					file.Write("AnimationSound");
					file.Write(milMo_VisualRepData.animations[k]);
					file.Write(milMo_VisualRepData.animationSounds[k]);
					if (k < milMo_VisualRepData.animationSoundTilings.Length)
					{
						file.Write(milMo_VisualRepData.animationSoundTilings[k]);
					}
				}
			}
		}
		if (milMo_VisualRepData != null && milMo_VisualRepData.groundMaterial != null && milMo_VisualRepData.groundMaterial.Length > 0 && (isTemplate || !milMo_VisualRepData.groundMaterial.Equals(_groundMaterial)))
		{
			file.AddRow();
			file.Write("GroundMaterial");
			file.Write(milMo_VisualRepData.groundMaterial);
		}
		bool flag = false;
		if (isTemplate)
		{
			flag = true;
		}
		else if (milMo_VisualRepData != null)
		{
			if (milMo_VisualRepData.particleEffects.Length != _particleEffectNames.Count)
			{
				flag = true;
			}
			else
			{
				int num3 = 0;
				foreach (string particleEffectName in _particleEffectNames)
				{
					if (particleEffectName != milMo_VisualRepData.particleEffects[num3] || _particleEffectOffsets[num3] != milMo_VisualRepData.particleOffsets[num3])
					{
						flag = true;
						break;
					}
					num3++;
				}
			}
		}
		if (flag && milMo_VisualRepData != null && milMo_VisualRepData.particleEffects.Length != 0)
		{
			int num4 = 0;
			string[] particleEffects = milMo_VisualRepData.particleEffects;
			foreach (string str in particleEffects)
			{
				file.AddRow();
				file.Write("ParticleEffect");
				file.Write(str);
				file.Write((milMo_VisualRepData.particleOffsets.Length > num4) ? milMo_VisualRepData.particleOffsets[num4] : Vector3.zero);
				num4++;
			}
		}
		if (milMo_VisualRepData != null && milMo_VisualRepData.spin.sqrMagnitude > 0f && (isTemplate || !MilMo_Utility.Equals(milMo_VisualRepData.spin, _spin)))
		{
			file.AddRow();
			file.Write("Spin");
			file.Write(milMo_VisualRepData.spin);
		}
		if (milMo_VisualRepData != null && milMo_VisualRepData.billboard && (isTemplate || milMo_VisualRepData.billboard != Billboard))
		{
			file.AddRow();
			file.Write("Billboard");
			file.Write(milMo_VisualRepData.billboard);
		}
		if (milMo_VisualRepData != null && milMo_VisualRepData.onlyLodsUseBillboard && (isTemplate || milMo_VisualRepData.onlyLodsUseBillboard != OnlyLodsUseBillboard))
		{
			file.AddRow();
			file.Write("OnlyLodsUseBillboard");
			file.Write(milMo_VisualRepData.onlyLodsUseBillboard);
		}
		if (milMo_VisualRepData != null && milMo_VisualRepData.blocker && (isTemplate || milMo_VisualRepData.blocker != Blocker))
		{
			file.AddRow();
			file.Write("Blocker");
			file.Write(milMo_VisualRepData.blocker);
		}
		if (milMo_VisualRepData != null && milMo_VisualRepData.loadPriority != MilMo_VisualRepData.LoadPriority.Auto)
		{
			MilMo_ResourceManager.Priority priority = milMo_VisualRepData.GetPriority();
			if (isTemplate || priority != _loadPriority || (!isTemplate && !_haveLoadPriority))
			{
				file.AddRow();
				file.Write("LoadPriority");
				file.Write(priority.ToString());
			}
		}
		if (milMo_VisualRepData != null && !MilMo_Utility.Equals(milMo_VisualRepData.uvAnimation, Vector2.zero) && (isTemplate || !MilMo_Utility.Equals(milMo_VisualRepData.uvAnimation, _uvAnimationSpeed)))
		{
			file.AddRow();
			file.Write("UVAnimation");
			file.Write(milMo_VisualRepData.uvAnimation);
		}
		if (milMo_VisualRepData != null && milMo_VisualRepData.silhouette && (isTemplate || milMo_VisualRepData.silhouette != _silhouette))
		{
			file.AddRow();
			file.Write("Silhouette");
		}
		if (milMo_VisualRepData != null && milMo_VisualRepData.walkable && !isTemplate && milMo_VisualRepData.walkable != _isWalkable)
		{
			file.AddRow();
			file.Write("Walkable");
		}
		if (milMo_VisualRepData != null && milMo_VisualRepData.walkableName.Length > 0 && !isTemplate && milMo_VisualRepData.walkableName != _walkableName)
		{
			file.AddRow();
			file.Write("WalkableName");
			file.Write(milMo_VisualRepData.walkableName);
		}
		if (milMo_VisualRepData != null && milMo_VisualRepData.walkableResolution != 32 && !isTemplate && milMo_VisualRepData.walkableResolution != _walkableResolution)
		{
			file.AddRow();
			file.Write("WalkableResolution");
			file.Write(milMo_VisualRepData.walkableResolution);
		}
		if (milMo_VisualRepData != null && milMo_VisualRepData.critterBlocker && (isTemplate || milMo_VisualRepData.critterBlocker != _isCritterBlocker))
		{
			file.AddRow();
			file.Write("CritterBlocker");
		}
		if (isTemplate && componentInChildren.gameObject.layer != 0)
		{
			file.AddRow();
			file.Write("Layer");
			file.Write(componentInChildren.gameObject.layer);
		}
		else if (componentInChildren.gameObject.layer != _layer && componentInChildren.gameObject.layer != 0)
		{
			file.AddRow();
			file.Write("Layer");
			file.Write(componentInChildren.gameObject.layer);
		}
		if (prop.GetComponent<AudioSourceWrapper>() != null)
		{
			MilMo_Audio.Write(prop.GetComponent<AudioSourceWrapper>(), file, isTemplate ? null : AudioSourceData);
		}
		if (milMo_VisualRepData != null && ((isTemplate && milMo_VisualRepData.treatAsTerrainForJump) || milMo_VisualRepData.treatAsTerrainForJump != _treatAsTerrainForJump))
		{
			file.AddRow();
			file.Write("TreatAsTerrainForJump");
			file.Write(milMo_VisualRepData.treatAsTerrainForJump);
		}
		if (milMo_VisualRepData != null && ((isTemplate && milMo_VisualRepData.noCameraCollision) || milMo_VisualRepData.noCameraCollision != _noCameraCollision))
		{
			file.AddRow();
			file.Write("NoCameraCollision");
			file.Write(milMo_VisualRepData.noCameraCollision);
		}
		SkinnedMeshRenderer componentInChildren3 = prop.GetComponentInChildren<SkinnedMeshRenderer>();
		if (componentInChildren3 != null)
		{
			if (isTemplate && componentInChildren3.updateWhenOffscreen)
			{
				file.AddRow();
				file.Write("UpdateWhenOffscreen");
				file.Write(componentInChildren3.updateWhenOffscreen);
			}
			else if (!isTemplate && componentInChildren3.updateWhenOffscreen != UpdateWhenOffscreen)
			{
				file.AddRow();
				file.Write("UpdateWhenOffscreen");
				file.Write(componentInChildren3.updateWhenOffscreen);
			}
		}
		if (milMo_VisualRepData != null && !milMo_VisualRepData.castShadows)
		{
			file.AddRow();
			file.Write("ReceiveShadows");
			file.Write("False");
		}
		if (milMo_VisualRepData != null && !milMo_VisualRepData.castShadows)
		{
			file.AddRow();
			file.Write("CastShadows");
			file.Write("False");
		}
		if (milMo_VisualRepData != null && ((isTemplate && milMo_VisualRepData.noPlayerCollisionRadius > 0f) || (!isTemplate && (double)Math.Abs(milMo_VisualRepData.noPlayerCollisionRadius - _noPlayerCollisionRadius) > 0.0)))
		{
			file.AddRow();
			file.Write("NoPlayerCollisionRadius");
			file.Write(milMo_VisualRepData.noPlayerCollisionRadius);
		}
		if (milMo_VisualRepData != null && !string.IsNullOrEmpty(milMo_VisualRepData.eventTag))
		{
			file.AddRow();
			file.Write("EventTag");
			file.Write(milMo_VisualRepData.eventTag);
		}
		file.AddRow();
		file.Write("</VISUALREP>");
	}
}
