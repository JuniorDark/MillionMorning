using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Config;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.Items;
using Code.Core.Items.Home;
using Code.Core.Items.Home.GridCells;
using Code.Core.Items.Home.HomeEquipment;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.HomeSurface;
using Code.Core.Items.Home.HomeEquipmentTemplate.FurnitureTemplate;
using Code.Core.Items.Home.HomeEquipmentTemplate.FurnitureTemplate.AttachableFurnitureTemplate;
using Code.Core.Items.Home.HomeEquipmentTemplate.HomeSurfaceTemplate;
using Code.Core.Network;
using Code.Core.Network.messages.client;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ObjectEffectSystem;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.Core.Visual;
using Code.World.GUI;
using Code.World.GUI.Homes;
using Code.World.Player;
using Core;
using UnityEngine;

namespace Code.World.Home;

public class MilMo_ActiveRoom : MilMo_HomeObject
{
	public class EntryPointData
	{
		public Vector3 Pos;

		public Quaternion Rot;
	}

	private readonly Vector3 _offsetFromOrigo;

	private readonly int[] _tileSize = new int[2];

	private readonly List<GameObject> _debugGridLines = new List<GameObject>();

	private long _currentWallpaperId;

	private long _currentFloorId;

	private long _entranceId;

	private readonly List<MilMo_HomeFurniture> _attachablesWaitingForNode = new List<MilMo_HomeFurniture>();

	private readonly bool _movingFurnitureAllowed;

	private readonly bool _isStartRoom;

	private readonly List<MilMoHomeDoorMarker> _entranceMarkers = new List<MilMoHomeDoorMarker>();

	public bool StartRoom => _item.Modifiers.ContainsKey("StartRoom");

	public long Id { get; }

	public Collider Collider { get; private set; }

	public Dictionary<long, MilMo_HomeFurniture> Furniture { get; }

	public MilMo_RoomGrid Grid { get; private set; }

	public MilMo_Floor Floor { get; private set; }

	public MilMo_Wallpaper Wallpaper { get; private set; }

	public float[] WallPositions { get; }

	public EntryPointData EntryPoint => GetDoorEntryPointData(_entranceId);

	public MilMo_HomeFurniture Entrance
	{
		get
		{
			if (!Furniture.ContainsKey(_entranceId))
			{
				return null;
			}
			return Furniture[_entranceId];
		}
	}

	public MilMo_VisualRep VisualRep { get; private set; }

	public MilMo_ActiveRoom(long id, long entranceId, bool movingFurnitureAllowed, bool isStartRoom, Vector3 offsetFromOrigo)
	{
		WallPositions = new float[4];
		Furniture = new Dictionary<long, MilMo_HomeFurniture>();
		Id = id;
		_entranceId = entranceId;
		_movingFurnitureAllowed = movingFurnitureAllowed;
		_isStartRoom = isStartRoom;
		_offsetFromOrigo = offsetFromOrigo;
	}

	public EntryPointData GetDoorEntryPointData(long doorId)
	{
		EntryPointData entryPointData = new EntryPointData();
		if (Furniture.TryGetValue(doorId, out var value))
		{
			FloorGridCell gridCellAtPosition = Grid.GetGridCellAtPosition(value.GameObject.transform.position);
			MilMo_Furniture item = value.Item;
			MilMo_PlayerControllerHome.TargetTile targetTile = ((item is MilMo_WallFurniture furniture) ? Grid.GetClosestFreeTileNextToFurniture(furniture, gridCellAtPosition) : ((item is MilMo_FloorFurniture furniture2) ? Grid.GetClosestFreeTileNextToFurniture(furniture2, gridCellAtPosition) : ((!(item is MilMo_AttachableFurniture furniture3)) ? null : Grid.GetClosestFreeTileNextToFurniture(furniture3, gridCellAtPosition))));
			MilMo_PlayerControllerHome.TargetTile targetTile2 = targetTile;
			if (targetTile2 != null)
			{
				entryPointData.Pos = new Vector3(1f * (float)targetTile2.Col + 0.5f, 0.01f, -1f * (float)targetTile2.Row - 0.5f) + _offsetFromOrigo;
				Vector3 vector = entryPointData.Pos - value.Item.GameObject.transform.position;
				vector.y = 0f;
				entryPointData.Rot = Quaternion.LookRotation(vector.normalized);
			}
			else
			{
				entryPointData.Pos = new Vector3(1f, 0.01f, -1f) + _offsetFromOrigo;
				entryPointData.Rot = Quaternion.LookRotation(Vector3.right, Vector3.up);
			}
		}
		else
		{
			entryPointData.Pos = new Vector3(1f, 0.01f, -1f) + _offsetFromOrigo;
			entryPointData.Rot = Quaternion.LookRotation(Vector3.right, Vector3.up);
		}
		return entryPointData;
	}

	public bool HasItem(long id)
	{
		if (Furniture.ContainsKey(id))
		{
			return true;
		}
		if (Floor != null && Floor.Id == id)
		{
			return true;
		}
		if (Wallpaper != null && Wallpaper.Id == id)
		{
			return true;
		}
		return false;
	}

	public void AddFurniture(MilMo_HomeFurniture furniture, bool forcePlace)
	{
		if (Furniture.ContainsKey(furniture.Item.Id))
		{
			furniture.Unload();
			return;
		}
		Furniture.Add(furniture.Item.Id, furniture);
		furniture.GrowFromGround();
		furniture.DoPlaceEffect();
		if (furniture.Item.Id == _entranceId)
		{
			if (_isStartRoom)
			{
				furniture.ActivateHomeExit();
			}
			else if (furniture.Item.Template.IsDoor)
			{
				furniture.ActivateRoomEntrance();
			}
			furniture.Item.IsRoomEntrance = true;
		}
		if (Grid != null && !TryPlaceFurniture(furniture, forcePlace))
		{
			RemoveAndMoveToStorage(furniture);
		}
		else
		{
			CheckForWaitingAttachables(furniture);
		}
	}

	public void ActivateFurniture(long furnitureId, short stateIndex)
	{
		if (!Furniture.TryGetValue(furnitureId, out var value))
		{
			Debug.LogWarning($"Got unknown furniture {furnitureId} in activation request");
		}
		else
		{
			value.Activate(stateIndex);
		}
	}

	public void MuteAllFurniture()
	{
		foreach (MilMo_HomeFurniture value in Furniture.Values)
		{
			value.Mute();
		}
	}

	public void UnmuteAllFurniture()
	{
		foreach (MilMo_HomeFurniture value in Furniture.Values)
		{
			value.Unmute();
		}
	}

	public List<MilMo_HomeEquipment> RemoveItem(long id)
	{
		List<MilMo_HomeEquipment> list = new List<MilMo_HomeEquipment>();
		if (Furniture.ContainsKey(id))
		{
			MilMo_HomeFurniture milMo_HomeFurniture = Furniture[id];
			foreach (MilMo_HomeFurniture.AttachNode attachNode in milMo_HomeFurniture.AttachNodes)
			{
				if (attachNode.AttachedFurniture != null)
				{
					attachNode.AttachedFurniture.Unload();
					Furniture.Remove(attachNode.AttachedFurniture.Item.Id);
					list.Add(attachNode.AttachedFurniture.Item);
				}
			}
			if (milMo_HomeFurniture.Item is MilMo_AttachableFurniture { IsOnFurniture: not false } milMo_AttachableFurniture)
			{
				GetFurniture(milMo_AttachableFurniture.AttachNode.FurnitureId).Detach(milMo_HomeFurniture);
			}
			Grid?.Remove(milMo_HomeFurniture.Item);
			milMo_HomeFurniture.Unload();
			Furniture.Remove(milMo_HomeFurniture.Item.Id);
			list.Add(milMo_HomeFurniture.Item);
		}
		else if (Floor != null && Floor.Id == id)
		{
			MilMo_HomeEquipment floor = Floor;
			Floor = null;
			if (_currentFloorId == id)
			{
				_currentFloorId = -1L;
			}
			list.Add(floor);
		}
		else if (Wallpaper != null && Wallpaper.Id == id)
		{
			MilMo_HomeEquipment wallpaper = Wallpaper;
			Wallpaper = null;
			if (_currentWallpaperId == id)
			{
				_currentWallpaperId = -1L;
			}
			list.Add(wallpaper);
		}
		else if (_currentWallpaperId == id)
		{
			_currentWallpaperId = -1L;
		}
		else if (_currentFloorId == id)
		{
			_currentFloorId = -1L;
		}
		for (int num = _attachablesWaitingForNode.Count - 1; num >= 0; num--)
		{
			if (_attachablesWaitingForNode[num].Item.Id == id)
			{
				_attachablesWaitingForNode.RemoveAt(num);
			}
			else if (_attachablesWaitingForNode[num].Item is MilMo_AttachableFurniture { IsOnFurniture: not false } milMo_AttachableFurniture2 && milMo_AttachableFurniture2.AttachNode.FurnitureId == id)
			{
				list.Add(milMo_AttachableFurniture2);
				Furniture.Remove(milMo_AttachableFurniture2.Id);
				_attachablesWaitingForNode[num].Unload();
				_attachablesWaitingForNode.RemoveAt(num);
			}
		}
		return list;
	}

	public void SetFloor(HomeEquipment equipment)
	{
		if (equipment.GetTemplateType() != "Floor")
		{
			return;
		}
		_currentFloorId = equipment.GetId();
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(equipment.GetTemplate(), delegate(MilMo_Template template, bool timeOut)
		{
			if (template == null || timeOut)
			{
				Debug.LogWarning("Failed to apply floor " + equipment.GetTemplate().GetPath() + ". Failed to load template.");
			}
			else if (equipment.GetId() == _currentFloorId)
			{
				if (!(template is MilMo_FloorTemplate milMo_FloorTemplate))
				{
					Debug.LogWarning("Failed to apply floor " + equipment.GetTemplate().GetPath() + ". Wrong template type.");
				}
				else
				{
					Floor = (MilMo_Floor)milMo_FloorTemplate.Instantiate(MilMo_Item.ReadModifiers(equipment.GetModifiers()));
					Floor.Read(equipment);
					Floor.AsyncLoadContent(ApplyFloor);
				}
			}
		});
	}

	private void ApplyFloor()
	{
		MilMo_EventSystem.NextFrame(delegate
		{
			if (Floor != null && Floor.Id == _currentFloorId && Floor.HomePackSurface.IsLoaded && VisualRep != null && !(VisualRep.GameObject == null) && VisualRep.MaterialsFinished && VisualRep.Materials.Count != 0)
			{
				Floor.Apply(VisualRep.GameObject);
				Texture2D texture = Floor.GetTexture(VisualRep.GameObject);
				VisualRep.Materials[0].Material.mainTexture = texture;
			}
		});
	}

	public void SetWallpaper(HomeEquipment equipment)
	{
		if (equipment.GetTemplateType() != "Wallpaper")
		{
			return;
		}
		_currentWallpaperId = equipment.GetId();
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(equipment.GetTemplate(), delegate(MilMo_Template template, bool timeOut)
		{
			if (template == null || timeOut)
			{
				Debug.LogWarning("Failed to apply wallpaper " + equipment.GetTemplate().GetPath() + ". Failed to load template.");
			}
			else if (equipment.GetId() == _currentWallpaperId)
			{
				if (!(template is MilMo_WallpaperTemplate milMo_WallpaperTemplate))
				{
					Debug.LogWarning("Failed to apply wallpaper " + equipment.GetTemplate().GetPath() + ". Wrong template type.");
				}
				else
				{
					Wallpaper = (MilMo_Wallpaper)milMo_WallpaperTemplate.Instantiate(MilMo_Item.ReadModifiers(equipment.GetModifiers()));
					Wallpaper.Read(equipment);
					Wallpaper.AsyncLoadContent(ApplyWallpaper);
				}
			}
		});
	}

	private void ApplyWallpaper()
	{
		MilMo_EventSystem.NextFrame(delegate
		{
			if (Wallpaper != null && Wallpaper.Id == _currentWallpaperId && Wallpaper.HomePackSurface.IsLoaded && VisualRep != null && !(VisualRep.GameObject == null) && VisualRep.MaterialsFinished && VisualRep.Materials.Count >= 2)
			{
				Wallpaper.Apply(VisualRep.GameObject);
				Texture2D texture = Wallpaper.GetTexture(VisualRep.GameObject);
				VisualRep.Materials[1].Material.mainTexture = texture;
			}
		});
	}

	public void MoveEquipment(ServerMoveHomeEquipment msg)
	{
		if (!Furniture.ContainsKey(msg.getId()) || Grid == null)
		{
			return;
		}
		MilMo_HomeFurniture furniture = Furniture[msg.getId()];
		MilMo_Furniture item = furniture.Item;
		MilMo_FloorFurniture milMo_FloorFurniture = item as MilMo_FloorFurniture;
		if (milMo_FloorFurniture == null)
		{
			if (!(item is MilMo_WallFurniture furniture2))
			{
				if (!(item is MilMo_AttachableFurniture milMo_AttachableFurniture))
				{
					return;
				}
				bool flag = !milMo_AttachableFurniture.Tile.ToString().Equals(msg.getGridCell());
				if (milMo_AttachableFurniture.IsOnFurniture && flag)
				{
					GetFurniture(milMo_AttachableFurniture.AttachNode.FurnitureId)?.Detach(furniture);
				}
				else if (milMo_AttachableFurniture.IsOnFloor)
				{
					Grid.Remove(milMo_AttachableFurniture);
				}
				milMo_AttachableFurniture.Tile = GridCell.Parse(msg.getGridCell());
				if (milMo_AttachableFurniture.IsOnFurniture)
				{
					furniture.Rotation = msg.getRotation();
					if (furniture != MilMo_PlayerControllerHome.SelectedFurniture && flag)
					{
						AttachAttachable(furniture);
					}
				}
				else
				{
					if (!milMo_AttachableFurniture.IsOnFloor)
					{
						return;
					}
					furniture.Rotation = msg.getRotation();
					furniture.SetPositionFromGridCell(this);
					if (furniture != MilMo_PlayerControllerHome.SelectedFurniture)
					{
						try
						{
							Grid.Add(milMo_AttachableFurniture);
							return;
						}
						catch (MilMo_Home.FurnitureGridException ex)
						{
							Debug.LogWarning($"Failed to move furniture {furniture.Item.Id}: {ex.Message}");
							RemoveAndMoveToStorage(furniture);
							return;
						}
					}
				}
				return;
			}
			Grid.Remove(furniture2);
			furniture.Item.Tile = GridCell.Parse(msg.getGridCell());
			furniture.Rotation = msg.getRotation();
			furniture.SetPositionFromGridCell(this);
			if (furniture == MilMo_PlayerControllerHome.SelectedFurniture)
			{
				return;
			}
			furniture.DoPlaceEffect();
			try
			{
				Grid.Add(furniture2);
				return;
			}
			catch (MilMo_Home.FurnitureGridException ex2)
			{
				Debug.LogWarning($"Failed to move furniture {furniture.Item.Id}: {ex2.Message}");
				RemoveAndMoveToStorage(furniture);
				return;
			}
		}
		Grid.Remove(milMo_FloorFurniture);
		furniture.Item.Tile = GridCell.Parse(msg.getGridCell());
		bool flag2 = furniture == MilMo_PlayerControllerHome.SelectedFurniture && !milMo_FloorFurniture.Template.IsSquare && (double)Math.Abs(furniture.Rotation - msg.getRotation()) > 0.001;
		furniture.Rotation = msg.getRotation();
		try
		{
			furniture.SetPositionFromGridCell(this);
		}
		catch (Exception ex3)
		{
			Debug.LogWarning($"Failed to move furniture {furniture.Item.Id}: {ex3.Message}");
			RemoveAndMoveToStorage(furniture);
			return;
		}
		if (furniture != MilMo_PlayerControllerHome.SelectedFurniture)
		{
			try
			{
				Grid.Add(milMo_FloorFurniture);
			}
			catch (MilMo_Home.FurnitureGridException ex4)
			{
				Debug.LogWarning($"Failed to move furniture {furniture.Item.Id}: {ex4.Message}");
				RemoveAndMoveToStorage(furniture);
				return;
			}
		}
		if (!flag2)
		{
			return;
		}
		MilMo_EventSystem.At(0.25f, delegate
		{
			try
			{
				Grid.Add(milMo_FloorFurniture);
				MilMo_EventSystem.Instance.PostEvent("go_to_furniture", furniture);
			}
			catch (MilMo_Home.FurnitureGridException ex5)
			{
				Debug.LogWarning($"Failed to move furniture {furniture.Item.Id}: {ex5.Message}");
				RemoveAndMoveToStorage(furniture);
			}
		});
	}

	private void RemoveAndMoveToStorage(MilMo_HomeFurniture furniture)
	{
		List<MilMo_HomeEquipment> list = RemoveItem(furniture.Item.Id);
		if (!_movingFurnitureAllowed)
		{
			return;
		}
		foreach (MilMo_HomeEquipment item in list)
		{
			Singleton<GameNetwork>.Instance.RequestMoveHomeEquipment(item.Id, "", item.Rotation, inStorage: true, 0L);
		}
	}

	private bool TryPlaceFurniture(MilMo_HomeFurniture furniture, bool forcePlace)
	{
		if (Grid == null)
		{
			return false;
		}
		try
		{
			furniture.SetPositionFromGridCell(this);
		}
		catch (ArgumentException)
		{
			Debug.LogWarning($"Failed to place furniture {furniture.Item.Id}: failed to set position from grid cell");
			return false;
		}
		while (true)
		{
			try
			{
				Grid.Add(furniture.Item);
				return true;
			}
			catch (MilMo_Home.FurnitureGridException ex2)
			{
				if (forcePlace && ex2.ConflictingFurniture != null)
				{
					MilMo_HomeFurniture furniture2 = GetFurniture(ex2.ConflictingFurniture.Id);
					if (furniture2 != null)
					{
						RemoveItem(furniture2.Item.Id);
						continue;
					}
					Debug.LogWarning($"Failed to place furniture {furniture.Item.Id}: {ex2.Message}");
					return false;
				}
				Debug.LogWarning($"Failed to place furniture {furniture.Item.Id}: {ex2.Message}");
				return false;
			}
		}
	}

	public void AttachAttachable(MilMo_HomeFurniture attachable)
	{
		AttachNode attachNode = ((MilMo_AttachableFurniture)attachable.Item).AttachNode;
		if (attachNode == null)
		{
			return;
		}
		MilMo_HomeFurniture furniture = GetFurniture(attachNode.FurnitureId);
		if (furniture != null)
		{
			if (!furniture.Attach(attachable, attachNode.NodeIndex) && _movingFurnitureAllowed)
			{
				Singleton<GameNetwork>.Instance.RequestMoveHomeEquipment(attachable.Item.Id, "", attachable.Rotation, inStorage: true, 0L);
			}
		}
		else
		{
			_attachablesWaitingForNode.Add(attachable);
			attachable.Position = new Vector3(1000f, 1000f, 1000f);
		}
	}

	public void HomeDeliveryBoxPickedUp(long itemId)
	{
		MilMo_HomeFurniture furniture = GetFurniture(itemId);
		if (furniture == null || !furniture.Item.Template.IsHomeDeliveryBox)
		{
			Debug.LogWarning("Got null or invalid box ID in home delivery box pickup message.");
			return;
		}
		IList<string> pickupEffects = ((MilMo_HomeDeliveryBoxTemplate)furniture.Item.Template).PickupEffects;
		float num = 0f;
		foreach (string item in pickupEffects)
		{
			MilMo_ObjectEffect objectEffect = MilMo_ObjectEffectSystem.GetObjectEffect(furniture.GameObject, item);
			if (objectEffect != null)
			{
				furniture.AddObjectEffect(objectEffect);
				num = Mathf.Max(num, objectEffect.Duration);
			}
		}
		MilMo_EventSystem.At(num, delegate
		{
			RemoveItem(itemId);
		});
	}

	private void CheckForWaitingAttachables(MilMo_HomeFurniture furniture)
	{
		for (int num = _attachablesWaitingForNode.Count - 1; num >= 0; num--)
		{
			if (_attachablesWaitingForNode[num].Item is MilMo_AttachableFurniture { IsOnFurniture: not false } milMo_AttachableFurniture && furniture.Item.Id == milMo_AttachableFurniture.AttachNode.FurnitureId)
			{
				if (!furniture.Attach(_attachablesWaitingForNode[num], milMo_AttachableFurniture.AttachNode.NodeIndex) && _movingFurnitureAllowed)
				{
					Singleton<GameNetwork>.Instance.RequestMoveHomeEquipment(milMo_AttachableFurniture.Id, "", milMo_AttachableFurniture.Rotation, inStorage: true, 0L);
				}
				_attachablesWaitingForNode.RemoveAt(num);
			}
		}
	}

	public override void Unload()
	{
		Debug.Log("Unload active room");
		base.Unload();
		HideDoorArrows();
		if (Collider != null && Collider.gameObject != null)
		{
			Collider.gameObject.layer = 2;
		}
		MilMo_VisualRepContainer.RemoveFromUpdate(VisualRep);
		MilMo_VisualRepContainer.DestroyVisualRep(VisualRep);
		VisualRep = null;
		foreach (GameObject debugGridLine in _debugGridLines)
		{
			MilMo_Global.Destroy(debugGridLine);
		}
		_debugGridLines.Clear();
		foreach (MilMo_HomeFurniture value in Furniture.Values)
		{
			value.Unload();
		}
		Furniture.Clear();
	}

	public override void Update()
	{
		base.Update();
		if (VisualRep == null)
		{
			return;
		}
		VisualRep.Update();
		foreach (MilMo_HomeFurniture value in Furniture.Values)
		{
			value.Update();
		}
	}

	public void FixedUpdate()
	{
		foreach (MilMo_HomeFurniture value in Furniture.Values)
		{
			value.FixedUpdate();
		}
	}

	public Vector3 GetGridCellPosition(GridCell gridCell)
	{
		if (Grid == null)
		{
			throw new InvalidOperationException("The grid has not been created yet.");
		}
		if (gridCell == null)
		{
			throw new ArgumentNullException("gridCell", "Invalid grid cell : Grid cell is null.");
		}
		string text = "";
		if (!(gridCell is FloorGridCell floorGridCell))
		{
			if (gridCell is WallGridCell { WallIndex: var wallIndex } wallGridCell)
			{
				int num = ((wallIndex == 0 || wallIndex == 2) ? 1 : 0);
				int wallIndex2 = wallGridCell.WallIndex;
				int num2 = ((wallIndex2 == 1 || wallIndex2 == 3) ? 1 : 0);
				Vector3 vector = new Vector3((float)num2 * WallPositions[wallGridCell.WallIndex], 0f, (float)(-num) * WallPositions[wallGridCell.WallIndex]);
				if (num * wallGridCell.Tile <= _tileSize[0] && num2 * wallGridCell.Tile <= _tileSize[1])
				{
					return vector + new Vector3(num * wallGridCell.Tile, 1.8f, -num2 * wallGridCell.Tile) + _offsetFromOrigo;
				}
				text = "Tile is outside room. (Tile is [" + num * wallGridCell.Tile + ", " + num2 * wallGridCell.Tile + "], room size is [" + _tileSize[0] + ", " + _tileSize[1] + "].)";
			}
		}
		else
		{
			if (floorGridCell.Col <= _tileSize[0] && floorGridCell.Row <= _tileSize[1])
			{
				return new Vector3((float)floorGridCell.Col * 1f, 0f, (float)(-floorGridCell.Row) * 1f) + _offsetFromOrigo;
			}
			text = "Tile is outside room. (Tile is [" + floorGridCell.Col + ", " + floorGridCell.Row + "], room size is [" + _tileSize[0] + ", " + _tileSize[1] + "].)";
		}
		Debug.LogWarning($"Trying to fetch position for invalid grid cell {gridCell} in home room {_item.Id}: {text}");
		throw new ArgumentException("Invalid grid cell " + gridCell?.ToString() + ": " + text);
	}

	public Vector3 GetPositionOnWall(int wallIndex, float tile, float height)
	{
		int num = ((wallIndex == 0 || wallIndex == 2) ? 1 : 0);
		int num2 = ((wallIndex == 1 || wallIndex == 3) ? 1 : 0);
		Vector3 vector = new Vector3((float)num2 * WallPositions[wallIndex], 0f, (float)(-num) * WallPositions[wallIndex]);
		if ((float)num * tile <= (float)_tileSize[0] && (float)num2 * tile <= (float)_tileSize[1])
		{
			return vector + new Vector3((float)num * tile, height, (float)(-num2) * tile) + _offsetFromOrigo;
		}
		throw new ArgumentException("Tile is outside room. (Tile is [" + (float)num * tile + ", " + (float)num2 * tile + "], room size is [" + _tileSize[0] + ", " + _tileSize[1] + "].)");
	}

	protected override void AsyncLoad(GameObjectDone callback)
	{
		_spawnPosition = _offsetFromOrigo;
		MilMo_VisualRepContainer.AsyncCreateVisualRep(_item.Template.VisualRep, null, _spawnPosition, Quaternion.Euler(0f, _targetRotation, 0f), new Vector3(1f, 1f, 1f), "Home", MilMo_ResourceManager.Priority.High, delegate(MilMo_VisualRep visualRep)
		{
			if (visualRep == null)
			{
				Debug.LogWarning("Failed to load visual rep " + _item.Template.VisualRep + " for home equipment " + _item.Template.Path);
				callback(success: false);
			}
			else
			{
				VisualRep = visualRep;
				VisualRep.RegisterMaterialsDoneCallback(delegate
				{
					ApplyFloor();
					ApplyWallpaper();
				});
				bool success = FinishLoad();
				callback(success);
			}
		});
	}

	protected override bool FinishLoad()
	{
		if (VisualRep == null)
		{
			return false;
		}
		_gameObject = VisualRep.GameObject;
		if (!base.FinishLoad())
		{
			Unload();
			return false;
		}
		VisualRep.SetLayerOnRenderObject(10);
		_gameObject.transform.position = _offsetFromOrigo;
		Collider = _gameObject.GetComponentInChildren<Collider>();
		if (Collider != null)
		{
			Collider.gameObject.layer = 26;
		}
		return CreateGrid();
	}

	private bool CreateGrid()
	{
		Vector3 origin = new Vector3(1f, 1.8f, -1f) + _offsetFromOrigo;
		if (Physics.Raycast(origin, new Vector3(0f, 0f, 1f), out var hitInfo, 2000f, 67108864) && hitInfo.collider == Collider)
		{
			WallPositions[0] = 0f - (hitInfo.distance - 1f);
			if (Physics.Raycast(origin, new Vector3(1f, 0f, 0f), out hitInfo, 1000f, 67108864) && hitInfo.collider == Collider)
			{
				WallPositions[1] = hitInfo.distance + 1f;
				if (Physics.Raycast(origin, new Vector3(0f, 0f, -1f), out hitInfo, 1000f, 67108864) && hitInfo.collider == Collider)
				{
					WallPositions[2] = hitInfo.distance + 1f;
					if (Physics.Raycast(origin, new Vector3(-1f, 0f, 0f), out hitInfo, 2000f, 67108864) && hitInfo.collider == Collider)
					{
						WallPositions[3] = 0f - (hitInfo.distance - 1f);
						_tileSize[0] = (int)Mathf.Floor(WallPositions[1] / 1f);
						_tileSize[1] = (int)Mathf.Floor(WallPositions[2] / 1f);
						if (_tileSize[0] <= 0 || _tileSize[1] <= 0)
						{
							Debug.LogWarning("Got invalid room size: [" + _tileSize[0] + ", " + _tileSize[1] + "]. Wall positions are: North: " + WallPositions[0] + "; East: " + WallPositions[1] + "; South: " + WallPositions[2] + "; West: " + WallPositions[3]);
							return false;
						}
						Grid = new MilMo_RoomGrid(_tileSize[1], _tileSize[0], _offsetFromOrigo);
						if (MilMo_Config.Instance.IsTrue("Home.ShowGrid", defaultValue: false))
						{
							for (int i = 0; i <= _tileSize[0]; i++)
							{
								float x = (float)i * 1f;
								Vector3 position = new Vector3(x, 1.8f, 0f - (WallPositions[0] + 0.1f)) + _offsetFromOrigo;
								Vector3 position2 = new Vector3(x, 0.01f, 0f - (WallPositions[0] + 0.1f)) + _offsetFromOrigo;
								Vector3 position3 = new Vector3(x, 0.01f, 0f - (WallPositions[2] - 0.1f)) + _offsetFromOrigo;
								Vector3 position4 = new Vector3(x, 1.8f, 0f - (WallPositions[2] - 0.1f)) + _offsetFromOrigo;
								GameObject gameObject = new GameObject("Longitude" + i);
								LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
								lineRenderer.useWorldSpace = true;
								lineRenderer.startWidth = 0.03f;
								lineRenderer.positionCount = 4;
								lineRenderer.SetPosition(0, position);
								lineRenderer.SetPosition(1, position2);
								lineRenderer.SetPosition(2, position3);
								lineRenderer.SetPosition(3, position4);
								lineRenderer.startColor = Color.red;
								_debugGridLines.Add(gameObject);
							}
							for (int j = 0; j <= _tileSize[1]; j++)
							{
								float z = 0f - (float)j * 1f;
								Vector3 position5 = new Vector3(WallPositions[3] + 0.1f, 1.8f, z) + _offsetFromOrigo;
								Vector3 position6 = new Vector3(WallPositions[3] + 0.1f, 0.01f, z) + _offsetFromOrigo;
								Vector3 position7 = new Vector3(WallPositions[1] - 0.1f, 0.01f, z) + _offsetFromOrigo;
								Vector3 position8 = new Vector3(WallPositions[1] - 0.1f, 1.8f, z) + _offsetFromOrigo;
								GameObject gameObject2 = new GameObject("Latitude" + j);
								LineRenderer lineRenderer2 = gameObject2.AddComponent<LineRenderer>();
								lineRenderer2.useWorldSpace = true;
								lineRenderer2.startWidth = 0.03f;
								lineRenderer2.positionCount = 4;
								lineRenderer2.SetPosition(0, position5);
								lineRenderer2.SetPosition(1, position6);
								lineRenderer2.SetPosition(2, position7);
								lineRenderer2.SetPosition(3, position8);
								lineRenderer2.startColor = Color.red;
								_debugGridLines.Add(gameObject2);
							}
						}
						List<MilMo_HomeFurniture> list = new List<MilMo_HomeFurniture>();
						foreach (MilMo_HomeFurniture value in Furniture.Values)
						{
							if (!TryPlaceFurniture(value, forcePlace: false))
							{
								list.Add(value);
							}
						}
						foreach (MilMo_HomeFurniture item in list)
						{
							RemoveAndMoveToStorage(item);
						}
						return true;
					}
					Debug.LogWarning("Wall 3 not found in room mesh " + VisualRep.FullPath);
					return false;
				}
				Debug.LogWarning("Wall 2 not found in room mesh " + VisualRep.FullPath);
				return false;
			}
			Debug.LogWarning("Wall 1 not found in room mesh " + VisualRep.FullPath);
			return false;
		}
		Debug.LogWarning("Wall 0 not found in room mesh " + VisualRep.FullPath);
		return false;
	}

	public bool RequestMoveFromStorage(MilMo_HomeEquipment equipment)
	{
		string targetGridCell;
		return RequestMoveFromStorage(equipment, out targetGridCell);
	}

	public bool RequestMoveFromStorage(MilMo_HomeEquipment equipment, out string targetGridCell)
	{
		targetGridCell = null;
		if (equipment.Template.IsRoom)
		{
			MilMo_Room milMo_Room = (MilMo_Room)equipment;
			if (milMo_Room.Entrance?.TransitionTarget == null)
			{
				Debug.LogWarning($"Trying to move room {milMo_Room.Id} from storage, but the room has no valid door leading to it.");
				return false;
			}
			equipment = milMo_Room.Entrance.TransitionTarget;
		}
		if (!(equipment is MilMo_Furniture milMo_Furniture))
		{
			if (!(equipment is MilMo_Floor) && !(equipment is MilMo_Wallpaper))
			{
				return false;
			}
			Singleton<GameNetwork>.Instance.RequestApplyRoomSkin(equipment.Id, Id);
			return true;
		}
		Vector3 position = MilMo_Player.Instance.Avatar.Position;
		FloorGridCell gridCellAtPosition = GetGridCellAtPosition(position.x, position.z);
		if (gridCellAtPosition == null)
		{
			return false;
		}
		int num = 0;
		float num2 = Mathf.Repeat(MilMo_Player.Instance.Avatar.GameObject.transform.rotation.eulerAngles.y, 360f);
		if (Mathf.Abs(num2) <= 45f)
		{
			num = 0;
		}
		else if (Mathf.Abs(num2 - 90f) <= 45f)
		{
			num = 1;
		}
		else if (Mathf.Abs(num2 - 180f) <= 45f)
		{
			num = 2;
		}
		else if (Mathf.Abs(num2 - 270f) <= 45f)
		{
			num = 3;
		}
		GridCell gridCell = null;
		int num3 = 0;
		if (!(milMo_Furniture is MilMo_WallFurniture furniture))
		{
			if (!(milMo_Furniture is MilMo_FloorFurniture furniture2))
			{
				if (milMo_Furniture is MilMo_AttachableFurniture furniture3)
				{
					gridCell = Grid.Floor.GetClosestSpaceForObject(gridCellAtPosition, furniture3, num);
				}
			}
			else
			{
				num3 = num * 90;
				gridCell = Grid.Floor.GetClosestSpaceForObject(gridCellAtPosition, furniture2, num3, num);
			}
		}
		else
		{
			WallGridCell preferredTile = new WallGridCell
			{
				WallIndex = num,
				Tile = ((num == 0 || num == 2) ? gridCellAtPosition.Col : gridCellAtPosition.Row)
			};
			gridCell = Grid.Walls.GetClosestSpaceForObject(preferredTile, furniture);
		}
		if (gridCell == null)
		{
			return false;
		}
		targetGridCell = gridCell.ToString();
		Singleton<GameNetwork>.Instance.RequestMoveHomeEquipment(milMo_Furniture.Id, gridCell.ToString(), num3, inStorage: false, Id);
		return true;
	}

	public bool RequestRotateFurniture(long id, MilMo_FloorFurniture.RotationDirection direction)
	{
		if (!Furniture.ContainsKey(id))
		{
			return false;
		}
		if (Grid == null)
		{
			return false;
		}
		MilMo_HomeFurniture milMo_HomeFurniture = Furniture[id];
		if (milMo_HomeFurniture.Item is MilMo_WallFurniture)
		{
			return false;
		}
		string gridCell = milMo_HomeFurniture.Item.Tile.ToString();
		float num = 0f;
		MilMo_Furniture item = milMo_HomeFurniture.Item;
		if (!(item is MilMo_AttachableFurniture milMo_AttachableFurniture))
		{
			if (item is MilMo_FloorFurniture milMo_FloorFurniture)
			{
				num = milMo_FloorFurniture.GetNextRotation(direction);
				if (!milMo_FloorFurniture.Template.IsSquare && Grid.TestCollision(milMo_FloorFurniture, milMo_FloorFurniture.Tile, num) != null)
				{
					FloorGridCell floorGridCell = null;
					while (floorGridCell == null && !Mathf.Approximately(num, milMo_FloorFurniture.Rotation))
					{
						floorGridCell = Grid.Floor.GetClosestSpaceForObject(milMo_FloorFurniture.Tile, milMo_FloorFurniture, num, 0);
						if (floorGridCell == null)
						{
							num = MilMo_FloorFurniture.GetNextRotation(num, direction, isSquare: false);
						}
					}
					if (floorGridCell == null)
					{
						return false;
					}
					gridCell = floorGridCell.ToString();
				}
			}
		}
		else
		{
			float num2 = ((direction != MilMo_FloorFurniture.RotationDirection.AntiClockwise) ? 1 : (-1));
			num = Mathf.Repeat(milMo_AttachableFurniture.Rotation + num2 * 45f, 360f);
		}
		Singleton<GameNetwork>.Instance.RequestMoveHomeEquipment(milMo_HomeFurniture.Item.Id, gridCell, num, inStorage: false, Id);
		return true;
	}

	public FloorGridCell GetGridCellAtPosition(float x, float z)
	{
		return Grid?.GetGridCellAtPosition(x, z);
	}

	public MilMo_HomeFurniture GetFurniture(long id)
	{
		Furniture.TryGetValue(id, out var value);
		return value;
	}

	public void SendRandomPositionForHomeDeliveryBox()
	{
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate("Item", "HomeDeliveryBoxBlue", delegate(MilMo_Template template, bool timeOut)
		{
			if (!timeOut && template is MilMo_AttachableFurnitureTemplate milMo_AttachableFurnitureTemplate)
			{
				MilMo_AttachableFurniture box = (MilMo_AttachableFurniture)milMo_AttachableFurnitureTemplate.Instantiate(new Dictionary<string, string>());
				List<string> possiblePositionsForHomeDeliveryBox = Grid.Floor.GetPossiblePositionsForHomeDeliveryBox(box);
				for (int num = possiblePositionsForHomeDeliveryBox.Count - 1; num >= 0; num--)
				{
					if ((GetGridCellPosition(GridCell.Parse(possiblePositionsForHomeDeliveryBox[num])) - EntryPoint.Pos).magnitude < 1f)
					{
						possiblePositionsForHomeDeliveryBox.RemoveAt(num);
					}
				}
				foreach (MilMo_HomeFurniture value in Furniture.Values)
				{
					possiblePositionsForHomeDeliveryBox.AddRange(from attachNode in value.AttachNodes
						where attachNode.AttachedFurniture == null
						select attachNode.ToString());
				}
				bool flag = possiblePositionsForHomeDeliveryBox.Count == 0;
				string gridCell = "";
				float rotation = 0f;
				long inRoom = 0L;
				if (!flag)
				{
					gridCell = possiblePositionsForHomeDeliveryBox[UnityEngine.Random.Range(0, possiblePositionsForHomeDeliveryBox.Count)];
					inRoom = Id;
					rotation = 45 * UnityEngine.Random.Range(0, 8);
				}
				IMessage message = new ClientHomeBoxPosition(inRoom, gridCell, rotation, (sbyte)(flag ? 1 : 0));
				Singleton<GameNetwork>.Instance.SendToGameServer(message);
			}
		});
	}

	public void ChangeEntrance(long newEntranceId, long oldEntranceId)
	{
		_entranceId = newEntranceId;
		base.Item.ChangeModifier("RoomEntrance", newEntranceId.ToString());
		if (Furniture.ContainsKey(newEntranceId))
		{
			Furniture[newEntranceId].ActivateRoomEntrance();
			Furniture[newEntranceId].Item.IsRoomEntrance = true;
		}
		if (Furniture.ContainsKey(oldEntranceId))
		{
			Furniture[oldEntranceId].DeactivateRoomEntrance();
			Furniture[oldEntranceId].Item.IsRoomEntrance = false;
		}
	}

	public void ShowSwapDoorIcon(long doorId, MilMoHomeDoorMarker.OnClickCallback clickCallback)
	{
		MilMo_HomeFurniture furniture = GetFurniture(doorId);
		if (furniture != null && furniture.Item.Template.IsDoor && furniture.Item.OtherSideOfDoorIsDifferent)
		{
			MilMoHomeDoorMarker milMoHomeDoorMarker = new MilMoHomeDoorMarker(MilMo_GlobalUI.GetSystemUI, furniture, "Batch01/Textures/Homes/IconRotateDoor", new Vector2(51f, 51f), clickCallback);
			_entranceMarkers.Add(milMoHomeDoorMarker);
			milMoHomeDoorMarker.Show();
		}
	}

	public void ShowDoorArrows(long doubleClickedDoorId)
	{
		if (_entranceMarkers.Count > 0)
		{
			return;
		}
		foreach (MilMo_HomeFurniture value in Furniture.Values)
		{
			if (value.Item.Template.IsDoor)
			{
				MilMoHomeDoorMarker milMoHomeDoorMarker = new MilMoHomeDoorMarker(MilMo_GlobalUI.GetSystemUI, value, "Batch01/Textures/Homes/ArrowGreen", new Vector2(64f, 64f), delegate(long id)
				{
					Singleton<GameNetwork>.Instance.RequestApplyDoorSkin(doubleClickedDoorId, id);
					HideDoorArrows();
				});
				_entranceMarkers.Add(milMoHomeDoorMarker);
				milMoHomeDoorMarker.Show();
			}
		}
	}

	public void HideDoorArrows()
	{
		foreach (MilMoHomeDoorMarker entranceMarker in _entranceMarkers)
		{
			entranceMarker.Remove();
		}
		_entranceMarkers.Clear();
	}
}
