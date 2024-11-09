using UnityEngine;

namespace Core.Input;

public class DisabledInputSystem : IInputType
{
	public bool GetKey(KeyCode key)
	{
		return false;
	}

	public bool GetKeyUp(KeyCode key)
	{
		return false;
	}

	public bool GetKeyDown(KeyCode key)
	{
		return false;
	}

	public float GetAxis(string axis)
	{
		return 0f;
	}

	public float GetAxisRaw(string axis)
	{
		return 0f;
	}

	public bool GetAnyKey()
	{
		return false;
	}

	public bool GetAnyKeyDown()
	{
		return false;
	}

	public Vector2 GetMousePosition()
	{
		return Vector2.zero;
	}

	public bool GetMouseButton(int button)
	{
		return false;
	}
}
