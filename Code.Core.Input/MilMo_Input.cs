using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Config;
using Code.Core.EventSystem;
using Code.Core.ResourceSystem;
using Core.Input;
using Core.Settings;
using UnityEngine;

namespace Code.Core.Input;

public class MilMo_Input : MonoBehaviour
{
	[Serializable]
	public enum ControlMode
	{
		Platform,
		PlatformFurnishing,
		Mmorpg,
		MmorpgFurnishing,
		NewMmorpg
	}

	public delegate bool GUIMouseFocusCallback();

	public delegate bool KeyboardFocusCallback();

	public delegate bool ClickObjectCallback();

	private struct KeyListenerStack
	{
		public readonly KeyCode Key;

		public readonly List<MilMo_KeyListener> Listeners;

		public KeyListenerStack(KeyCode keyCode)
		{
			Key = keyCode;
			Listeners = new List<MilMo_KeyListener>();
		}
	}

	private static bool _devMode;

	private static MilMo_ControlScheme _basicControls;

	private static MilMo_ControlScheme _platformerControls;

	private static MilMo_ControlScheme _mmorpgControls;

	private static MilMo_ControlScheme _newMmoControls;

	private static MilMo_ControlScheme _mmorpgControlsFurnishing;

	private static MilMo_ControlScheme _currentControls;

	private const float MOUSE0_UP_TIMEOUT = 0.5f;

	private const float MOUSE1_UP_TIMEOUT = 0.5f;

	private static readonly List<KeyListenerStack> StackedKeyListeners = new List<KeyListenerStack>();

	private static float _testClickObjectTime;

	private static bool _dontAllowMouse0;

	private static float _lastMouse0Down;

	private static float _lastMouse1Down;

	private bool _leftDPadClicked;

	private bool _rightDPadClicked;

	private bool _upDPadClicked;

	private bool _downDPadClicked;

	public static float HorizontalAxis => _currentControls.HorizontalAxis;

	public static float VerticalAxis => _currentControls.VerticalAxis;

	public static float RotationAxis => _currentControls.RotationAxis;

	public static float ZoomAxis => _currentControls.ZoomAxis;

	public static bool DontAllowMouse0 { get; set; }

	public static GUIMouseFocusCallback CurrentGUIMouseFocusCallback { get; set; }

	public static KeyboardFocusCallback CurrentKeyboardFocusCallback { get; set; }

	public static ClickObjectCallback CurrentClickObjectCallback { get; set; }

	public static ControlMode Mode { get; private set; }

	public static ControlMode DefaultMode { get; private set; }

	public static void AddKeyListener(MilMo_KeyListener listener, bool addToBottom = false)
	{
		foreach (KeyListenerStack stackedKeyListener in StackedKeyListeners)
		{
			if (stackedKeyListener.Key == listener.Key)
			{
				if (addToBottom)
				{
					stackedKeyListener.Listeners.Insert(0, listener);
				}
				else
				{
					stackedKeyListener.Listeners.Add(listener);
				}
				if (stackedKeyListener.Key == KeyCode.Return && _devMode)
				{
					Debug.Log("AddKeyListener::Callback: " + listener.Callback.Method.DeclaringType?.ToString() + "::" + listener.Callback.Method?.ToString() + ", Enabled: " + listener.Enabled);
				}
				return;
			}
		}
		KeyListenerStack item = new KeyListenerStack(listener.Key);
		item.Listeners.Add(listener);
		StackedKeyListeners.Add(item);
	}

	public static void RemoveKeyListener(MilMo_KeyListener listener)
	{
		RemoveKeyListener(listener, removeAll: true);
	}

	private static void RemoveKeyListener(MilMo_KeyListener listener, bool removeAll)
	{
		using IEnumerator<KeyListenerStack> enumerator = StackedKeyListeners.Where((KeyListenerStack stack) => stack.Key == listener.Key).GetEnumerator();
		if (!enumerator.MoveNext())
		{
			return;
		}
		KeyListenerStack current = enumerator.Current;
		if (removeAll)
		{
			while (current.Listeners.Remove(listener))
			{
			}
		}
		else
		{
			int num = current.Listeners.LastIndexOf(listener);
			if (num >= 0 && num < current.Listeners.Count)
			{
				current.Listeners.RemoveAt(num);
			}
		}
		if (current.Listeners.Count == 0)
		{
			StackedKeyListeners.Remove(current);
		}
		if (current.Key == KeyCode.Return && _devMode)
		{
			Debug.Log("RemoveKeyListener::Callback: " + listener.Callback.Method.DeclaringType?.ToString() + "::" + listener.Callback.Method?.ToString() + ", Enabled: " + listener.Enabled);
		}
	}

	private static void Initialize()
	{
		_devMode = MilMo_Config.Instance.IsTrue("Debug.Input", defaultValue: false);
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadLocal("Controls") ?? MilMo_SimpleFormat.LoadLocal("Default/Controls");
		if (milMo_SFFile == null)
		{
			throw new ApplicationException("Base controls file not found!");
		}
		_basicControls = new MilMo_ControlScheme(null, milMo_SFFile);
		MilMo_SFFile milMo_SFFile2 = MilMo_SimpleFormat.LoadLocal("ControlsPlatform") ?? MilMo_SimpleFormat.LoadLocal("Default/ControlsPlatform");
		if (milMo_SFFile2 == null)
		{
			throw new ApplicationException("ControlsPlatform file not found!");
		}
		_platformerControls = new MilMo_ControlScheme(_basicControls, milMo_SFFile2);
		MilMo_SFFile milMo_SFFile3 = MilMo_SimpleFormat.LoadLocal("ControlsMMORPG") ?? MilMo_SimpleFormat.LoadLocal("Default/ControlsMMORPG");
		if (milMo_SFFile3 == null)
		{
			throw new ApplicationException("ControlsMMORPG file not found!");
		}
		_mmorpgControls = new MilMo_ControlScheme(_basicControls, milMo_SFFile3);
		_newMmoControls = new MilMo_ControlScheme(_basicControls, milMo_SFFile3);
		MilMo_SFFile file = MilMo_SimpleFormat.LoadLocal("ControlsMMORPGModFurnishing") ?? MilMo_SimpleFormat.LoadLocal("Default/ControlsMMORPGModFurnishing");
		if (milMo_SFFile3 == null)
		{
			throw new ApplicationException("ControlsMMORPGModFurnishing file not found!");
		}
		_mmorpgControlsFurnishing = new MilMo_ControlScheme(_mmorpgControls, file);
		if (!SetDefaultControlMode(Settings.ControlMode))
		{
			throw new ApplicationException("Unknown control scheme " + Settings.ControlMode);
		}
	}

	public static bool SetDefaultControlMode(ControlModeSetting newDefault)
	{
		switch (newDefault)
		{
		case ControlModeSetting.Platform:
			DefaultMode = ControlMode.Platform;
			break;
		case ControlModeSetting.MMORPG:
			DefaultMode = ControlMode.Mmorpg;
			break;
		case ControlModeSetting.NewMMORPG:
			DefaultMode = ControlMode.NewMmorpg;
			break;
		default:
			return false;
		}
		ControlMode mode = Mode;
		bool num = mode == ControlMode.MmorpgFurnishing || mode == ControlMode.PlatformFurnishing;
		SetControlMode(DefaultMode);
		MilMo_EventSystem.Instance.PostEvent("button_ResetCamera", null);
		if (num)
		{
			ActivateFurnishingModeControlsModifier();
		}
		return true;
	}

	public static void SetControlMode(ControlMode newMode)
	{
		switch (newMode)
		{
		case ControlMode.Mmorpg:
			_currentControls = _mmorpgControls;
			break;
		case ControlMode.Platform:
			_currentControls = _platformerControls;
			break;
		case ControlMode.NewMmorpg:
			_currentControls = _newMmoControls;
			break;
		case ControlMode.MmorpgFurnishing:
			_currentControls = _mmorpgControlsFurnishing;
			break;
		case ControlMode.PlatformFurnishing:
			_currentControls = _platformerControls;
			break;
		}
		Mode = newMode;
	}

	public static void ActivateFurnishingModeControlsModifier()
	{
		switch (Mode)
		{
		case ControlMode.Mmorpg:
			SetControlMode(ControlMode.MmorpgFurnishing);
			break;
		case ControlMode.Platform:
			SetControlMode(ControlMode.PlatformFurnishing);
			break;
		case ControlMode.NewMmorpg:
			SetControlMode(ControlMode.MmorpgFurnishing);
			break;
		case ControlMode.PlatformFurnishing:
		case ControlMode.MmorpgFurnishing:
			break;
		}
	}

	public static void ResetToDefaultControlScheme()
	{
		SetControlMode(DefaultMode);
	}

	private void FixedUpdate()
	{
		try
		{
			_dontAllowMouse0 = CurrentGUIMouseFocusCallback != null && CurrentGUIMouseFocusCallback();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	private void Awake()
	{
		Initialize();
	}

	private void Update()
	{
		try
		{
			if (InputSwitch.AnyKey)
			{
				MilMo_EventSystem.Instance.PostEvent("reset_afk_timer", null);
			}
			_currentControls.ResetAxes();
			if (CurrentClickObjectCallback != null && CurrentClickObjectCallback())
			{
				_testClickObjectTime = Time.time;
			}
			_dontAllowMouse0 = _dontAllowMouse0 || Time.time - _testClickObjectTime < 0.5f || DontAllowMouse0;
			TestStackedKeyListeners();
			TestDPad();
			if (CurrentKeyboardFocusCallback != null && CurrentKeyboardFocusCallback())
			{
				return;
			}
			bool mouse = false;
			bool mouse2 = false;
			Vector2 mousePosition = InputSwitch.MousePosition;
			if (mousePosition.x >= 0f && mousePosition.y >= 0f && mousePosition.x < (float)Screen.width && mousePosition.y < (float)Screen.height)
			{
				if (!_dontAllowMouse0)
				{
					if (InputSwitch.GetKeyDown(KeyCode.Mouse0))
					{
						_lastMouse0Down = Time.time;
					}
					else if (InputSwitch.GetKeyUp(KeyCode.Mouse0) && Time.time - _lastMouse0Down < 0.5f)
					{
						mouse = true;
					}
				}
				if (InputSwitch.GetKeyDown(KeyCode.Mouse1))
				{
					_lastMouse1Down = Time.time;
				}
				else if (InputSwitch.GetKeyUp(KeyCode.Mouse1) && Time.time - _lastMouse1Down < 0.5f)
				{
					mouse2 = true;
				}
			}
			_currentControls.CheckInput(mouse, mouse2, _dontAllowMouse0);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	public static bool GetKey(string action)
	{
		if ((CurrentKeyboardFocusCallback != null && CurrentKeyboardFocusCallback()) || string.IsNullOrEmpty(action))
		{
			return false;
		}
		return _currentControls.GetKey(action);
	}

	public static bool GetKeyDown(string action)
	{
		if ((CurrentKeyboardFocusCallback != null && CurrentKeyboardFocusCallback()) || string.IsNullOrEmpty(action))
		{
			return false;
		}
		return _currentControls.GetKeyDown(action);
	}

	public static bool GetKey(KeyCode key, bool useKeyboardFocus, bool useMouseFocus = false)
	{
		if (CurrentKeyboardFocusCallback != null && CurrentKeyboardFocusCallback() && useKeyboardFocus)
		{
			return false;
		}
		if (CurrentGUIMouseFocusCallback != null && CurrentGUIMouseFocusCallback() && useMouseFocus)
		{
			return false;
		}
		return InputSwitch.GetKey(key);
	}

	public static bool GetKeyUp(KeyCode key, bool useKeyboardFocus, bool useMouseFocus = false)
	{
		if (CurrentKeyboardFocusCallback != null && CurrentKeyboardFocusCallback() && useKeyboardFocus)
		{
			return false;
		}
		if (CurrentGUIMouseFocusCallback != null && CurrentGUIMouseFocusCallback() && useMouseFocus)
		{
			return false;
		}
		return InputSwitch.GetKeyUp(key);
	}

	public static bool GetKeyDown(KeyCode key, bool useKeyboardFocus, bool useMouseFocus = false)
	{
		if (CurrentKeyboardFocusCallback != null && CurrentKeyboardFocusCallback() && useKeyboardFocus)
		{
			return false;
		}
		if (CurrentGUIMouseFocusCallback != null && CurrentGUIMouseFocusCallback() && useMouseFocus)
		{
			return false;
		}
		return InputSwitch.GetKeyDown(key);
	}

	private static void TestStackedKeyListeners()
	{
		if (StackedKeyListeners.Count == 0)
		{
			return;
		}
		List<MilMo_KeyListener> list = new List<MilMo_KeyListener>();
		foreach (KeyListenerStack item in StackedKeyListeners.Where((KeyListenerStack stack) => InputSwitch.GetKeyDown(stack.Key)))
		{
			for (int num = item.Listeners.Count - 1; num >= 0; num--)
			{
				if (item.Listeners[num].Enabled)
				{
					list.Add(item.Listeners[num]);
					break;
				}
			}
		}
		foreach (MilMo_KeyListener item2 in list)
		{
			item2.Callback?.Invoke(item2);
			RemoveKeyListener(item2, removeAll: false);
		}
	}

	private void TestDPad()
	{
		float axis = InputSwitch.GetAxis("D Pad X");
		float axis2 = InputSwitch.GetAxis("D Pad Y");
		if (axis < 0f)
		{
			if (!_leftDPadClicked)
			{
				_leftDPadClicked = true;
				MilMo_EventSystem.Instance.PostEvent("button_Left", new List<KeyCode> { KeyCode.LeftArrow });
			}
		}
		else
		{
			_leftDPadClicked = false;
		}
		if (axis > 0f)
		{
			if (!_rightDPadClicked)
			{
				_rightDPadClicked = true;
				MilMo_EventSystem.Instance.PostEvent("button_Right", new List<KeyCode> { KeyCode.RightArrow });
			}
		}
		else
		{
			_rightDPadClicked = false;
		}
		if (axis2 > 0f)
		{
			if (!_upDPadClicked)
			{
				_upDPadClicked = true;
				MilMo_EventSystem.Instance.PostEvent("button_Up", new List<KeyCode> { KeyCode.UpArrow });
			}
		}
		else
		{
			_upDPadClicked = false;
		}
		if (axis2 < 0f)
		{
			if (!_downDPadClicked)
			{
				_downDPadClicked = true;
				MilMo_EventSystem.Instance.PostEvent("button_Down", new List<KeyCode> { KeyCode.DownArrow });
			}
		}
		else
		{
			_downDPadClicked = false;
		}
	}
}
