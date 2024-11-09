using Code.Core.ResourceSystem;
using Code.World.CharBuilder;
using Core.Utilities;
using Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AvatarBuilder;

public class NameHandler : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField chosenName;

	[SerializeField]
	private Button randomButton;

	[SerializeField]
	private TMP_Text errorText;

	private AvatarEditorUI _avatarEditorUI;

	private AvatarEditor _avatarEditor;

	private bool _isValid;

	private const int MAX_LENGTH = 20;

	private void Awake()
	{
		if (chosenName == null)
		{
			Debug.LogWarning("Unable to get chosenName!");
		}
		else if (randomButton == null)
		{
			Debug.LogWarning("Unable to get randomButton!");
		}
		else if (errorText == null)
		{
			Debug.LogWarning("Unable to get errorText!");
		}
	}

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		_avatarEditorUI = GameObject.FindGameObjectWithTag("AvatarEditorUI").GetComponent<AvatarEditorUI>();
		if (_avatarEditorUI == null)
		{
			Debug.LogWarning("Unable to get AvatarEditorUI!");
			return;
		}
		_avatarEditor = Object.FindObjectOfType<AvatarEditor>();
		if (_avatarEditor == null)
		{
			Debug.LogWarning("Unable to get AvatarEditor!");
			return;
		}
		errorText.text = "";
		randomButton.onClick.AddListener(RandomizeName);
		chosenName.onValueChanged.AddListener(ValidateName);
		chosenName.onEndEdit.AddListener(UpdateName);
	}

	public bool IsValid()
	{
		return _isValid;
	}

	private void UpdateName(string avatarName)
	{
		_avatarEditor.SetAvatarName(avatarName);
	}

	private void RandomizeName()
	{
		string validName = ValidNameGenerator.GetValidName();
		chosenName.text = validName;
		_avatarEditor.SetAvatarName(validName);
	}

	private void ValidateName(string arg0)
	{
		if (arg0.Length > 20)
		{
			string text = arg0[..20];
			chosenName.text = text;
		}
		MilMo_BadWordFilter.StringIntegrity stringIntegrity = MilMo_BadWordFilter.GetStringIntegrity(arg0);
		if (stringIntegrity == MilMo_BadWordFilter.StringIntegrity.OK || stringIntegrity == MilMo_BadWordFilter.StringIntegrity.Empty)
		{
			_isValid = true;
			SetErrorMessage("");
		}
		else
		{
			string localizedString = LocalizationUtility.GetLocalizedString("CharBuilder", "CharBuilder_INVALID");
			SetErrorMessage(localizedString);
			_isValid = false;
		}
	}

	public void SetErrorMessage(string message)
	{
		errorText.text = message;
	}
}
