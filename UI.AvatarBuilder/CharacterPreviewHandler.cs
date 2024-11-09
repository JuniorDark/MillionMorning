using System;
using Code.Core.Avatar;
using Code.World.CharBuilder;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI.AvatarBuilder;

public class CharacterPreviewHandler : MonoBehaviour
{
	[SerializeField]
	private Transform container;

	private PlayerInput _playerInput;

	[SerializeField]
	public float speed;

	private AvatarEditor _avatarEditor;

	private InputAction _click;

	private InputAction _drag;

	private bool _block;

	private bool _wasBlocked;

	private void Start()
	{
		Init();
	}

	private void Update()
	{
		UpdateIsPointerOverUI();
	}

	private void OnDestroy()
	{
		StopListeners();
	}

	private void Init()
	{
		_avatarEditor = UnityEngine.Object.FindObjectOfType<AvatarEditor>(includeInactive: true);
		if (_avatarEditor == null)
		{
			Debug.LogError(base.name + ": Unable to find AvatarEditor");
			return;
		}
		_playerInput = UnityEngine.Object.FindObjectOfType<PlayerInput>();
		if (_playerInput == null)
		{
			Debug.LogError(base.name + ": Unable to find PlayerInput");
			return;
		}
		SetupPlayerInput();
		StartListeners();
	}

	private void SetupPlayerInput()
	{
		InputActionMap currentActionMap = _playerInput.currentActionMap;
		try
		{
			_click = currentActionMap.FindAction("Click", throwIfNotFound: true);
			_drag = currentActionMap.FindAction("Drag", throwIfNotFound: true);
			_click.Enable();
			_drag.Enable();
		}
		catch (Exception ex)
		{
			Debug.LogError(base.name + ": Unable to setup PlayerInput");
			Debug.LogError(ex.Message);
		}
	}

	private void StartListeners()
	{
		if (!(_avatarEditor == null) && _click != null)
		{
			_avatarEditor.OnInitialized += MoveAvatars;
			_click.started += OnClickStarted;
			_click.canceled += OnClickCancelled;
		}
	}

	private void StopListeners()
	{
		if (!(_avatarEditor == null) && _click != null)
		{
			_avatarEditor.OnInitialized -= MoveAvatars;
			_click.started -= OnClickStarted;
			_click.canceled -= OnClickCancelled;
		}
	}

	private Mouse GetCurrentMouse()
	{
		return Mouse.current;
	}

	private void UpdateIsPointerOverUI()
	{
		_block = EventSystem.current.IsPointerOverGameObject();
	}

	private void OnClickStarted(InputAction.CallbackContext obj)
	{
		if (_block)
		{
			_wasBlocked = true;
			return;
		}
		Cursor.lockState = CursorLockMode.Confined;
		_drag.performed += OnDragPerformed;
	}

	private void OnClickCancelled(InputAction.CallbackContext obj)
	{
		if (_wasBlocked)
		{
			_wasBlocked = false;
			return;
		}
		Cursor.lockState = CursorLockMode.None;
		_drag.performed -= OnDragPerformed;
	}

	private void OnDragPerformed(InputAction.CallbackContext obj)
	{
		float x = GetCurrentMouse().delta.ReadValue().x;
		Vector3 eulers = new Vector3(0f, 0f - x, 0f).normalized * speed;
		container.Rotate(eulers);
	}

	private void MoveAvatars()
	{
		AvatarHandler[] avatarHandlers = _avatarEditor.GetAvatarHandlers();
		if (avatarHandlers.Length == 0)
		{
			Debug.LogError(base.name + ": Unable to find AvatarHandlers");
			return;
		}
		AvatarHandler[] array = avatarHandlers;
		foreach (AvatarHandler avatarHandler in array)
		{
			SetContainer(avatarHandler.GetAvatar());
		}
	}

	private void SetContainer(MilMo_Avatar obj)
	{
		obj.GameObject.transform.parent = container;
	}
}
