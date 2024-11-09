using System.Collections.Generic;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture.AttachableFurniture;
using Code.Core.Network.types;

namespace Code.Core.Items.Home.HomeEquipmentTemplate.FurnitureTemplate.AttachableFurnitureTemplate;

public sealed class MilMo_HomeDeliveryBoxTemplate : MilMo_AttachableFurnitureTemplate, IMilMo_OpenableBox
{
	public string IconPathClosed => IconPath;

	public string IconPathOpen => IconPath + "Open";

	public IList<string> PickupEffects { get; private set; }

	public override bool IsHomeDeliveryBox => true;

	private MilMo_HomeDeliveryBoxTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public new static MilMo_HomeDeliveryBoxTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_HomeDeliveryBoxTemplate(category, path, filePath, "HomeDeliveryBox");
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_HomeDeliveryBox(this, modifiers);
	}

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		if (!base.LoadFromNetwork(t))
		{
			return false;
		}
		if (!(t is HomeDeliveryBoxTemplate homeDeliveryBoxTemplate))
		{
			return false;
		}
		PickupEffects = homeDeliveryBoxTemplate.GetPickupEffects();
		return true;
	}

	public void PostOpenEvent()
	{
	}
}
