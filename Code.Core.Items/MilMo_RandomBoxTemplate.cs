namespace Code.Core.Items;

public abstract class MilMo_RandomBoxTemplate : MilMo_ItemTemplate
{
	public override string IconPath => "Content/Items/" + VisualRepPath + "Icon" + VisualRepName;

	protected MilMo_RandomBoxTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}
}
