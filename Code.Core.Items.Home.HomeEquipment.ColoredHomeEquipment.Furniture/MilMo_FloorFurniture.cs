using System.Collections.Generic;
using Code.Core.Items.Home.FurnitureGrid;
using Code.Core.Items.Home.GridCells;
using Code.Core.Items.Home.HomeEquipmentTemplate;
using Code.Core.Items.Home.HomeEquipmentTemplate.FurnitureTemplate;
using UnityEngine;

namespace Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture;

public class MilMo_FloorFurniture : MilMo_Furniture
{
	public enum RotationDirection
	{
		Clockwise,
		AntiClockwise
	}

	public MilMo_FurnitureFloorGrid Grid => Template.GetGridForRotation(base.Rotation);

	public new MilMo_FloorFurnitureTemplate Template => ((MilMo_Item)this).Template as MilMo_FloorFurnitureTemplate;

	public new FloorGridCell Tile
	{
		get
		{
			return base.Tile as FloorGridCell;
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
			if (!Template.IsSmall)
			{
				return "MoveLargeRotate";
			}
			return "MoveSmallRotate";
		}
	}

	public MilMo_FloorFurniture(MilMo_FurnitureTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public float GetNextRotation(RotationDirection direction)
	{
		return GetNextRotation(base.Rotation, direction, Template.IsSquare);
	}

	public static float GetNextRotation(float startRotation, RotationDirection direction, bool isSquare)
	{
		float num = (isSquare ? 45 : 90);
		float num2 = ((direction != RotationDirection.AntiClockwise) ? 1 : (-1));
		return Mathf.Repeat(startRotation + num2 * num, 360f);
	}
}
