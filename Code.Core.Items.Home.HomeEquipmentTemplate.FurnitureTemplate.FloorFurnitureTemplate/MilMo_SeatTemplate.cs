using System.Collections.Generic;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture.FloorFurniture;
using Code.Core.Network.types;

namespace Code.Core.Items.Home.HomeEquipmentTemplate.FurnitureTemplate.FloorFurnitureTemplate;

public sealed class MilMo_SeatTemplate : MilMo_FloorFurnitureTemplate
{
	private readonly List<MilMo_SitPointTemplate> _mSitNodes = new List<MilMo_SitPointTemplate>();

	public List<MilMo_SitPointTemplate> SitNodes => _mSitNodes;

	private MilMo_SeatTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public new static MilMo_SeatTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_SeatTemplate(category, path, filePath, "Seat");
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_Seat(this, modifiers);
	}

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		if (!base.LoadFromNetwork(t))
		{
			return false;
		}
		if (!(t is SeatTemplate seatTemplate))
		{
			return true;
		}
		foreach (SeatSitNode sitNode in seatTemplate.GetSitNodes())
		{
			MilMo_SitPointTemplate item = new MilMo_SitPointTemplate(sitNode);
			_mSitNodes.Add(item);
		}
		return true;
	}
}
