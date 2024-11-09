using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.Portal;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using Core.Input;
using UnityEngine;

namespace Code.Core.Input;

public class MilMo_ControlScheme
{
	private struct EmoteBinding
	{
		public readonly string Emote;

		public readonly List<KeyCode> Keys;

		public EmoteBinding(string emote, List<KeyCode> keys)
		{
			Emote = emote;
			Keys = keys;
		}
	}

	private class EventToPost
	{
		public readonly string Event;

		private readonly List<KeyCode> _keys;

		private readonly object _parameter;

		public EventToPost(string evt, List<KeyCode> keys, object parameter)
		{
			Event = evt;
			_keys = keys;
			_parameter = parameter;
		}

		public bool IsOverriddenBy(EventToPost otherEvent)
		{
			if (otherEvent._keys.Count > _keys.Count)
			{
				return _keys.All((KeyCode key) => otherEvent._keys.Contains(key));
			}
			return false;
		}

		public void Post()
		{
			MilMo_EventSystem.Instance.PostEvent(Event, _parameter);
		}
	}

	private static readonly char[] KeyCodeSeparators = new char[1] { '+' };

	private readonly Dictionary<string, List<List<KeyCode>>> _unParameterizedBindings = new Dictionary<string, List<List<KeyCode>>>();

	private readonly Dictionary<string, string> _keyCodeStrToAction = new Dictionary<string, string>();

	private readonly List<EmoteBinding> _emoteBindings = new List<EmoteBinding>();

	private readonly List<string> _anyKeyBindings = new List<string>();

	private readonly Dictionary<string, string> _customBindings = new Dictionary<string, string>();

	public float HorizontalAxis { get; private set; }

	public float VerticalAxis { get; private set; }

	public float RotationAxis { get; private set; }

	public float ZoomAxis { get; private set; }

	public MilMo_ControlScheme(MilMo_ControlScheme baseControls, MilMo_SFFile file)
	{
		if (baseControls != null)
		{
			foreach (KeyValuePair<string, List<List<KeyCode>>> unParameterizedBinding in baseControls._unParameterizedBindings)
			{
				string key = unParameterizedBinding.Key;
				List<List<KeyCode>> value = unParameterizedBinding.Value.Select((List<KeyCode> aggregateKey) => aggregateKey.ToList()).ToList();
				_unParameterizedBindings.Add(key, value);
			}
			foreach (KeyValuePair<string, string> item in baseControls._keyCodeStrToAction)
			{
				_keyCodeStrToAction.Add(item.Key, item.Value);
			}
			foreach (KeyValuePair<string, string> customBinding in baseControls._customBindings)
			{
				_customBindings.Add(customBinding.Key, customBinding.Value);
			}
			foreach (EmoteBinding emoteBinding in baseControls._emoteBindings)
			{
				_emoteBindings.Add(emoteBinding);
			}
			foreach (string anyKeyBinding in baseControls._anyKeyBindings)
			{
				_anyKeyBindings.Add(anyKeyBinding);
			}
		}
		if (file == null)
		{
			return;
		}
		while (file.NextRow())
		{
			string @string = file.GetString();
			if (file.IsNext("Unbind"))
			{
				RemoveBindingForKeyCode(SortKeycodesInStr(@string));
			}
		}
		file.Reset();
		while (file.NextRow())
		{
			string string2 = file.GetString();
			if (file.IsNext("Unbind"))
			{
				continue;
			}
			if (string2 == "AnyKey")
			{
				_anyKeyBindings.Add(file.GetString());
				continue;
			}
			string2 = SortKeycodesInStr(string2);
			if (_keyCodeStrToAction.ContainsKey(string2))
			{
				Debug.LogWarning("Multiple bindings for key '" + string2 + "' found in " + file.Path);
				continue;
			}
			if (string2 == "ScrollWheelForward" || string2 == "ScrollWheelBackward")
			{
				string string3 = file.GetString();
				_customBindings.Add(string3, string2);
				_keyCodeStrToAction.Add(string2, string3);
				continue;
			}
			List<KeyCode> list = KeycodeStrToList(string2);
			if (list.Count == 0)
			{
				Debug.LogWarning("Invalid keycode '" + string2 + "' found in " + file.Path);
				continue;
			}
			if (file.IsNext("PlayEmote"))
			{
				string string4 = file.GetString();
				_emoteBindings.Add(new EmoteBinding(string4, list));
				_keyCodeStrToAction.Add(string2, "PlayEmote");
				continue;
			}
			string string5 = file.GetString();
			if (!_unParameterizedBindings.ContainsKey(string5))
			{
				List<List<KeyCode>> value2 = new List<List<KeyCode>> { list };
				_unParameterizedBindings.Add(string5, value2);
			}
			else
			{
				_unParameterizedBindings[string5].Add(list);
			}
			_keyCodeStrToAction.Add(string2, string5);
		}
	}

	public bool GetKey(string action)
	{
		if (!_unParameterizedBindings.ContainsKey(action))
		{
			return InputSwitch.GetKey(MilMo_Utility.GetKeyCode(action));
		}
		return _unParameterizedBindings[action].Select((List<KeyCode> keys) => keys.All(InputSwitch.GetKey)).Any((bool allDown) => allDown);
	}

	public bool GetKeyDown(string action)
	{
		if (!_unParameterizedBindings.ContainsKey(action))
		{
			return InputSwitch.GetKeyDown(MilMo_Utility.GetKeyCode(action));
		}
		return _unParameterizedBindings[action].Select((List<KeyCode> keys) => keys.All(InputSwitch.GetKeyDown)).Any((bool allDown) => allDown);
	}

	public void CheckInput(bool mouse0, bool mouse1, bool dontAllowMouse0)
	{
		if (InputSwitch.AnyKeyDown && !InputSwitch.GetKeyDown(KeyCode.Mouse1) && (!dontAllowMouse0 || !InputSwitch.GetKeyDown(KeyCode.Mouse0)))
		{
			foreach (string anyKeyBinding in _anyKeyBindings)
			{
				MilMo_EventSystem.Instance.PostEvent("button_" + anyKeyBinding, null);
			}
		}
		foreach (KeyValuePair<string, string> customBinding in _customBindings)
		{
			string key2 = customBinding.Key;
			string value = customBinding.Value;
			float axisRaw = InputSwitch.GetAxisRaw("Mouse ScrollWheel");
			if (value == "ScrollWheelBackward" && axisRaw > 0f && (Screen.fullScreen || (MilMo_Portal.Instance != null && MilMo_Portal.Instance.AllowZoomOutsideFullscreen)) && MilMo_UserInterfaceManager.MouseFocus == null)
			{
				MilMo_EventSystem.Instance.PostEvent("button_" + key2, null);
				ZoomAxis -= 1f;
			}
			else if (value == "ScrollWheelForward" && axisRaw < 0f && (Screen.fullScreen || (MilMo_Portal.Instance != null && MilMo_Portal.Instance.AllowZoomOutsideFullscreen)) && MilMo_UserInterfaceManager.MouseFocus == null)
			{
				MilMo_EventSystem.Instance.PostEvent("button_" + key2, null);
				ZoomAxis += 1f;
			}
		}
		List<EventToPost> list = new List<EventToPost>();
		List<EventToPost> list2 = new List<EventToPost>();
		foreach (KeyValuePair<string, List<List<KeyCode>>> unParameterizedBinding in _unParameterizedBindings)
		{
			string key3 = unParameterizedBinding.Key;
			using (IEnumerator<List<KeyCode>> enumerator4 = unParameterizedBinding.Value.Where((List<KeyCode> keys) => Triggered(keys, mouse0, mouse1, dontAllowMouse0)).GetEnumerator())
			{
				if (enumerator4.MoveNext())
				{
					List<KeyCode> current4 = enumerator4.Current;
					list.Add(new EventToPost("button_" + key3, current4, null));
				}
			}
			if (key3 != "Forward" && key3 != "Backward" && key3 != "Right" && key3 != "Left" && key3 != "RotateRight" && key3 != "RotateLeft" && key3 != "ZoomIn" && key3 != "ZoomOut")
			{
				continue;
			}
			foreach (List<KeyCode> item in unParameterizedBinding.Value)
			{
				if (item.All((KeyCode key) => MilMo_Input.GetKey(key, useKeyboardFocus: false)))
				{
					list2.Add(new EventToPost(key3, item, null));
				}
			}
		}
		VerticalAxis = InputSwitch.GetAxis("Vertical");
		HorizontalAxis = InputSwitch.GetAxis("Horizontal");
		list.AddRange(from em in _emoteBindings
			where Triggered(em.Keys, mouse0, mouse1, dontAllowMouse0)
			select new EventToPost("play_emote", em.Keys, em.Emote));
		foreach (EventToPost item2 in list)
		{
			bool flag = true;
			foreach (EventToPost item3 in list)
			{
				if (item2 != item3 && item2.IsOverriddenBy(item3))
				{
					flag = false;
				}
			}
			if (flag)
			{
				item2.Post();
			}
		}
		foreach (EventToPost axisUpdate in list2)
		{
			bool flag2 = true;
			foreach (EventToPost item4 in list2.Where((EventToPost possibleOverride) => axisUpdate != possibleOverride && axisUpdate.IsOverriddenBy(possibleOverride)))
			{
				_ = item4;
				flag2 = false;
			}
			if (!flag2)
			{
				continue;
			}
			switch (axisUpdate.Event)
			{
			case "Forward":
				VerticalAxis += 1f;
				continue;
			case "Backward":
				VerticalAxis -= 1f;
				continue;
			case "Right":
				HorizontalAxis += 1f;
				continue;
			case "Left":
				HorizontalAxis -= 1f;
				continue;
			case "RotateRight":
				RotationAxis += 1f;
				continue;
			case "RotateLeft":
				RotationAxis -= 1f;
				continue;
			}
			if (axisUpdate.Event == "ZoomIn" && (Screen.fullScreen || MilMo_Portal.Instance == null || MilMo_Portal.Instance.AllowZoomOutsideFullscreen))
			{
				ZoomAxis -= 0.2f;
			}
			else if (axisUpdate.Event == "ZoomOut" && (Screen.fullScreen || MilMo_Portal.Instance == null || MilMo_Portal.Instance.AllowZoomOutsideFullscreen))
			{
				ZoomAxis += 0.2f;
			}
		}
		ClampAxes();
	}

	private void ClampAxes()
	{
		VerticalAxis = Mathf.Clamp(VerticalAxis, -1f, 1f);
		HorizontalAxis = Mathf.Clamp(HorizontalAxis, -1f, 1f);
		RotationAxis = Mathf.Clamp(RotationAxis, -1f, 1f);
		ZoomAxis = Mathf.Clamp(ZoomAxis, -1f, 1f);
	}

	public void ResetAxes()
	{
		VerticalAxis = 0f;
		HorizontalAxis = 0f;
		RotationAxis = 0f;
		ZoomAxis = 0f;
	}

	private static bool Triggered(List<KeyCode> keys, bool mouse0, bool mouse1, bool dontAllowMouse0)
	{
		bool result = true;
		if (keys.Count == 1 && ((keys[0] == KeyCode.Mouse0 && !mouse0) || (keys[0] == KeyCode.Mouse1 && !mouse1) || (keys[0] != KeyCode.Mouse0 && keys[0] != KeyCode.Mouse1 && !InputSwitch.GetKeyDown(keys[0]))))
		{
			result = false;
		}
		else if (keys.Count > 1)
		{
			bool flag = true;
			bool flag2 = false;
			foreach (KeyCode key in keys)
			{
				if ((key == KeyCode.Mouse0 && dontAllowMouse0) || !InputSwitch.GetKey(key))
				{
					flag = false;
					break;
				}
				if (InputSwitch.GetKeyDown(key))
				{
					flag2 = true;
				}
			}
			result = flag && flag2;
		}
		return result;
	}

	private static bool Equals(List<KeyCode> list1, List<KeyCode> list2)
	{
		if (list1.Count != list2.Count)
		{
			return false;
		}
		return !list1.Where((KeyCode t, int i) => t != list2[i]).Any();
	}

	private static List<KeyCode> KeycodeStrToList(string keyCodeStr)
	{
		List<string> list = keyCodeStr.Split(KeyCodeSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
		list.Sort();
		List<KeyCode> list2 = new List<KeyCode>();
		foreach (string item in list)
		{
			KeyCode keyCode = MilMo_Utility.GetKeyCode(item);
			if (keyCode == KeyCode.Joystick3Button19)
			{
				Debug.LogWarning("Trying to bind to unknown keycode '" + item + "'.");
				return new List<KeyCode>();
			}
			list2.Add(keyCode);
		}
		return list2;
	}

	private static string SortKeycodesInStr(string keyCodeStr)
	{
		List<string> list = keyCodeStr.Split(KeyCodeSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
		list.Sort();
		string text = "";
		if (list.Count > 0)
		{
			text += list[0];
		}
		for (int i = 1; i < list.Count; i++)
		{
			text = text + "+" + list[i];
		}
		return text;
	}

	private void RemoveBindingForKeyCode(string keyCodeStr)
	{
		if (keyCodeStr.Equals("AnyKey"))
		{
			_anyKeyBindings.Clear();
		}
		else
		{
			if (!_keyCodeStrToAction.ContainsKey(keyCodeStr))
			{
				return;
			}
			string text = _keyCodeStrToAction[keyCodeStr];
			_keyCodeStrToAction.Remove(keyCodeStr);
			if (keyCodeStr == "ScrollWheelForward" || keyCodeStr == "ScrollWheelBackward")
			{
				_customBindings.Remove(text);
				return;
			}
			List<KeyCode> list = KeycodeStrToList(keyCodeStr);
			if (text == "PlayEmote")
			{
				for (int num = _emoteBindings.Count - 1; num >= 0; num--)
				{
					if (Equals(_emoteBindings[num].Keys, list))
					{
						_emoteBindings.RemoveAt(num);
						break;
					}
				}
				return;
			}
			for (int num2 = _unParameterizedBindings[text].Count - 1; num2 >= 0; num2--)
			{
				if (Equals(_unParameterizedBindings[text][num2], list))
				{
					_unParameterizedBindings[text].RemoveAt(num2);
					break;
				}
			}
		}
	}
}
