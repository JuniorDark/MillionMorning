using System.Collections.Generic;
using Code.Core.ResourceSystem;

namespace Code.Core.BodyPack;

public sealed class ColorGroup
{
	private const string LOC_STRING_PREFIX = "BodyPacks_ColorGroups_";

	public string GroupName { get; }

	public MilMo_LocString DisplayName => MilMo_Localization.GetLocString("BodyPacks_ColorGroups_" + GroupName);

	public IList<int> ColorIndices { get; }

	public ColorGroup(string groupName, IList<int> colorIndices)
	{
		GroupName = groupName;
		ColorIndices = colorIndices;
	}
}
