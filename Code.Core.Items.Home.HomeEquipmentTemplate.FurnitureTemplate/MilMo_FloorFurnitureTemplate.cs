using System;
using System.Collections.Generic;
using Code.Core.Items.Home.FurnitureGrid;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture;
using Code.Core.Network.types;
using UnityEngine;

namespace Code.Core.Items.Home.HomeEquipmentTemplate.FurnitureTemplate;

public class MilMo_FloorFurnitureTemplate : MilMo_FurnitureTemplate
{
	private readonly List<MilMo_FurnitureFloorGrid> _mRotatedGrids = new List<MilMo_FurnitureFloorGrid>();

	private bool _mIsCarpet;

	public bool IsSquare { get; private set; }

	public string MoveIdleAnimation
	{
		get
		{
			if (!IsSmall)
			{
				return "MoveLargeIdle";
			}
			return "MoveSmallIdle";
		}
	}

	public string PullAnimation
	{
		get
		{
			if (!IsSmall)
			{
				return "MoveLargePull";
			}
			return "MoveSmallPull";
		}
	}

	public string PushAnimation
	{
		get
		{
			if (!IsSmall)
			{
				return "MoveLargePush";
			}
			return "MoveSmallPush";
		}
	}

	public string StrafeAnimation
	{
		get
		{
			if (!IsSmall)
			{
				return "MoveLargeStrafe";
			}
			return "MoveSmallStrafe";
		}
	}

	public bool IsSmall { get; private set; }

	public override bool IsCarpet => _mIsCarpet;

	protected MilMo_FloorFurnitureTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public static MilMo_FloorFurnitureTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_FloorFurnitureTemplate(category, path, filePath, "FloorFurniture");
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_FloorFurniture(this, modifiers);
	}

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		if (!base.LoadFromNetwork(t))
		{
			return false;
		}
		if (t is Code.Core.Network.types.FloorFurnitureTemplate floorFurnitureTemplate)
		{
			IsSquare = floorFurnitureTemplate.GetGrid().GetIsSquare() != 0;
			_mRotatedGrids.Add(new MilMo_FurnitureFloorGrid(floorFurnitureTemplate.GetGrid()));
			IsSmall = floorFurnitureTemplate.GetIsSmall() != 0;
			_mIsCarpet = floorFurnitureTemplate.GetIsCarpet() != 0;
		}
		if (IsSquare)
		{
			return true;
		}
		_mRotatedGrids.Add(_mRotatedGrids[_mRotatedGrids.Count - 1].GetRotatedCopy());
		_mRotatedGrids.Add(_mRotatedGrids[_mRotatedGrids.Count - 1].GetRotatedCopy());
		_mRotatedGrids.Add(_mRotatedGrids[_mRotatedGrids.Count - 1].GetRotatedCopy());
		return true;
	}

	public MilMo_FurnitureFloorGrid GetGridForRotation(float rotation)
	{
		if (IsSquare)
		{
			return _mRotatedGrids[0];
		}
		if (Mathf.Approximately(rotation, 0f))
		{
			return _mRotatedGrids[0];
		}
		if (Mathf.Approximately(rotation, 90f))
		{
			return _mRotatedGrids[1];
		}
		if (Mathf.Approximately(rotation, 180f))
		{
			return _mRotatedGrids[2];
		}
		if (Mathf.Approximately(rotation, 270f))
		{
			return _mRotatedGrids[3];
		}
		throw new ArgumentOutOfRangeException("A non-square piece of furniture can only be rotated in 90-degree-steps");
	}
}
