using Localization;
using UI.HUD.Dialogues;
using UI.Sprites;
using UnityEngine;

namespace Core;

public class ExceptionManager : MonoBehaviour
{
	private bool _exceptionOccured;

	private void Awake()
	{
		Application.logMessageReceived += HandleException;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnDestroy()
	{
		Application.logMessageReceived -= HandleException;
	}

	private void HandleException(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Exception && !_exceptionOccured)
		{
			_exceptionOccured = true;
			DialogueSpawner.SpawnOkModal(new LocalizedStringWithArgument("Generic_4771"), new LocalizedStringWithArgument("Generic_4772"), new AddressableSpriteLoader("ErrorIcon"), null);
		}
	}
}
