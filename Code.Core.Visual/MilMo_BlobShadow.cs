using Code.Core.Global;
using UnityEngine;

namespace Code.Core.Visual;

[ExecuteInEditMode]
public class MilMo_BlobShadow : MonoBehaviour
{
	private static Material _blobMaterial;

	private static bool _disableBlobShadows;

	private Projector _projector;

	private GameObject _gameObject;

	private const string BLOB_MATERIAL_PATH = "BlobShadow/ShadowMaterial";

	private Material _material;

	private MilMo_BlobShadowData _data;

	private bool _enabled;

	private bool _initialized;

	public bool Enabled
	{
		set
		{
			_enabled = value;
			_projector.enabled = value;
			_gameObject.SetActive(value);
		}
	}

	public static void EnableBlobShadows()
	{
		_disableBlobShadows = false;
	}

	public static void DisableBlobShadows()
	{
		_disableBlobShadows = true;
	}

	public static void AsyncLoadContent()
	{
		if (!(_blobMaterial != null))
		{
			Material material = Resources.Load<Material>("BlobShadow/ShadowMaterial");
			if (material != null)
			{
				_blobMaterial = material;
			}
		}
	}

	public void AsyncLoad(MilMo_BlobShadowData data)
	{
		_data = data;
		if (_data != null)
		{
			Initialize();
			InitializeParameters();
		}
	}

	private void Initialize()
	{
		if (_initialized)
		{
			return;
		}
		_gameObject = new GameObject("BlobShadow");
		_gameObject.transform.parent = base.gameObject.transform;
		_projector = _gameObject.AddComponent<Projector>();
		_projector.enabled = false;
		_material = _blobMaterial;
		_projector.material = _blobMaterial;
		if (!string.IsNullOrEmpty(_data.Material))
		{
			Material material = Resources.Load<Material>("BlobShadow/" + _data.Material);
			if ((bool)material)
			{
				_material = material;
				_projector.material = material;
			}
		}
		_gameObject.transform.localPosition = _data.Offset;
		_gameObject.transform.localRotation = Quaternion.Euler(90f, 180f, 0f);
		_initialized = true;
	}

	private void InitializeParameters()
	{
		_projector.nearClipPlane = 0.1f;
		_projector.fieldOfView = _data.Fov;
		_projector.aspectRatio = 1f;
		_projector.orthographic = _data.Orthographic;
		_projector.orthographicSize = _data.OrthographicSize;
		IgnoreLayer(1);
	}

	private void Awake()
	{
		if (_data != null)
		{
			Initialize();
		}
	}

	private void LateUpdate()
	{
		if (_disableBlobShadows && _enabled)
		{
			Enabled = false;
		}
		if (_disableBlobShadows || !_enabled || !_initialized || !_projector.material || _data == null)
		{
			return;
		}
		if (_data.AutoFarPlaneMode)
		{
			if (Physics.Raycast(_gameObject.transform.position, Vector3.down, out var hitInfo, 50f))
			{
				_projector.farClipPlane = hitInfo.distance + 0.2f;
				_projector.enabled = hitInfo.distance > _data.ActivateHeight && hitInfo.distance < _data.DeactivationHeight;
			}
			else
			{
				_projector.enabled = false;
			}
		}
		else
		{
			_projector.farClipPlane = _data.FarPlane;
		}
		_gameObject.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
	}

	public void Destroy()
	{
		MilMo_Global.Destroy(_gameObject);
		MilMo_Global.Destroy(_projector);
		MilMo_Global.Destroy(this);
		if (_material != _blobMaterial)
		{
			MilMo_Global.Destroy(_material);
		}
	}

	public void IgnoreLayer(int layer)
	{
		_projector.ignoreLayers |= 1 << layer;
	}

	public void IgnoreMask(int mask)
	{
		_projector.ignoreLayers |= mask;
	}
}
