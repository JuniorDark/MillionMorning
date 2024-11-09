using System;

namespace Code.World.GUI.Homes;

[Serializable]
public struct HomeSetting
{
	public string name;

	public sbyte accessLevel;

	public bool Equals(HomeSetting other)
	{
		if (name == other.name)
		{
			return accessLevel == other.accessLevel;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is HomeSetting other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(name, accessLevel);
	}

	public static bool operator ==(HomeSetting c1, HomeSetting c2)
	{
		return c1.Equals(c2);
	}

	public static bool operator !=(HomeSetting c1, HomeSetting c2)
	{
		return !c1.Equals(c2);
	}
}
