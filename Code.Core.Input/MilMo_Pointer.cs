using System;
using Code.Core.Utility;
using Code.World;
using Code.World.Player;
using Core.Input;
using UnityEngine;

namespace Code.Core.Input;

public class MilMo_Pointer : MonoBehaviour
{
	private const float DOUBLE_CLICK_SPEED = 0.3f;

	private const float DOUBLE_CLICK_MAX_MOVE_SQUARED = 25f;

	public static MilMo_PointerIcon PointerIcon;

	public static bool GamepadPointer;

	public static Vector2 PointerSens;

	public static bool LeftDoubleClick;

	public static bool LeftButton;

	public static bool RightClick;

	public static bool RightButton;

	public static bool MiddleButton;

	public static float ScrollDelta;

	public static Vector2 Move;

	public static Vector2 Position;

	private static float _lastLeftClickTime;

	private static Vector2 _lastLeftClickPosition;

	public static bool GamepadClick { get; set; }

	public static bool LeftClick { get; set; }

	private void Start()
	{
		PointerSens.x = 10f;
		PointerSens.y = 10f;
		PointerIcon = base.gameObject.AddComponent<MilMo_PointerIcon>();
	}

	private void FixedUpdate()
	{
		try
		{
			if (Cursor.lockState == CursorLockMode.Confined || Cursor.lockState == CursorLockMode.None)
			{
				MilMo_Utility.SetUnlockedMode();
			}
			float axis = InputSwitch.GetAxis("Mouse X");
			float axis2 = InputSwitch.GetAxis("Mouse Y");
			if ((double)Math.Abs(axis) > 0.0 || (double)Math.Abs(axis2) > 0.0)
			{
				Move.x = axis;
				Move.y = axis2;
				Position = InputSwitch.MousePosition;
				Position.y = (float)Screen.height - InputSwitch.MousePosition.y;
				PointerIcon.SetEnabled(e: false);
			}
			if (!MilMo_World.Instance || MilMo_World.Instance.PlayerController.Type == MilMo_PlayerControllerBase.ControllerType.InGUIApp)
			{
				float axis3 = InputSwitch.GetAxis("Horizontal");
				float axis4 = InputSwitch.GetAxis("Vertical");
				if ((double)Math.Abs(axis3) > 0.0 || (double)Math.Abs(axis4) > 0.0)
				{
					Move.x = axis3;
					Move.y = axis4;
					Position.x += axis3 * PointerSens.x;
					Position.y -= axis4 * PointerSens.y;
					PointerIcon.SetEnabled(e: true);
					if (Position.x < 0f)
					{
						Position.x = 0f;
					}
					if (Position.x > (float)Screen.width)
					{
						Position.x = Screen.width;
					}
					if (Position.y < 0f)
					{
						Position.y = 0f;
					}
					if (Position.y > (float)Screen.height)
					{
						Position.y = Screen.height;
					}
				}
			}
			if (Position.x < 0f || Position.x > (float)Screen.width || Position.y < 0f || Position.y > (float)Screen.height)
			{
				LeftButton = false;
				LeftClick = false;
				RightButton = false;
				RightClick = false;
				MiddleButton = false;
				LeftDoubleClick = false;
				return;
			}
			ScrollDelta = InputSwitch.GetAxis("Mouse ScrollWheel");
			if (InputSwitch.GetMouseButton(0) || GamepadClick)
			{
				LeftButton = true;
				LeftClick = false;
				GamepadClick = false;
			}
			else
			{
				if (!LeftButton)
				{
					LeftClick = false;
					LeftDoubleClick = false;
				}
				else if (Time.time - _lastLeftClickTime <= 0.3f && (Position - _lastLeftClickPosition).sqrMagnitude <= 25f)
				{
					LeftDoubleClick = true;
				}
				else
				{
					LeftClick = true;
					_lastLeftClickTime = Time.time;
					_lastLeftClickPosition = Position;
				}
				LeftButton = false;
			}
			if (InputSwitch.GetMouseButton(1))
			{
				RightButton = true;
				RightClick = false;
			}
			else
			{
				RightClick = RightButton;
				RightButton = false;
			}
			MiddleButton = InputSwitch.GetMouseButton(2);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}
}
