using System;
using System.Collections.Generic;
using Code.Core.Camera.CameraActions;
using Code.Core.Command;
using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using Code.World;
using Core.Analytics;
using Core.GameEvent;
using Core.Input;
using UnityEngine;

namespace Code.Core.Camera;

public class MilMo_MovieCameraController : MilMo_ActionCameraController
{
	private const string THE_DEFAULT_SETTINGS_FILENAME = "MovieCameraDefaults";

	private bool _isPlaying;

	private readonly List<MilMo_TimerEvent> _queuedEvents = new List<MilMo_TimerEvent>();

	private readonly List<MilMo_CameraAction> _latestMovie = new List<MilMo_CameraAction>();

	private string _currentCutsceneName;

	private readonly InputController _inputController;

	public MilMo_MovieCameraController()
	{
		MilMo_Command.Instance.RegisterCommand("ActionCam.Set", base.Debug_Set);
		DefaultSettings();
		_inputController = InputController.Get();
	}

	public override void HookUp()
	{
		base.HookUp();
		MilMo_CameraController.CameraComponent.nearClipPlane = 0.1f;
		GameEvent.OnExitCutsceneEvent = (Action)Delegate.Combine(GameEvent.OnExitCutsceneEvent, new Action(OnExitCutscene));
		_inputController.SetCutsceneController();
		GameEvent.ShowHUDEvent.RaiseEvent(args: false);
		MilMo_UserInterface.EnabledGlobal(shouldEnable: false);
	}

	public override void Unhook()
	{
		base.Unhook();
		GameEvent.OnExitCutsceneEvent = (Action)Delegate.Remove(GameEvent.OnExitCutsceneEvent, new Action(OnExitCutscene));
		_inputController.RestorePreviousController();
		Analytics.CutsceneEnd(_currentCutsceneName);
		_currentCutsceneName = "";
		GameEvent.ShowHUDEvent.RaiseEvent(args: true);
		MilMo_UserInterface.EnabledGlobal(shouldEnable: true);
		MilMo_Utility.SetUnlockedMode();
	}

	private void OnExitCutscene()
	{
		FadeIn(0.5f);
		Analytics.CutsceneSkip(_currentCutsceneName);
		StopMovie(defaultSettings: true);
		MilMo_EventSystem.Instance.PostEvent("button_ResetCamera", null);
		MilMo_World.Instance.Camera.HookupCurrentPlayCamera();
	}

	public override void Update()
	{
		if (base.HookedUp)
		{
			MilMo_CameraController.UpdateAudioListenerPosition();
		}
	}

	public override void FixedUpdate()
	{
		if (base.HookedUp)
		{
			Mover.FixedUpdate();
		}
	}

	public void ExecuteScript(MilMo_SFFile file)
	{
		if (file != null)
		{
			if (file.IsNext("CameraSettings"))
			{
				ReadSettingsScript(file);
			}
			else if (file.IsNext("Movie"))
			{
				StopMovie(defaultSettings: false);
				ReadMovieScript(file);
				ReplayLatestMovie();
				_currentCutsceneName = file.Name;
				Analytics.CutsceneStart(_currentCutsceneName);
			}
		}
	}

	private void ReplayLatestMovie()
	{
		if (_latestMovie.Count == 0)
		{
			return;
		}
		StopMovie(defaultSettings: false);
		foreach (MilMo_CameraAction item in _latestMovie)
		{
			_queuedEvents.Add(MilMo_EventSystem.At(item.Time, item.Execute, this));
		}
		_isPlaying = true;
	}

	public void StopMovie(bool defaultSettings)
	{
		foreach (MilMo_TimerEvent queuedEvent in _queuedEvents)
		{
			MilMo_EventSystem.RemoveTimerEvent(queuedEvent);
		}
		_queuedEvents.Clear();
		_isPlaying = false;
		if (defaultSettings)
		{
			DefaultSettings();
		}
	}

	private void ReadMovieScript(MilMo_SFFile file)
	{
		_latestMovie.Clear();
		int filesToWaitFor = 1;
		MilMo_CameraActionExec milMo_CameraActionExec = new MilMo_CameraActionExec(0f, delegate
		{
			filesToWaitFor--;
			if (filesToWaitFor != 0 || !_isPlaying)
			{
				return;
			}
			foreach (MilMo_CameraAction item in _latestMovie)
			{
				_queuedEvents.Add(MilMo_EventSystem.At(item.Time, item.Execute, this));
			}
		});
		milMo_CameraActionExec.SetScript("MovieCameraDefaults");
		_latestMovie.Add(milMo_CameraActionExec);
		while (file.NextRow())
		{
			if (!file.IsNext("At"))
			{
				continue;
			}
			float @float = file.GetFloat();
			MilMo_CameraAction milMo_CameraAction;
			if (file.IsNext("GoTo"))
			{
				milMo_CameraAction = new MilMo_CameraActionGoTo(@float);
			}
			else if (file.IsNext("GoToNow"))
			{
				milMo_CameraAction = new MilMo_CameraActionGoToNow(@float);
			}
			else if (file.IsNext("Impulse"))
			{
				milMo_CameraAction = new MilMo_CameraActionImpulse(@float);
			}
			else if (file.IsNext("RandomImpulse"))
			{
				milMo_CameraAction = new MilMo_CameraActionRandomImpulse(@float);
			}
			else if (file.IsNext("RotateTo"))
			{
				milMo_CameraAction = new MilMo_CameraActionRotateTo(@float);
			}
			else if (file.IsNext("RotateToNow"))
			{
				milMo_CameraAction = new MilMo_CameraActionRotateToNow(@float);
			}
			else if (file.IsNext("LookAt"))
			{
				milMo_CameraAction = new MilMo_CameraActionLookAt(@float);
			}
			else if (file.IsNext("LookAtNow"))
			{
				milMo_CameraAction = new MilMo_CameraActionLookAtNow(@float);
			}
			else if (file.IsNext("StopLookAt"))
			{
				milMo_CameraAction = new MilMo_CameraActionStopLookAt(@float);
			}
			else if (file.IsNext("AngleImpulse"))
			{
				milMo_CameraAction = new MilMo_CameraActionAngleImpulse(@float);
			}
			else if (file.IsNext("RandomAngleImpulse"))
			{
				milMo_CameraAction = new MilMo_CameraActionRandomAngleImpulse(@float);
			}
			else if (file.IsNext("ZoomTo"))
			{
				milMo_CameraAction = new MilMo_CameraActionZoomTo(@float);
			}
			else if (file.IsNext("ZoomToNow"))
			{
				milMo_CameraAction = new MilMo_CameraActionZoomToNow(@float);
			}
			else if (file.IsNext("Shake"))
			{
				milMo_CameraAction = new MilMo_CameraActionShake(@float);
			}
			else if (file.IsNext("FadeIn"))
			{
				milMo_CameraAction = new MilMo_CameraActionFadeIn(@float);
			}
			else if (file.IsNext("FadeOut"))
			{
				milMo_CameraAction = new MilMo_CameraActionFadeOut(@float);
			}
			else if (file.IsNext("Set"))
			{
				milMo_CameraAction = new MilMo_CameraActionSet(@float);
			}
			else if (file.IsNext("Exec"))
			{
				filesToWaitFor++;
				milMo_CameraAction = new MilMo_CameraActionExec(@float, delegate
				{
					filesToWaitFor--;
					if (filesToWaitFor != 0 || !_isPlaying)
					{
						return;
					}
					foreach (MilMo_CameraAction item2 in _latestMovie)
					{
						_queuedEvents.Add(MilMo_EventSystem.At(item2.Time, item2.Execute, this));
					}
				});
			}
			else
			{
				if (!file.IsNext("EndMovie"))
				{
					Debug.LogWarning("Unknown command " + file.PeekString() + " in movie script " + file.Path);
					continue;
				}
				milMo_CameraAction = new MilMo_CameraActionEndMovie(@float);
			}
			milMo_CameraAction.Read(file);
			_latestMovie.Add(milMo_CameraAction);
		}
	}

	private void DefaultSettings()
	{
		MilMo_SimpleFormat.AsyncLoad("Content/CameraScripts/MovieCameraDefaults", delegate(MilMo_SFFile file)
		{
			if (file == null)
			{
				Debug.LogWarning("File Content/CameraScripts/MovieCameraDefaults not found.");
			}
			else
			{
				file.NextRow();
				if (file.PeekIsNext("CameraSettings"))
				{
					ReadSettingsScript(file);
				}
				else
				{
					Debug.LogWarning("Unknown camera script type '" + file.GetString() + "' in Content/CameraScripts/MovieCameraDefaults");
				}
			}
		});
	}
}
