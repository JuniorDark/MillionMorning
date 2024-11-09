using Code.World.GUI.Homes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Window.Home.Room;

public class RoomTabContent : MonoBehaviour
{
	[SerializeField]
	private HomeSettingsMenu homeSettingsMenu;

	[SerializeField]
	private Button save;

	[Header("Input")]
	[SerializeField]
	private TMP_InputField nameInput;

	[SerializeField]
	private TMP_Text errorMessage;

	[Header("Privacy")]
	[SerializeField]
	private Transform privacy;

	[SerializeField]
	private Toggle isPublic;

	[SerializeField]
	private Toggle isFriendsOnly;

	[SerializeField]
	private Toggle isPrivate;

	[SerializeField]
	private ToggleGroup group;

	private MilMo_RoomSettingHandler _roomController;

	private RoomSetting _setting;

	private bool _hasErrors;

	private void Awake()
	{
		errorMessage.gameObject.SetActive(value: false);
	}

	private void OnEnable()
	{
		_roomController = new MilMo_RoomSettingHandler();
		_setting = _roomController.GetSettings();
		homeSettingsMenu.Persist += SaveSetting;
		InitializeInputField();
		InitializeToggles();
		ShowPrivacySettings(!_setting.IsStartingRoom);
	}

	private void OnDisable()
	{
		homeSettingsMenu.Persist -= SaveSetting;
	}

	private void InitializeInputField()
	{
		nameInput.text = _setting.Name;
	}

	public void OnError()
	{
		_hasErrors = true;
		errorMessage.gameObject.SetActive(value: true);
		save.interactable = false;
	}

	public void OnPass()
	{
		_hasErrors = false;
		errorMessage.gameObject.SetActive(value: false);
		save.interactable = true;
	}

	public void UpdateRoomName(string newName)
	{
		if (!(newName == _setting.Name) && !_hasErrors)
		{
			_setting.Name = newName;
		}
	}

	private void InitializeToggles()
	{
		UpdateToggles(_setting.AccessLevel);
	}

	private void ShowPrivacySettings(bool value)
	{
		privacy.gameObject.SetActive(value);
	}

	private void UpdateToggles(int value)
	{
		group.SetAllTogglesOff();
		isPublic.isOn = false;
		isFriendsOnly.isOn = false;
		isPrivate.isOn = false;
		switch (value)
		{
		case 0:
			isPublic.Select();
			isPublic.isOn = true;
			break;
		case 1:
			isFriendsOnly.Select();
			isFriendsOnly.isOn = true;
			break;
		case 2:
			isPrivate.Select();
			isPrivate.isOn = true;
			break;
		}
	}

	public void UpdateAccessLevel(int accessLevel)
	{
		_setting.AccessLevel = (sbyte)accessLevel;
	}

	private void SaveSetting()
	{
		_roomController.UpdateSetting(_setting);
		_roomController.Persist();
	}
}
