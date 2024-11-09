using System;
using System.Reflection;

namespace Code.World.GUI.PVP;

public static class MatchModes
{
	public static string ObjectiveName(this MilMo_MatchMode m)
	{
		return GetAttr(m).ObjectiveName;
	}

	public static string Title(this MilMo_MatchMode m)
	{
		return GetAttr(m).Title;
	}

	public static string Description(this MilMo_MatchMode m)
	{
		return GetAttr(m).Description;
	}

	private static MatchModeAttr GetAttr(MilMo_MatchMode m)
	{
		return (MatchModeAttr)Attribute.GetCustomAttribute(ForValue(m), typeof(MatchModeAttr));
	}

	private static MemberInfo ForValue(MilMo_MatchMode m)
	{
		return typeof(MilMo_MatchMode).GetField(Enum.GetName(typeof(MilMo_MatchMode), m));
	}
}
