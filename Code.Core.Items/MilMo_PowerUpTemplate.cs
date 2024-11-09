using Code.Core.Network.types;

namespace Code.Core.Items;

public abstract class MilMo_PowerUpTemplate : MilMo_ItemTemplate
{
	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		base.LoadFromNetwork(t);
		return true;
	}

	protected MilMo_PowerUpTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}
}
