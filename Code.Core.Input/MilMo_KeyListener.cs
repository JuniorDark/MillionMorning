using Code.Core.EventSystem;
using UnityEngine;

namespace Code.Core.Input;

public class MilMo_KeyListener
{
	public delegate bool EnabledTest();

	private readonly EnabledTest _enabled;

	public KeyCode Key { get; }

	public bool Enabled
	{
		get
		{
			if (_enabled != null)
			{
				return _enabled();
			}
			return true;
		}
	}

	public MilMo_EventSystem.MilMo_EventCallback Callback { get; }

	public MilMo_KeyListener(KeyCode key, MilMo_EventSystem.MilMo_EventCallback callback, EnabledTest enabledTest)
	{
		Key = key;
		Callback = callback;
		_enabled = enabledTest;
	}
}
