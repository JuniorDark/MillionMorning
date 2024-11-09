using Code.Core.Network;
using Code.Core.Network.messages.client;
using Code.World.Home;
using Core;

namespace Code.World.GUI.Homes;

public class MilMo_RoomSettingHandler : MilMo_SettingsHandler<RoomSetting>
{
	public MilMo_RoomSettingHandler()
	{
		Load();
		OriginalHash = base.CurrentSetting.GetHashCode();
	}

	private void Load()
	{
		LoadSetting();
	}

	protected override void LoadSetting()
	{
		string value = "Room";
		string value2 = "0";
		int id = 0;
		bool isStartingRoom = false;
		if (MilMo_Home.CurrentHome?.CurrentRoom != null)
		{
			MilMo_Home.CurrentHome?.CurrentRoom.Item.Modifiers.TryGetValue("RoomName", out value);
			MilMo_Home.CurrentHome?.CurrentRoom.Item.Modifiers.TryGetValue("RoomAccessLevel", out value2);
			id = (int)MilMo_Home.CurrentHome.CurrentRoom.Id;
			isStartingRoom = MilMo_Home.CurrentHome.IsStartingRoom(id);
		}
		sbyte accessLevel = (sbyte)((value2 != null) ? sbyte.Parse(value2) : 0);
		base.CurrentSetting = new RoomSetting(id, value, accessLevel, isStartingRoom);
	}

	public override void Persist()
	{
		if (OriginalHash != base.CurrentSetting.GetHashCode())
		{
			Singleton<GameNetwork>.Instance.SendToGameServer(new ClientUpdateRoomSettings(base.CurrentSetting.Id, base.CurrentSetting.Name, base.CurrentSetting.AccessLevel));
			MilMo_Home.CurrentHome?.CurrentRoom.Item.ChangeModifier("RoomName", base.CurrentSetting.Name);
			MilMo_Home.CurrentHome?.CurrentRoom.Item.ChangeModifier("RoomAccessLevel", base.CurrentSetting.AccessLevel.ToString());
		}
	}
}
