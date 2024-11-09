using System;
using System.Collections.Generic;
using Code.Core.Camera;
using Code.Core.Global;
using Code.World.Level;
using Code.World.Player;
using Core.Settings;
using UnityEngine;

namespace Code.Core.Visual.Water;

[ExecuteInEditMode]
public class MilMo_Water : MonoBehaviour
{
	public enum WaterMode
	{
		Simple,
		Reflective,
		Refractive
	}

	public WaterMode waterMode = WaterMode.Refractive;

	public bool disablePixelLights;

	public int textureSize = 1024;

	public float clipPlaneOffset = 0.07f;

	private const int IGNORED_LAYERS = 34;

	public LayerMask reflectLayers = -35;

	public LayerMask refractLayers = -35;

	private readonly Dictionary<UnityEngine.Camera, UnityEngine.Camera> _reflectionCameras = new Dictionary<UnityEngine.Camera, UnityEngine.Camera>();

	private readonly Dictionary<UnityEngine.Camera, UnityEngine.Camera> _refractionCameras = new Dictionary<UnityEngine.Camera, UnityEngine.Camera>();

	private RenderTexture _reflectionTexture;

	private RenderTexture _refractionTexture;

	private WaterMode _hardwareWaterSupport = WaterMode.Refractive;

	private int _oldReflectionTextureSize;

	private int _oldRefractionTextureSize;

	private static bool _insideWater;

	private Renderer _renderer;

	public string preset = "Lagoon";

	private static bool _useDepthTexture = true;

	private double _lastCheckTime;

	private void GetRenderer()
	{
		if (!(_lastCheckTime > (double)(Time.time + 500f)))
		{
			_renderer = GetComponentInChildren<Renderer>();
			_lastCheckTime = Time.time;
		}
	}

	public void OnWillRenderObject()
	{
		if (!base.enabled)
		{
			return;
		}
		if (!_renderer)
		{
			GetRenderer();
		}
		if (!_renderer || !_renderer.sharedMaterial || !_renderer.enabled)
		{
			return;
		}
		UnityEngine.Camera current = UnityEngine.Camera.current;
		if (!current)
		{
			return;
		}
		CheckDepthTextureSupport(current);
		if (!_insideWater)
		{
			_insideWater = true;
			_hardwareWaterSupport = FindHardwareWaterSupport();
			WaterMode waterMode = GetWaterMode();
			CreateWaterObjects(current, out var reflectionCamera, out var refractionCamera);
			Transform obj = base.transform;
			Vector3 position = obj.position;
			Vector3 up = obj.up;
			int pixelLightCount = QualitySettings.pixelLightCount;
			if (disablePixelLights)
			{
				QualitySettings.pixelLightCount = 0;
			}
			UpdateCameraModes(current, reflectionCamera);
			UpdateCameraModes(current, refractionCamera);
			if (waterMode >= WaterMode.Reflective)
			{
				float w = 0f - Vector3.Dot(up, position) - clipPlaneOffset;
				Vector4 plane = new Vector4(up.x, up.y, up.z, w);
				Matrix4x4 reflectionMat = Matrix4x4.zero;
				CalculateReflectionMatrix(ref reflectionMat, plane);
				Vector3 position2 = current.transform.position;
				Vector3 position3 = reflectionMat.MultiplyPoint(position2);
				reflectionCamera.worldToCameraMatrix = current.worldToCameraMatrix * reflectionMat;
				Vector4 clipPlane = CameraSpacePlane(reflectionCamera, position, up, 1f);
				reflectionCamera.projectionMatrix = current.CalculateObliqueMatrix(clipPlane);
				reflectionCamera.cullingMask = -17 & reflectLayers.value;
				reflectionCamera.targetTexture = _reflectionTexture;
				bool invertCulling = GL.invertCulling;
				GL.invertCulling = !invertCulling;
				Transform obj2 = reflectionCamera.transform;
				obj2.position = position3;
				Vector3 eulerAngles = current.transform.eulerAngles;
				obj2.eulerAngles = new Vector3(0f - eulerAngles.x, eulerAngles.y, eulerAngles.z);
				reflectionCamera.Render();
				reflectionCamera.transform.position = position2;
				GL.invertCulling = invertCulling;
				_renderer.sharedMaterial.SetTexture(Shader.PropertyToID("_ReflectionTex"), _reflectionTexture);
			}
			if (waterMode >= WaterMode.Refractive)
			{
				refractionCamera.worldToCameraMatrix = current.worldToCameraMatrix;
				Vector4 clipPlane2 = CameraSpacePlane(refractionCamera, position, up, -1f);
				refractionCamera.projectionMatrix = current.CalculateObliqueMatrix(clipPlane2);
				refractionCamera.cullingMatrix = current.projectionMatrix * current.worldToCameraMatrix;
				refractionCamera.cullingMask = -17 & refractLayers.value;
				refractionCamera.targetTexture = _refractionTexture;
				Transform obj3 = refractionCamera.transform;
				Transform transform = current.transform;
				obj3.position = transform.position;
				obj3.rotation = transform.rotation;
				refractionCamera.Render();
				_renderer.sharedMaterial.SetTexture(Shader.PropertyToID("_RefractionTex"), _refractionTexture);
			}
			HandleWaterEffect();
			if (disablePixelLights)
			{
				QualitySettings.pixelLightCount = pixelLightCount;
			}
			switch (waterMode)
			{
			case WaterMode.Simple:
				Shader.EnableKeyword("WATER_SIMPLE");
				Shader.DisableKeyword("WATER_REFLECTIVE");
				Shader.DisableKeyword("WATER_REFRACTIVE");
				break;
			case WaterMode.Reflective:
				Shader.DisableKeyword("WATER_SIMPLE");
				Shader.EnableKeyword("WATER_REFLECTIVE");
				Shader.DisableKeyword("WATER_REFRACTIVE");
				break;
			case WaterMode.Refractive:
				Shader.DisableKeyword("WATER_SIMPLE");
				Shader.DisableKeyword("WATER_REFLECTIVE");
				Shader.EnableKeyword("WATER_REFRACTIVE");
				break;
			}
			_insideWater = false;
		}
	}

	private void CheckDepthTextureSupport(UnityEngine.Camera cam)
	{
		if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			_useDepthTexture = false;
		}
		if (_useDepthTexture)
		{
			cam.depthTextureMode |= DepthTextureMode.Depth;
			Shader.EnableKeyword("DEPTH_TEXTURE_ON");
			Shader.DisableKeyword("DEPTH_TEXTURE_OFF");
		}
		else
		{
			Shader.DisableKeyword("DEPTH_TEXTURE_ON");
			Shader.EnableKeyword("DEPTH_TEXTURE_OFF");
		}
	}

	private static void HandleWaterEffect()
	{
		bool flag = ShouldEnableWaterEffects() && MilMo_Camera.Instance.InsideWater();
		if (MilMo_Player.Instance.Silhouette != null)
		{
			if (MilMo_Player.Instance.InHub || MilMo_Player.Instance.InShop || MilMo_Player.InHome)
			{
				MilMo_Player.Instance.Silhouette.Enable(enable: false);
			}
			else
			{
				MilMo_Player.Instance.Silhouette.Enable(!flag);
			}
		}
		if ((bool)MilMo_Global.ImageEffectsHandler)
		{
			MilMo_Global.ImageEffectsHandler.EnableEffects("water", flag);
		}
	}

	private static bool ShouldEnableWaterEffects()
	{
		if (!MilMo_Player.Instance.InHub && !MilMo_Player.Instance.InShop && !MilMo_Player.Instance.EnteringShop && !MilMo_Player.InHome && MilMo_Level.CurrentLevel != null && !MilMo_WaterManager.DisableUnderWaterEffect)
		{
			return MilMo_Camera.Instance != null;
		}
		return false;
	}

	private void OnDisable()
	{
		if ((bool)_reflectionTexture)
		{
			UnityEngine.Object.DestroyImmediate(_reflectionTexture);
			_reflectionTexture = null;
		}
		if ((bool)_refractionTexture)
		{
			UnityEngine.Object.DestroyImmediate(_refractionTexture);
			_refractionTexture = null;
		}
		foreach (KeyValuePair<UnityEngine.Camera, UnityEngine.Camera> reflectionCamera in _reflectionCameras)
		{
			UnityEngine.Object.DestroyImmediate(reflectionCamera.Value.gameObject);
		}
		_reflectionCameras.Clear();
		foreach (KeyValuePair<UnityEngine.Camera, UnityEngine.Camera> refractionCamera in _refractionCameras)
		{
			UnityEngine.Object.DestroyImmediate(refractionCamera.Value.gameObject);
		}
		_refractionCameras.Clear();
	}

	private void Update()
	{
		if ((bool)_renderer)
		{
			Material sharedMaterial = _renderer.sharedMaterial;
			if ((bool)sharedMaterial)
			{
				float num = sharedMaterial.GetFloat(Shader.PropertyToID("_WaveSpeed")) * 10f;
				float @float = sharedMaterial.GetFloat(Shader.PropertyToID("_WaveScale"));
				Vector4 value = new Vector4(@float, @float, @float * 0.4f, @float * 0.45f);
				double num2 = (double)Time.timeSinceLevelLoad / 20.0;
				sharedMaterial.SetVector(value: new Vector4((float)Math.IEEERemainder((double)(num * value.x) * num2, 1.0), (float)Math.IEEERemainder((double)(num * value.y) * num2, 1.0), (float)Math.IEEERemainder((double)(num * value.z) * num2, 1.0), (float)Math.IEEERemainder((double)(num * value.w) * num2, 1.0)), nameID: Shader.PropertyToID("_WaveOffset"));
				sharedMaterial.SetVector(Shader.PropertyToID("_WaveScale4"), value);
			}
		}
	}

	private static void UpdateCameraModes(UnityEngine.Camera src, UnityEngine.Camera dest)
	{
		if (dest == null)
		{
			return;
		}
		dest.clearFlags = src.clearFlags;
		dest.backgroundColor = src.backgroundColor;
		if (src.clearFlags == CameraClearFlags.Skybox)
		{
			Skybox component = src.GetComponent<Skybox>();
			Skybox component2 = dest.GetComponent<Skybox>();
			if (!component || !component.material)
			{
				if ((bool)component2)
				{
					component2.enabled = false;
				}
			}
			else if ((bool)component2)
			{
				component2.enabled = true;
				component2.material = component.material;
			}
		}
		dest.farClipPlane = src.farClipPlane;
		dest.nearClipPlane = src.nearClipPlane;
		dest.orthographic = src.orthographic;
		dest.fieldOfView = src.fieldOfView;
		dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize;
		dest.depthTextureMode = src.depthTextureMode;
	}

	private void CreateWaterObjects(UnityEngine.Camera currentCamera, out UnityEngine.Camera reflectionCamera, out UnityEngine.Camera refractionCamera)
	{
		WaterMode waterMode = GetWaterMode();
		reflectionCamera = null;
		refractionCamera = null;
		if (waterMode >= WaterMode.Reflective)
		{
			if (!_reflectionTexture || _oldReflectionTextureSize != textureSize)
			{
				if ((bool)_reflectionTexture)
				{
					UnityEngine.Object.DestroyImmediate(_reflectionTexture);
				}
				_reflectionTexture = new RenderTexture(textureSize, textureSize, 16)
				{
					name = "__WaterReflection" + GetInstanceID(),
					isPowerOfTwo = true,
					hideFlags = HideFlags.DontSave
				};
				_oldReflectionTextureSize = textureSize;
			}
			_reflectionCameras.TryGetValue(currentCamera, out reflectionCamera);
			if (!reflectionCamera)
			{
				GameObject gameObject = new GameObject("Water Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(UnityEngine.Camera), typeof(Skybox));
				reflectionCamera = gameObject.GetComponent<UnityEngine.Camera>();
				reflectionCamera.enabled = false;
				Transform obj = reflectionCamera.transform;
				Transform transform = base.transform;
				obj.position = transform.position;
				obj.rotation = transform.rotation;
				reflectionCamera.gameObject.AddComponent<FlareLayer>();
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				_reflectionCameras[currentCamera] = reflectionCamera;
			}
		}
		if (waterMode < WaterMode.Refractive)
		{
			return;
		}
		if (!_refractionTexture || _oldRefractionTextureSize != textureSize)
		{
			if ((bool)_refractionTexture)
			{
				UnityEngine.Object.DestroyImmediate(_refractionTexture);
			}
			_refractionTexture = new RenderTexture(textureSize, textureSize, 16)
			{
				name = "__WaterRefraction" + GetInstanceID(),
				isPowerOfTwo = true,
				hideFlags = HideFlags.DontSave
			};
			_oldRefractionTextureSize = textureSize;
		}
		_refractionCameras.TryGetValue(currentCamera, out refractionCamera);
		if (!refractionCamera)
		{
			GameObject gameObject2 = new GameObject("Water Refr Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(UnityEngine.Camera), typeof(Skybox));
			refractionCamera = gameObject2.GetComponent<UnityEngine.Camera>();
			refractionCamera.enabled = false;
			Transform obj2 = refractionCamera.transform;
			Transform transform2 = base.transform;
			obj2.position = transform2.position;
			obj2.rotation = transform2.rotation;
			refractionCamera.gameObject.AddComponent<FlareLayer>();
			gameObject2.hideFlags = HideFlags.HideAndDontSave;
			_refractionCameras[currentCamera] = refractionCamera;
		}
	}

	private WaterMode GetWaterMode()
	{
		switch (Settings.QualityTier)
		{
		case Settings.QualityTierSetting.Low:
			waterMode = WaterMode.Simple;
			break;
		case Settings.QualityTierSetting.Medium:
		case Settings.QualityTierSetting.High:
			waterMode = WaterMode.Refractive;
			break;
		default:
			waterMode = WaterMode.Reflective;
			break;
		}
		if (_hardwareWaterSupport >= waterMode)
		{
			return waterMode;
		}
		return _hardwareWaterSupport;
	}

	private WaterMode FindHardwareWaterSupport()
	{
		if (!_renderer)
		{
			return WaterMode.Simple;
		}
		Material sharedMaterial = _renderer.sharedMaterial;
		if (!sharedMaterial)
		{
			return WaterMode.Simple;
		}
		string text = sharedMaterial.GetTag("WATERMODE", searchFallbacks: false);
		if (!(text == "Refractive"))
		{
			if (text == "Reflective")
			{
				return WaterMode.Reflective;
			}
			return WaterMode.Simple;
		}
		return WaterMode.Refractive;
	}

	private Vector4 CameraSpacePlane(UnityEngine.Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 point = pos + normal * clipPlaneOffset;
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 lhs = worldToCameraMatrix.MultiplyPoint(point);
		Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(rhs.x, rhs.y, rhs.z, 0f - Vector3.Dot(lhs, rhs));
	}

	private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
	{
		reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
		reflectionMat.m01 = -2f * plane[0] * plane[1];
		reflectionMat.m02 = -2f * plane[0] * plane[2];
		reflectionMat.m03 = -2f * plane[3] * plane[0];
		reflectionMat.m10 = -2f * plane[1] * plane[0];
		reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
		reflectionMat.m12 = -2f * plane[1] * plane[2];
		reflectionMat.m13 = -2f * plane[3] * plane[1];
		reflectionMat.m20 = -2f * plane[2] * plane[0];
		reflectionMat.m21 = -2f * plane[2] * plane[1];
		reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
		reflectionMat.m23 = -2f * plane[3] * plane[2];
		reflectionMat.m30 = 0f;
		reflectionMat.m31 = 0f;
		reflectionMat.m32 = 0f;
		reflectionMat.m33 = 1f;
	}
}
