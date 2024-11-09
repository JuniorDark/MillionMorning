using System;
using System.Collections.Generic;
using Code.Core.Global;
using Code.Core.Items.Home.FurnitureGrid;
using Code.Core.Items.Home.GridCells;
using Code.Core.Items.Home.HomeEquipmentTemplate;
using Code.Core.Items.Home.HomeEquipmentTemplate.FurnitureTemplate;
using UnityEngine;

namespace Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture;

public sealed class MilMo_WallFurniture : MilMo_Furniture
{
	public new MilMo_WallFurnitureTemplate Template => ((MilMo_Item)this).Template as MilMo_WallFurnitureTemplate;

	public MilMo_FurnitureWallGrid Grid => Template.Grid;

	public new WallGridCell Tile
	{
		get
		{
			return base.Tile as WallGridCell;
		}
		set
		{
			base.Tile = value;
		}
	}

	public override string RotateAnimation
	{
		get
		{
			throw new NotSupportedException("Wall furniture can't be rotated");
		}
	}

	public MilMo_WallFurniture(MilMo_FurnitureTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public override bool ShouldFade()
	{
		if (base.GameObject == null)
		{
			return false;
		}
		Vector3 from = -base.GameObject.transform.right;
		Vector3 position = base.GameObject.transform.position;
		Vector3 to = MilMo_Global.Camera.transform.position - position;
		to.y = 0f;
		return Vector3.Angle(from, to) > 90f;
	}
}
