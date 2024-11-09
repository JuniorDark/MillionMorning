using System;

namespace Code.World.GUI.PVP;

internal class MatchModeAttr : Attribute
{
	public string ObjectiveName { get; private set; }

	public string Title { get; private set; }

	public string Description { get; private set; }

	internal MatchModeAttr(string objectiveName, string title, string description)
	{
		ObjectiveName = objectiveName;
		Title = title;
		Description = description;
	}
}
