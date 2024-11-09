using Code.Core.Avatar;
using Code.Core.Items;
using Code.Core.Utility;
using Code.Core.Visual.PostEffects;
using UnityEngine;

namespace Code.World.CharBuilder.MakeOverStudio;

public class MilMo_AvatarIcon
{
	private enum AvatarPart
	{
		Mouth,
		Eyes,
		Hair
	}

	private static int _positionOffsetBoy;

	private static int _positionOffsetGirl;

	private static readonly Vector3 MouthOffset = new Vector3(0f, 0.9f, -0.8f);

	private static readonly Vector3 EyesOffset = new Vector3(0f, 1f, -0.9f);

	private static readonly Vector3 HairOffset = new Vector3(0f, 1.05f, -3f);

	private static readonly Vector3 WigStockAngle = new Vector3(0f, 220f, 0f);

	private static readonly Vector3 DefaultAngle = new Vector3(0f, 180f, 0f);

	private MilMo_Avatar _boy;

	private MilMo_Avatar _girl;

	private GameObject _gameObject;

	private Camera _camera;

	private Vector3 _positionOffset;

	private string _boyIdentifier;

	private string _girlIdentifier;

	private bool _isBoyActive = true;

	private bool _hairEnabled = true;

	private MilMo_ColorOverlay _overlay;

	public bool Enabled
	{
		set
		{
			if (_camera != null)
			{
				_camera.enabled = value;
			}
		}
	}

	public MilMo_AvatarIcon()
	{
		_gameObject = new GameObject("AvatarIcon" + MilMo_Utility.RandomID(), typeof(Camera), typeof(MilMo_ColorOverlay));
		_camera = _gameObject.GetComponent<Camera>();
		_camera.clearFlags = CameraClearFlags.Color;
		_camera.cullingMask = 16384;
		_camera.fieldOfView = 12f;
		_camera.depth = 10f;
		_camera.rect = new Rect(0.42f, 0.4f, 0.1f, 0.06f);
		_camera.nearClipPlane = 0.01f;
		_camera.farClipPlane = 8f;
		_camera.enabled = false;
		_overlay = _gameObject.GetComponent(typeof(MilMo_ColorOverlay)) as MilMo_ColorOverlay;
		_positionOffset = AvatarPartOffset(AvatarPart.Hair);
	}

	public void CreateBoy(string skinColor, string eyeColor, int hairColor, string mouth, string eyes, string eyebrows, MilMo_Wearable hairStyle)
	{
		if (_boy != null)
		{
			return;
		}
		_boy = new MilMo_Avatar(thumbnailMode: false);
		_boy.BodyPackManager.SetMainTextureSize(MilMo_AvatarGlobalLODSettings.LocalAvatarTextureSize);
		_boy.BodyPackManager.CompressMainTexture = false;
		_boyIdentifier = "AvatarIcon[Boy]" + _camera.rect.ToString();
		_boy.SetInitializedCallback(delegate(MilMo_Avatar avatar, string userTag)
		{
			if (userTag.Equals(_boyIdentifier))
			{
				_boy.DisableBlobShadows();
				_boy.GameObject.SetActive(_isBoyActive);
				_boy.Renderer.enabled = _isBoyActive;
				_boy.GameObject.transform.position = new Vector3(2f * (float)_positionOffsetBoy, -50f, 0f);
				_boy.GameObject.transform.Rotate(Vector3.up, 180f);
				_boy.GameObject.GetComponent<Animation>().enabled = false;
				_positionOffsetBoy++;
				UpdateCameraPosition();
				if (_hairEnabled)
				{
					ShowHair();
				}
				else
				{
					HideHair();
				}
			}
		}, _boyIdentifier);
		_boy.InitLocal(_boyIdentifier, 0, skinColor, eyeColor, hairColor, mouth, eyes, eyebrows, hairStyle, 1f);
	}

	public void CreateGirl(string skinColor, string eyeColor, int hairColor, string mouth, string eyes, string eyebrows, MilMo_Wearable hairStyle)
	{
		if (_girl != null)
		{
			return;
		}
		_girl = new MilMo_Avatar(thumbnailMode: false);
		_girl.BodyPackManager.SetMainTextureSize(MilMo_AvatarGlobalLODSettings.LocalAvatarTextureSize);
		_girl.BodyPackManager.CompressMainTexture = false;
		_girlIdentifier = "AvatarIcon[Girl]" + _camera.rect.ToString();
		_girl.SetInitializedCallback(delegate(MilMo_Avatar avatar, string userTag)
		{
			if (userTag.Equals(_girlIdentifier))
			{
				_girl.DisableBlobShadows();
				_girl.GameObject.SetActive(!_isBoyActive);
				_girl.Renderer.enabled = !_isBoyActive;
				_girl.GameObject.transform.position = new Vector3(2f * (float)_positionOffsetGirl, -54f, 0f);
				_girl.GameObject.transform.Rotate(Vector3.up, 180f);
				_girl.GameObject.GetComponent<Animation>().enabled = false;
				_positionOffsetGirl++;
				UpdateCameraPosition();
				if (_hairEnabled)
				{
					ShowHair();
				}
				else
				{
					HideHair();
				}
			}
		}, _girlIdentifier);
		_girl.InitLocal(_girlIdentifier, 1, skinColor, eyeColor, hairColor, mouth, eyes, eyebrows, hairStyle, 1f);
	}

	public void Destroy()
	{
		if (_boy != null)
		{
			_boy.Destroy();
		}
		if (_girl != null)
		{
			_girl.Destroy();
		}
		Object.Destroy(_overlay);
		_overlay = null;
		Object.Destroy(_camera);
		_camera = null;
		Object.Destroy(_gameObject);
		_gameObject = null;
	}

	public void ShowBoy()
	{
		_boy.GameObject.SetActive(value: true);
		_boy.Renderer.enabled = true;
		_girl.GameObject.SetActive(value: false);
		_girl.Renderer.enabled = false;
		_girl.GameObject.transform.eulerAngles = DefaultAngle;
		_boy.GameObject.transform.eulerAngles = DefaultAngle;
		_isBoyActive = true;
		UpdateCameraPosition();
	}

	public void ShowGirl()
	{
		_boy.GameObject.SetActive(value: false);
		_boy.Renderer.enabled = false;
		_girl.GameObject.SetActive(value: true);
		_girl.Renderer.enabled = true;
		_girl.GameObject.transform.eulerAngles = DefaultAngle;
		_boy.GameObject.transform.eulerAngles = DefaultAngle;
		_isBoyActive = false;
		UpdateCameraPosition();
	}

	public void ShowMouths()
	{
		_positionOffset = AvatarPartOffset(AvatarPart.Mouth);
		UpdateCameraPosition();
		_girl.GameObject.transform.eulerAngles = DefaultAngle;
		_boy.GameObject.transform.eulerAngles = DefaultAngle;
		_boy.Renderer.enabled = true;
		_girl.Renderer.enabled = true;
	}

	public void ShowEyes()
	{
		_positionOffset = AvatarPartOffset(AvatarPart.Eyes);
		UpdateCameraPosition();
		_girl.GameObject.transform.eulerAngles = DefaultAngle;
		_boy.GameObject.transform.eulerAngles = DefaultAngle;
		_boy.Renderer.enabled = true;
		_girl.Renderer.enabled = true;
	}

	public void ShowHairStyles()
	{
		_positionOffset = AvatarPartOffset(AvatarPart.Hair);
		UpdateCameraPosition();
		_boy.Renderer.enabled = false;
		_girl.Renderer.enabled = false;
		_girl.GameObject.transform.eulerAngles = WigStockAngle;
		_boy.GameObject.transform.eulerAngles = WigStockAngle;
	}

	public void ShowHair()
	{
		if (_boy != null)
		{
			_boy.ShowHair();
		}
		if (_girl != null)
		{
			_girl.ShowHair();
		}
		_hairEnabled = true;
	}

	public void HideHair()
	{
		if (_boy != null)
		{
			_boy.HideHair();
		}
		if (_girl != null)
		{
			_girl.HideHair();
		}
		_hairEnabled = false;
	}

	public void Idle(bool enabled)
	{
		if (_boy != null)
		{
			_boy.GameObject.GetComponent<Animation>().enabled = enabled;
			if (enabled)
			{
				_boy.PlayAnimation(_boy.IdleAnimation);
			}
		}
		if (_girl != null)
		{
			_girl.GameObject.GetComponent<Animation>().enabled = enabled;
			if (enabled)
			{
				_girl.PlayAnimation(_girl.IdleAnimation);
			}
		}
	}

	public void ChangeSkinColor(string skinColor)
	{
		_boy.ChangeSkinColor(skinColor);
		_girl.ChangeSkinColor(skinColor);
	}

	public void ChangeEyeColor(string eyeColor)
	{
		_boy.ChangeEyeColor(eyeColor);
		_girl.ChangeEyeColor(eyeColor);
	}

	public void Apply()
	{
		_boy.AsyncApply();
		_girl.AsyncApply();
	}

	public void ApplyGender(int gender)
	{
		switch (gender)
		{
		case 0:
			_boy.AsyncApply();
			break;
		case 1:
			_girl.AsyncApply();
			break;
		}
	}

	public void Highlight(Color color)
	{
		_overlay.GoToColor(color);
	}

	public void HighlightNow(Color color)
	{
		_overlay.SnapColor(color);
	}

	private void UpdateCameraPosition()
	{
		if (_boy.GameObject.activeSelf)
		{
			_camera.transform.position = _boy.Position + _positionOffset;
		}
		else if (_girl.GameObject.activeSelf)
		{
			_camera.transform.position = _girl.Position + _positionOffset;
		}
		else
		{
			Debug.LogWarning("No avatar active in avatar icon at " + _camera.rect.ToString());
		}
	}

	private static Vector3 AvatarPartOffset(AvatarPart part)
	{
		return part switch
		{
			AvatarPart.Mouth => MouthOffset, 
			AvatarPart.Eyes => EyesOffset, 
			AvatarPart.Hair => HairOffset, 
			_ => new Vector3(0f, 0f, 0f), 
		};
	}

	public void SetScreenRect(Rect rect)
	{
		if ((bool)_camera)
		{
			rect.y = (float)Screen.height - rect.height - rect.y;
			rect.x /= Screen.width;
			rect.y /= Screen.height;
			rect.width /= Screen.width;
			rect.height /= Screen.height;
			_camera.rect = rect;
		}
	}
}
