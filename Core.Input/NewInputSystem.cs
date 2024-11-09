using Code.Core.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input;

public class NewInputSystem : IInputType
{
	private const float POINTER_ORBIT_SENSITIVITY = 0.05f;

	private const float GAMEPAD_STICK_SENSITIVITY = 0.1f;

	private const float SCROLL_SENSITIVITY = 0.01f;

	private const float D_PAD_SENSITIVITY = 0.1f;

	public bool GetKey(KeyCode keycode)
	{
		Key keyFromKeyCode = MilMo_Utility.GetKeyFromKeyCode(keycode);
		if (keyFromKeyCode != Key.OEM5)
		{
			return Keyboard.current[keyFromKeyCode].isPressed;
		}
		if (InputSwitch.IsPointerOverUI())
		{
			return false;
		}
		return keycode switch
		{
			KeyCode.Mouse0 => Mouse.current.leftButton.isPressed, 
			KeyCode.Mouse1 => Mouse.current.rightButton.isPressed, 
			_ => false, 
		};
	}

	public bool GetKeyUp(KeyCode keycode)
	{
		Key keyFromKeyCode = MilMo_Utility.GetKeyFromKeyCode(keycode);
		if (keyFromKeyCode != Key.OEM5 && Keyboard.current[keyFromKeyCode].wasReleasedThisFrame)
		{
			return true;
		}
		if (InputSwitch.IsPointerOverUI())
		{
			return false;
		}
		return keycode switch
		{
			KeyCode.Mouse0 => Mouse.current.leftButton.wasReleasedThisFrame, 
			KeyCode.Mouse1 => Mouse.current.rightButton.wasReleasedThisFrame, 
			_ => false, 
		};
	}

	public bool GetKeyDown(KeyCode keycode)
	{
		Key keyFromKeyCode = MilMo_Utility.GetKeyFromKeyCode(keycode);
		if (keyFromKeyCode != Key.OEM5 && Keyboard.current[keyFromKeyCode].wasPressedThisFrame)
		{
			return true;
		}
		if (InputSwitch.IsPointerOverUI())
		{
			return false;
		}
		return keycode switch
		{
			KeyCode.Mouse0 => Mouse.current.leftButton.wasPressedThisFrame, 
			KeyCode.Mouse1 => Mouse.current.rightButton.wasPressedThisFrame, 
			_ => false, 
		};
	}

	public float GetAxis(string axis)
	{
		switch (axis)
		{
		case "Mouse X":
			return Mouse.current.delta.x.ReadValue() * 0.05f;
		case "Mouse Y":
			return Mouse.current.delta.y.ReadValue() * 0.05f;
		case "Mouse ScrollWheel":
			return Mouse.current.scroll.y.ReadValue() * 0.01f;
		default:
			if (Gamepad.current == null)
			{
				return 0f;
			}
			return axis switch
			{
				"Horizontal" => Gamepad.current.leftStick.x.ReadValue() * 0.1f, 
				"Vertical" => Gamepad.current.leftStick.y.ReadValue() * 0.1f, 
				"Camera X" => Gamepad.current.rightStick.x.ReadValue() * 0.1f, 
				"Camera Y" => Gamepad.current.rightStick.y.ReadValue() * 0.1f, 
				"D Pad X" => Gamepad.current.dpad.x.ReadValue() * 0.1f, 
				"D Pad Y" => Gamepad.current.dpad.y.ReadValue() * 0.1f, 
				_ => 0f, 
			};
		}
	}

	public float GetAxisRaw(string axis)
	{
		switch (axis)
		{
		case "Mouse X":
			return Mouse.current.delta.x.ReadValue() * 0.05f;
		case "Mouse Y":
			return Mouse.current.delta.y.ReadValue() * 0.05f;
		case "Mouse ScrollWheel":
			return Mouse.current.scroll.y.ReadValue() * 0.01f;
		default:
			if (Gamepad.current == null)
			{
				return 0f;
			}
			return axis switch
			{
				"Horizontal" => Gamepad.current.leftStick.x.ReadValue() * 0.1f, 
				"Vertical" => Gamepad.current.leftStick.y.ReadValue() * 0.1f, 
				"Camera X" => Gamepad.current.rightStick.x.ReadValue() * 0.1f, 
				"Camera Y" => Gamepad.current.rightStick.y.ReadValue() * 0.1f, 
				"D Pad X" => Gamepad.current.dpad.x.ReadValue() * 0.1f, 
				"D Pad Y" => Gamepad.current.dpad.y.ReadValue() * 0.1f, 
				_ => 0f, 
			};
		}
	}

	public bool GetAnyKey()
	{
		if (!Keyboard.current.anyKey.isPressed)
		{
			if (!InputSwitch.IsPointerOverUI())
			{
				if (!Mouse.current.leftButton.isPressed && !Mouse.current.rightButton.isPressed)
				{
					return Mouse.current.middleButton.isPressed;
				}
				return true;
			}
			return false;
		}
		return true;
	}

	public bool GetAnyKeyDown()
	{
		if (!Keyboard.current.anyKey.wasPressedThisFrame)
		{
			if (!InputSwitch.IsPointerOverUI())
			{
				if (!Mouse.current.leftButton.wasPressedThisFrame && !Mouse.current.rightButton.wasPressedThisFrame)
				{
					return Mouse.current.middleButton.wasPressedThisFrame;
				}
				return true;
			}
			return false;
		}
		return true;
	}

	public Vector2 GetMousePosition()
	{
		return Mouse.current.position.ReadValue();
	}

	public bool GetMouseButton(int button)
	{
		return button switch
		{
			0 => Mouse.current.leftButton.isPressed, 
			1 => Mouse.current.rightButton.isPressed, 
			_ => false, 
		};
	}
}
