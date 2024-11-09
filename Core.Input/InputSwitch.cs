using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Core.Input;

public class InputSwitch : MonoBehaviour
{
	private static IInputType _inputType;

	private static IInputType _newInputType;

	private static IInputType _disabledInputType;

	private static Vector3 _lastMousePosition;

	public static bool AnyKey
	{
		get
		{
			if (Application.isFocused)
			{
				return _inputType.GetAnyKey();
			}
			return false;
		}
	}

	public static bool AnyKeyDown
	{
		get
		{
			if (Application.isFocused)
			{
				return _inputType.GetAnyKeyDown();
			}
			return false;
		}
	}

	public static Vector2 MousePosition
	{
		get
		{
			Vector2 vector = (Application.isFocused ? _inputType.GetMousePosition() : Vector2.zero);
			if (vector != Vector2.zero)
			{
				_lastMousePosition = vector;
			}
			return _lastMousePosition;
		}
	}

	public static bool IsMouseOverGameWindow
	{
		get
		{
			Vector2 mousePosition = _inputType.GetMousePosition();
			if (!(0f > mousePosition.x) && !(0f > mousePosition.y) && !((float)Screen.width < mousePosition.x))
			{
				return !((float)Screen.height < mousePosition.y);
			}
			return false;
		}
	}

	public void Awake()
	{
		_newInputType = new NewInputSystem();
		_inputType = _newInputType;
		_disabledInputType = new DisabledInputSystem();
		_lastMousePosition = default(Vector3);
	}

	public void Update()
	{
		if (Keyboard.current.f1Key.wasPressedThisFrame)
		{
			ChangeInputType((_inputType is NewInputSystem) ? _disabledInputType : _newInputType);
		}
	}

	public static bool IsDisabled()
	{
		return _inputType == _disabledInputType;
	}

	public static bool IsPointerOverUI()
	{
		return EventSystem.current.IsPointerOverGameObject();
	}

	public static void ChangeInputType(IInputType inputType)
	{
		_inputType = inputType;
	}

	public static void SetEnabled(bool enable)
	{
		ChangeInputType(enable ? _newInputType : _disabledInputType);
	}

	public static bool GetKey(KeyCode key)
	{
		if (Application.isFocused)
		{
			return _inputType.GetKey(key);
		}
		return false;
	}

	public static bool GetKeyUp(KeyCode key)
	{
		if (Application.isFocused)
		{
			return _inputType.GetKeyUp(key);
		}
		return false;
	}

	public static bool GetKeyDown(KeyCode key)
	{
		if (Application.isFocused)
		{
			return _inputType.GetKeyDown(key);
		}
		return false;
	}

	public static float GetAxis(string axis)
	{
		if (!Application.isFocused || IsPointerOverUI())
		{
			return 0f;
		}
		return _inputType.GetAxis(axis);
	}

	public static float GetAxisRaw(string axis)
	{
		if (!Application.isFocused || IsPointerOverUI())
		{
			return 0f;
		}
		return _inputType.GetAxisRaw(axis);
	}

	public static bool GetMouseButton(int button)
	{
		if (Application.isFocused && !IsPointerOverUI())
		{
			return _inputType.GetMouseButton(button);
		}
		return false;
	}
}
