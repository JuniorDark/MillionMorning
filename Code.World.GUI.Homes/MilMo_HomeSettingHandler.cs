using System;
using Code.Core.Network;
using Code.Core.Network.messages.client;
using Code.Core.ResourceSystem;
using Code.World.Home;
using Core;

namespace Code.World.GUI.Homes;

public class MilMo_HomeSettingHandler : MilMo_SettingsHandler<HomeSetting>
{
	private readonly MilMo_Home _currentHome;

	private const string DEFAULT_NAME = "Homes_13405";

	private bool _inRaffle;

	public event Action<bool> InRaffle;

	public MilMo_HomeSettingHandler()
	{
		_currentHome = MilMo_Home.CurrentHome;
		Load();
		MilMo_Home.RaffleChange += RaffleChange;
	}

	private void Load()
	{
		LoadSetting();
	}

	protected override void LoadSetting()
	{
		bool flag = _currentHome == null;
		string @string = MilMo_Localization.GetLocString("Homes_13405").String;
		base.CurrentSetting = new HomeSetting
		{
			name = (flag ? @string : _currentHome.HomeName),
			accessLevel = 1
		};
		OriginalHash = base.CurrentSetting.GetHashCode();
	}

	public void JoinRaffle()
	{
		Singleton<GameNetwork>.Instance.SendToGameServer(new ClientAddPlayerToHomeOfTheDayRaffle());
	}

	private void RaffleChange(bool inRaffle)
	{
		this.InRaffle?.Invoke(inRaffle);
	}

	public override void Persist()
	{
		if (OriginalHash != base.CurrentSetting.GetHashCode())
		{
			Singleton<GameNetwork>.Instance.SendToGameServer(new ClientUpdateHomeSettings(base.CurrentSetting.name, base.CurrentSetting.accessLevel));
			_currentHome.HomeName = base.CurrentSetting.name;
		}
	}
}
