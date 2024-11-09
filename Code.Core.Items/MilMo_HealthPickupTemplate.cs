using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Items;

public class MilMo_HealthPickupTemplate : MilMo_PowerUpTemplate
{
	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		base.LoadFromNetwork(t);
		return true;
	}

	public static MilMo_HealthPickupTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_HealthPickupTemplate(category, path, filePath, "HealthPickup");
	}

	protected MilMo_HealthPickupTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_HealthPickup(this, modifiers);
	}
}
