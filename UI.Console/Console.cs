using System;
using System.Collections.Generic;
using Code.Core.Command;
using Code.Core.EventSystem;
using Core;
using Core.GameEvent;
using Core.Input;
using TMPro;
using UI.Elements.Window;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Console;

public class Console : UIWindow
{
	[Header("Elements")]
	[SerializeField]
	private TMP_InputField inputField;

	[SerializeField]
	private TMP_Text candidate;

	[SerializeField]
	private ScrollRect scrollRect;

	[Header("Messages")]
	[SerializeField]
	private ConsoleLogSO consoleLogSO;

	[SerializeField]
	private int maxMessageSize = 255;

	private string _currentInput = "";

	private string _suggestedInput = "";

	private readonly LinkedList<string> _history = new LinkedList<string>();

	private LinkedListNode<string> _historyNode;

	private List<string> _candidates = new List<string>();

	private int _currentCandidateIndex;

	private MilMo_GenericReaction _commandResponseListener;

	protected override void Awake()
	{
		base.Awake();
		_commandResponseListener = MilMo_EventSystem.Listen("command_response", OnCommandResponse);
		_commandResponseListener.Repeating = true;
		if (inputField == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing inputField");
		}
		else if (scrollRect == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing scrollRect");
		}
		else if (candidate == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing candidate");
		}
		else if (!consoleLogSO)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing console log");
		}
		else
		{
			ResetState();
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (inputField != null)
		{
			inputField.gameObject.SetActive(value: true);
		}
		FocusInput();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (inputField != null)
		{
			inputField.gameObject.SetActive(value: false);
		}
	}

	protected void OnDestroy()
	{
		MilMo_EventSystem.RemoveReaction(_commandResponseListener);
		_commandResponseListener = null;
	}

	public override void Open()
	{
		if (Singleton<InputController>.Instance != null)
		{
			GameEvent.OnExit = (Action)Delegate.Combine(GameEvent.OnExit, new Action(Close));
			Singleton<InputController>.Instance.SetConsoleController();
		}
		base.gameObject.SetActive(value: true);
	}

	public override void Close()
	{
		if (Singleton<InputController>.Instance != null)
		{
			GameEvent.OnExit = (Action)Delegate.Remove(GameEvent.OnExit, new Action(Close));
			Singleton<InputController>.Instance.RestorePreviousController();
		}
		OnDeselect();
		base.gameObject.SetActive(value: false);
	}

	private void ResetState()
	{
		ResetInput();
		ResetCandidate();
		ResetScrollPosition();
	}

	private void ResetScrollPosition()
	{
		if (scrollRect != null)
		{
			scrollRect.verticalNormalizedPosition = 0f;
		}
	}

	private void ResetInput()
	{
		SetInputText("");
	}

	private void SetInputText(string value)
	{
		if (inputField != null)
		{
			inputField.SetTextWithoutNotify(value ?? "");
		}
		_currentInput = value ?? "";
	}

	private void FocusInput()
	{
		if ((bool)inputField)
		{
			inputField.ActivateInputField();
		}
	}

	public void OnSelect()
	{
		GameEvent.OnHistoryBackward = (Action)Delegate.Combine(GameEvent.OnHistoryBackward, new Action(HistoryBackward));
		GameEvent.OnHistoryForward = (Action)Delegate.Combine(GameEvent.OnHistoryForward, new Action(HistoryForward));
		GameEvent.OnNextCandidate = (Action)Delegate.Combine(GameEvent.OnNextCandidate, new Action(NextCandidate));
		GameEvent.OnSend = (Action)Delegate.Combine(GameEvent.OnSend, new Action(OnSend));
	}

	public void OnDeselect()
	{
		GameEvent.OnHistoryBackward = (Action)Delegate.Remove(GameEvent.OnHistoryBackward, new Action(HistoryBackward));
		GameEvent.OnHistoryForward = (Action)Delegate.Remove(GameEvent.OnHistoryForward, new Action(HistoryForward));
		GameEvent.OnNextCandidate = (Action)Delegate.Remove(GameEvent.OnNextCandidate, new Action(NextCandidate));
		GameEvent.OnSend = (Action)Delegate.Remove(GameEvent.OnSend, new Action(OnSend));
	}

	public void OnValueChanged(string value)
	{
		if (value.Length >= maxMessageSize)
		{
			inputField.text = value.Substring(0, maxMessageSize);
			return;
		}
		bool hasChanged = !string.Equals(value, _currentInput, StringComparison.CurrentCultureIgnoreCase);
		bool hasInput = value.Length > _currentInput.Length;
		_currentInput = value;
		RefreshCandidate(hasChanged, hasInput);
	}

	private void ResetCandidate()
	{
		if (candidate != null)
		{
			candidate.text = "";
		}
		_candidates.Clear();
		_currentCandidateIndex = 0;
		_suggestedInput = "";
	}

	private void RefreshCandidate(bool hasChanged, bool hasInput)
	{
		if (_currentInput.Length < 1 || _currentInput.LastIndexOf(' ') != -1)
		{
			ResetCandidate();
			return;
		}
		if (hasChanged)
		{
			ResetCandidate();
		}
		if (hasInput)
		{
			_candidates = MilMo_Command.Instance.GetCandidates(_currentInput);
			SetCandidate();
		}
	}

	private void NextCandidate()
	{
		_currentCandidateIndex++;
		SetCandidate();
	}

	private void SetCandidate()
	{
		if (_candidates.Count >= 1)
		{
			if (_currentCandidateIndex >= _candidates.Count)
			{
				_currentCandidateIndex = 0;
			}
			_suggestedInput = _candidates[_currentCandidateIndex];
			if (candidate != null)
			{
				candidate.text = _suggestedInput;
			}
		}
	}

	private void HistoryBackward()
	{
		if (_historyNode != null)
		{
			if (inputField.text == _historyNode?.Value)
			{
				_historyNode = _historyNode.Previous ?? _historyNode;
			}
			ResetState();
			SetInputText(_historyNode?.Value);
		}
	}

	private void HistoryForward()
	{
		if (_historyNode != null)
		{
			_historyNode = _historyNode.Next;
			ResetState();
			SetInputText(_historyNode?.Value);
			if (_historyNode == null)
			{
				_historyNode = _history.Last;
			}
		}
	}

	private void OnCommandResponse(object o)
	{
		if (o is string message)
		{
			AddMessageToLog(message);
		}
	}

	private void AddMessageToLog(string message)
	{
		if (!string.IsNullOrEmpty(message) && consoleLogSO != null)
		{
			consoleLogSO.Add(message);
		}
	}

	private void OnSend()
	{
		string text = ((_suggestedInput != "") ? _suggestedInput : _currentInput);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		text = text.Trim();
		if (!(text == ""))
		{
			if (MilMo_Command.Instance.HandleCommand(text) != "No such command")
			{
				AddMessageToLog(text);
			}
			_historyNode = _history.AddLast(text);
			ResetState();
			FocusInput();
		}
	}
}
