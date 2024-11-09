using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_PowerUp : MilMo_Item
{
	public new MilMo_PowerUpTemplate Template => base.Template as MilMo_PowerUpTemplate;

	public MilMo_PowerUp(MilMo_PowerUpTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public override bool IsWearable()
	{
		return false;
	}

	public override bool IsWieldable()
	{
		return false;
	}

	public override bool AutoPickup()
	{
		return true;
	}
}
