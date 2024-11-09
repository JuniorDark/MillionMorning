using System;
using Code.Core.EventSystem;
using Code.World.Level;
using Core;

namespace Code.World.Tutorial;

public sealed class MilMo_TutorialTrigger
{
	private readonly string _event;

	private readonly string _object;

	private readonly string _world;

	private readonly string _level;

	private readonly int _targetActivationCount;

	private int _currentActivationCount;

	private readonly bool _isAreaTrigger;

	private MilMo_GenericReaction _listener;

	private MilMo_EventSystem.MilMo_Callback _triggerCallback;

	public MilMo_TutorialTrigger(string evt, string obj, int activations, string world, string level)
	{
		_event = evt;
		_object = obj;
		_level = level;
		_world = world;
		_targetActivationCount = activations;
		_isAreaTrigger = evt.Equals("Area");
	}

	public void StartListening(MilMo_EventSystem.MilMo_Callback triggerCallback)
	{
		_triggerCallback = triggerCallback;
		_listener = MilMo_EventSystem.Listen("tutorial_" + _event, delegate(object o)
		{
			if (IsCorrectWorldAndLevel())
			{
				if (_isAreaTrigger)
				{
					if (o is MilMo_TutorialArea milMo_TutorialArea && milMo_TutorialArea.FullName.Equals(_world + ":" + _level + ":" + _object))
					{
						_currentActivationCount++;
						if (_currentActivationCount >= _targetActivationCount)
						{
							_triggerCallback();
						}
					}
				}
				else
				{
					string value = o as string;
					if (string.IsNullOrEmpty(_object) || _object.Equals("Any", StringComparison.InvariantCultureIgnoreCase) || _object.Equals(value, StringComparison.InvariantCultureIgnoreCase))
					{
						_currentActivationCount++;
						if (_currentActivationCount >= _targetActivationCount)
						{
							_triggerCallback();
						}
					}
				}
			}
		});
		if (_isAreaTrigger)
		{
			Singleton<MilMo_TutorialManager>.Instance.RegisterAreaListener(_world + ":" + _level + ":" + _object);
		}
		_listener.Repeating = true;
	}

	public void StopListening()
	{
		MilMo_EventSystem.RemoveReaction(_listener);
		_listener = null;
		if (_isAreaTrigger)
		{
			Singleton<MilMo_TutorialManager>.Instance.UnregisterAreaListener(_world + ":" + _level + ":" + _object);
		}
	}

	private bool IsCorrectWorldAndLevel()
	{
		if (string.IsNullOrEmpty(_level) && string.IsNullOrEmpty(_world))
		{
			return true;
		}
		MilMo_Level currentLevel = MilMo_Level.CurrentLevel;
		if (currentLevel == null)
		{
			return false;
		}
		if (currentLevel.World.Equals(_world))
		{
			if (!string.IsNullOrEmpty(_level))
			{
				return currentLevel.Name.Equals(_level);
			}
			return true;
		}
		return false;
	}
}
