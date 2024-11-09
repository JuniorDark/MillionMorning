using Code.World.GUI.Homes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Window.Home.Home;

public class HomeTabContent : MonoBehaviour
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

	[Header("Raffle")]
	[SerializeField]
	private Button raffle;

	[SerializeField]
	private Transform raffleContainer;

	private MilMo_HomeSettingHandler _homeController;

	private HomeSetting _setting;

	private bool _hasErrors;

	private void Awake()
	{
		errorMessage.gameObject.SetActive(value: false);
	}

	private void OnEnable()
	{
		_homeController = new MilMo_HomeSettingHandler();
		_setting = _homeController.GetSettings();
		InitializeInputField();
		homeSettingsMenu.Persist += SaveSetting;
		_homeController.InRaffle += HideRaffle;
	}

	private void OnDisable()
	{
		homeSettingsMenu.Persist -= SaveSetting;
	}

	private void InitializeInputField()
	{
		nameInput.text = _setting.name;
	}

	public void UpdateRoomName(string newName)
	{
		if (!(newName == _setting.name) && !_hasErrors)
		{
			_setting.name = newName;
		}
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

	public void JoinRaffle()
	{
		Debug.Log("JoinRaffle");
		_homeController.JoinRaffle();
	}

	private void HideRaffle(bool value)
	{
		raffleContainer.gameObject.SetActive(!value);
	}

	private void SaveSetting()
	{
		_homeController.UpdateSetting(_setting);
		_homeController.Persist();
	}
}
