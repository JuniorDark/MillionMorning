using System;

namespace Code.World.GUI.Homes;

[Serializable]
public struct RoomSetting
{
	public int Id { get; set; }

	public string Name { get; set; }

	public sbyte AccessLevel { get; set; }

	public bool IsStartingRoom { get; set; }

	public RoomSetting(int id, string name, sbyte accessLevel, bool isStartingRoom)
	{
		Id = id;
		Name = name;
		AccessLevel = accessLevel;
		IsStartingRoom = isStartingRoom;
	}

	public bool Equals(RoomSetting other)
	{
		if (Name == other.Name)
		{
			return AccessLevel == other.AccessLevel;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is RoomSetting other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Name, AccessLevel);
	}

	public static bool operator ==(RoomSetting c1, RoomSetting c2)
	{
		return c1.Equals(c2);
	}

	public static bool operator !=(RoomSetting c1, RoomSetting c2)
	{
		return !c1.Equals(c2);
	}
}
