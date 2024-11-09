using System.Collections.Generic;
using Code.Core.Items.Home.FurnitureGrid;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture;
using Code.Core.Network.types;

namespace Code.Core.Items.Home.HomeEquipmentTemplate.FurnitureTemplate;

public sealed class MilMo_WallFurnitureTemplate : MilMo_FurnitureTemplate
{
	private bool _mIsCurtain;

	public MilMo_FurnitureWallGrid Grid { get; private set; }

	public override bool IsCurtain => _mIsCurtain;

	private MilMo_WallFurnitureTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public static MilMo_WallFurnitureTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_WallFurnitureTemplate(category, path, filePath, "WallFurniture");
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_WallFurniture(this, modifiers);
	}

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		if (!base.LoadFromNetwork(t))
		{
			return false;
		}
		WallFurnitureTemplate wallFurnitureTemplate = t as WallFurnitureTemplate;
		Grid = new MilMo_FurnitureWallGrid((byte)wallFurnitureTemplate.GetWidth(), wallFurnitureTemplate.GetPivot());
		_mIsCurtain = wallFurnitureTemplate.GetIsCurtain() != 0;
		return true;
	}
}
