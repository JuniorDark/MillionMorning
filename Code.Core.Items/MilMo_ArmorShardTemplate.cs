using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Items;

public sealed class MilMo_ArmorShardTemplate : MilMo_PowerUpTemplate
{
	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		base.LoadFromNetwork(t);
		return true;
	}

	public static MilMo_ArmorShardTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_ArmorShardTemplate(category, path, filePath, "ArmorShard");
	}

	private MilMo_ArmorShardTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_ArmorShard(this, modifiers);
	}
}
