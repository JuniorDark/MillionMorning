using System;
using System.Collections.Generic;
using Code.Core.Camera;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.Input;
using Code.Core.Items.Home;
using Code.Core.Items.Home.GridCells;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture;
using Code.Core.Network;
using Code.Core.Sound;
using Code.Core.Utility;
using Code.World.GUI.Homes;
using Code.World.Home;
using Code.World.Level;
using Core;
using Core.GameEvent;
using Core.Input;
using Core.Interaction;
using UnityEngine;

namespace Code.World.Player;

public class MilMo_PlayerControllerHome : MilMo_PlayerControllerSocial
{
	private enum PlayMode
	{
		Default,
		Furnishing,
		HoldFurniture
	}

	private enum HandsEnum
	{
		Grab,
		Release,
		ArrowUp,
		ArrowUpRed,
		ArrowDown,
		ArrowDownRed,
		ArrowLeft,
		ArrowLeftRed,
		ArrowRight,
		ArrowRightRed,
		ArrowIn,
		None
	}

	private enum MouseOverResult
	{
		GUI,
		Room,
		Furniture,
		SelectedFurniture,
		Nothing
	}

	private enum MoveFurnitureMode
	{
		None,
		GOToFurniture,
		HoldFloor,
		HoldWall,
		CarryWall,
		HoldAttachable,
		CarryAttachable
	}

	public class TargetTile
	{
		public readonly int Row;

		public readonly int Col;

		public readonly float Rotation;

		public TargetTile(int row, int col, float rotation)
		{
			Row = row;
			Col = col;
			Rotation = rotation;
		}
	}

	private class FurnitureTarget
	{
		public TargetTile TargetTile;

		public MilMo_HomeFurniture Furniture;
	}

	private MouseOverResult _mouseOver = MouseOverResult.Nothing;

	private RaycastHit _mouseOverHitInfo;

	private static MilMo_HomeFurniture _selectedFurniture;

	private PlayMode _playMode;

	public MilMo_ClickFurniturePopup FurnishingMenu;

	private static List<MilMo_Widget> _hands;

	private static readonly Vector3 HeldAttachableLocalPosOffset = new Vector3(0.15f, 0f, 0f);

	private static readonly Quaternion HeldAttachableLocalRotation = Quaternion.Euler(316f, 0f, 164f);

	private static readonly Vector3 HeldWallFurnitureLocalPosOffset = new Vector3(-0.075f, 0f, 0f);

	private static readonly Quaternion HeldWallFurnitureLocalRotation = Quaternion.Euler(41.62f, 189.33f, 202.35f);

	private static float _turnOffFurnitureCollisionTimeout = 1f;

	private static float _turnOffFurnitureCollisionDuration = 1.35f;

	private MilMo_GenericReaction _gridChangedReaction;

	private MilMo_GenericReaction _goToFurnitureReaction;

	private MilMo_GenericReaction _moveFurnitureToStorageReaction;

	private MilMo_GenericReaction _moveFurnitureFromStorageReaction;

	private MilMo_GenericReaction _enterFurnishingModeReaction;

	private MilMo_GenericReaction _exitFurnishingModeReaction;

	private MilMo_GenericReaction _attachNodeMarkerClickedReaction;

	private FurnitureTarget _furnitureTarget;

	private MilMo_HomeFurniture.AttachNode _mTargetAttachNode;

	private MilMo_HomeFurniture _nextFurnitureTarget;

	private MilMo_HomeEquipment _furnitureToMoveFromStorage;

	private MoveFurnitureMode _mode;

	private Vector3 _movingFurnitureOffset = Vector3.zero;

	private Vector3 _targetPosition;

	private bool _hasTargetPosition;

	private bool _shouldRelease;

	private bool _furnitureCollisionEnabled = true;

	private MilMo_AudioClip _mAttachNodeMarkerSound;

	public override ControllerType Type => ControllerType.Home;

	protected override bool AllowRotateToCamera
	{
		get
		{
			MoveFurnitureMode mode = _mode;
			return mode == MoveFurnitureMode.None || mode == MoveFurnitureMode.CarryAttachable || mode == MoveFurnitureMode.CarryWall;
		}
	}

	public static MilMo_HomeFurniture SelectedFurniture => _selectedFurniture;

	public MilMo_PlayerControllerHome()
	{
		if (_hands == null)
		{
			_hands = new List<MilMo_Widget>();
			_hands.Add(CreateHandWidget("Batch01/Textures/Homes/IconGrab"));
			_hands.Add(CreateHandWidget("Batch01/Textures/Homes/IconCheck"));
			_hands.Add(CreateHandWidget("Batch01/Textures/Homes/IconArrowUpGreen"));
			_hands.Add(CreateHandWidget("Batch01/Textures/Homes/IconArrowUpRed"));
			_hands.Add(CreateHandWidget("Batch01/Textures/Homes/IconArrowDownGreen"));
			_hands.Add(CreateHandWidget("Batch01/Textures/Homes/IconArrowDownRed"));
			_hands.Add(CreateHandWidget("Batch01/Textures/Homes/IconArrowLeftGreen"));
			_hands.Add(CreateHandWidget("Batch01/Textures/Homes/IconArrowLeftRed"));
			_hands.Add(CreateHandWidget("Batch01/Textures/Homes/IconArrowRightGreen"));
			_hands.Add(CreateHandWidget("Batch01/Textures/Homes/IconArrowRightRed"));
			_hands.Add(CreateHandWidget("Batch01/Textures/Homes/ArrowGreen"));
		}
		_gridChangedReaction = MilMo_EventSystem.Listen("room_grid_changed", GridChangedReaction);
		_gridChangedReaction.Repeating = true;
		_goToFurnitureReaction = MilMo_EventSystem.Listen("go_to_furniture", GoToFurniture);
		_goToFurnitureReaction.Repeating = true;
		_moveFurnitureToStorageReaction = MilMo_EventSystem.Listen("move_furniture_to_storage", MoveFurnitureToStorage);
		_moveFurnitureToStorageReaction.Repeating = true;
		_moveFurnitureFromStorageReaction = MilMo_EventSystem.Listen("request_move_from_storage", MoveFurnitureFromStorage);
		_moveFurnitureFromStorageReaction.Repeating = true;
		_enterFurnishingModeReaction = MilMo_EventSystem.Listen("enter_furnishing_mode", EnterFurnishingMode);
		_enterFurnishingModeReaction.Repeating = true;
		_exitFurnishingModeReaction = MilMo_EventSystem.Listen("exit_furnishing_mode", ExitFurnishingMode);
		_exitFurnishingModeReaction.Repeating = true;
		_attachNodeMarkerClickedReaction = MilMo_EventSystem.Listen("attachnode_marker_clicked", AttachNodeMarkerClicked);
		_attachNodeMarkerClickedReaction.Repeating = true;
		GameEvent.EnterFurnishingModeEvent?.RaiseEvent(args: false);
		MilMo_EventSystem.RemoveReaction(AttackButtonReaction);
	}

	public override void UpdatePlayer()
	{
		if (MilMo_Player.Instance == null || MilMo_Player.Instance.Avatar == null || MilMo_Player.Instance.Avatar.GameObject == null || MilMo_Player.Instance.Avatar.IsDestroyed)
		{
			return;
		}
		if (!MilMo_Player.InMyHome && _playMode != 0)
		{
			ExitFurnishingMode(playSound: false);
		}
		if (_playMode == PlayMode.Furnishing && _furnitureToMoveFromStorage != null)
		{
			if (!MilMo_Home.CurrentHome.CurrentRoom.RequestMoveFromStorage(_furnitureToMoveFromStorage))
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
			}
			_furnitureToMoveFromStorage = null;
		}
		HandleMouse();
		HandleHoldAttachable();
		MoveFurnitureMode mode = _mode;
		if ((mode == MoveFurnitureMode.None || mode == MoveFurnitureMode.CarryWall || mode == MoveFurnitureMode.CarryAttachable || mode == MoveFurnitureMode.HoldAttachable) && !_hasTargetPosition)
		{
			base.UpdatePlayer();
		}
		else
		{
			MilMo_PlayerControllerBase.Player.Update();
		}
		if (_mode == MoveFurnitureMode.HoldAttachable && (_hasTargetPosition || GotMoveInput))
		{
			PickUpAttachable();
		}
		MoveTowardsTarget();
		if ((_mode == MoveFurnitureMode.HoldFloor || _mode == MoveFurnitureMode.HoldWall) && !HandleMovingFurniture())
		{
			ExitHoldFurnitureMode();
			return;
		}
		HandleCarryAttachable();
		HandleCarryWallFurniture();
		if (_furnitureCollisionEnabled && MilMo_PlayerControllerBase.LastCollisionMoveFailed && !GotMoveInput)
		{
			MilMo_PlayerControllerBase.CollisionMoveStartFailTime = Time.time;
		}
		else if (_furnitureCollisionEnabled && MilMo_PlayerControllerBase.LastCollisionMoveFailed && GotMoveInput && Time.time - MilMo_PlayerControllerBase.CollisionMoveStartFailTime > _turnOffFurnitureCollisionTimeout)
		{
			_furnitureCollisionEnabled = false;
			MilMo_EventSystem.Instance.PostEvent("disable_furniture_collision", null);
			MilMo_EventSystem.At(_turnOffFurnitureCollisionDuration, delegate
			{
				_furnitureCollisionEnabled = true;
				MilMo_EventSystem.Instance.PostEvent("enable_furniture_collision", null);
			});
		}
	}

	private void MoveTowardsTarget()
	{
		if (_hasTargetPosition)
		{
			MoveFurnitureMode mode = _mode;
			if (mode == MoveFurnitureMode.None || mode == MoveFurnitureMode.CarryAttachable || mode == MoveFurnitureMode.CarryWall)
			{
				goto IL_0024;
			}
		}
		if (_mode != MoveFurnitureMode.GOToFurniture)
		{
			return;
		}
		goto IL_0024;
		IL_0024:
		if (_hasTargetPosition)
		{
			Vector3 position = MilMo_PlayerControllerBase.Player.Avatar.Position;
			CurrentMoveDirection = _targetPosition - position;
			float sqrMagnitude = CurrentMoveDirection.sqrMagnitude;
			CurrentMoveDirection.Normalize();
			float num = Time.deltaTime * MilMo_PlayerControllerBase.WalkSpeed;
			Vector3 pos;
			if (num * num > sqrMagnitude)
			{
				pos = _targetPosition;
				_hasTargetPosition = false;
				Stop();
				if (_mTargetAttachNode != null)
				{
					AttachNode attachNode = new AttachNode(_mTargetAttachNode.BelongsToFurniture.Item.Id, _mTargetAttachNode.Index);
					_selectedFurniture.Item.Tile = attachNode;
					_mTargetAttachNode.BelongsToFurniture.Attach(_selectedFurniture, _mTargetAttachNode.Index);
					Singleton<GameNetwork>.Instance.RequestMoveHomeEquipment(_selectedFurniture.Item.Id, attachNode.ToString(), _selectedFurniture.Item.Rotation, inStorage: false, _selectedFurniture.Item.InRoomId);
					ExitHoldFurnitureMode();
					_mTargetAttachNode = null;
				}
			}
			else
			{
				pos = position + CurrentMoveDirection * num;
			}
			MilMo_PlayerControllerBase.Collision(pos);
			if (MilMo_PlayerControllerBase.LastCollisionMoveFailed)
			{
				_hasTargetPosition = false;
				if (_mode != MoveFurnitureMode.GOToFurniture)
				{
					if (_mTargetAttachNode != null)
					{
						AttachNode attachNode2 = new AttachNode(_mTargetAttachNode.BelongsToFurniture.Item.Id, _mTargetAttachNode.Index);
						_selectedFurniture.Item.Tile = attachNode2;
						_mTargetAttachNode.BelongsToFurniture.Attach(_selectedFurniture, _mTargetAttachNode.Index);
						Singleton<GameNetwork>.Instance.RequestMoveHomeEquipment(_selectedFurniture.Item.Id, attachNode2.ToString(), _selectedFurniture.Item.Rotation, inStorage: false, _selectedFurniture.Item.InRoomId);
						ExitHoldFurnitureMode();
						_mTargetAttachNode = null;
					}
					Stop();
					return;
				}
				Vector3 targetPosition = _targetPosition;
				targetPosition.y = 0f;
				Vector3 position2 = MilMo_Player.Instance.Avatar.Position;
				position2.y = 0f;
				if (!MilMo_Utility.Equals(targetPosition, position2, 0.1f))
				{
					ExitHoldFurnitureMode();
					return;
				}
			}
			Vector3 currentMoveDirection = CurrentMoveDirection;
			currentMoveDirection.y = 0f;
			if (!MilMo_Utility.Equals(currentMoveDirection, Vector3.zero))
			{
				MilMo_PlayerControllerBase.TargetRotation = Quaternion.LookRotation(currentMoveDirection);
				MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation = Quaternion.Lerp(MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation, MilMo_PlayerControllerBase.TargetRotation, MilMo_PlayerControllerBase.RotationSpeed * Time.deltaTime);
			}
		}
		if (_mode != MoveFurnitureMode.GOToFurniture)
		{
			return;
		}
		Vector3 targetPosition2 = _targetPosition;
		targetPosition2.y = 0f;
		Vector3 position3 = MilMo_Player.Instance.Avatar.Position;
		position3.y = 0f;
		if (MilMo_Utility.Equals(targetPosition2, position3, 0.1f))
		{
			MilMo_PlayerControllerBase.TargetRotation = Quaternion.Euler(0f, _furnitureTarget.TargetTile.Rotation, 0f);
			MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation = Quaternion.Lerp(MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation, MilMo_PlayerControllerBase.TargetRotation, MilMo_PlayerControllerBase.RotationSpeed * Time.deltaTime);
			if (MilMo_Utility.Equals(MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.eulerAngles.y, _furnitureTarget.TargetTile.Rotation, 1f))
			{
				ArriveAtFurniture();
			}
		}
	}

	private void HandleHoldAttachable()
	{
		if (_mode != MoveFurnitureMode.HoldAttachable)
		{
			return;
		}
		MilMo_RoomGrid currentRoomGrid = GetCurrentRoomGrid();
		if (currentRoomGrid == null || !_shouldRelease)
		{
			return;
		}
		try
		{
			currentRoomGrid.Add(_selectedFurniture.Item);
			ExitHoldFurnitureMode();
		}
		catch (MilMo_Home.FurnitureGridException)
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
			if (_selectedFurniture.IsRoomEntrance)
			{
				_shouldRelease = false;
			}
			else
			{
				MoveFurnitureToStorage(null);
			}
		}
	}

	private void HandleCarryAttachable()
	{
		if (_mode != MoveFurnitureMode.CarryAttachable)
		{
			return;
		}
		_selectedFurniture.GameObject.transform.position = (MilMo_PlayerControllerBase.Player.Avatar.RightHand.position + MilMo_PlayerControllerBase.Player.Avatar.LeftHand.position) * 0.5f;
		_selectedFurniture.GameObject.transform.localPosition += HeldAttachableLocalPosOffset;
		_selectedFurniture.GameObject.transform.localRotation = HeldAttachableLocalRotation;
		if (!_shouldRelease)
		{
			return;
		}
		MilMo_RoomGrid currentRoomGrid = GetCurrentRoomGrid();
		if (currentRoomGrid == null)
		{
			return;
		}
		FloorGridCell gridCellAtPosition = currentRoomGrid.GetGridCellAtPosition(MilMo_PlayerControllerBase.Player.Avatar.Position + MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.forward.normalized * 0.5f);
		if (gridCellAtPosition == null)
		{
			gridCellAtPosition = currentRoomGrid.GetGridCellAtPosition(MilMo_PlayerControllerBase.Player.Avatar.Position);
			if (gridCellAtPosition == null)
			{
				return;
			}
		}
		MilMo_RoomGrid.Collision collision = currentRoomGrid.TestCollision(gridCellAtPosition);
		GridCell tile = _selectedFurniture.Item.Tile;
		if (collision == null)
		{
			try
			{
				_selectedFurniture.Item.Tile = gridCellAtPosition;
				currentRoomGrid.Add(_selectedFurniture.Item);
				Singleton<GameNetwork>.Instance.RequestMoveHomeEquipment(_selectedFurniture.Item.Id, gridCellAtPosition.ToString(), _selectedFurniture.Item.Rotation, inStorage: false, _selectedFurniture.Item.InRoomId);
				ExitHoldFurnitureMode();
				return;
			}
			catch (MilMo_Home.FurnitureGridException)
			{
			}
		}
		else if (collision.FurnitureCollision)
		{
			MilMo_HomeFurniture furniture = MilMo_Home.CurrentHome.CurrentRoom.GetFurniture(collision.CollidedFurniture.Id);
			MilMo_HomeFurniture.AttachNode attachNode = furniture?.GetClosestFreeAttachNode(MilMo_PlayerControllerBase.Player.Avatar.Position);
			if (attachNode != null)
			{
				AttachNode attachNode2 = new AttachNode(collision.CollidedFurniture.Id, attachNode.Index);
				_selectedFurniture.Item.Tile = attachNode2;
				furniture.Attach(_selectedFurniture, attachNode.Index);
				Singleton<GameNetwork>.Instance.RequestMoveHomeEquipment(_selectedFurniture.Item.Id, attachNode2.ToString(), _selectedFurniture.Item.Rotation, inStorage: false, _selectedFurniture.Item.InRoomId);
				ExitHoldFurnitureMode();
				return;
			}
		}
		if (_selectedFurniture.IsRoomEntrance)
		{
			_selectedFurniture.Item.Tile = tile;
			_nextFurnitureTarget = null;
			_shouldRelease = false;
		}
		else
		{
			MoveFurnitureToStorage(null);
		}
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
	}

	private void HandleCarryWallFurniture()
	{
		if (_mode != MoveFurnitureMode.CarryWall)
		{
			return;
		}
		Vector3 position = (MilMo_PlayerControllerBase.Player.Avatar.RightHand.position + MilMo_PlayerControllerBase.Player.Avatar.LeftHand.position) * 0.5f;
		_selectedFurniture.GameObject.transform.localRotation = HeldWallFurnitureLocalRotation;
		position -= _selectedFurniture.GameObject.transform.up * (_selectedFurniture.BottomOffset.y + HeldWallFurnitureLocalPosOffset.y);
		position -= _selectedFurniture.GameObject.transform.right * (_selectedFurniture.BottomOffset.x + HeldWallFurnitureLocalPosOffset.x);
		position -= _selectedFurniture.GameObject.transform.forward * (_selectedFurniture.BottomOffset.z + HeldWallFurnitureLocalPosOffset.z);
		_selectedFurniture.GameObject.transform.position = position;
		MilMo_RoomGrid currentRoomGrid = GetCurrentRoomGrid();
		if (currentRoomGrid == null)
		{
			return;
		}
		if (!MilMo_Utility.Equals(CurrentMoveDirection, Vector3.zero))
		{
			Vector3 position2 = MilMo_Player.Instance.Avatar.Position;
			position2.y += 1f;
			if (Physics.Raycast(position2, CurrentMoveDirection, out var hitInfo, 0.75f, 67108864))
			{
				WallGridCell wallGridCellAtPosition = currentRoomGrid.GetWallGridCellAtPosition(hitInfo.point);
				if (wallGridCellAtPosition != null && currentRoomGrid.TestCollision(_selectedFurniture.Item as MilMo_WallFurniture, wallGridCellAtPosition) == null)
				{
					_selectedFurniture.GameObject.transform.parent = null;
					_selectedFurniture.PutDown();
					MilMo_Player.Instance.Avatar.UnstackAnimation("LandIdle", "HoldWallMountableIdle", unstackAll: true);
					MilMo_Player.Instance.Avatar.UnstackAnimation("Walk", "HoldWallMountableWalk", unstackAll: true);
					_selectedFurniture.Item.Tile = wallGridCellAtPosition;
					_selectedFurniture.SetPositionFromGridCell(MilMo_Home.CurrentHome.CurrentRoom);
					currentRoomGrid.Add(_selectedFurniture.Item);
					RotateCameraToFurnitureWall();
					Singleton<GameNetwork>.Instance.RequestMoveHomeEquipment(_selectedFurniture.Item.Id, _selectedFurniture.Item.Tile.ToString(), _selectedFurniture.Item.Rotation, inStorage: false, _selectedFurniture.Item.InRoomId);
					ExitHoldFurnitureMode();
					return;
				}
			}
		}
		if (!_shouldRelease)
		{
			return;
		}
		try
		{
			if (MilMo_Home.CurrentHome.CurrentRoom.RequestMoveFromStorage(_selectedFurniture.Item, out var targetGridCell))
			{
				if (GridCell.Parse(targetGridCell) is WallGridCell wallGridCell)
				{
					MilMo_World.Instance.Camera.homeCameraController.RotateTo(wallGridCell.WallIndex * 90);
				}
				ExitHoldFurnitureMode();
			}
			else
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
			}
		}
		catch (MilMo_Home.FurnitureGridException)
		{
			if (_selectedFurniture.IsRoomEntrance)
			{
				_shouldRelease = false;
			}
			else
			{
				MoveFurnitureToStorage(null);
			}
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
		}
	}

	private void AttachNodeMarkerClicked(object nodeAsObj)
	{
		if (!(nodeAsObj is MilMo_HomeFurniture.AttachNode attachNode))
		{
			return;
		}
		if (_mode == MoveFurnitureMode.HoldAttachable)
		{
			PickUpAttachable();
		}
		if (_mode == MoveFurnitureMode.CarryAttachable)
		{
			Vector3 position = MilMo_Player.Instance.Avatar.Position;
			Vector3 vector = position - attachNode.Transform.position;
			vector.y = 0f;
			float num = 0.75f;
			if (vector.sqrMagnitude > num * num)
			{
				_targetPosition = attachNode.Transform.position + vector.normalized * num;
			}
			else
			{
				_targetPosition = position;
			}
			_hasTargetPosition = true;
			_targetPosition.y = 0f;
			_mTargetAttachNode = attachNode;
			MilMo_PlayerControllerBase.MovementState = MovementStates.Forward;
			MilMo_PlayerControllerBase.PlayMoveAnimation();
		}
	}

	private bool HandleMovingFurniture()
	{
		if (_selectedFurniture?.Item == null || _selectedFurniture.GameObject == null)
		{
			return false;
		}
		MilMo_WallFurniture milMo_WallFurniture = _selectedFurniture.Item as MilMo_WallFurniture;
		MilMo_FloorFurniture milMo_FloorFurniture = _selectedFurniture.Item as MilMo_FloorFurniture;
		if ((milMo_WallFurniture == null && _mode == MoveFurnitureMode.HoldWall) || (milMo_FloorFurniture == null && _mode == MoveFurnitureMode.HoldFloor))
		{
			return false;
		}
		if (!_hasTargetPosition)
		{
			if (_shouldRelease)
			{
				try
				{
					GetCurrentRoomGrid()?.Add(_selectedFurniture.Item);
					Singleton<GameNetwork>.Instance.RequestMoveHomeEquipment(_selectedFurniture.Item.Id, _selectedFurniture.Item.Tile.ToString(), _selectedFurniture.Item.Rotation, inStorage: false, _selectedFurniture.Item.InRoomId);
					return false;
				}
				catch (MilMo_Home.FurnitureGridException)
				{
					MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
					if (!_selectedFurniture.IsRoomEntrance)
					{
						MoveFurnitureToStorage(null);
						return false;
					}
					_shouldRelease = false;
				}
			}
			if (FurnishingMenu == null)
			{
				return false;
			}
			float num = MilMo_Input.HorizontalAxis;
			float num2 = MilMo_Input.VerticalAxis;
			if (num == 0f && num2 == 0f && _mouseOver == MouseOverResult.Room && Cursor.lockState != CursorLockMode.Locked && MilMo_Input.GetKeyDown(KeyCode.Mouse0, useKeyboardFocus: false, useMouseFocus: true))
			{
				Vector2 mousePosition = InputSwitch.MousePosition;
				Vector2 vector;
				if (_mode == MoveFurnitureMode.HoldFloor)
				{
					vector = FurnishingMenu.Pos;
					vector.y = (float)Screen.height - vector.y;
				}
				else
				{
					vector = MilMo_Global.Camera.WorldToScreenPoint(MilMo_Player.Instance.Avatar.Position);
				}
				if (Mathf.Abs(mousePosition.x - vector.x) > Mathf.Abs(mousePosition.y - vector.y))
				{
					num = ((mousePosition.x > vector.x) ? 1 : (-1));
				}
				else
				{
					num2 = ((mousePosition.y > vector.y) ? 1 : (-1));
				}
			}
			GotMoveInput = num != 0f || num2 != 0f;
			if (GotMoveInput)
			{
				MilMo_Home.CurrentHome.CurrentRoom.HideDoorArrows();
				FloorGridCell floorGridCell = GetCurrentRoomGrid()?.GetGridCellAtPosition(MilMo_Player.Instance.Avatar.Position);
				if (floorGridCell == null)
				{
					return false;
				}
				CurrentMoveDirection = GetMoveDirectionForFurnitureMove(num, num2);
				if (_mode == MoveFurnitureMode.HoldFloor)
				{
					if (!MilMo_Utility.Equals(CurrentMoveDirection.x, 0f, 0.01f) && !MilMo_Utility.Equals(CurrentMoveDirection.z, 0f, 0.01f))
					{
						CurrentMoveDirection.x = 0f;
					}
				}
				else if (milMo_WallFurniture != null)
				{
					if ((milMo_WallFurniture.Tile.WallIndex == 0 && CurrentMoveDirection.z > 0f) || (milMo_WallFurniture.Tile.WallIndex == 2 && CurrentMoveDirection.z < 0f))
					{
						CurrentMoveDirection.z = 0f;
					}
					else if ((milMo_WallFurniture.Tile.WallIndex == 1 && CurrentMoveDirection.x > 0f) || (milMo_WallFurniture.Tile.WallIndex == 3 && CurrentMoveDirection.x < 0f))
					{
						CurrentMoveDirection.x = 0f;
					}
					else if ((milMo_WallFurniture.Tile.WallIndex == 0 && (double)CurrentMoveDirection.z < -0.01) || (milMo_WallFurniture.Tile.WallIndex == 2 && (double)CurrentMoveDirection.z > 0.01) || (milMo_WallFurniture.Tile.WallIndex == 1 && (double)CurrentMoveDirection.x < -0.01) || (milMo_WallFurniture.Tile.WallIndex == 3 && (double)CurrentMoveDirection.x > 0.01))
					{
						PickUpWallItem();
						return true;
					}
				}
				CurrentMoveDirection.Normalize();
				if (MilMo_Utility.Equals(CurrentMoveDirection, Vector3.zero))
				{
					Stop();
					return true;
				}
				if (!GetNewFurnitureMoveData(CurrentMoveDirection, floorGridCell, out var newFurnitureTile, out var newPlayerTile, out var movingFurnitureOffset, out var playerRotation, out var snap))
				{
					_shouldRelease = true;
					return true;
				}
				_targetPosition = new Vector3((float)newPlayerTile.Col * 1f + 0.5f, 0f, (float)(-newPlayerTile.Row) * 1f - 0.5f) + MilMo_Home.HomeOffset;
				_selectedFurniture.Item.Tile = newFurnitureTile;
				_movingFurnitureOffset = movingFurnitureOffset;
				_hasTargetPosition = true;
				if (_mode == MoveFurnitureMode.HoldFloor)
				{
					HandleMoveFloorFurnitureAnimation(milMo_FloorFurniture, CurrentMoveDirection);
				}
				else
				{
					HandleMoveWallFurnitureAnimation(milMo_WallFurniture, CurrentMoveDirection);
				}
				if (snap)
				{
					Vector3 targetPosition = _targetPosition;
					targetPosition.y += 3f;
					if (Physics.Raycast(targetPosition, Vector3.down, out var hitInfo, 4f, 201326592))
					{
						_targetPosition.y = hitInfo.point.y;
					}
					MilMo_Player.Instance.Avatar.GameObject.transform.position = _targetPosition;
					MilMo_Player.Instance.Avatar.GameObject.transform.rotation = Quaternion.Euler(0f, playerRotation, 0f);
					Stop();
					_selectedFurniture.SetPositionFromGridCell(MilMo_Home.CurrentHome.CurrentRoom);
					_hasTargetPosition = false;
					RotateCameraToFurnitureWall();
				}
			}
			else if (MilMo_PlayerControllerBase.MovementState != 0)
			{
				Stop();
			}
		}
		MoveTowardsTargetWithFurniture();
		return true;
	}

	private void RotateCameraToFurnitureWall()
	{
		WallGridCell wallGridCell = (_selectedFurniture?.Item as MilMo_WallFurniture)?.Tile;
		if (wallGridCell != null)
		{
			MilMo_World.Instance.Camera.homeCameraController.RotateTo(wallGridCell.WallIndex * 90);
		}
	}

	private bool GetNewFurnitureMoveData(Vector3 moveDirection, FloorGridCell currentPlayerTile, out GridCell newFurnitureTile, out FloorGridCell newPlayerTile, out Vector3 movingFurnitureOffset, out float playerRotation, out bool snap)
	{
		newFurnitureTile = null;
		newPlayerTile = null;
		movingFurnitureOffset = _movingFurnitureOffset;
		playerRotation = MilMo_Player.Instance.Avatar.GameObject.transform.eulerAngles.y;
		snap = false;
		MilMo_RoomGrid currentRoomGrid = GetCurrentRoomGrid();
		if (_mode == MoveFurnitureMode.HoldFloor)
		{
			if (_selectedFurniture.Item is MilMo_FloorFurniture milMo_FloorFurniture)
			{
				FloorGridCell floorGridCell = new FloorGridCell(milMo_FloorFurniture.Tile);
				floorGridCell.Row -= Mathf.RoundToInt(moveDirection.z);
				floorGridCell.Col += Mathf.RoundToInt(moveDirection.x);
				if (currentRoomGrid.TestCollision(milMo_FloorFurniture, floorGridCell, milMo_FloorFurniture.Rotation) != null)
				{
					return false;
				}
				newFurnitureTile = floorGridCell;
			}
		}
		else
		{
			MilMo_WallFurniture milMo_WallFurniture = _selectedFurniture.Item as MilMo_WallFurniture;
			int num = Mathf.RoundToInt(moveDirection.x - moveDirection.z);
			if (milMo_WallFurniture != null)
			{
				WallGridCell wallGridCell = new WallGridCell(milMo_WallFurniture.Tile);
				wallGridCell.Tile += num;
				MilMo_RoomGrid.Collision collision = currentRoomGrid.TestCollision(milMo_WallFurniture, wallGridCell);
				if (collision != null)
				{
					int num2 = (MilMo_Utility.Equals(Vector3.Cross(movingFurnitureOffset, moveDirection), Vector3.up) ? 1 : (-1));
					int num3 = 1;
					while (collision != null && collision.WallCollision && num3 < 4)
					{
						int wallIndex = wallGridCell.WallIndex;
						wallGridCell.WallIndex = wallIndex + num2;
						if (wallGridCell.WallIndex > 3)
						{
							wallGridCell.WallIndex = 0;
						}
						else if (wallGridCell.WallIndex < 0)
						{
							wallGridCell.WallIndex = 3;
						}
						if (wallIndex == 0 || wallIndex == 3)
						{
							wallGridCell.Tile = Mathf.FloorToInt(milMo_WallFurniture.Grid.Pivot);
						}
						else
						{
							wallGridCell.Tile = Mathf.FloorToInt((float)(currentRoomGrid.Walls.GetWall((byte)wallGridCell.WallIndex).Length - 1) - ((float)(int)milMo_WallFurniture.Grid.Width - milMo_WallFurniture.Grid.Pivot - 1f));
						}
						collision = currentRoomGrid.TestCollision(milMo_WallFurniture, wallGridCell);
						num3++;
					}
					if (collision != null)
					{
						return false;
					}
					if (wallGridCell.WallIndex == 0)
					{
						newPlayerTile = new FloorGridCell(0, 0);
						if (num2 > 0)
						{
							newPlayerTile.Col = milMo_WallFurniture.Grid.Width - 1 - currentPlayerTile.Row;
						}
						else
						{
							newPlayerTile.Col = currentPlayerTile.Row + (currentRoomGrid.Walls.GetWall(0).Length - milMo_WallFurniture.Grid.Width);
						}
						movingFurnitureOffset = new Vector3(0f, 0f, 1f);
					}
					else if (wallGridCell.WallIndex == 1)
					{
						newPlayerTile = new FloorGridCell(0, currentRoomGrid.Walls.GetWall(0).Length - 1);
						if (num2 > 0)
						{
							newPlayerTile.Row = currentPlayerTile.Col - (currentRoomGrid.Walls.GetWall(0).Length - milMo_WallFurniture.Grid.Width);
						}
						else
						{
							newPlayerTile.Row = currentRoomGrid.Walls.GetWall(2).Length - 1 - currentPlayerTile.Col + (currentRoomGrid.Walls.GetWall(1).Length - milMo_WallFurniture.Grid.Width);
						}
						movingFurnitureOffset = new Vector3(1f, 0f, 0f);
					}
					else if (wallGridCell.WallIndex == 2)
					{
						newPlayerTile = new FloorGridCell(currentRoomGrid.Walls.GetWall(1).Length - 1, 0);
						if (num2 > 0)
						{
							newPlayerTile.Col = currentRoomGrid.Walls.GetWall(2).Length - 1 - currentPlayerTile.Row + (currentRoomGrid.Walls.GetWall(1).Length - milMo_WallFurniture.Grid.Width);
						}
						else
						{
							newPlayerTile.Col = currentPlayerTile.Row - (currentRoomGrid.Walls.GetWall(3).Length - milMo_WallFurniture.Grid.Width);
						}
						movingFurnitureOffset = new Vector3(0f, 0f, -1f);
					}
					else if (wallGridCell.WallIndex == 3)
					{
						newPlayerTile = new FloorGridCell(0, 0);
						if (num2 > 0)
						{
							newPlayerTile.Row = currentPlayerTile.Col + (currentRoomGrid.Walls.GetWall(3).Length - milMo_WallFurniture.Grid.Width);
						}
						else
						{
							newPlayerTile.Row = milMo_WallFurniture.Grid.Width - 1 - currentPlayerTile.Col;
						}
						movingFurnitureOffset = new Vector3(-1f, 0f, 0f);
					}
					playerRotation = wallGridCell.WallIndex * 90;
					snap = true;
				}
				newFurnitureTile = wallGridCell;
			}
		}
		if (newPlayerTile == null)
		{
			newPlayerTile = new FloorGridCell(currentPlayerTile);
			newPlayerTile.Row -= Mathf.RoundToInt(moveDirection.z);
			newPlayerTile.Col += Mathf.RoundToInt(moveDirection.x);
		}
		if (newPlayerTile.Col >= 0 && newPlayerTile.Col < currentRoomGrid.Columns && newPlayerTile.Row >= 0)
		{
			return newPlayerTile.Row < currentRoomGrid.Rows;
		}
		return false;
	}

	private Vector3 GetMoveDirectionForFurnitureMove(float horizontalAxis, float verticalAxis)
	{
		float num = Mathf.Repeat(MilMo_Global.Camera.transform.eulerAngles.y, 360f);
		Quaternion quaternion = Quaternion.Euler(0f, Mathf.Repeat(Mathf.RoundToInt(num / 90f) * 90, 360f), 0f);
		Vector3 vector = quaternion * Vector3.forward;
		Vector3 vector2 = quaternion * Vector3.right;
		Vector3 zero = Vector3.zero;
		zero += vector2 * horizontalAxis;
		zero += vector * verticalAxis;
		zero.y = 0f;
		return zero;
	}

	private HandsEnum GetCurrentArrowType()
	{
		if (_playMode == PlayMode.Default)
		{
			return HandsEnum.None;
		}
		if (_playMode == PlayMode.Furnishing)
		{
			return HandsEnum.None;
		}
		MoveFurnitureMode mode = _mode;
		if (mode == MoveFurnitureMode.CarryWall || mode == MoveFurnitureMode.HoldAttachable || mode == MoveFurnitureMode.CarryAttachable)
		{
			Vector2 mousePosition = InputSwitch.MousePosition;
			Vector3 vector = MilMo_Global.Camera.WorldToScreenPoint(MilMo_Player.Instance.Avatar.Position);
			if (Mathf.Abs(mousePosition.x - vector.x) > Mathf.Abs(mousePosition.y - vector.y))
			{
				if (mousePosition.x > vector.x)
				{
					return HandsEnum.ArrowRight;
				}
				return HandsEnum.ArrowLeft;
			}
			if (mousePosition.y > vector.y)
			{
				return HandsEnum.ArrowUp;
			}
			return HandsEnum.ArrowDown;
		}
		mode = _mode;
		if (mode == MoveFurnitureMode.HoldFloor || mode == MoveFurnitureMode.HoldWall)
		{
			if (_shouldRelease)
			{
				return HandsEnum.None;
			}
			if (_mouseOver != MouseOverResult.Room)
			{
				return HandsEnum.None;
			}
			if (FurnishingMenu == null)
			{
				return HandsEnum.None;
			}
			MilMo_RoomGrid currentRoomGrid = GetCurrentRoomGrid();
			if (currentRoomGrid == null)
			{
				return HandsEnum.None;
			}
			float num = 0f;
			float num2 = 0f;
			Vector2 mousePosition2 = InputSwitch.MousePosition;
			Vector2 vector2;
			if (_mode == MoveFurnitureMode.HoldFloor)
			{
				vector2 = FurnishingMenu.Pos;
				vector2.y = (float)Screen.height - vector2.y;
			}
			else
			{
				vector2 = MilMo_Global.Camera.WorldToScreenPoint(MilMo_Player.Instance.Avatar.Position);
			}
			if (Mathf.Abs(mousePosition2.x - vector2.x) > Mathf.Abs(mousePosition2.y - vector2.y))
			{
				num = ((mousePosition2.x > vector2.x) ? 1 : (-1));
			}
			else
			{
				num2 = ((mousePosition2.y > vector2.y) ? 1 : (-1));
			}
			HandsEnum handsEnum;
			if ((double)Math.Abs(num - 1f) < 0.001)
			{
				handsEnum = HandsEnum.ArrowRight;
			}
			else if ((double)Math.Abs(num - -1f) < 0.001)
			{
				handsEnum = HandsEnum.ArrowLeft;
			}
			else if ((double)Math.Abs(num2 - 1f) < 0.001)
			{
				handsEnum = HandsEnum.ArrowUp;
			}
			else
			{
				if (!((double)Math.Abs(num2 - -1f) < 0.001))
				{
					return HandsEnum.None;
				}
				handsEnum = HandsEnum.ArrowDown;
			}
			HandsEnum result = handsEnum + 1;
			FloorGridCell gridCellAtPosition = currentRoomGrid.GetGridCellAtPosition(MilMo_Player.Instance.Avatar.Position);
			if (gridCellAtPosition == null)
			{
				return HandsEnum.None;
			}
			Vector3 moveDirectionForFurnitureMove = GetMoveDirectionForFurnitureMove(num, num2);
			if (_mode == MoveFurnitureMode.HoldWall)
			{
				MilMo_WallFurniture milMo_WallFurniture = _selectedFurniture.Item as MilMo_WallFurniture;
				if (milMo_WallFurniture != null && ((milMo_WallFurniture.Tile.WallIndex == 0 && (double)moveDirectionForFurnitureMove.z > 0.01) || (milMo_WallFurniture.Tile.WallIndex == 2 && (double)moveDirectionForFurnitureMove.z < -0.01) || (milMo_WallFurniture.Tile.WallIndex == 1 && (double)moveDirectionForFurnitureMove.x > 0.01) || (milMo_WallFurniture.Tile.WallIndex == 3 && (double)moveDirectionForFurnitureMove.x < -0.01)))
				{
					return result;
				}
				if (milMo_WallFurniture != null && ((milMo_WallFurniture.Tile.WallIndex == 0 && (double)moveDirectionForFurnitureMove.z < -0.01) || (milMo_WallFurniture.Tile.WallIndex == 2 && (double)moveDirectionForFurnitureMove.z > 0.01) || (milMo_WallFurniture.Tile.WallIndex == 1 && (double)moveDirectionForFurnitureMove.x < -0.01) || (milMo_WallFurniture.Tile.WallIndex == 3 && (double)moveDirectionForFurnitureMove.x > 0.01)))
				{
					return handsEnum;
				}
			}
			moveDirectionForFurnitureMove.Normalize();
			if (MilMo_Utility.Equals(moveDirectionForFurnitureMove, Vector3.zero))
			{
				return HandsEnum.None;
			}
			if (!GetNewFurnitureMoveData(moveDirectionForFurnitureMove, gridCellAtPosition, out var _, out var _, out var _, out var _, out var _))
			{
				return result;
			}
			return handsEnum;
		}
		return HandsEnum.None;
	}

	private void MoveTowardsTargetWithFurniture()
	{
		if (MilMo_Home.CurrentHome == null || MilMo_Home.CurrentHome.CurrentRoom == null || MilMo_Home.CurrentHome.CurrentRoom.WallPositions == null)
		{
			return;
		}
		Vector3 vector = MilMo_Player.Instance.Avatar.Position;
		if (_hasTargetPosition)
		{
			Vector3 vector2 = vector;
			float y = vector2.y;
			vector2.y = 0f;
			vector = Vector3.Lerp(vector2, _targetPosition, 5f * Time.deltaTime);
			if (MilMo_Utility.Equals(vector, _targetPosition, 0.05f))
			{
				vector = _targetPosition;
				_hasTargetPosition = false;
			}
			Vector3 vector3 = vector - vector2;
			vector.y = y;
			_selectedFurniture.GameObject.transform.position += vector3;
			if (_selectedFurniture.Item is MilMo_WallFurniture milMo_WallFurniture)
			{
				int wallIndex = milMo_WallFurniture.Tile.WallIndex;
				Vector3 position = _selectedFurniture.GameObject.transform.position;
				if (wallIndex == 0 || wallIndex == 2)
				{
					position.z = 0f - MilMo_Home.CurrentHome.CurrentRoom.WallPositions[wallIndex] + MilMo_Home.HomeOffset.z;
				}
				else
				{
					position.x = MilMo_Home.CurrentHome.CurrentRoom.WallPositions[wallIndex] + MilMo_Home.HomeOffset.x;
				}
				_selectedFurniture.GameObject.transform.position = position;
			}
		}
		Vector3 origin = vector;
		origin.y = 3f;
		if (Physics.Raycast(origin, Vector3.down, out var hitInfo, 4f, 201326592))
		{
			vector.y = Mathf.Lerp(vector.y, hitInfo.point.y, 5f * Time.deltaTime);
		}
		MilMo_Player.Instance.Avatar.GameObject.transform.position = vector;
	}

	private void HandleMoveWallFurnitureAnimation(MilMo_WallFurniture furniture, Vector3 moveDirection)
	{
		if (!GotMoveInput)
		{
			return;
		}
		MovementStates movementStates = MilMo_PlayerControllerBase.MovementState;
		Vector3 a = Vector3.Cross(_movingFurnitureOffset, moveDirection);
		if (MilMo_Utility.Equals(a, Vector3.up))
		{
			movementStates = MovementStates.FurnitureWallStrafeRight;
		}
		else if (MilMo_Utility.Equals(a, Vector3.down))
		{
			movementStates = MovementStates.FurnitureWallStrafeLeft;
		}
		if (movementStates != MilMo_PlayerControllerBase.MovementState)
		{
			MilMo_PlayerControllerBase.MovementState = movementStates;
			if (MilMo_PlayerControllerBase.MovementState == MovementStates.FurnitureWallStrafeRight)
			{
				MilMo_Player.Instance.Avatar.PlayAnimation("MoveWallMountableStrafe");
			}
			else if (MilMo_PlayerControllerBase.MovementState == MovementStates.FurnitureWallStrafeLeft)
			{
				MilMo_Player.Instance.Avatar.PlayAnimation("MoveWallMountableStrafe");
			}
		}
		MovementStates movementState = MilMo_PlayerControllerBase.MovementState;
		if ((movementState == MovementStates.FurnitureWallStrafeRight || movementState == MovementStates.FurnitureWallStrafeLeft) && MilMo_Player.Instance.Avatar.GameObject.GetComponent<Animation>() != null && MilMo_Player.Instance.Avatar.GameObject.GetComponent<Animation>()["MoveWallMountableStrafe"] != null)
		{
			MilMo_Player.Instance.Avatar.GameObject.GetComponent<Animation>()["MoveWallMountableStrafe"].speed = ((MilMo_PlayerControllerBase.MovementState == MovementStates.FurnitureWallStrafeRight) ? 1f : (-1f));
		}
		MilMo_GuiSoundManager.Instance.PlaySoundFx(furniture.Template.IsCurtain ? MilMo_Home.CurtainMoveSound.AudioClip : MilMo_Home.WallItemMoveSound.AudioClip);
	}

	private void HandleMoveFloorFurnitureAnimation(MilMo_FloorFurniture furniture, Vector3 moveDirection)
	{
		if (!GotMoveInput)
		{
			return;
		}
		MovementStates movementStates = MilMo_PlayerControllerBase.MovementState;
		if (MilMo_Utility.Equals(moveDirection, _movingFurnitureOffset))
		{
			movementStates = MovementStates.FurniturePush;
		}
		else if (MilMo_Utility.Equals(moveDirection, -_movingFurnitureOffset))
		{
			movementStates = MovementStates.FurniturePull;
		}
		else
		{
			Vector3 a = Vector3.Cross(_movingFurnitureOffset, moveDirection);
			if (MilMo_Utility.Equals(a, Vector3.up))
			{
				movementStates = MovementStates.FurnitureStrafeRight;
			}
			else if (MilMo_Utility.Equals(a, Vector3.down))
			{
				movementStates = MovementStates.FurnitureStrafeLeft;
			}
		}
		if (movementStates != MilMo_PlayerControllerBase.MovementState)
		{
			MilMo_PlayerControllerBase.MovementState = movementStates;
			if (MilMo_PlayerControllerBase.MovementState == MovementStates.FurniturePull)
			{
				MilMo_Player.Instance.Avatar.PlayAnimation(furniture.Template.PullAnimation);
			}
			else if (MilMo_PlayerControllerBase.MovementState == MovementStates.FurniturePush)
			{
				MilMo_Player.Instance.Avatar.PlayAnimation(furniture.Template.PushAnimation);
			}
			else if (MilMo_PlayerControllerBase.MovementState == MovementStates.FurnitureStrafeRight)
			{
				MilMo_Player.Instance.Avatar.PlayAnimation(furniture.Template.StrafeAnimation);
			}
			else if (MilMo_PlayerControllerBase.MovementState == MovementStates.FurnitureStrafeLeft)
			{
				MilMo_Player.Instance.Avatar.PlayAnimation(furniture.Template.StrafeAnimation);
			}
		}
		MovementStates movementState = MilMo_PlayerControllerBase.MovementState;
		if ((movementState == MovementStates.FurnitureStrafeLeft || movementState == MovementStates.FurnitureStrafeRight) && MilMo_Player.Instance.Avatar.GameObject.GetComponent<Animation>() != null && MilMo_Player.Instance.Avatar.GameObject.GetComponent<Animation>()[furniture.Template.StrafeAnimation] != null)
		{
			MilMo_Player.Instance.Avatar.GameObject.GetComponent<Animation>()[furniture.Template.StrafeAnimation].speed = ((MilMo_PlayerControllerBase.MovementState == MovementStates.FurnitureStrafeRight) ? 1f : (-1f));
		}
		MilMo_GuiSoundManager.Instance.PlaySoundFx(furniture.Template.IsCarpet ? MilMo_Home.CarpetMoveSound.AudioClip : MilMo_Home.FloorItemMoveSound.AudioClip);
	}

	private void ExitHoldFurnitureMode()
	{
		string animationName = "";
		if (_mode == MoveFurnitureMode.HoldFloor)
		{
			MilMo_HomeFurniture selectedFurniture = _selectedFurniture;
			if (selectedFurniture != null && selectedFurniture.Item != null)
			{
				animationName = ((MilMo_FloorFurniture)_selectedFurniture.Item).Template.MoveIdleAnimation;
				goto IL_0123;
			}
		}
		if (_mode == MoveFurnitureMode.HoldWall)
		{
			animationName = "MoveWallMountableIdle";
		}
		else
		{
			MoveFurnitureMode mode = _mode;
			if (mode == MoveFurnitureMode.CarryAttachable || mode == MoveFurnitureMode.CarryWall)
			{
				if (_mode == MoveFurnitureMode.CarryWall || !((MilMo_AttachableFurniture)(_selectedFurniture?.Item)).IsOnFurniture)
				{
					_selectedFurniture.GameObject.transform.parent = null;
				}
				animationName = "HoldWallMountableIdle";
				MilMo_Player.Instance.Avatar.UnstackAnimation("Walk", "HoldWallMountableWalk", unstackAll: true);
				if (MilMo_Player.Instance.Avatar.SuperAlivenessManager != null)
				{
					MilMo_Player.Instance.Avatar.SuperAlivenessManager.ForceEnable();
				}
				_selectedFurniture?.PutDown();
			}
			else if (_mode == MoveFurnitureMode.HoldAttachable && _selectedFurniture != null)
			{
				animationName = ((MilMo_AttachableFurniture)_selectedFurniture.Item)?.HoldIdleAnimation;
			}
		}
		goto IL_0123;
		IL_0123:
		MilMo_Player.Instance.Avatar.UnstackAnimation("LandIdle", animationName, unstackAll: true);
		if (_selectedFurniture != null && _selectedFurniture.Item != null)
		{
			if (_selectedFurniture.Item.Template.IsCurtain)
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.CurtainReleaseSound.AudioClip);
			}
			else if (_selectedFurniture.Item is MilMo_WallFurniture)
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.WallItemReleaseSound.AudioClip);
			}
			else if (_selectedFurniture.Item.Template.IsCarpet)
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.CarpetReleaseSound.AudioClip);
			}
			else if (_selectedFurniture.Item is MilMo_FloorFurniture)
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.FloorItemReleaseSound.AudioClip);
			}
			else if (_selectedFurniture.Item is MilMo_AttachableFurniture)
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.AttachableReleaseSound.AudioClip);
			}
		}
		_mode = MoveFurnitureMode.None;
		_selectedFurniture = null;
		MilMo_HomeFurniture.ShowAttachNodeMarkers = false;
		_mTargetAttachNode = null;
		CloseFurnishingMenu();
		GameEvent.EnterHoldFurnitureModeEvent?.RaiseEvent(args: false);
		_playMode = PlayMode.Furnishing;
		Stop();
		if (_nextFurnitureTarget != null)
		{
			GoToFurniture(_nextFurnitureTarget);
			_nextFurnitureTarget = null;
		}
		if (MilMo_Player.InMyHome)
		{
			MilMo_EventSystem.Instance.PostEvent("tutorial_ReleaseFurniture", "");
		}
		MilMo_Home.CurrentHome.CurrentRoom.HideDoorArrows();
		Singleton<InteractionManager>.Instance.Enable();
	}

	private void ArriveAtFurniture()
	{
		if (_mode != MoveFurnitureMode.GOToFurniture)
		{
			return;
		}
		MilMo_RoomGrid currentRoomGrid = GetCurrentRoomGrid();
		if (currentRoomGrid != null)
		{
			Vector3 position = MilMo_Player.Instance.Avatar.Position;
			FloorGridCell gridCellAtPosition = currentRoomGrid.GetGridCellAtPosition(position);
			if (gridCellAtPosition != null)
			{
				if (_furnitureTarget.Furniture.Item is MilMo_FloorFurniture milMo_FloorFurniture)
				{
					FloorGridCell closestGridCellForFurniture = currentRoomGrid.GetClosestGridCellForFurniture(milMo_FloorFurniture, gridCellAtPosition);
					if (closestGridCellForFurniture != null)
					{
						_movingFurnitureOffset = new Vector3(closestGridCellForFurniture.Col - gridCellAtPosition.Col, 0f, -(closestGridCellForFurniture.Row - gridCellAtPosition.Row));
						currentRoomGrid.Remove(milMo_FloorFurniture);
						MilMo_Player.Instance.Avatar.StackAnimation("LandIdle", milMo_FloorFurniture.Template.MoveIdleAnimation);
						_mode = MoveFurnitureMode.HoldFloor;
					}
				}
				else if (_furnitureTarget.Furniture.Item is MilMo_WallFurniture milMo_WallFurniture)
				{
					currentRoomGrid.Remove(milMo_WallFurniture);
					if (milMo_WallFurniture.Tile.WallIndex == 0)
					{
						_movingFurnitureOffset = new Vector3(0f, 0f, 1f);
					}
					else if (milMo_WallFurniture.Tile.WallIndex == 1)
					{
						_movingFurnitureOffset = new Vector3(1f, 0f, 0f);
					}
					else if (milMo_WallFurniture.Tile.WallIndex == 2)
					{
						_movingFurnitureOffset = new Vector3(0f, 0f, -1f);
					}
					else if (milMo_WallFurniture.Tile.WallIndex == 3)
					{
						_movingFurnitureOffset = new Vector3(-1f, 0f, 0f);
					}
					MilMo_Player.Instance.Avatar.StackAnimation("LandIdle", "MoveWallMountableIdle");
					_mode = MoveFurnitureMode.HoldWall;
				}
				else if (_furnitureTarget.Furniture.Item is MilMo_AttachableFurniture milMo_AttachableFurniture)
				{
					MilMo_HomeFurniture.ShowAttachNodeMarkers = true;
					MilMo_Player.Instance.Avatar.StackAnimation("LandIdle", milMo_AttachableFurniture.HoldIdleAnimation);
					_mode = MoveFurnitureMode.HoldAttachable;
				}
				_selectedFurniture = _furnitureTarget.Furniture;
				MilMo_Player.Instance.Avatar.PlayAnimation(MilMo_Player.Instance.Avatar.IdleAnimation);
				Singleton<GameNetwork>.Instance.SendGrabFurniture(_selectedFurniture.Item.Id, gridCellAtPosition.ToString());
				_hasTargetPosition = false;
				_shouldRelease = false;
			}
		}
		_furnitureTarget = null;
		if (_mode != MoveFurnitureMode.GOToFurniture)
		{
			EnterHoldFurnitureMode();
		}
	}

	private void PickUpAttachable()
	{
		if (_mode != MoveFurnitureMode.HoldAttachable || !(_selectedFurniture.Item is MilMo_AttachableFurniture))
		{
			return;
		}
		if (_selectedFurniture.Item is MilMo_AttachableFurniture milMo_AttachableFurniture)
		{
			MilMo_Player.Instance.Avatar.UnstackAnimation("LandIdle", milMo_AttachableFurniture.HoldIdleAnimation, unstackAll: true);
			if (milMo_AttachableFurniture.IsOnFloor)
			{
				MilMo_RoomGrid currentRoomGrid = GetCurrentRoomGrid();
				if (currentRoomGrid == null)
				{
					return;
				}
				currentRoomGrid.Remove(milMo_AttachableFurniture);
			}
			else
			{
				MilMo_Home.CurrentHome.CurrentRoom.GetFurniture(milMo_AttachableFurniture.AttachNode.FurnitureId)?.Detach(_selectedFurniture);
			}
		}
		if (_mAttachNodeMarkerSound == null)
		{
			_mAttachNodeMarkerSound = new MilMo_AudioClip("Content/Sounds/Batch01/GUI/ObjectMarkerShow");
		}
		_selectedFurniture.GameObject.transform.parent = MilMo_PlayerControllerBase.Player.Avatar.RightHand;
		_selectedFurniture.GameObject.transform.position = (MilMo_PlayerControllerBase.Player.Avatar.RightHand.position + MilMo_PlayerControllerBase.Player.Avatar.LeftHand.position) * 0.5f;
		_selectedFurniture.GameObject.transform.localRotation = Quaternion.identity;
		_selectedFurniture.PickUp();
		MilMo_Player.Instance.Avatar.StackAnimation("LandIdle", "HoldWallMountableIdle");
		MilMo_Player.Instance.Avatar.StackAnimation("Walk", "HoldWallMountableWalk");
		if (MilMo_Player.Instance.Avatar.SuperAlivenessManager != null)
		{
			MilMo_Player.Instance.Avatar.SuperAlivenessManager.ForceDisable();
		}
		CloseFurnishingMenu();
		_mode = MoveFurnitureMode.CarryAttachable;
	}

	private void PickUpWallItem()
	{
		if (_mode != MoveFurnitureMode.HoldWall || !(_selectedFurniture.Item is MilMo_WallFurniture))
		{
			return;
		}
		MilMo_RoomGrid currentRoomGrid = GetCurrentRoomGrid();
		if (currentRoomGrid == null)
		{
			return;
		}
		MilMo_PlayerControllerBase.CurrentSpeed = Vector3.zero;
		if (_selectedFurniture.Item is MilMo_WallFurniture milMo_WallFurniture)
		{
			int wallIndex = milMo_WallFurniture.Tile.WallIndex;
			MilMo_Player.Instance.Avatar.UnstackAnimation("LandIdle", "MoveWallMountableIdle", unstackAll: true);
			currentRoomGrid.Remove(milMo_WallFurniture);
			_selectedFurniture.GameObject.transform.parent = MilMo_PlayerControllerBase.Player.Avatar.RightHand;
			_selectedFurniture.GameObject.transform.position = (MilMo_PlayerControllerBase.Player.Avatar.RightHand.position + MilMo_PlayerControllerBase.Player.Avatar.LeftHand.position) * 0.5f;
			_selectedFurniture.GameObject.transform.localRotation = Quaternion.identity;
			_selectedFurniture.PickUp();
			MilMo_Player.Instance.Avatar.StackAnimation("LandIdle", "HoldWallMountableIdle");
			MilMo_Player.Instance.Avatar.StackAnimation("Walk", "HoldWallMountableWalk");
			if (MilMo_Player.Instance.Avatar.SuperAlivenessManager != null)
			{
				MilMo_Player.Instance.Avatar.SuperAlivenessManager.ForceDisable();
			}
			CloseFurnishingMenu();
			_mode = MoveFurnitureMode.CarryWall;
			Stop();
			LockAndTurn(Quaternion.Euler(0f, wallIndex * 90 + 180, 0f), 1f, playMoveAnimationOnUnlock: false, null);
		}
	}

	private void RotateFurniture(MilMo_FloorFurniture.RotationDirection direction)
	{
		if ((_mode != MoveFurnitureMode.HoldAttachable && _mode != MoveFurnitureMode.HoldFloor) || _selectedFurniture == null)
		{
			return;
		}
		if (MilMo_Home.CurrentHome.CurrentRoom.RequestRotateFurniture(_selectedFurniture.Item.Id, direction))
		{
			MilMo_Player.Instance.Avatar.PlayAnimation(_selectedFurniture.Item.RotateAnimation);
			if (_selectedFurniture.Item.Template.IsCarpet)
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.CarpetRotateSound.AudioClip);
			}
			else if (_selectedFurniture.Item is MilMo_AttachableFurniture)
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.AttachableRotateSound.AudioClip);
			}
			else
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.FloorItemRotateSound.AudioClip);
			}
		}
		else
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
		}
	}

	private void MoveFurnitureToStorage(object o)
	{
		if (_playMode == PlayMode.HoldFurniture && _selectedFurniture != null && !_selectedFurniture.IsRoomEntrance)
		{
			long itemId = _selectedFurniture.Item.Id;
			MilMo_EventSystem.At(0.4f, delegate
			{
				Singleton<GameNetwork>.Instance.RequestMoveHomeEquipment(itemId, "", 0f, inStorage: true, 0L);
			});
			_selectedFurniture.ShrinkAndMoveToStorage();
			ExitHoldFurnitureMode();
		}
	}

	private void MoveFurnitureFromStorage(object itemAsObj)
	{
		_furnitureToMoveFromStorage = itemAsObj as MilMo_HomeEquipment;
		if (_playMode == PlayMode.HoldFurniture)
		{
			_shouldRelease = true;
		}
		if (_playMode == PlayMode.Default)
		{
			EnterFurnishingMode(null);
		}
	}

	public override void Exit()
	{
		base.Exit();
		MilMo_EventSystem.RemoveReaction(_goToFurnitureReaction);
		_goToFurnitureReaction = null;
		MilMo_EventSystem.RemoveReaction(_gridChangedReaction);
		_gridChangedReaction = null;
		MilMo_EventSystem.RemoveReaction(_moveFurnitureToStorageReaction);
		_moveFurnitureToStorageReaction = null;
		MilMo_EventSystem.RemoveReaction(_moveFurnitureFromStorageReaction);
		_moveFurnitureFromStorageReaction = null;
		MilMo_EventSystem.RemoveReaction(_enterFurnishingModeReaction);
		_enterFurnishingModeReaction = null;
		MilMo_EventSystem.RemoveReaction(_exitFurnishingModeReaction);
		_exitFurnishingModeReaction = null;
		MilMo_EventSystem.RemoveReaction(_attachNodeMarkerClickedReaction);
		_attachNodeMarkerClickedReaction = null;
		if (_playMode != 0)
		{
			ExitFurnishingMode(playSound: false);
		}
	}

	private void GoToFurniture(object furnitureAsObj)
	{
		if (!(furnitureAsObj is MilMo_HomeFurniture milMo_HomeFurniture))
		{
			return;
		}
		MilMo_RoomGrid currentRoomGrid = GetCurrentRoomGrid();
		if (currentRoomGrid == null)
		{
			return;
		}
		_furnitureTarget = new FurnitureTarget();
		_furnitureTarget.Furniture = milMo_HomeFurniture;
		Vector3 position = MilMo_Player.Instance.Avatar.Position;
		if (milMo_HomeFurniture.Item is MilMo_WallFurniture furniture)
		{
			_furnitureTarget.TargetTile = currentRoomGrid.GetClosestTileNextToFurniture(furniture, currentRoomGrid.GetGridCellAtPosition(position));
		}
		else if (milMo_HomeFurniture.Item is MilMo_FloorFurniture furniture2)
		{
			_furnitureTarget.TargetTile = currentRoomGrid.GetClosestTileNextToFurniture(furniture2, currentRoomGrid.GetGridCellAtPosition(position));
		}
		else if (milMo_HomeFurniture.Item is MilMo_AttachableFurniture)
		{
			_furnitureTarget.TargetTile = currentRoomGrid.Floor.GetClosestTileNextToTile(currentRoomGrid.GetGridCellAtPosition(milMo_HomeFurniture.Position), currentRoomGrid.GetGridCellAtPosition(position));
		}
		if (_furnitureTarget.TargetTile == null)
		{
			_furnitureTarget = null;
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
			return;
		}
		if (_playMode == PlayMode.HoldFurniture)
		{
			ExitHoldFurnitureMode();
		}
		_targetPosition = new Vector3((float)_furnitureTarget.TargetTile.Col * 1f + 0.5f, 0f, (float)(-_furnitureTarget.TargetTile.Row) * 1f - 0.5f) + MilMo_Home.HomeOffset;
		_hasTargetPosition = true;
		_mode = MoveFurnitureMode.GOToFurniture;
		MilMo_PlayerControllerBase.MovementState = MovementStates.Forward;
		MilMo_PlayerControllerBase.PlayMoveAnimation();
	}

	private void GridChangedReaction(object gridAsObj)
	{
		if (_furnitureTarget != null)
		{
			GoToFurniture(_furnitureTarget.Furniture);
		}
	}

	private MilMo_RoomGrid GetCurrentRoomGrid()
	{
		return (MilMo_Home.CurrentHome?.CurrentRoom)?.Grid;
	}

	private void EnterHoldFurnitureMode()
	{
		if (MilMo_Player.Instance == null || !MilMo_Player.InMyHome)
		{
			return;
		}
		MilMo_EventSystem.Instance.PostEvent("tutorial_GrabFurniture", "");
		_playMode = PlayMode.HoldFurniture;
		CreateFurnishingMenu(_selectedFurniture);
		if (!_selectedFurniture.IsRoomEntrance)
		{
			GameEvent.EnterHoldFurnitureModeEvent?.RaiseEvent(args: true);
		}
		if (_selectedFurniture.Item.Template.IsCurtain)
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.CurtainGrabSound.AudioClip);
		}
		else if (_selectedFurniture.Item is MilMo_WallFurniture)
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.WallItemGrabSound.AudioClip);
		}
		else if (_selectedFurniture.Item.Template.IsCarpet)
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.CarpetGrabSound.AudioClip);
		}
		else if (_selectedFurniture.Item is MilMo_FloorFurniture)
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.FloorItemGrabSound.AudioClip);
		}
		else if (_selectedFurniture.Item is MilMo_AttachableFurniture)
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.AttachableGrabSound.AudioClip);
		}
		RotateCameraToFurnitureWall();
		Singleton<InteractionManager>.Instance.Disable();
		if (!_selectedFurniture.Item.Template.IsDoor || !_selectedFurniture.Item.OtherSideOfDoorIsDifferent)
		{
			return;
		}
		MilMo_HomeFurniture switchableDoor = _selectedFurniture;
		MilMo_EventSystem.At(0.1f, delegate
		{
			if (_selectedFurniture == switchableDoor && _mode == MoveFurnitureMode.HoldWall && MilMo_Home.CurrentHome != null && MilMo_Home.CurrentHome.CurrentRoom != null)
			{
				MilMo_Home.CurrentHome.CurrentRoom.ShowSwapDoorIcon(_selectedFurniture.Item.Id, delegate(long id)
				{
					Singleton<GameNetwork>.Instance.RequestSwapDoorPair(id);
					MilMo_Home.CurrentHome.CurrentRoom.HideDoorArrows();
					ExitHoldFurnitureMode();
				});
			}
		});
	}

	private void EnterFurnishingMode(object o)
	{
		if (MilMo_Player.Instance != null && MilMo_Player.InMyHome)
		{
			MilMo_EventSystem.Instance.PostEvent("tutorial_EnterFurnishingMode", "");
			MilMo_Camera.Instance.homeCameraController.EnterFurnishingMode();
			GameEvent.EnterFurnishingModeEvent?.RaiseEvent(args: true);
			Singleton<InteractionManager>.Instance.SetMinimumPrio(3);
			_playMode = PlayMode.Furnishing;
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.FurnitureModeOnSound.AudioClip);
			if (MilMo_Instance.CurrentInstance != null)
			{
				MilMo_Instance.CurrentInstance.IgnoreClickOnObjects = true;
			}
		}
	}

	private void ExitFurnishingMode(object o)
	{
		ExitFurnishingMode(playSound: true);
	}

	private void ExitFurnishingMode(bool playSound)
	{
		if (_playMode == PlayMode.HoldFurniture)
		{
			try
			{
				GetCurrentRoomGrid().Add(_selectedFurniture.Item);
				Singleton<GameNetwork>.Instance.RequestMoveHomeEquipment(_selectedFurniture.Item.Id, _selectedFurniture.Item.Tile.ToString(), _selectedFurniture.Item.Rotation, inStorage: false, _selectedFurniture.Item.InRoomId);
				ExitHoldFurnitureMode();
			}
			catch (MilMo_Home.FurnitureGridException)
			{
				if (!_selectedFurniture.IsRoomEntrance)
				{
					MoveFurnitureToStorage(null);
				}
				else
				{
					Debug.LogWarning("Failed to place home exit in grid when exiting furnishing mode while holding the home exit.");
				}
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
			}
		}
		MilMo_EventSystem.Instance.PostEvent("tutorial_ExitFurnishingMode", "");
		MilMo_Camera.Instance.homeCameraController.ExitFurnishingMode();
		GameEvent.EnterFurnishingModeEvent?.RaiseEvent(args: false);
		Singleton<InteractionManager>.Instance.SetMinimumPrio(0);
		_playMode = PlayMode.Default;
		if (playSound)
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.FurnitureModeOffSound.AudioClip);
		}
		if (MilMo_Instance.CurrentInstance != null)
		{
			MilMo_Instance.CurrentInstance.IgnoreClickOnObjects = false;
		}
		MilMo_Home.CurrentHome.CurrentRoom.HideDoorArrows();
	}

	private MilMo_Widget CreateHandWidget(string texture)
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(MilMo_World.Instance.UI);
		milMo_Widget.SetTexture(texture);
		milMo_Widget.SetScale(32f, 32f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.CenterCenter);
		milMo_Widget.Enabled = true;
		milMo_Widget.FadeToDefaultColor = false;
		milMo_Widget.SetFadeSpeed(0.1f);
		milMo_Widget.AllowPointerFocus = false;
		milMo_Widget.SetAlpha(0f);
		milMo_Widget.DisableAtZeroAlpha = false;
		milMo_Widget.PosMover.Drag = new Vector2(0.5f, 0.5f);
		milMo_Widget.PosMover.Pull = new Vector2(0.25f, 0.25f);
		MilMo_World.Instance.UI.AddChild(milMo_Widget);
		return milMo_Widget;
	}

	private void CreateFurnishingMenu(MilMo_HomeFurniture furniture)
	{
		if (FurnishingMenu != null)
		{
			if (FurnishingMenu.CurrentFurniture != furniture)
			{
				throw new InvalidOperationException("There can only be one furnishing menu at the same time.");
			}
		}
		else
		{
			FurnishingMenu = new MilMo_ClickFurniturePopup(MilMo_World.Instance.UI, RotateFurnitureClockwise, RotateFurnitureAntiClockwise, furniture);
		}
	}

	private void CloseFurnishingMenu()
	{
		if (FurnishingMenu != null)
		{
			FurnishingMenu.Close();
			FurnishingMenu = null;
		}
	}

	private void HandleMouse()
	{
		Vector2 position = MilMo_Pointer.Position;
		if (_playMode == PlayMode.Default || Cursor.lockState == CursorLockMode.Locked)
		{
			foreach (MilMo_Widget hand in _hands)
			{
				hand.SetAlpha(0f);
				hand.GoTo(position);
			}
			_mouseOver = MouseOverResult.Nothing;
			return;
		}
		int num = -1;
		TestMouseOverObject();
		MilMo_HomeFurniture milMo_HomeFurniture = null;
		if (_playMode == PlayMode.HoldFurniture)
		{
			MouseOverResult mouseOver = _mouseOver;
			if (mouseOver == MouseOverResult.Nothing || mouseOver == MouseOverResult.SelectedFurniture)
			{
				num = 1;
			}
		}
		if (_mouseOver == MouseOverResult.Furniture)
		{
			if (_mouseOverHitInfo.collider != null)
			{
				foreach (MilMo_HomeFurniture value in MilMo_Home.CurrentHome.CurrentRoom.Furniture.Values)
				{
					if (!(value.GameObject == null))
					{
						Collider componentInChildren = value.GameObject.GetComponentInChildren<Collider>();
						if (componentInChildren != null && componentInChildren == _mouseOverHitInfo.collider)
						{
							milMo_HomeFurniture = value;
							break;
						}
					}
				}
			}
			if (milMo_HomeFurniture == null || (_mode == MoveFurnitureMode.HoldFloor && !_selectedFurniture.Item.Template.IsCarpet && milMo_HomeFurniture.Item.Template.IsCarpet))
			{
				_mouseOver = MouseOverResult.Room;
			}
			else
			{
				num = 0;
			}
		}
		if (_mouseOver == MouseOverResult.Room)
		{
			num = (int)GetCurrentArrowType();
		}
		for (int i = 0; i < _hands.Count; i++)
		{
			_hands[i].AlphaTo((i == num) ? 1 : 0);
			_hands[i].GoTo(position);
			MilMo_World.Instance.UI.BringToFront(_hands[i]);
		}
		if (num <= -1 || !MilMo_Input.GetKeyDown(KeyCode.Mouse0, useKeyboardFocus: false, useMouseFocus: true))
		{
			return;
		}
		switch (num)
		{
		case 1:
			_shouldRelease = true;
			return;
		case 0:
			if (_playMode == PlayMode.HoldFurniture)
			{
				_nextFurnitureTarget = milMo_HomeFurniture;
				_shouldRelease = true;
			}
			else if (milMo_HomeFurniture != null)
			{
				GoToFurniture(milMo_HomeFurniture);
			}
			return;
		}
		MoveFurnitureMode mode = _mode;
		if ((mode == MoveFurnitureMode.None || mode == MoveFurnitureMode.CarryWall || mode == MoveFurnitureMode.CarryAttachable || mode == MoveFurnitureMode.HoldAttachable) && !_hasTargetPosition)
		{
			_targetPosition = _mouseOverHitInfo.point;
			_targetPosition.y = 0f;
			_hasTargetPosition = true;
			MilMo_PlayerControllerBase.MovementState = MovementStates.Forward;
			MilMo_PlayerControllerBase.PlayMoveAnimation();
		}
	}

	public void HideHands()
	{
		foreach (MilMo_Widget hand in _hands)
		{
			hand.SetAlpha(0f);
		}
	}

	private void TestMouseOverObject()
	{
		_mouseOverHitInfo = default(RaycastHit);
		if (Cursor.lockState == CursorLockMode.Locked)
		{
			_mouseOver = MouseOverResult.Nothing;
		}
		else if (MilMo_UserInterfaceManager.FinalMouseFocus != null)
		{
			_mouseOver = MouseOverResult.GUI;
		}
		else if (Physics.Raycast(MilMo_Global.MainCamera.ScreenPointToRay(InputSwitch.MousePosition), out _mouseOverHitInfo, 100f, 201326592))
		{
			if (_mouseOverHitInfo.collider == null)
			{
				_mouseOver = MouseOverResult.Nothing;
			}
			else if (_selectedFurniture != null && _selectedFurniture.Collider == _mouseOverHitInfo.collider)
			{
				_mouseOver = MouseOverResult.SelectedFurniture;
			}
			else if (_mouseOverHitInfo.collider.gameObject.layer == 26)
			{
				_mouseOver = MouseOverResult.Room;
			}
			else if (_mouseOverHitInfo.collider.gameObject.layer == 27)
			{
				_mouseOver = MouseOverResult.Furniture;
			}
		}
		else
		{
			_mouseOver = MouseOverResult.Nothing;
		}
	}

	protected override void HandleLockedMode()
	{
		if (Mathf.Abs(Mathf.DeltaAngle(MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation.eulerAngles.y, MilMo_PlayerControllerBase.TargetRotation.eulerAngles.y)) > 1f)
		{
			MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation = Quaternion.Lerp(MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation, MilMo_PlayerControllerBase.TargetRotation, 15f * Time.deltaTime);
			MilMo_PlayerControllerBase.IsTurning = true;
		}
		else
		{
			MilMo_PlayerControllerBase.IsTurning = false;
			Unlock();
		}
	}

	private void RotateFurnitureClockwise()
	{
		RotateFurniture(MilMo_FloorFurniture.RotationDirection.Clockwise);
	}

	private void RotateFurnitureAntiClockwise()
	{
		RotateFurniture(MilMo_FloorFurniture.RotationDirection.AntiClockwise);
	}
}
