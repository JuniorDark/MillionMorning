using System;
using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.Items.Home.GridCells;
using Code.Core.Items.Home.HomeEquipmentTemplate;
using Code.Core.Network.types;
using Code.World.Home;

namespace Code.Core.Items.Home;

public abstract class MilMo_HomeEquipment : MilMo_Item, IUsable
{
	public new MilMo_HomeEquipmentTemplate Template => base.Template as MilMo_HomeEquipmentTemplate;

	public long Id { get; private set; }

	public GridCell Tile { get; set; }

	public float Rotation { get; set; }

	public bool InStorage { get; set; }

	public virtual bool VisibleInStorage => InStorage;

	public string StorageCategory => Template.BagCategory;

	public long InRoomId { get; set; }

	public override bool IsWearable()
	{
		return false;
	}

	public override bool IsWieldable()
	{
		return false;
	}

	protected MilMo_HomeEquipment(MilMo_ItemTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public virtual void Read(Code.Core.Network.types.HomeEquipment equipmentData)
	{
		Id = equipmentData.GetId();
		if (!string.IsNullOrEmpty(equipmentData.GetGridCell()))
		{
			Tile = GridCell.Parse(equipmentData.GetGridCell());
		}
		Rotation = equipmentData.GetRotation();
		InStorage = equipmentData.GetInStorage() != 0;
		InRoomId = equipmentData.GetInRoom();
	}

	public bool Use(int entryId)
	{
		if (Template is MilMo_FurnitureTemplate { IsDoor: not false })
		{
			MilMo_Home.CurrentHome.CurrentRoom.ShowDoorArrows(Id);
			MilMo_EventSystem.Instance.PostEvent("enter_furnishing_mode", null);
			return true;
		}
		MilMo_EventSystem.Instance.PostEvent("request_move_from_storage", this);
		return true;
	}

	public void RegisterOnUsed(Action onUsed)
	{
		throw new NotImplementedException();
	}

	public void UnregisterOnUsed(Action onUsed)
	{
		throw new NotImplementedException();
	}

	public void RegisterOnFailedToUse(Action onFail)
	{
		throw new NotImplementedException();
	}

	public void UnregisterOnFailedToUse(Action onFail)
	{
		throw new NotImplementedException();
	}
}
