using System;
using System.Threading.Tasks;
using Code.Core.Global;
using Code.World;
using Code.World.CharBuilder;
using Code.World.Player;
using Core;
using Core.Analytics;
using Core.Audio.AudioData;
using Localization;
using UI.AvatarBuilder.Headers;
using UI.HUD.Dialogues;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AvatarBuilder.Handlers;

public class GlobalHandler : MonoBehaviour
{
	[SerializeField]
	private Button randomizeAvatarButton;

	[SerializeField]
	private Button startButton;

	[SerializeField]
	private GameObject processScreen;

	private AvatarEditor _avatarEditor;

	private AvatarEditorHeader[] _categories;

	private NameHandler _nameHandler;

	[Header("Sounds")]
	[SerializeField]
	private UIAudioCueSO doneSound;

	[SerializeField]
	private UIAudioCueSO wrongSound;

	private void Awake()
	{
		if (randomizeAvatarButton == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find randomizeAvatar");
			return;
		}
		if (startButton == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find startButton");
			return;
		}
		if (wrongSound == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find wrong sound");
		}
		if (doneSound == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find done sound");
		}
	}

	private void Start()
	{
		_avatarEditor = UnityEngine.Object.FindObjectOfType<AvatarEditor>();
		if (_avatarEditor == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find AvatarEditor");
			return;
		}
		_categories = UnityEngine.Object.FindObjectsOfType<AvatarEditorHeader>();
		_nameHandler = UnityEngine.Object.FindObjectOfType<NameHandler>();
		randomizeAvatarButton.onClick.AddListener(RandomizeAvatar);
		startButton.onClick.AddListener(StartGame);
	}

	private bool CheckValidation()
	{
		return _nameHandler.IsValid();
	}

	private void ChangeToCategory(AvatarEditorCategory selectedCategory)
	{
		AvatarEditorHeader[] categories = _categories;
		foreach (AvatarEditorHeader avatarEditorHeader in categories)
		{
			if (avatarEditorHeader.category == selectedCategory)
			{
				avatarEditorHeader.ToggleOn();
				break;
			}
		}
	}

	private void OnDestroy()
	{
		randomizeAvatarButton.onClick.RemoveListener(RandomizeAvatar);
		startButton.onClick.RemoveListener(StartGame);
	}

	private async void StartGame()
	{
		AvatarSelection selection = _avatarEditor.GetCurrentSelection();
		if (selection == null)
		{
			Debug.LogWarning("Selection is null wtf?!");
			return;
		}
		processScreen.SetActive(value: true);
		if (!CheckValidation())
		{
			wrongSound.PlayAudioCue();
			ChangeToCategory(AvatarEditorCategory.Profile);
			SetInputError("CharBuilder_INVALID");
			processScreen.SetActive(value: false);
			return;
		}
		if (!HandleNameResponse(await new CheckAvatarName().Check(selection.AvatarName)))
		{
			processScreen.SetActive(value: false);
			return;
		}
		if (!HandleCreateAvatarResponse(await CreateAvatarRequest(selection), out var status))
		{
			MilMoAnalyticsHandler.CharacterCreatedFailed(selection.AvatarName, GetStatusText(status));
			Debug.Log("MilMo_CharBuilder::AvatarCreated - Failure");
			processScreen.SetActive(value: false);
			return;
		}
		Debug.Log("MilMo_CharBuilder::AvatarCreated - Success");
		MilMoAnalyticsHandler.CharacterCreated(selection.AvatarName);
		if (MilMo_Player.Instance != null)
		{
			MilMo_Player.Instance.InCharBuilder = false;
		}
		else
		{
			Debug.LogWarning(base.gameObject.name + ": Unable to find PlayerInstance");
		}
		if (doneSound != null)
		{
			doneSound.PlayAudioCue();
		}
		else
		{
			Debug.LogError(base.gameObject.name + ": Unable to find DoneSound");
		}
		if (processScreen != null)
		{
			processScreen.SetActive(value: false);
		}
		else
		{
			Debug.LogError(base.gameObject.name + ": Unable to find ProcessScreen");
		}
		MilMoAnalyticsHandler.CharbuilderDone();
		Debug.Log("MilMo_CharBuilder::Charbuilder_done");
		SceneManager sceneManager = UnityEngine.Object.FindObjectOfType<SceneManager>();
		if (sceneManager != null)
		{
			sceneManager.ExitAvatarEditor();
			MilMo_World.Activate();
			MilMo_World.Instance.LoginToNewServer(MilMo_Global.RemoteGameHost, MilMo_Global.AuthorizationToken, null);
		}
		else
		{
			Debug.LogError(base.gameObject.name + ": Unable to find SceneManager");
		}
	}

	private bool HandleNameResponse(AvatarNameResponse response)
	{
		switch (response)
		{
		case AvatarNameResponse.Valid:
			return true;
		case AvatarNameResponse.Invalid:
			SpawnWarning("CharBuilder_46", "CharBuilder_52");
			SetInputError("CharBuilder_INVALID");
			ChangeToCategory(AvatarEditorCategory.Profile);
			break;
		case AvatarNameResponse.Taken:
			SpawnWarning("CharBuilder_46", "CharBuilder_53");
			SetInputError("CharBuilder_TAKEN");
			ChangeToCategory(AvatarEditorCategory.Profile);
			break;
		case AvatarNameResponse.Timeout:
			SpawnWarning("CharBuilder_4741", "CharBuilder_4742");
			break;
		}
		wrongSound.PlayAudioCue();
		return false;
	}

	private bool HandleCreateAvatarResponse(RequestStatus creationStatus, out RequestStatus status)
	{
		switch (creationStatus)
		{
		case RequestStatus.Valid:
			status = RequestStatus.Valid;
			return true;
		case RequestStatus.Invalid:
			SpawnWarning("CharBuilder_51", "CharBuilder_55");
			wrongSound.PlayAudioCue();
			status = RequestStatus.Invalid;
			return false;
		case RequestStatus.InProgress:
			status = RequestStatus.InProgress;
			return false;
		case RequestStatus.Timeout:
			SpawnWarning("CharBuilder_4741", "CharBuilder_4742");
			wrongSound.PlayAudioCue();
			status = RequestStatus.Timeout;
			return false;
		default:
			wrongSound.PlayAudioCue();
			status = RequestStatus.Invalid;
			return false;
		}
	}

	private string GetStatusText(RequestStatus status)
	{
		return status switch
		{
			RequestStatus.Invalid => "Invalid", 
			RequestStatus.InProgress => "InProgress", 
			RequestStatus.Valid => "Valid", 
			RequestStatus.Timeout => "Timeout", 
			_ => throw new ArgumentOutOfRangeException("status", status, null), 
		};
	}

	private async Task<RequestStatus> CreateAvatarRequest(AvatarSelection selection)
	{
		return await new CreateAvatarRequest().Check(selection);
	}

	private void RandomizeAvatar()
	{
		_avatarEditor.GenerateRandomSelection();
	}

	private void SetInputError(string errorMessage)
	{
		LocalizedStringWithArgument localizedStringWithArgument = new LocalizedStringWithArgument(errorMessage, null);
		_nameHandler.SetErrorMessage(localizedStringWithArgument.GetMessage());
	}

	private void SpawnWarning(string caption, string message)
	{
		LocalizedStringWithArgument caption2 = new LocalizedStringWithArgument(caption, null);
		LocalizedStringWithArgument message2 = new LocalizedStringWithArgument(message, null);
		DialogueSpawner.SpawnWarningModalDialogue(caption2, message2);
	}
}
