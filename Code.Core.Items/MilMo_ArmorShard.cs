using System.Collections.Generic;

namespace Code.Core.Items;

public sealed class MilMo_ArmorShard : MilMo_PowerUp
{
	public new MilMo_ArmorShardTemplate Template => ((MilMo_Item)this).Template as MilMo_ArmorShardTemplate;

	public MilMo_ArmorShard(MilMo_PowerUpTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public override bool MayPickUp()
	{
		return true;
	}
}
