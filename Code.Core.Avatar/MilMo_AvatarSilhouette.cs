using Code.Core.Global;
using Code.Core.ResourceSystem;
using Code.World.Player;
using UnityEngine;

namespace Code.Core.Avatar;

public class MilMo_AvatarSilhouette : MonoBehaviour
{
	public UnityEngine.Camera silhouetteCamera;

	private Shader _shader;

	private Material _material;

	[Range(-2f, 2f)]
	public float nearScale = 1f;

	[Range(-2f, 2f)]
	public float nearBias = 0.1f;

	[Range(-2f, 2f)]
	public float farScale = 1f;

	[Range(-200f, 200f)]
	public float farBias;

	public float distLerp;

	public float distScale;

	private void Awake()
	{
		_shader = MilMo_ResourceManager.LoadShaderLocal("Shaders/AvatarSilhouette");
		_material = new Material(_shader);
		GameObject obj = base.gameObject;
		obj.transform.parent = MilMo_Global.Camera.transform;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = Quaternion.identity;
		silhouetteCamera = base.gameObject.AddComponent<UnityEngine.Camera>();
		if (!(silhouetteCamera == null))
		{
			silhouetteCamera.name = "Silhouette";
			silhouetteCamera.allowMSAA = false;
			silhouetteCamera.allowHDR = false;
			silhouetteCamera.cullingMask = 12288;
			silhouetteCamera.clearFlags = CameraClearFlags.Nothing;
			silhouetteCamera.fieldOfView = MilMo_Global.Camera.fieldOfView;
			silhouetteCamera.nearClipPlane = MilMo_Global.Camera.nearClipPlane;
			silhouetteCamera.farClipPlane = MilMo_Global.Camera.farClipPlane;
			silhouetteCamera.depth = 1f;
			silhouetteCamera.SetReplacementShader(_shader, "");
			silhouetteCamera.enabled = true;
		}
	}

	public void OnDestroy()
	{
		Object.Destroy(_material);
		Object.Destroy(silhouetteCamera);
	}

	public void Enable(bool enable)
	{
		base.enabled = enable;
		silhouetteCamera.enabled = enable;
	}

	private void Update()
	{
		if (MilMo_Player.Instance != null && (bool)MilMo_Global.Camera)
		{
			float magnitude = (MilMo_Player.Instance.Avatar.Position - MilMo_Global.Camera.transform.position).magnitude;
			distLerp = 1f - (magnitude - 2f) / 10f;
			silhouetteCamera.enabled = distLerp <= 1f;
			distLerp = Mathf.Clamp(distLerp, 0f, 1f);
			distLerp *= distLerp;
			distScale = distLerp * 1.7f + (1f - distLerp) * 1.14f;
			silhouetteCamera.fieldOfView = MilMo_Global.Camera.fieldOfView;
			silhouetteCamera.nearClipPlane = MilMo_Global.Camera.nearClipPlane * distScale * nearScale + nearBias;
			silhouetteCamera.farClipPlane = MilMo_Global.Camera.farClipPlane * farScale + farBias;
		}
	}
}
