using System.Collections.Generic;
using Code.Core.Items.Home.HomeEquipmentTemplate;

namespace Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.HomeSurface;

public sealed class MilMo_Floor : MilMo_HomeSurface
{
	public MilMo_Floor(MilMo_HomeSurfaceTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers, "Floor")
	{
	}
}
