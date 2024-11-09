namespace Code.Core.Items.Home;

public abstract class MilMo_HomeEquipmentTemplate : MilMo_ItemTemplate
{
	public virtual bool IsRoom => false;

	public virtual bool IsHomeDeliveryBox => false;

	public virtual bool IsSkin => false;

	protected MilMo_HomeEquipmentTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}
}
