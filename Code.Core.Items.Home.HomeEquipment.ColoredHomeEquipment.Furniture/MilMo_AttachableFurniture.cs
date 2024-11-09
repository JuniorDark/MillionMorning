using System.Collections.Generic;
using Code.Core.Items.Home.GridCells;
using Code.Core.Items.Home.HomeEquipmentTemplate;
using Code.Core.Items.Home.HomeEquipmentTemplate.FurnitureTemplate;

namespace Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture;

public class MilMo_AttachableFurniture : MilMo_Furniture
{
	public new MilMo_AttachableFurnitureTemplate Template => ((MilMo_Item)this).Template as MilMo_AttachableFurnitureTemplate;

	public bool IsOnFloor
	{
		get
		{
			if (!base.InStorage)
			{
				return base.Tile is FloorGridCell;
			}
			return false;
		}
	}

	public bool IsOnFurniture
	{
		get
		{
			if (!base.InStorage)
			{
				return base.Tile is AttachNode;
			}
			return false;
		}
	}

	public FloorGridCell FloorTile => base.Tile as FloorGridCell;

	public AttachNode AttachNode => base.Tile as AttachNode;

	public string HoldIdleAnimation
	{
		get
		{
			if (!IsOnFloor)
			{
				return "MoveLargeIdle";
			}
			return "MoveSmallIdle";
		}
	}

	public override string RotateAnimation
	{
		get
		{
			if (!IsOnFloor)
			{
				return "MoveLargeRotate";
			}
			return "MoveSmallRotate";
		}
	}

	public MilMo_AttachableFurniture(MilMo_FurnitureTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}
}
