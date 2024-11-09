using Cinemachine;
using Code.Core.Avatar;
using Code.World.CharBuilder;
using UnityEngine;

namespace Core.Avatar;

public class AvatarEditorCamera : MonoBehaviour
{
	[SerializeField]
	private Transform targetContainer;

	[SerializeField]
	private CinemachineVirtualCamera startCamera;

	[SerializeField]
	private CinemachineVirtualCamera bodyCamera;

	[SerializeField]
	private CinemachineVirtualCamera clothesCamera;

	[SerializeField]
	private CinemachineVirtualCamera shoesCamera;

	[SerializeField]
	private CinemachineVirtualCamera faceCamera;

	[SerializeField]
	private CinemachineVirtualCamera hairCamera;

	private AvatarEditor _avatarEditor;

	private MilMo_Avatar _avatar;

	public void Start()
	{
		GetAvatarEditor();
		StartListeners();
		ResetCameras();
	}

	private void OnDestroy()
	{
		StopListeners();
	}

	private void GetAvatarEditor()
	{
		_avatarEditor = Object.FindObjectOfType<AvatarEditor>();
		if (_avatarEditor == null)
		{
			Debug.LogError(base.name + ": Unable to find AvatarEditor");
		}
	}

	private void ResetCameras()
	{
		StopLookAtStuff();
		startCamera.gameObject.SetActive(value: false);
		startCamera.gameObject.SetActive(value: true);
	}

	private void StartListeners()
	{
		if ((bool)_avatarEditor)
		{
			_avatarEditor.OnGenderChanged += UpdateCurrentAvatar;
			_avatarEditor.OnAvatarApply += UpdateTargetContainerHeight;
			_avatarEditor.OnInitialized += LookAtBody;
		}
	}

	private void StopListeners()
	{
		if ((bool)_avatarEditor)
		{
			_avatarEditor.OnGenderChanged -= UpdateCurrentAvatar;
			_avatarEditor.OnAvatarApply -= UpdateTargetContainerHeight;
			_avatarEditor.OnInitialized -= LookAtBody;
		}
	}

	private void UpdateCurrentAvatar()
	{
		_avatar = _avatarEditor.CurrentAvatarHandler.GetAvatar();
		UpdateTargetContainerHeight();
	}

	private void UpdateTargetContainerHeight()
	{
		if (_avatar != null)
		{
			targetContainer.localScale = new Vector3(1f, _avatar.Height, 1f);
		}
	}

	private void StopLookAtStuff()
	{
		bodyCamera.gameObject.SetActive(value: false);
		clothesCamera.gameObject.SetActive(value: false);
		shoesCamera.gameObject.SetActive(value: false);
		faceCamera.gameObject.SetActive(value: false);
		hairCamera.gameObject.SetActive(value: false);
	}

	public void LookAtBody()
	{
		StopLookAtStuff();
		bodyCamera.gameObject.SetActive(value: true);
	}

	public void LookAtClothes()
	{
		StopLookAtStuff();
		clothesCamera.gameObject.SetActive(value: true);
	}

	public void LookAtShoes()
	{
		StopLookAtStuff();
		shoesCamera.gameObject.SetActive(value: true);
	}

	public void LookAtFace()
	{
		StopLookAtStuff();
		faceCamera.gameObject.SetActive(value: true);
	}

	public void LookAtHair()
	{
		StopLookAtStuff();
		hairCamera.gameObject.SetActive(value: true);
	}
}
