using UnityEngine;

namespace Core.Input;

public interface IInputType
{
	bool GetKey(KeyCode key);

	bool GetKeyUp(KeyCode key);

	bool GetKeyDown(KeyCode key);

	float GetAxis(string axis);

	float GetAxisRaw(string axis);

	bool GetAnyKey();

	bool GetAnyKeyDown();

	Vector2 GetMousePosition();

	bool GetMouseButton(int button);
}
