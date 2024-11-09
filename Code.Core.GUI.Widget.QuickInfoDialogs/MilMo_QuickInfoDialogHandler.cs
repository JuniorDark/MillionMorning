using System.Collections.Generic;
using Code.Core.GUI.Widget.SimpleWindow;
using Code.World.GUI;
using Code.World.Player;
using UnityEngine;

namespace Code.Core.GUI.Widget.QuickInfoDialogs;

public sealed class MilMo_QuickInfoDialogHandler
{
	private static MilMo_QuickInfoDialogHandler _instance;

	private MilMo_QuickInfoDialog _currentDialog;

	private readonly Queue<MilMo_QuickInfoDialog> _quickInfoDialogQueue;

	private bool _isPaused;

	private readonly MilMo_Window _window;

	public static bool IsCreated => _instance != null;

	private MilMo_Player Player => MilMo_Player.Instance;

	public static MilMo_QuickInfoDialogHandler GetInstance()
	{
		return _instance ?? (_instance = new MilMo_QuickInfoDialogHandler());
	}

	private MilMo_QuickInfoDialogHandler()
	{
		_quickInfoDialogQueue = new Queue<MilMo_QuickInfoDialog>();
		_window = new MilMo_Window(MilMo_GlobalUI.GetSystemUI);
		_window.SetPosition(Screen.width, Screen.height);
		_window.IsInvisible = false;
		_window.Draggable = false;
		_window.TargetPos = new Vector2(Screen.width, (float)Screen.height - (float)Screen.height * 0.12f);
		_window.TargetScale = new Vector2(0f, 0f);
		_window.SpawnPos = new Vector2(Screen.width, (float)Screen.height - (float)Screen.height * 0.12f);
		_window.SpawnScale = new Vector2(0f, 0f);
		_window.FixedRes = true;
		_window.ExitScale = new Vector2(0f, 0f);
		_window.AllowPointerFocus = true;
		_window.Open();
		_window.SetAlpha(0f);
		_window.Enabled = false;
		_window.UI.AddChild(_window);
	}

	internal void AddQuickInfoDialogToQueue(MilMo_QuickInfoDialog dialog)
	{
		_quickInfoDialogQueue.Enqueue(dialog);
	}

	public void Update()
	{
		if (Player == null || Player.InDialogue || _isPaused)
		{
			return;
		}
		if (_currentDialog == null && _quickInfoDialogQueue.Count > 0)
		{
			_currentDialog = _quickInfoDialogQueue.Dequeue();
			_window.Enabled = true;
			_window.AddChild(_currentDialog);
			_currentDialog.QueuedOpen();
		}
		else if (_currentDialog != null)
		{
			if (!_currentDialog.IsActive)
			{
				_window.Enabled = false;
				_window.RemoveChild(_currentDialog);
				_currentDialog = null;
			}
			else
			{
				_window.BringToFront();
				_window.BringToFront(_currentDialog);
				_window.SetScale(_currentDialog.Scale.x + 26f, _currentDialog.Scale.y + 26f);
				_window.SetPosition((float)Screen.width - _window.Scale.x, (float)Screen.height - (float)Screen.height * 0.12f - _window.Scale.y);
			}
		}
	}

	public void PauseQueue()
	{
		_isPaused = true;
		if (_currentDialog != null)
		{
			_currentDialog.ForceClose();
			_window.RemoveChild(_currentDialog);
			_window.Enabled = false;
			_currentDialog = null;
		}
	}

	public void ResumeQueue()
	{
		_isPaused = false;
	}
}
