using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.Items;
using Code.Core.Items.Home;
using Code.Core.Items.Home.GridCells;
using Code.Core.Items.Home.HomeEquipment;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture;
using Code.Core.Music;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Template;
using Code.World.Environment;
using Code.World.GUI.Homes;
using Code.World.GUI.LoadingScreen;
using Code.World.Inventory;
using Code.World.Level;
using Code.World.Player;
using Core;
using Core.Analytics;
using Core.GameEvent;
using Localization;
using UI.HUD.Dialogues;
using UI.Sprites;
using UnityEngine;

namespace Code.World.Home;

public sealed class MilMo_Home : MilMo_Instance
{
	public class FurnitureGridException : Exception
	{
		public MilMo_Furniture ConflictingFurniture { get; }

		public FurnitureGridException(MilMo_Furniture conflictingFurniture, string message)
			: base(message)
		{
			ConflictingFurniture = conflictingFurniture;
		}
	}

	public delegate void LoadingDone(bool success);

	public const float TILE_SIZE = 1f;

	public const float WALL_GRID_HEIGHT = 1.8f;

	public static Vector3 HomeOffset = new Vector3(1f, 0f, 1f) * 100f;

	public static Color AmbientLightColor = new Color(0.8f, 0.77f, 0.89f);

	public static Color DirectionalLightColor = new Color(1f, 0.88f, 0.71f);

	public static float DirectionalLightIntensity = 0.8f;

	public static Vector3 LightDirection = new Vector3(75f, 90f, 90f);

	public static bool RequestLoadNewItemsAfterFinishLoad = false;

	private static bool _inRaffle;

	private bool _allRoomEquipmentArrivedFromServer;

	private int _loadingEquipment;

	private LoadingDone _loadingDoneCallback;

	private readonly Queue<IMessage> _equipmentUpdateQueue = new Queue<IMessage>();

	private readonly List<long> _loadingStorageItems = new List<long>();

	private readonly Dictionary<long, MilMo_Furniture> _storageDoorsWaitingForTarget = new Dictionary<long, MilMo_Furniture>();

	private readonly Dictionary<long, MilMo_Room> _storageRoomsWaitingForEntrance = new Dictionary<long, MilMo_Room>();

	private readonly List<MilMo_InventoryEntry> _entries = new List<MilMo_InventoryEntry>();

	private long _startRoomId = -1L;

	private MilMo_WallFurniture _wallFurnitureMovedFromShopToWall;

	private float _cameraForcedRotation = -1f;

	private float _entryPointRotation;

	private bool _isLocalPlayerHome;

	private bool _firstLoadDone;

	public static MilMo_AudioClip AttachableDetachSound;

	public static MilMo_AudioClip AttachablePlaceSound;

	public static MilMo_AudioClip AttachableRotateSound;

	public static MilMo_AudioClip AttachableGrabSound;

	public static MilMo_AudioClip AttachableReleaseSound;

	public static MilMo_AudioClip CarpetMoveSound;

	public static MilMo_AudioClip CarpetPlaceSound;

	public static MilMo_AudioClip CarpetRotateSound;

	public static MilMo_AudioClip CarpetGrabSound;

	public static MilMo_AudioClip CarpetReleaseSound;

	public static MilMo_AudioClip CurtainAttachSound;

	public static MilMo_AudioClip CurtainDetachSound;

	public static MilMo_AudioClip CurtainMoveSound;

	public static MilMo_AudioClip CurtainGrabSound;

	public static MilMo_AudioClip CurtainReleaseSound;

	public static MilMo_AudioClip FloorItemMoveSound;

	public static MilMo_AudioClip FloorItemPlaceSound;

	public static MilMo_AudioClip FloorItemRotateSound;

	public static MilMo_AudioClip FloorItemGrabSound;

	public static MilMo_AudioClip FloorItemReleaseSound;

	public static MilMo_AudioClip WallItemDetachSound;

	public static MilMo_AudioClip WallItemMoveSound;

	public static MilMo_AudioClip WallItemPlaceSound;

	public static MilMo_AudioClip WallItemGrabSound;

	public static MilMo_AudioClip WallItemReleaseSound;

	public static MilMo_AudioClip WallItemSwooshSound;

	public static MilMo_AudioClip UseableFurnitureToggleSound;

	public static MilMo_AudioClip KeysMenuOpenSound;

	public static MilMo_AudioClip KeysMenuCloseSound;

	public static MilMo_AudioClip FurnitureModeOnSound;

	public static MilMo_AudioClip FurnitureModeOffSound;

	public static MilMo_AudioClip MoveToStorageSound;

	public static MilMo_AudioClip DoorOpenSound;

	public static MilMo_AudioClip DoorCloseSound;

	private bool _haveInitedSounds;

	private MilMo_GenericReaction _equipmentAddedReaction;

	private MilMo_GenericReaction _equipmentMovedReaction;

	private MilMo_GenericReaction _equipmentMovedFromStorageReaction;

	private MilMo_GenericReaction _equipmentMovedFromOtherRoomReaction;

	private MilMo_GenericReaction _applyRoomSkinReaction;

	private MilMo_GenericReaction _roomEntranceChangedReaction;

	private MilMo_GenericReaction _furnitureActivated;

	private MilMo_GenericReaction _playerChangeRoomReaction;

	private MilMo_GenericReaction _generateHomeDeliveryBoxPositionReaction;

	private MilMo_GenericReaction _homeDeliveryBoxPickedUpReaction;

	private MilMo_GenericReaction _homeRoomChangeFailedReaction;

	private string _homeName;

	private MilMo_HomeVote.RatingData _ratingData = new MilMo_HomeVote.RatingData(0f, 0, 15);

	public static bool InRaffle
	{
		get
		{
			return _inRaffle;
		}
		private set
		{
			_inRaffle = value;
			MilMo_Home.RaffleChange?.Invoke(value);
		}
	}

	public Dictionary<long, MilMo_HomeEquipment> Storage { get; } = new Dictionary<long, MilMo_HomeEquipment>();


	public static MilMo_Home CurrentHome => MilMo_Instance.CurrentInstance as MilMo_Home;

	public string HomeName
	{
		get
		{
			return _homeName;
		}
		set
		{
			_homeName = value;
			this.OnHomeNameUpdated?.Invoke();
		}
	}

	public override Type InstanceType => Type.Home;

	public MilMo_ActiveRoom.EntryPointData CurrentEntryPoint => CurrentRoom.EntryPoint;

	public override MilMo_LocString ShopDisplayName => MilMo_Localization.GetLocString("Homes_6819");

	public override Texture2D Icon => null;

	public MilMo_ActiveRoom CurrentRoom { get; private set; }

	public bool HasDefaultEquipmentOnly { get; private set; }

	public string OwnerID { get; private set; }

	public static event Action<bool> RaffleChange;

	public event Action OnHomeNameUpdated;

	public bool IsStartingRoom(int id)
	{
		return id == _startRoomId;
	}

	public void AsyncLoad(string ownerId, string homeName, RoomInfo roomInfo, bool hasDefaultEquipmentOnly, LoadingDone callback)
	{
		if (string.IsNullOrEmpty(MilMo_Instance.PlayerInstance.Id))
		{
			MilMo_EventSystem.Listen("player_info_loaded", delegate
			{
				AsyncLoad(ownerId, homeName, roomInfo, hasDefaultEquipmentOnly, callback);
			});
			return;
		}
		HomeName = homeName;
		if (HomeName == string.Empty)
		{
			HomeName = MilMo_Localization.GetLocString("Homes_13405").String;
		}
		OwnerID = ownerId;
		_isLocalPlayerHome = ownerId == MilMo_Instance.PlayerInstance.Id;
		HasDefaultEquipmentOnly = hasDefaultEquipmentOnly;
		_startRoomId = roomInfo.GetItem().GetId();
		_loadingDoneCallback = callback;
		MilMo_Environment.SceneLightComponent.type = LightType.Directional;
		MilMo_Environment.SceneLightComponent.shadows = LightShadows.None;
		MilMo_Environment.SceneLightComponent.renderMode = LightRenderMode.ForcePixel;
		MilMo_Environment.SceneLightComponent.color = DirectionalLightColor;
		MilMo_Environment.SceneLightComponent.intensity = DirectionalLightIntensity;
		MilMo_Environment.SceneLight.transform.eulerAngles = LightDirection;
		RenderSettings.ambientLight = AmbientLightColor;
		RenderSettings.fog = false;
		Environment.StoreRenderSettings();
		LoadEventListenersQueue();
		if (!_haveInitedSounds)
		{
			InitializeSounds();
		}
		LoadRoom(roomInfo);
	}

	private void LoadRoom(RoomInfo roomInfo)
	{
		MilMo_LoadingScreen.Instance.GotHomeEquipmentCount(roomInfo.GetItemCount() + 1);
		_allRoomEquipmentArrivedFromServer = false;
		CurrentRoom = new MilMo_ActiveRoom(roomInfo.GetItem().GetId(), roomInfo.GetEntrance(), _isLocalPlayerHome, _startRoomId == roomInfo.GetItem().GetId(), HomeOffset);
		_loadingEquipment++;
		CurrentRoom.Read(roomInfo.GetItem(), delegate(bool success)
		{
			if (!success)
			{
				FinishLoad(success: false);
			}
			else
			{
				_loadingEquipment--;
				MilMo_LoadingScreen.Instance.HomeEquipmentLoaded();
				if (_allRoomEquipmentArrivedFromServer && _loadingEquipment <= 0)
				{
					FinishLoad(success: true);
				}
			}
		});
		MilMo_EventSystem.Listen("all_equipment_in_room_sent", delegate
		{
			MilMo_EventSystem.At(1f, delegate
			{
				_allRoomEquipmentArrivedFromServer = true;
				if (_loadingEquipment <= 0)
				{
					FinishLoad(success: true);
				}
			});
		});
		Singleton<GameNetwork>.Instance.RequestHomeEquipment(OwnerID, CurrentRoom.Id);
	}

	private void LoadEventListenersQueue()
	{
		_equipmentAddedReaction = MilMo_EventSystem.Listen("home_equipment_add", EquipmentAdded);
		_equipmentAddedReaction.Repeating = true;
		_equipmentMovedReaction = MilMo_EventSystem.Listen("home_equipment_move", QueueEquipmentUpdate);
		_equipmentMovedReaction.Repeating = true;
		_roomEntranceChangedReaction = MilMo_EventSystem.Listen("room_entrance_changed", QueueEquipmentUpdate);
		_roomEntranceChangedReaction.Repeating = true;
		_applyRoomSkinReaction = MilMo_EventSystem.Listen("apply_room_skin", QueueEquipmentUpdate);
		_applyRoomSkinReaction.Repeating = true;
		_equipmentMovedFromStorageReaction = MilMo_EventSystem.Listen("home_equipment_move_from_storage", QueueEquipmentUpdate);
		_equipmentMovedFromStorageReaction.Repeating = true;
		_equipmentMovedFromOtherRoomReaction = MilMo_EventSystem.Listen("home_equipment_move_from_other_room", QueueEquipmentUpdate);
		_equipmentMovedFromOtherRoomReaction.Repeating = true;
		_homeRoomChangeFailedReaction = MilMo_EventSystem.Listen("home_room_change_failed", RoomChangeFailed);
		_homeRoomChangeFailedReaction.Repeating = true;
	}

	protected override void LoadEventListeners()
	{
		base.LoadEventListeners();
		_equipmentAddedReaction = MilMo_EventSystem.Listen("home_equipment_add", EquipmentAdded);
		_equipmentAddedReaction.Repeating = true;
		_equipmentMovedReaction = MilMo_EventSystem.Listen("home_equipment_move", MoveEquipment);
		_equipmentMovedReaction.Repeating = true;
		_roomEntranceChangedReaction = MilMo_EventSystem.Listen("room_entrance_changed", ChangeRoomEntrance);
		_roomEntranceChangedReaction.Repeating = true;
		_applyRoomSkinReaction = MilMo_EventSystem.Listen("apply_room_skin", ApplyRoomSkin);
		_applyRoomSkinReaction.Repeating = true;
		_equipmentMovedFromStorageReaction = MilMo_EventSystem.Listen("home_equipment_move_from_storage", MoveEquipmentFromStorage);
		_equipmentMovedFromStorageReaction.Repeating = true;
		_equipmentMovedFromOtherRoomReaction = MilMo_EventSystem.Listen("home_equipment_move_from_other_room", MoveEquipmentFromOtherRoom);
		_equipmentMovedFromOtherRoomReaction.Repeating = true;
		_furnitureActivated = MilMo_EventSystem.Listen("home_furniture_activated", FurnitureActivated);
		_furnitureActivated.Repeating = true;
		_generateHomeDeliveryBoxPositionReaction = MilMo_EventSystem.Listen("generate_home_delivery_box_position", GenerateHomeDeliveryBoxPosition);
		_generateHomeDeliveryBoxPositionReaction.Repeating = true;
		_homeDeliveryBoxPickedUpReaction = MilMo_EventSystem.Listen("home_delivery_box_picked_up", HomeDeliveryBoxPickedUp);
		_homeDeliveryBoxPickedUpReaction.Repeating = true;
		_playerChangeRoomReaction = MilMo_EventSystem.Listen("player_change_room_in_home", PlayerChangeRoom);
		_playerChangeRoomReaction.Repeating = true;
		_homeRoomChangeFailedReaction = MilMo_EventSystem.Listen("home_room_change_failed", RoomChangeFailed);
		_homeRoomChangeFailedReaction.Repeating = true;
		MilMo_EventSystem.Listen("home_of_the_day_raffle_response", HomeOfTHeDayRaffleResponse).Repeating = true;
	}

	protected override void UnloadEventListeners()
	{
		base.UnloadEventListeners();
		MilMo_EventSystem.RemoveReaction(_equipmentAddedReaction);
		_equipmentAddedReaction = null;
		MilMo_EventSystem.RemoveReaction(_equipmentMovedReaction);
		_equipmentMovedReaction = null;
		MilMo_EventSystem.RemoveReaction(_roomEntranceChangedReaction);
		_roomEntranceChangedReaction = null;
		MilMo_EventSystem.RemoveReaction(_applyRoomSkinReaction);
		_applyRoomSkinReaction = null;
		MilMo_EventSystem.RemoveReaction(_equipmentMovedFromStorageReaction);
		_equipmentMovedFromStorageReaction = null;
		MilMo_EventSystem.RemoveReaction(_equipmentMovedFromOtherRoomReaction);
		_equipmentMovedFromOtherRoomReaction = null;
		MilMo_EventSystem.RemoveReaction(_furnitureActivated);
		_furnitureActivated = null;
		MilMo_EventSystem.RemoveReaction(_playerChangeRoomReaction);
		_playerChangeRoomReaction = null;
		MilMo_EventSystem.RemoveReaction(_homeRoomChangeFailedReaction);
		_homeRoomChangeFailedReaction = null;
		MilMo_EventSystem.RemoveReaction(_generateHomeDeliveryBoxPositionReaction);
		_generateHomeDeliveryBoxPositionReaction = null;
		MilMo_EventSystem.RemoveReaction(_homeDeliveryBoxPickedUpReaction);
		_homeDeliveryBoxPickedUpReaction = null;
	}

	public override void Unload()
	{
		if (Singleton<Analytics>.Instance != null)
		{
			Singleton<Analytics>.Instance.LevelQuit("Home");
		}
		base.Unload();
		CurrentRoom?.Unload();
		if (MilMo_Instance.PlayerInstance.Avatar != null)
		{
			MilMo_Instance.PlayerInstance.Avatar.Room = "";
		}
		MilMo_UserInterface.CurrentRoom = "";
		_firstLoadDone = false;
	}

	public override void Update()
	{
		CurrentRoom?.Update();
		if (_cameraForcedRotation > 0f)
		{
			MilMo_World.Instance.Camera.homeCameraController.RotateTo(_cameraForcedRotation);
			_cameraForcedRotation = -1f;
		}
		UpdateAvatars();
	}

	public override void FixedUpdate()
	{
		CurrentRoom?.FixedUpdate();
		base.FixedUpdate();
	}

	private void EquipmentAdded(object msgObject)
	{
		if (!(msgObject is ServerHomeEquipment serverHomeEquipment))
		{
			return;
		}
		if (serverHomeEquipment.getIsNew() != 0)
		{
			HasDefaultEquipmentOnly = false;
		}
		foreach (HomeEquipment item in serverHomeEquipment.getItems())
		{
			LoadEquipment(item, serverHomeEquipment.getIsNew() != 0, forcePlace: false);
		}
	}

	private void QueueEquipmentUpdate(object msgAsObj)
	{
		if (msgAsObj is IMessage item)
		{
			_equipmentUpdateQueue.Enqueue(item);
		}
	}

	private void MoveEquipment(object msgAsObj)
	{
		if (!(msgAsObj is ServerMoveHomeEquipment serverMoveHomeEquipment))
		{
			return;
		}
		long id = serverMoveHomeEquipment.getId();
		List<MilMo_HomeEquipment> list = null;
		if (CurrentRoom != null && CurrentRoom.HasItem(id))
		{
			if (serverMoveHomeEquipment.getInRoom() == CurrentRoom.Id && serverMoveHomeEquipment.getInStorage() == 0)
			{
				CurrentRoom.MoveEquipment(serverMoveHomeEquipment);
			}
			else
			{
				list = CurrentRoom.RemoveItem(id);
			}
		}
		if (serverMoveHomeEquipment.getInStorage() == 0 || Storage.ContainsKey(id))
		{
			return;
		}
		if (list != null)
		{
			foreach (MilMo_HomeEquipment item in list)
			{
				item.InRoomId = serverMoveHomeEquipment.getInRoom();
				item.InStorage = true;
				AddToStorage(item);
				if (_isLocalPlayerHome)
				{
					MilMo_EventSystem.Instance.PostEvent("tutorial_MoveFurnitureToStorage", "");
				}
			}
			return;
		}
		if (_isLocalPlayerHome)
		{
			Singleton<GameNetwork>.Instance.RequestHomeEquipmentItem(id);
		}
	}

	private void ChangeRoomEntrance(object msgAsObj)
	{
		if (!(msgAsObj is ServerRoomEntranceChanged serverRoomEntranceChanged))
		{
			return;
		}
		if (CurrentRoom != null && serverRoomEntranceChanged.getRoomId() == CurrentRoom.Id)
		{
			CurrentRoom.ChangeEntrance(serverRoomEntranceChanged.getNewEntranceId(), serverRoomEntranceChanged.getOldEntranceId());
		}
		if (Storage.ContainsKey(serverRoomEntranceChanged.getNewEntranceId()))
		{
			MilMo_Furniture milMo_Furniture = (MilMo_Furniture)Storage[serverRoomEntranceChanged.getNewEntranceId()];
			bool visibleInStorage = milMo_Furniture.VisibleInStorage;
			milMo_Furniture.IsRoomEntrance = true;
			if (!milMo_Furniture.VisibleInStorage && visibleInStorage)
			{
				RemoveEntry(milMo_Furniture);
			}
		}
		if (Storage.ContainsKey(serverRoomEntranceChanged.getOldEntranceId()))
		{
			MilMo_Furniture milMo_Furniture2 = (MilMo_Furniture)Storage[serverRoomEntranceChanged.getOldEntranceId()];
			bool visibleInStorage2 = milMo_Furniture2.VisibleInStorage;
			milMo_Furniture2.IsRoomEntrance = false;
			if (milMo_Furniture2.VisibleInStorage && milMo_Furniture2.WantedTransitionTargetId != serverRoomEntranceChanged.getNewEntranceId() && !visibleInStorage2)
			{
				AddEntry(milMo_Furniture2);
			}
		}
		if (Storage.ContainsKey(serverRoomEntranceChanged.getRoomId()))
		{
			Storage[serverRoomEntranceChanged.getRoomId()].ChangeModifier("RoomEntrance", serverRoomEntranceChanged.getNewEntranceId().ToString());
		}
	}

	private void ApplyRoomSkin(object msgAsObj)
	{
		if (!(msgAsObj is ServerApplyRoomSkin serverApplyRoomSkin))
		{
			return;
		}
		if (CurrentHome.OwnerID == MilMo_Instance.PlayerInstance.Id)
		{
			RemoveFromStorage(serverApplyRoomSkin.getItem().GetId());
		}
		if (CurrentRoom == null || serverApplyRoomSkin.getItem().GetInRoom() != CurrentRoom.Id)
		{
			return;
		}
		if (serverApplyRoomSkin.getItem().GetTemplateType() == "Floor")
		{
			if (CurrentRoom.Floor != null && !Storage.ContainsKey(CurrentRoom.Floor.Id) && CurrentRoom.Floor.Id != serverApplyRoomSkin.getItem().GetId() && CurrentHome.OwnerID == MilMo_Instance.PlayerInstance.Id)
			{
				AddToStorage(CurrentRoom.Floor);
			}
			CurrentRoom.SetFloor(serverApplyRoomSkin.getItem());
		}
		else if (serverApplyRoomSkin.getItem().GetTemplateType() == "Wallpaper")
		{
			if (CurrentRoom.Wallpaper != null && !Storage.ContainsKey(CurrentRoom.Wallpaper.Id) && CurrentRoom.Wallpaper.Id != serverApplyRoomSkin.getItem().GetId() && CurrentHome.OwnerID == MilMo_Instance.PlayerInstance.Id)
			{
				AddToStorage(CurrentRoom.Wallpaper);
			}
			CurrentRoom.SetWallpaper(serverApplyRoomSkin.getItem());
		}
	}

	private void MoveEquipmentFromStorage(object msgAsObj)
	{
		if (!(msgAsObj is ServerMoveHomeEquipmentFromStorage serverMoveHomeEquipmentFromStorage))
		{
			return;
		}
		MilMo_HomeEquipment item = RemoveFromStorage(serverMoveHomeEquipmentFromStorage.getEquipment().GetId());
		if (item == null)
		{
			LoadEquipment(serverMoveHomeEquipmentFromStorage.getEquipment(), isNew: false, forcePlace: false);
		}
		else
		{
			if (!(item is MilMo_Furniture milMo_Furniture) || serverMoveHomeEquipmentFromStorage.getEquipment().GetInRoom() != CurrentRoom.Id)
			{
				return;
			}
			milMo_Furniture.InRoomId = serverMoveHomeEquipmentFromStorage.getEquipment().GetInRoom();
			milMo_Furniture.InStorage = serverMoveHomeEquipmentFromStorage.getEquipment().GetInStorage() != 0;
			milMo_Furniture.Tile = GridCell.Parse(serverMoveHomeEquipmentFromStorage.getEquipment().GetGridCell());
			milMo_Furniture.Rotation = serverMoveHomeEquipmentFromStorage.getEquipment().GetRotation();
			MilMo_HomeFurniture furniture = new MilMo_HomeFurniture(_isLocalPlayerHome);
			if (_isLocalPlayerHome)
			{
				MilMo_Furniture transitionTarget = milMo_Furniture.TransitionTarget;
				if (transitionTarget != null && transitionTarget.IsRoomEntrance)
				{
					MilMo_EventSystem.Instance.PostEvent("tutorial_PlaceRoom", "");
				}
			}
			milMo_Furniture.LeadsToRoomName = ((Furniture)serverMoveHomeEquipmentFromStorage.getEquipment()).GetLeadsToRoomName();
			furniture.Load(milMo_Furniture, delegate(bool success)
			{
				if (!success)
				{
					Debug.LogWarning("Failed to create " + item.Template.Name + " when moving item from storage to room.");
					MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
				}
				else if (furniture.Item.InRoomId == CurrentRoom.Id)
				{
					CurrentRoom.AddFurniture(furniture, forcePlace: false);
					if (furniture.Item.Template.IsCarpet)
					{
						MilMo_GuiSoundManager.Instance.PlaySoundFx(CarpetPlaceSound.AudioClip);
					}
					else if (furniture.Item is MilMo_FloorFurniture)
					{
						MilMo_GuiSoundManager.Instance.PlaySoundFx(FloorItemPlaceSound.AudioClip);
					}
					else if (furniture.Item.Template.IsCurtain)
					{
						MilMo_GuiSoundManager.Instance.PlaySoundFx(CurtainAttachSound.AudioClip);
					}
					else if (furniture.Item is MilMo_WallFurniture)
					{
						if (_wallFurnitureMovedFromShopToWall != null)
						{
							if (_wallFurnitureMovedFromShopToWall.Id != item.Id)
							{
								return;
							}
							_wallFurnitureMovedFromShopToWall = item as MilMo_WallFurniture;
							if (_wallFurnitureMovedFromShopToWall == null)
							{
								return;
							}
							WallGridCell tile = _wallFurnitureMovedFromShopToWall.Tile;
							if (tile == null)
							{
								return;
							}
							_cameraForcedRotation = tile.WallIndex * 90;
							_wallFurnitureMovedFromShopToWall = null;
						}
						float delay = 0f;
						if ((MilMo_Instance.PlayerInstance.Avatar.Position - furniture.Position).sqrMagnitude > 7f)
						{
							MilMo_GuiSoundManager.Instance.PlaySoundFx(WallItemSwooshSound.AudioClip);
							delay = 0.3f;
						}
						MilMo_EventSystem.At(delay, delegate
						{
							MilMo_GuiSoundManager.Instance.PlaySoundFx(WallItemPlaceSound.AudioClip);
						});
					}
					else if (furniture.Item is MilMo_AttachableFurniture)
					{
						MilMo_GuiSoundManager.Instance.PlaySoundFx(AttachablePlaceSound.AudioClip);
					}
				}
				else
				{
					furniture.Unload();
				}
			});
		}
	}

	private void MoveEquipmentFromOtherRoom(object msgAsObj)
	{
		if (msgAsObj is ServerMoveHomeEquipmentFromOtherRoom serverMoveHomeEquipmentFromOtherRoom)
		{
			long id = serverMoveHomeEquipmentFromOtherRoom.getEquipment().GetId();
			if (CurrentRoom != null && CurrentRoom.Id == serverMoveHomeEquipmentFromOtherRoom.getOldRoomId() && CurrentRoom.HasItem(id))
			{
				CurrentRoom.RemoveItem(id);
			}
			else if (CurrentRoom != null && CurrentRoom.Id == serverMoveHomeEquipmentFromOtherRoom.getEquipment().GetInRoom() && !CurrentRoom.HasItem(id))
			{
				LoadEquipment(serverMoveHomeEquipmentFromOtherRoom.getEquipment(), isNew: false, forcePlace: true);
			}
		}
	}

	private void FurnitureActivated(object msgAsObj)
	{
		if (msgAsObj is ServerFurnitureActivated serverFurnitureActivated)
		{
			CurrentRoom.ActivateFurniture(serverFurnitureActivated.getFurnitureId(), serverFurnitureActivated.getStateIndex());
		}
	}

	private void LoadEquipment(HomeEquipment item, bool isNew, bool forcePlace)
	{
		if (item.GetInStorage() != 0)
		{
			AddToStorage(item, isNew);
		}
		else
		{
			if (item.GetInRoom() != CurrentRoom.Id)
			{
				return;
			}
			if (item.GetTemplateType() == "FloorFurniture" || item.GetTemplateType() == "WallFurniture" || item.GetTemplateType() == "AttachableFurniture")
			{
				bool isInitialEquipment = !_allRoomEquipmentArrivedFromServer;
				if (isInitialEquipment)
				{
					_loadingEquipment++;
				}
				MilMo_HomeFurniture obj = new MilMo_HomeFurniture(_isLocalPlayerHome);
				obj.Read(item, delegate(bool success)
				{
					if (!success)
					{
						if (isInitialEquipment)
						{
							_loadingEquipment--;
							Debug.LogWarning($"Failed to load equipment item {item.GetTemplate().GetPath()} with id {item.GetId()}");
						}
					}
					else
					{
						if (obj.Item.InRoomId == CurrentRoom.Id)
						{
							CurrentRoom.AddFurniture(obj, forcePlace);
						}
						else
						{
							obj.Unload();
						}
						if (isInitialEquipment)
						{
							_loadingEquipment--;
							MilMo_LoadingScreen.Instance.HomeEquipmentLoaded();
							if (_allRoomEquipmentArrivedFromServer && _loadingEquipment <= 0)
							{
								FinishLoad(success: true);
							}
						}
					}
				});
			}
			else if (item.GetTemplateType() == "Floor")
			{
				CurrentRoom.SetFloor(item);
			}
			else if (item.GetTemplateType() == "Wallpaper")
			{
				CurrentRoom.SetWallpaper(item);
			}
		}
	}

	private void FinishLoad(bool success)
	{
		if (!success)
		{
			CurrentRoom.Unload();
			_loadingDoneCallback(success: false);
			return;
		}
		while (_equipmentUpdateQueue.Count > 0)
		{
			IMessage message = _equipmentUpdateQueue.Dequeue();
			if (!(message is ServerMoveHomeEquipment))
			{
				if (!(message is ServerApplyRoomSkin))
				{
					if (!(message is ServerMoveHomeEquipmentFromStorage))
					{
						if (!(message is ServerMoveHomeEquipmentFromOtherRoom))
						{
							if (message is ServerRoomEntranceChanged)
							{
								ChangeRoomEntrance(message);
							}
						}
						else
						{
							MoveEquipmentFromOtherRoom(message);
						}
					}
					else
					{
						MoveEquipmentFromStorage(message);
					}
				}
				else
				{
					ApplyRoomSkin(message);
				}
			}
			else
			{
				MoveEquipment(message);
			}
		}
		if (_isLocalPlayerHome && !_firstLoadDone)
		{
			Singleton<GameNetwork>.Instance.RequestHomeStorage(OwnerID);
		}
		MilMo_Instance.CurrentInstance = this;
		string text = ((CurrentRoom != null) ? CurrentRoom.Id.ToString() : "");
		MilMo_Instance.PlayerInstance.Avatar.Room = text;
		MilMo_UserInterface.CurrentRoom = text;
		UnloadEventListeners();
		LoadEventListeners();
		_firstLoadDone = true;
		_loadingDoneCallback(success: true);
	}

	private void AddToStorage(HomeEquipment itemStruct, bool isNew)
	{
		if (!_isLocalPlayerHome)
		{
			return;
		}
		if (_loadingStorageItems.Contains(itemStruct.GetId()) || Storage.ContainsKey(itemStruct.GetId()))
		{
			Debug.LogWarning("Trying to add item " + itemStruct.GetId() + " to storage but it has already been added");
			return;
		}
		_loadingStorageItems.Add(itemStruct.GetId());
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(itemStruct.GetTemplate(), delegate(MilMo_Template template, bool timeOut)
		{
			if (timeOut || !(template is MilMo_HomeEquipmentTemplate milMo_HomeEquipmentTemplate))
			{
				Debug.LogWarning("Failed to get home equipment template " + itemStruct.GetTemplate().GetPath() + " when adding home item to storage.");
			}
			else if (_loadingStorageItems.Contains(itemStruct.GetId()))
			{
				MilMo_Item milMo_Item = milMo_HomeEquipmentTemplate.Instantiate(MilMo_Item.ReadModifiers(itemStruct.GetModifiers()));
				MilMo_HomeEquipment item = milMo_Item as MilMo_HomeEquipment;
				if (item != null)
				{
					item.Read(itemStruct);
					AddToStorage(item);
					_loadingStorageItems.RemoveAll((long l) => l == item.Id);
					if (isNew && _isLocalPlayerHome && MilMo_Instance.PlayerInstance.InShop && CurrentRoom != null && item.InRoomId == 0L)
					{
						CurrentRoom.RequestMoveFromStorage(item);
						if (item is MilMo_WallFurniture wallFurnitureMovedFromShopToWall)
						{
							_wallFurnitureMovedFromShopToWall = wallFurnitureMovedFromShopToWall;
						}
					}
					else if (item is MilMo_Room)
					{
						MilMo_EventSystem.Instance.PostEvent("tutorial_RoomInStorage", "");
					}
				}
			}
		});
	}

	private void AddToStorage(MilMo_HomeEquipment item)
	{
		if (!_isLocalPlayerHome)
		{
			return;
		}
		if (!(item is MilMo_Furniture milMo_Furniture))
		{
			if (item is MilMo_Room { NeedsEntrance: not false } milMo_Room)
			{
				if (Storage.ContainsKey(milMo_Room.WantedEntranceId))
				{
					milMo_Room.SetEntrance((MilMo_Furniture)Storage[milMo_Room.WantedEntranceId]);
				}
				else if (_storageRoomsWaitingForEntrance.ContainsKey(milMo_Room.WantedEntranceId))
				{
					_storageRoomsWaitingForEntrance[milMo_Room.WantedEntranceId] = milMo_Room;
				}
				else
				{
					_storageRoomsWaitingForEntrance.Add(milMo_Room.WantedEntranceId, milMo_Room);
				}
			}
		}
		else
		{
			if (milMo_Furniture.NeedsTransitionTarget)
			{
				long wantedTransitionTargetId = milMo_Furniture.WantedTransitionTargetId;
				MilMo_Furniture milMo_Furniture2 = null;
				if (_storageDoorsWaitingForTarget.ContainsKey(wantedTransitionTargetId))
				{
					milMo_Furniture2 = _storageDoorsWaitingForTarget[wantedTransitionTargetId];
					_storageDoorsWaitingForTarget.Remove(wantedTransitionTargetId);
				}
				else if (Storage.ContainsKey(wantedTransitionTargetId))
				{
					milMo_Furniture2 = (MilMo_Furniture)Storage[wantedTransitionTargetId];
				}
				if (milMo_Furniture2 != null)
				{
					if (!milMo_Furniture2.NeedsTransitionTarget && milMo_Furniture2.TransitionTarget.Id != milMo_Furniture.Id)
					{
						goto IL_01af;
					}
					if (milMo_Furniture2.NeedsTransitionTarget)
					{
						milMo_Furniture2.SetTransitionTarget(milMo_Furniture);
						AddEntry(milMo_Furniture2);
					}
					milMo_Furniture.SetTransitionTarget(milMo_Furniture2);
				}
				else if (_storageDoorsWaitingForTarget.ContainsKey(milMo_Furniture.Id))
				{
					_storageDoorsWaitingForTarget[milMo_Furniture.Id] = milMo_Furniture;
				}
				else
				{
					_storageDoorsWaitingForTarget.Add(milMo_Furniture.Id, milMo_Furniture);
				}
			}
			if (_storageRoomsWaitingForEntrance.ContainsKey(milMo_Furniture.Id))
			{
				_storageRoomsWaitingForEntrance[milMo_Furniture.Id].SetEntrance(milMo_Furniture);
				_storageRoomsWaitingForEntrance.Remove(milMo_Furniture.Id);
			}
		}
		goto IL_01af;
		IL_01af:
		if (Storage.ContainsKey(item.Id))
		{
			Storage[item.Id] = item;
			return;
		}
		Storage.Add(item.Id, item);
		AddEntry(item);
	}

	private MilMo_HomeEquipment RemoveFromStorage(long itemId)
	{
		_loadingStorageItems.RemoveAll((long l) => l == itemId);
		if (!Storage.ContainsKey(itemId))
		{
			return null;
		}
		MilMo_HomeEquipment milMo_HomeEquipment = Storage[itemId];
		Storage.Remove(itemId);
		RemoveEntry(milMo_HomeEquipment);
		return milMo_HomeEquipment;
	}

	private MilMo_InventoryEntry GetEntry(MilMo_HomeEquipment item)
	{
		return _entries.FirstOrDefault((MilMo_InventoryEntry entry) => entry.Item.Identifier == item.Identifier);
	}

	private void AddEntry(MilMo_HomeEquipment item)
	{
		if (item != null && item.VisibleInStorage)
		{
			MilMo_InventoryEntry entry = GetEntry(item);
			if (entry != null)
			{
				entry.Amount++;
				entry.Item = item;
				return;
			}
			MilMo_InventoryEntry milMo_InventoryEntry = new MilMo_InventoryEntry();
			milMo_InventoryEntry.Item = item;
			milMo_InventoryEntry.Id = (int)item.Id;
			milMo_InventoryEntry.Amount = 1;
			_entries.Add(milMo_InventoryEntry);
			GameEvent.InventoryItemAddedEvent?.RaiseEvent(milMo_InventoryEntry);
		}
	}

	private void RemoveEntry(MilMo_HomeEquipment item)
	{
		if (item == null)
		{
			return;
		}
		MilMo_InventoryEntry entry = GetEntry(item);
		if (entry == null)
		{
			return;
		}
		if (entry.Amount > 1)
		{
			entry.Amount--;
			entry.Item = Storage.Values.FirstOrDefault((MilMo_HomeEquipment storageItem) => storageItem.Identifier == item.Identifier);
		}
		else
		{
			_entries.Remove(entry);
			GameEvent.InventoryItemRemovedEvent?.RaiseEvent(entry);
		}
	}

	public void GenerateHomeDeliveryBoxPosition(object o)
	{
		CurrentRoom?.SendRandomPositionForHomeDeliveryBox();
	}

	private void HomeDeliveryBoxPickedUp(object msgAsObj)
	{
		if (msgAsObj is ServerHomeDeliveryBoxPickedUp serverHomeDeliveryBoxPickedUp && CurrentRoom != null && CurrentRoom.HasItem(serverHomeDeliveryBoxPickedUp.getItemId()))
		{
			CurrentRoom.HomeDeliveryBoxPickedUp(serverHomeDeliveryBoxPickedUp.getItemId());
		}
	}

	public override string GetMaterialAtPosition(Vector3 pos, float rayCastYOffset, out bool terrain)
	{
		terrain = false;
		return "GenericHomeFloor";
	}

	private void PlayerChangeRoom(object msgAsObj)
	{
		if (!(msgAsObj is ServerPlayerChangeRoomInHome serverPlayerChangeRoomInHome))
		{
			return;
		}
		string playerId = serverPlayerChangeRoomInHome.getPlayerId();
		if (playerId == MilMo_Instance.PlayerInstance.Avatar.Id)
		{
			LocalPlayerChangeRoom(serverPlayerChangeRoomInHome.getRoomInfo(), serverPlayerChangeRoomInHome.getEnteredThroughDoorId());
			return;
		}
		MilMo_RemotePlayer remotePlayer = GetRemotePlayer(playerId);
		if (remotePlayer != null && remotePlayer.Avatar != null)
		{
			remotePlayer.Avatar.Room = serverPlayerChangeRoomInHome.getRoomInfo().GetItem().GetId()
				.ToString();
			if (!(remotePlayer.Avatar.Room != MilMo_Instance.PlayerInstance.Avatar.Room) && CurrentRoom != null)
			{
				MilMo_ActiveRoom.EntryPointData doorEntryPointData = CurrentRoom.GetDoorEntryPointData(serverPlayerChangeRoomInHome.getEnteredThroughDoorId());
				remotePlayer.Teleport(doorEntryPointData.Pos, doorEntryPointData.Rot);
			}
		}
	}

	private static void RoomChangeFailed(object o)
	{
		DialogueSpawner.SpawnQuickInfoDialogue(new LocalizedStringWithArgument("World_389"), new LocalizedStringWithArgument("Homes_13306"), "IconHomesKey60x60");
	}

	private void LocalPlayerChangeRoom(RoomInfo roomInfo, long enterThroughDoorId)
	{
		if (CurrentRoom != null)
		{
			using IEnumerator<MilMo_HomeFurniture> enumerator = CurrentRoom.Furniture.Values.Where((MilMo_HomeFurniture furniture) => furniture.Item.WantedTransitionTargetId == enterThroughDoorId && furniture.Item.DoorEnterSound != null).GetEnumerator();
			if (enumerator.MoveNext())
			{
				MilMo_HomeFurniture current = enumerator.Current;
				MilMo_GuiSoundManager.Instance.PlaySoundFx(current.Item.DoorEnterSound);
			}
		}
		if (MilMo_LoadingScreen.Instance.LoadingState != MilMo_LoadingScreen.State.LoadRoom)
		{
			MilMo_LoadingScreen.Instance.LoadRoomFade(0f, null);
		}
		MilMo_EventSystem.At(1f, delegate
		{
			CurrentRoom?.Unload();
			_loadingDoneCallback = delegate(bool success)
			{
				if (!success)
				{
					DialogueSpawner.SpawnOkModal(new LocalizedStringWithArgument("Homes_9342"), new LocalizedStringWithArgument("Homes_9341"), new AddressableSpriteLoader("WarningIcon"), null);
					Debug.LogWarning("Failed to load room " + roomInfo.GetItem().GetId());
				}
				else
				{
					MilMo_ActiveRoom.EntryPointData entryPointData;
					if (CurrentRoom.HasItem(enterThroughDoorId))
					{
						entryPointData = CurrentRoom.GetDoorEntryPointData(enterThroughDoorId);
						MilMo_HomeFurniture furniture2 = CurrentRoom.GetFurniture(enterThroughDoorId);
						if (furniture2 != null && furniture2.Item.DoorExitSound != null)
						{
							AudioSourceWrapper audioSourceWrapper = furniture2.GameObject.GetComponent<AudioSourceWrapper>();
							if (audioSourceWrapper == null)
							{
								audioSourceWrapper = furniture2.GameObject.AddComponent<AudioSourceWrapper>();
							}
							audioSourceWrapper.Clip = furniture2.Item.DoorExitSound;
							audioSourceWrapper.Loop = false;
							audioSourceWrapper.Play();
						}
					}
					else
					{
						entryPointData = CurrentRoom.EntryPoint;
					}
					if (MilMo_World.Instance.PlayerController != null)
					{
						MilMo_PlayerControllerBase.Teleport(entryPointData.Pos, entryPointData.Rot);
					}
					else
					{
						MilMo_Instance.PlayerInstance.Avatar.GameObject.transform.position = entryPointData.Pos;
						MilMo_Instance.PlayerInstance.Avatar.GameObject.transform.rotation = entryPointData.Rot;
					}
					if (MilMo_World.Instance.Camera != null)
					{
						MilMo_World.Instance.Camera.SetupPosition();
						MilMo_World.Instance.Camera.homeCameraController.SetPan(entryPointData.Rot.eulerAngles.y);
					}
					MilMo_LoadingScreen.Instance.Hide();
					MilMo_World.HudHandler.ShowRoomName(isEntering: false);
				}
			};
			LoadRoom(roomInfo);
		});
	}

	private static void HomeOfTHeDayRaffleResponse(object msgAsObj)
	{
		if (msgAsObj is ServerAddPlayerToHomeOfTheDayRaffleResponse serverAddPlayerToHomeOfTheDayRaffleResponse)
		{
			InRaffle = true;
			LocalizedStringWithArgument message = new LocalizedStringWithArgument((serverAddPlayerToHomeOfTheDayRaffleResponse.getResult() == 1) ? "StartScreen_13509" : "StartScreen_13508");
			DialogueSpawner.SpawnQuickInfoDialogue(new LocalizedStringWithArgument("StartScreen_13507"), message, "IconMyHome");
		}
	}

	public MilMo_HomeVote.RatingData GetRatingData()
	{
		return _ratingData;
	}

	public void ShowVoteData(bool canVote, byte voteValue, float currentScore)
	{
		MilMo_World.HudHandler.ShowVoting(voteValue, canVote, currentScore);
	}

	public void SetVoteScore(float score, int numVotes)
	{
		_ratingData = new MilMo_HomeVote.RatingData(score, numVotes, 15);
	}

	public override void StartPlayMusic()
	{
		MilMo_Music.Instance.StopCurrent();
	}

	public void MuteFurnitureSounds()
	{
		CurrentRoom?.MuteAllFurniture();
	}

	public void UnmuteFurnitureSounds()
	{
		CurrentRoom?.UnmuteAllFurniture();
	}

	public void PlayEnterRoomSound()
	{
		AudioClip clip = ((CurrentRoom?.Entrance == null || !(CurrentRoom.Entrance.Item?.DoorEnterSound)) ? DoorOpenSound.AudioClip : CurrentRoom.Entrance.Item?.DoorEnterSound);
		MilMo_GuiSoundManager.Instance.PlaySoundFx(clip);
	}

	public void PlayExitRoomSound()
	{
		AudioClip clip = ((CurrentRoom?.Entrance == null || !(CurrentRoom.Entrance.Item?.DoorExitSound)) ? DoorCloseSound.AudioClip : CurrentRoom.Entrance.Item?.DoorExitSound);
		MilMo_GuiSoundManager.Instance.PlaySoundFx(clip);
	}

	protected override void PlayRemotePlayerLeaveSound()
	{
		MilMo_GuiSoundManager.Instance.PlaySoundFx(DoorCloseSound.AudioClip);
	}

	public override void PlayRemotePlayerJoinSound()
	{
		MilMo_GuiSoundManager.Instance.PlaySoundFx(DoorOpenSound.AudioClip);
	}

	private void InitializeSounds()
	{
		_haveInitedSounds = true;
		AttachableDetachSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/AttachableDetach");
		AttachablePlaceSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/AttachablePlace");
		AttachableRotateSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/AttachableRotate");
		AttachableGrabSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/AttachableGrab");
		AttachableReleaseSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/GenericRelease");
		CarpetMoveSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/CarpetMove");
		CarpetPlaceSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/CarpetPlace");
		CarpetRotateSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/CarpetRotate");
		CarpetGrabSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/CarpetGrab");
		CarpetReleaseSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/GenericRelease");
		CurtainAttachSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/CurtainAttach");
		CurtainDetachSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/CurtainDetach");
		CurtainMoveSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/CurtainMove");
		CurtainGrabSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/CurtainGrab");
		CurtainReleaseSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/CurtainAttach");
		FloorItemMoveSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/FloorMove");
		FloorItemPlaceSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/FloorPlace");
		FloorItemRotateSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/FloorRotate");
		FloorItemGrabSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/FloorGrab");
		FloorItemReleaseSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/GenericRelease");
		WallItemDetachSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/WallDetach");
		WallItemMoveSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/WallMove");
		WallItemPlaceSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/WallPlace");
		WallItemGrabSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/WallGrab");
		WallItemReleaseSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/WallPlace");
		WallItemSwooshSound = new MilMo_AudioClip("Content/Sounds/Batch01/GUI/Generic/Swap");
		UseableFurnitureToggleSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/UsableFurnitureToggle");
		KeysMenuOpenSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/KeysMenuOpen");
		KeysMenuCloseSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/KeysMenuClose");
		FurnitureModeOffSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/FurnitureModeOff");
		FurnitureModeOnSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/FurnitureModeOn");
		MoveToStorageSound = new MilMo_AudioClip("Content/Sounds/Batch01/Homes/StorageStore");
		DoorOpenSound = new MilMo_AudioClip("Content/Sounds/Batch01/Doors/ModernDoorClose");
		DoorCloseSound = new MilMo_AudioClip("Content/Sounds/Batch01/Doors/ModernDoorClose");
	}
}
