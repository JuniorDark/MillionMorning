using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.Template;
using Code.World.CharacterShop;
using Core;
using UnityEngine;

namespace Code.World.Home;

public class MilMo_ShopRoomLoader
{
	public delegate void RoomLoaded(bool success, MilMo_ActiveRoom room);

	private readonly RoomLoaded _callback;

	private readonly MilMo_ShopRoomTemplate _shopRoomTemplate;

	private MilMo_RoomPresetTemplate _roomPresetTemplate;

	private int _loadingEquipment;

	private MilMo_ActiveRoom _room;

	private bool _loadingDone;

	private bool _hasStartedLoadingAllEquipment;

	public static void AsyncLoad(MilMo_ShopRoomTemplate shopRoomTemplate, RoomLoaded callback)
	{
		new MilMo_ShopRoomLoader(shopRoomTemplate, callback).LoadInternal();
	}

	private MilMo_ShopRoomLoader(MilMo_ShopRoomTemplate shopRoomTemplate, RoomLoaded callback)
	{
		_shopRoomTemplate = shopRoomTemplate;
		_callback = callback;
	}

	private void LoadInternal()
	{
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(_shopRoomTemplate.RoomPresetTemplateReference, RoomPresetLoaded);
	}

	private void RoomPresetLoaded(MilMo_Template template, bool timeout)
	{
		_roomPresetTemplate = template as MilMo_RoomPresetTemplate;
		if (template == null || timeout || _roomPresetTemplate == null)
		{
			_callback(success: false, null);
			return;
		}
		_room = new MilMo_ActiveRoom(0L, 0L, movingFurnitureAllowed: false, isStartRoom: false, MilMo_Home.HomeOffset + new Vector3(100f, 0f, 100f));
		_loadingEquipment++;
		HomeEquipment equipmentData = new HomeEquipment(0L, "", 0f, 0, 0L, "Room", _roomPresetTemplate.RoomTemplateReference, new List<string>());
		_room.Read(equipmentData, delegate(bool success)
		{
			if (!success)
			{
				FinishLoad(success: false);
			}
			else
			{
				_loadingEquipment--;
				if (_hasStartedLoadingAllEquipment && _loadingEquipment <= 0)
				{
					FinishLoad(success: true);
				}
			}
		});
		LoadEquipment(_roomPresetTemplate.Door);
		foreach (HomeEquipment item in _roomPresetTemplate.Equipment)
		{
			LoadEquipment(item);
		}
		_hasStartedLoadingAllEquipment = true;
	}

	private void LoadEquipment(HomeEquipment item)
	{
		if (item.GetTemplateType() == "FloorFurniture" || item.GetTemplateType() == "WallFurniture" || item.GetTemplateType() == "AttachableFurniture")
		{
			_loadingEquipment++;
			MilMo_HomeFurniture obj = new MilMo_HomeFurniture(pickupDeliveryBoxAllowed: false);
			obj.Read(item, delegate(bool success)
			{
				if (!success)
				{
					_loadingEquipment--;
					Debug.LogWarning($"Failed to load equipment item {item.GetTemplate().GetPath()} with id {item.GetId()}");
					if (_hasStartedLoadingAllEquipment && _loadingEquipment <= 0)
					{
						FinishLoad(success: true);
					}
				}
				else
				{
					_room.AddFurniture(obj, forcePlace: false);
					_loadingEquipment--;
					if (_hasStartedLoadingAllEquipment && _loadingEquipment <= 0)
					{
						FinishLoad(success: true);
					}
				}
			});
		}
		else if (item.GetTemplateType() == "Floor")
		{
			_room.SetFloor(item);
		}
		else if (item.GetTemplateType() == "Wallpaper")
		{
			_room.SetWallpaper(item);
		}
	}

	private void FinishLoad(bool success)
	{
		if (!_loadingDone)
		{
			_loadingDone = true;
			if (!success)
			{
				_room.Unload();
				_callback(success: false, null);
			}
			else
			{
				_callback(success: true, _room);
			}
		}
	}
}
