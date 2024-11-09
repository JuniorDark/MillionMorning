using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.Items.Home;
using Code.Core.Items.Home.FurnitureGrid;
using Code.Core.Items.Home.GridCells;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture.FloorFurniture;
using Code.Core.Items.Home.HomeEquipmentTemplate;
using Code.Core.Network;
using Code.Core.ObjectEffectSystem;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Utility;
using Code.Core.Visual;
using Code.Core.Visual.Effect;
using Code.World.Chat.ChatRoom;
using Code.World.GUI.Homes;
using Code.World.Inventory;
using Code.World.Player;
using Core;
using Core.GameEvent;
using Core.Interaction;
using UI;
using UI.Marker.Object;
using UnityEngine;

namespace Code.World.Home;

public class MilMo_HomeFurniture : MilMo_HomeObject, IHasInteraction
{
	public class AttachNode
	{
		public readonly Transform Transform;

		public readonly short Index;

		public readonly MilMo_HomeFurniture BelongsToFurniture;

		public MilMo_HomeFurniture AttachedFurniture;

		public readonly MilMoAttachNodeMarker Marker;

		public AttachNode(Transform transform, short index, MilMo_HomeFurniture belongsToFurniture)
		{
			Transform = transform;
			Index = index;
			BelongsToFurniture = belongsToFurniture;
			Marker = new MilMoAttachNodeMarker(MilMo_World.Instance.UI, transform, delegate
			{
				MilMo_EventSystem.Instance.PostEvent("attachnode_marker_clicked", this);
			});
			Marker.Enabled = false;
		}

		public override string ToString()
		{
			return "A" + BelongsToFurniture.Item.Id + ":" + Index;
		}
	}

	public static bool ShowAttachNodeMarkers = false;

	private const float ROTATION_INTERPOLATION_SPEED = 5f;

	private const float MARKER_INTERACTION_RADIUS = 0.7f;

	private const string ATTACH_NODE_TRANSFORM_NAME = "AttachNode";

	private MilMo_Effect _placeEffect;

	private float[] _startGlobalAlpha;

	private Renderer _fadeRenderer;

	private MilMo_VisualRep _visualRep;

	private bool _haveStates;

	private readonly IList<MilMo_ObjectEffect> _objectEffects = new List<MilMo_ObjectEffect>();

	private short _pendingState = -1;

	private Vector3 _bottomOfObject;

	private bool _collisionDisabledExternally;

	private Material[] _originalMaterials;

	private Material[] _fadeMaterials;

	private bool _isUsingFadeMaterials;

	private bool _haveDoorMaterial;

	private bool _isHomeExit;

	private float _zScaleSign = 1f;

	private bool _lerpToTargetRotation = true;

	private bool _setScaleFromMover = true;

	private bool _wallFade = true;

	private MilMo_ObjectMover _scaleMover;

	private Vector3 _originalScale = Vector3.zero;

	private readonly bool _pickupDeliveryBoxAllowed;

	private MilMo_GenericReaction _enableCollisionReaction;

	private MilMo_GenericReaction _disableCollisionReaction;

	private static readonly int Alpha = Shader.PropertyToID("_Alpha");

	private ObjectMarker _marker;

	private MilMo_ChatRoom _chatRoom;

	public new MilMo_Furniture Item => _item as MilMo_Furniture;

	public Collider Collider { get; private set; }

	public List<AttachNode> AttachNodes { get; } = new List<AttachNode>();


	public Vector3 BottomOffset
	{
		get
		{
			if (!(_gameObject == null))
			{
				return _bottomOfObject;
			}
			return Vector3.zero;
		}
	}

	public bool IsRoomEntrance { get; private set; }

	public MilMo_HomeFurniture(bool pickupDeliveryBoxAllowed)
	{
		_pickupDeliveryBoxAllowed = pickupDeliveryBoxAllowed;
	}

	public void Activate(short stateIndex)
	{
		if (Item?.Template == null)
		{
			_pendingState = stateIndex;
			return;
		}
		if (_item.Template is MilMo_FurnitureTemplate { States: var states })
		{
			if (Item.CurrentState != -1)
			{
				states[Item.CurrentState].Deactivate(this);
			}
			Item.CurrentState = stateIndex;
			if (Item.CurrentState != -1)
			{
				states[Item.CurrentState].Activate(this);
			}
		}
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.UseableFurnitureToggleSound.AudioClip);
	}

	public void AddObjectEffect(MilMo_ObjectEffect objectEffect)
	{
		_objectEffects.Add(objectEffect);
	}

	public void ClearObjectEffects()
	{
		foreach (MilMo_ObjectEffect objectEffect in _objectEffects)
		{
			objectEffect.Destroy();
		}
		_objectEffects.Clear();
	}

	public void ActivateHomeExit()
	{
		_isHomeExit = true;
		MirrorZ(shouldMirrorZ: false);
		IsRoomEntrance = true;
	}

	public void ActivateRoomEntrance()
	{
		IsRoomEntrance = true;
		MirrorZ(shouldMirrorZ: true);
	}

	public void DeactivateRoomEntrance()
	{
		IsRoomEntrance = false;
		MirrorZ(shouldMirrorZ: false);
		if (_isHomeExit)
		{
			_isHomeExit = false;
		}
	}

	public void SetPositionFromGridCell(MilMo_ActiveRoom room)
	{
		if (Item is MilMo_AttachableFurniture && !((MilMo_AttachableFurniture)Item).IsOnFloor)
		{
			if (((MilMo_AttachableFurniture)Item).IsOnFurniture)
			{
				room.AttachAttachable(this);
			}
			return;
		}
		Vector3 gridCellPosition = room.GetGridCellPosition(_item.Tile);
		MilMo_Furniture item = Item;
		if (item is MilMo_FloorFurniture || item is MilMo_AttachableFurniture)
		{
			MilMo_FurnitureFloorGrid milMo_FurnitureFloorGrid = ((Item is MilMo_FloorFurniture milMo_FloorFurniture) ? milMo_FloorFurniture.Grid : new MilMo_FurnitureFloorGrid());
			gridCellPosition.x += 1f * (milMo_FurnitureFloorGrid.PivotCol - Mathf.Floor(milMo_FurnitureFloorGrid.PivotCol));
			gridCellPosition.z -= 1f * (milMo_FurnitureFloorGrid.PivotRow - Mathf.Floor(milMo_FurnitureFloorGrid.PivotRow));
		}
		else if (Item is MilMo_WallFurniture { Grid: var grid })
		{
			int wallIndex = ((WallGridCell)Item.Tile).WallIndex;
			float num = 1f * (grid.Pivot - Mathf.Floor(grid.Pivot));
			switch (wallIndex)
			{
			case 0:
			case 2:
				gridCellPosition.x += num;
				break;
			case 1:
			case 3:
				gridCellPosition.z -= num;
				break;
			}
			base.Rotation = Mathf.Repeat((wallIndex - 1) * 90, 360f);
		}
		base.Position = gridCellPosition;
		RefreshMarkerPosition();
	}

	public bool Attach(MilMo_HomeFurniture attachable, short nodeIndex)
	{
		if (attachable.GameObject == null || !(attachable.Item is MilMo_AttachableFurniture))
		{
			return false;
		}
		using (IEnumerator<AttachNode> enumerator = AttachNodes.Where((AttachNode node) => node.Index == nodeIndex).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				AttachNode current = enumerator.Current;
				if (current.AttachedFurniture != null && current.AttachedFurniture != attachable)
				{
					return false;
				}
				current.AttachedFurniture = attachable;
				attachable.GameObject.transform.parent = current.Transform;
				RefreshMarkerPosition();
				attachable.GameObject.transform.localPosition = Vector3.zero;
				attachable.GameObject.transform.localRotation = Quaternion.Euler(0f, attachable.Item.Rotation, 0f);
				return true;
			}
		}
		return false;
	}

	private void RefreshMarkerPosition()
	{
		if ((bool)_marker)
		{
			_marker.SetPosition(GetPosition());
		}
		if (_chatRoom == null)
		{
			return;
		}
		foreach (MilMo_SitPoint sitPoint in _chatRoom.SitPoints)
		{
			sitPoint.RefreshMarkerPosition();
		}
	}

	public void Detach(MilMo_HomeFurniture attachable)
	{
		using IEnumerator<AttachNode> enumerator = AttachNodes.Where((AttachNode node) => node.AttachedFurniture != null && node.AttachedFurniture.Item.Id == attachable.Item.Id).GetEnumerator();
		if (enumerator.MoveNext())
		{
			AttachNode current = enumerator.Current;
			current.AttachedFurniture.EnableCollision(null);
			current.AttachedFurniture = null;
			attachable.GameObject.transform.parent = null;
		}
	}

	public AttachNode GetClosestFreeAttachNode(Vector3 closeToWorldPos)
	{
		AttachNode result = null;
		float num = float.MaxValue;
		foreach (AttachNode attachNode in AttachNodes)
		{
			float num2 = Vector3.SqrMagnitude(attachNode.Transform.position - closeToWorldPos);
			if (attachNode.AttachedFurniture == null && num2 < num)
			{
				result = attachNode;
				num = num2;
			}
		}
		return result;
	}

	public override void Unload()
	{
		if (_gameObject != null && Item.IsChatroom)
		{
			MilMo_ChatRoomManager.Instance.UnloadChatRoom(_gameObject, savePlayersInChatroom: false);
		}
		Item.UnloadContent();
		MilMo_EventSystem.RemoveReaction(_enableCollisionReaction);
		MilMo_EventSystem.RemoveReaction(_disableCollisionReaction);
		if (_haveDoorMaterial)
		{
			Material[] originalMaterials = _originalMaterials;
			for (int i = 0; i < originalMaterials.Length; i++)
			{
				UnityEngine.Object.Destroy(originalMaterials[i]);
			}
			originalMaterials = _fadeMaterials;
			for (int i = 0; i < originalMaterials.Length; i++)
			{
				UnityEngine.Object.Destroy(originalMaterials[i]);
			}
		}
		ClearObjectEffects();
		foreach (AttachNode attachNode in AttachNodes)
		{
			attachNode.Marker.Destroy();
		}
		base.Unload();
		if (_marker != null)
		{
			_marker.Remove();
		}
	}

	public override void UpdateRotation()
	{
		if (Item is MilMo_WallFurniture)
		{
			base.UpdateRotation();
		}
		else
		{
			if (!(Item is MilMo_AttachableFurniture) || ((MilMo_AttachableFurniture)Item).IsOnFloor)
			{
				if (!_lerpToTargetRotation || _gameObject == null || !(Mathf.Abs(Mathf.DeltaAngle(_gameObject.transform.eulerAngles.y, _targetRotation)) > 0.01f))
				{
					return;
				}
				_gameObject.transform.rotation = Quaternion.Slerp(_gameObject.transform.rotation, Quaternion.Euler(0f, _targetRotation, 0f), 5f * Time.deltaTime);
				if (Mathf.Abs(Mathf.DeltaAngle(_gameObject.transform.eulerAngles.y, _targetRotation)) > 0.01f)
				{
					DisableCollision(null);
					{
						foreach (AttachNode attachNode in AttachNodes)
						{
							attachNode.AttachedFurniture?.DisableCollision(null);
						}
						return;
					}
				}
				EnableCollision(null);
				{
					foreach (AttachNode attachNode2 in AttachNodes)
					{
						attachNode2.AttachedFurniture?.EnableCollision(null);
					}
					return;
				}
			}
			if (_lerpToTargetRotation && _gameObject != null && Mathf.Abs(Mathf.DeltaAngle(_gameObject.transform.localEulerAngles.y, _targetRotation)) > 0.01f)
			{
				_gameObject.transform.localRotation = Quaternion.Slerp(_gameObject.transform.localRotation, Quaternion.Euler(0f, _targetRotation, 0f), 5f * Time.deltaTime);
			}
		}
	}

	protected override void AsyncLoad(GameObjectDone callback)
	{
		Item.AsyncLoadContent(delegate(GameObject gameObject)
		{
			_gameObject = gameObject;
			bool success = FinishLoad();
			callback(success);
		});
	}

	protected override bool FinishLoad()
	{
		if (!base.FinishLoad())
		{
			return false;
		}
		_visualRep = MilMo_VisualRepContainer.GetVisualRep(Item.GameObject);
		if (_visualRep == null)
		{
			Debug.LogWarning("No visual rep found in game object when loading furniture " + Item.Template.Identifier);
			return false;
		}
		if (_visualRep.Renderer == null)
		{
			Debug.LogWarning("No renderer found in visual rep when loading furniture" + Item.Template.Identifier);
			return false;
		}
		Item.AsyncApply();
		if (Item.IsChatroom && _item is MilMo_Seat milMo_Seat)
		{
			_chatRoom = new MilMo_ChatRoom(milMo_Seat.Id, milMo_Seat.Template.SitNodes, _gameObject);
			MilMo_ChatRoomManager.Instance.AddChatRoom(_chatRoom);
		}
		InitializeAttachNodes();
		Collider = _gameObject.GetComponentInChildren<Collider>();
		if (Collider != null)
		{
			MeshCollider meshCollider = Collider as MeshCollider;
			if (meshCollider != null)
			{
				meshCollider.convex = true;
			}
			Collider.gameObject.layer = 27;
		}
		_fadeRenderer = ((_visualRep.MeshHeld != null) ? _visualRep.MeshHeld.Renderer : _visualRep.Renderer);
		bool activeSelf = _fadeRenderer.gameObject.activeSelf;
		_fadeRenderer.gameObject.SetActive(value: true);
		MeshFilter componentInChildren = _fadeRenderer.gameObject.GetComponentInChildren<MeshFilter>();
		_fadeRenderer.gameObject.SetActive(activeSelf);
		if (componentInChildren != null && componentInChildren.mesh != null)
		{
			Bounds bounds = componentInChildren.mesh.bounds;
			_bottomOfObject = bounds.center;
			Vector3 extents = bounds.extents;
			_bottomOfObject.y -= extents.y;
			_bottomOfObject.x -= extents.x;
		}
		InitializeMaterials();
		InitializeUsable();
		RegisterEventHandlers();
		InitializeScaleMover();
		return true;
	}

	private void InitializeAttachNodes()
	{
		Transform[] componentsInChildren = _gameObject.GetComponentsInChildren<Transform>(includeInactive: true);
		Transform transform = null;
		Transform[] array = componentsInChildren;
		foreach (Transform transform2 in array)
		{
			if (!(transform2 == null))
			{
				if (transform2.name.StartsWith("AttachNode", StringComparison.InvariantCultureIgnoreCase))
				{
					MilMo_Global.Destroy(transform2.gameObject);
				}
				else if (transform2.name.Equals("Mesh"))
				{
					transform = transform2;
				}
			}
		}
		if (transform == null)
		{
			return;
		}
		foreach (MilMo_AttachNodeTemplate attachNode in Item.Template.AttachNodes)
		{
			GameObject gameObject = new GameObject("AttachNode" + attachNode.Id);
			gameObject.transform.parent = transform;
			gameObject.transform.localPosition = attachNode.Position;
			gameObject.transform.localRotation = Quaternion.Euler(attachNode.Rotation);
			AttachNodes.Add(new AttachNode(gameObject.transform, attachNode.Id, this));
		}
	}

	private void RegisterEventHandlers()
	{
		_enableCollisionReaction = MilMo_EventSystem.Listen("enable_furniture_collision", EnableCollision);
		_enableCollisionReaction.Repeating = true;
		_disableCollisionReaction = MilMo_EventSystem.Listen("disable_furniture_collision", DisableCollision);
		_disableCollisionReaction.Repeating = true;
	}

	private void InitializeMaterials()
	{
		_startGlobalAlpha = new float[_fadeRenderer.materials.Length];
		for (int i = 0; i < _fadeRenderer.materials.Length; i++)
		{
			if (_fadeRenderer.materials[i].shader.name == "Junebug/Door")
			{
				_haveDoorMaterial = true;
				_startGlobalAlpha[i] = 1f;
			}
			else if (_fadeRenderer.materials[i].HasProperty("_Alpha"))
			{
				_startGlobalAlpha[i] = _fadeRenderer.materials[i].GetFloat(Alpha);
			}
		}
		if (!_haveDoorMaterial)
		{
			return;
		}
		Material[] materials = _fadeRenderer.materials;
		_originalMaterials = new Material[materials.Length];
		_fadeMaterials = new Material[materials.Length];
		for (int j = 0; j < _fadeRenderer.materials.Length; j++)
		{
			Material[] materials2 = _fadeRenderer.materials;
			_originalMaterials[j] = new Material(materials2[j]);
			Material material = new Material(materials2[j]);
			if (material.shader.name == "Junebug/Door")
			{
				material.shader = MilMo_ResourceManager.LoadShaderLocal("Shaders/FurnitureDiffuse");
			}
			_fadeMaterials[j] = material;
		}
	}

	private void InitializeScaleMover()
	{
		_originalScale = base.GameObject.transform.localScale;
		_scaleMover = new MilMo_ObjectMover();
		_scaleMover.SetUpdateFunc(2);
		_scaleMover.ScalePull = 0.06f;
		_scaleMover.ScaleDrag = 0.7f;
		_scaleMover.Scale = _originalScale;
		_scaleMover.Pause();
	}

	private void InitializeUsable()
	{
		if (!Item.IsChatroom)
		{
			if (Item.Template.IsHomeDeliveryBox && _pickupDeliveryBoxAllowed)
			{
				CreateMarker();
			}
			if (Item.Template.IsDoor)
			{
				CreateMarker();
			}
			_haveStates = Item.Template.States.Count > 0;
			if (_haveStates)
			{
				CreateMarker();
			}
			if (_pendingState != -1)
			{
				Item.CurrentState = _pendingState;
				Activate(_pendingState);
				_pendingState = -1;
			}
			else if (Item.CurrentState != -1)
			{
				Activate(Item.CurrentState);
			}
		}
	}

	private void CreateMarker()
	{
		MilMo_LocString objectName = new MilMo_LocString("", removeTags: false);
		_marker = WorldSpaceManager.GetWorldSpaceObject<ObjectMarker>(ObjectMarker.AddressableAddressCapsule);
		if (_marker != null)
		{
			_marker.Initialize(this, objectName, 0.7f);
		}
	}

	public void UpdateFade()
	{
		if (Item is MilMo_WallFurniture)
		{
			Fade(_wallFade && Item.ShouldFade());
		}
	}

	public override void Update()
	{
		UpdateFade();
		if (_placeEffect != null && !_placeEffect.Update())
		{
			_placeEffect.DestroyWhenDone();
			_placeEffect = null;
		}
		for (int num = _objectEffects.Count - 1; num >= 0; num--)
		{
			if (!_objectEffects[num].Update())
			{
				_objectEffects.RemoveAt(num);
			}
		}
		foreach (AttachNode attachNode in AttachNodes)
		{
			if (attachNode.Marker != null)
			{
				bool flag = attachNode.AttachedFurniture == null && ShowAttachNodeMarkers;
				if (flag && !attachNode.Marker.Enabled && MilMo_Player.InMyHome)
				{
					MilMo_EventSystem.Instance.PostEvent("tutorial_ShowAttachNodes", "");
				}
				attachNode.Marker.Enabled = flag;
			}
		}
		RefreshMarkerPosition();
		base.Update();
	}

	public void FixedUpdate()
	{
		if (!(base.GameObject == null))
		{
			_scaleMover.Update();
			if (_setScaleFromMover)
			{
				base.GameObject.transform.localScale = _scaleMover.Scale;
			}
		}
	}

	public void Mute()
	{
		if (!(_gameObject == null))
		{
			AudioSourceWrapper[] componentsInChildren = _gameObject.GetComponentsInChildren<AudioSourceWrapper>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}
	}

	public void Unmute()
	{
		if (!(_gameObject == null))
		{
			AudioSourceWrapper[] componentsInChildren = _gameObject.GetComponentsInChildren<AudioSourceWrapper>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = true;
			}
		}
	}

	public void ShrinkAndMoveToStorage(float pull = 0.08f, float drag = 0.6f)
	{
		_scaleMover.ScalePull = pull;
		_scaleMover.ScaleDrag = drag;
		_scaleMover.Scale = _originalScale;
		_scaleMover.ScaleTo(new Vector3(0f, 0f, 0f));
		MilMo_InventoryEntry temporaryEntry = new MilMo_InventoryEntry
		{
			Item = Item
		};
		Item.AsyncGetIcon(delegate
		{
			GameEvent.MoveItemToStorageEvent?.RaiseEvent(temporaryEntry);
		});
	}

	public void GrowFromGround()
	{
		GrowFromGround(0.06f, 0.7f);
	}

	private void GrowFromGround(float pull, float drag)
	{
		_scaleMover.ScalePull = pull;
		_scaleMover.ScaleDrag = drag;
		_scaleMover.Scale = Vector3.zero;
		_scaleMover.ScaleTo(_originalScale);
	}

	public void DoPlaceEffect()
	{
		if (Item is MilMo_WallFurniture && !((MilMo_WallFurniture)Item).Template.IsCurtain)
		{
			_placeEffect = MilMo_EffectContainer.GetEffect("PlaceWallFurniturePuff", _visualRep.GameObject);
		}
		if (Item is MilMo_FloorFurniture)
		{
			_placeEffect = MilMo_EffectContainer.GetEffect("PlaceFurniturePuff", _visualRep.GameObject);
		}
	}

	private void Fade(bool fadeOut)
	{
		if (_fadeRenderer == null)
		{
			return;
		}
		if (_wallFade && _visualRep != null)
		{
			_visualRep.UseMeshHeld = fadeOut;
		}
		if (fadeOut)
		{
			if (_haveDoorMaterial && !_isUsingFadeMaterials)
			{
				_fadeRenderer.materials = _fadeMaterials;
				_isUsingFadeMaterials = true;
			}
			for (int i = 0; i < _fadeRenderer.materials.Length; i++)
			{
				if (!_fadeRenderer.materials[i].HasProperty("_Alpha"))
				{
					continue;
				}
				float @float = _fadeRenderer.materials[i].GetFloat(Alpha);
				if (!(@float <= 0f))
				{
					_fadeRenderer.materials[i].SetFloat(Alpha, Mathf.Lerp(@float, 0f, 2.5f * Time.deltaTime));
					if (Collider != null)
					{
						Collider.gameObject.layer = 2;
					}
				}
			}
			return;
		}
		for (int j = 0; j < _fadeRenderer.materials.Length; j++)
		{
			if (!_fadeRenderer.materials[j].HasProperty("_Alpha"))
			{
				continue;
			}
			float float2 = _fadeRenderer.materials[j].GetFloat(Alpha);
			if (_haveDoorMaterial && _isUsingFadeMaterials && MilMo_Utility.Equals(float2, _startGlobalAlpha[j], 0.5f))
			{
				_fadeRenderer.materials = _originalMaterials;
				_isUsingFadeMaterials = false;
			}
			if (!(float2 >= _startGlobalAlpha[j]))
			{
				float num = (_haveDoorMaterial ? 8f : 1f);
				_fadeRenderer.materials[j].SetFloat(Alpha, Mathf.Lerp(float2, _startGlobalAlpha[j], num * Time.deltaTime));
				if (Collider != null)
				{
					Collider.gameObject.layer = 27;
				}
			}
		}
	}

	public void PickUp()
	{
		_collisionDisabledExternally = true;
		ShouldCollideWithPlayer(shouldCollide: false);
		_lerpToTargetRotation = false;
		_setScaleFromMover = false;
		_wallFade = false;
		if (Item is MilMo_AttachableFurniture)
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.AttachableDetachSound.AudioClip);
		}
		else if (Item.Template.IsCurtain)
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.CurtainDetachSound.AudioClip);
		}
		else if (Item is MilMo_WallFurniture)
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_Home.WallItemDetachSound.AudioClip);
		}
		if (_visualRep != null)
		{
			_visualRep.UseMeshHeld = true;
		}
	}

	public void PutDown()
	{
		_collisionDisabledExternally = false;
		ShouldCollideWithPlayer(shouldCollide: true);
		_lerpToTargetRotation = true;
		_setScaleFromMover = true;
		_wallFade = true;
		if (_visualRep != null)
		{
			_visualRep.UseMeshHeld = false;
		}
	}

	private void MirrorZ(bool shouldMirrorZ)
	{
		_zScaleSign = ((!shouldMirrorZ) ? 1 : (-1));
		_originalScale.z = _zScaleSign * Mathf.Abs(_originalScale.z);
		Vector3 localScale = _gameObject.transform.localScale;
		localScale.z = _zScaleSign * Mathf.Abs(localScale.z);
		_gameObject.transform.localScale = localScale;
		_scaleMover.Scale.z = _zScaleSign * Mathf.Abs(_scaleMover.Scale.z);
		_scaleMover.TargetScale.z = _zScaleSign * Mathf.Abs(_scaleMover.TargetScale.z);
	}

	private void DisableCollision(object o)
	{
		ShouldCollideWithPlayer(shouldCollide: false);
	}

	private void EnableCollision(object o)
	{
		if (!_collisionDisabledExternally)
		{
			ShouldCollideWithPlayer(shouldCollide: true);
		}
	}

	private void ShouldCollideWithPlayer(bool shouldCollide)
	{
		if (!(Collider == null))
		{
			MeshCollider meshCollider = Collider as MeshCollider;
			if (meshCollider != null && meshCollider.convex)
			{
				Collider.isTrigger = !shouldCollide;
			}
		}
	}

	public int GetPrio()
	{
		if (!Item.Template.IsDoor)
		{
			return 2;
		}
		return 3;
	}

	public void UseReaction()
	{
		if (_item is MilMo_Seat)
		{
			Singleton<GameNetwork>.Instance.RequestEnterChatRoom(_chatRoom.Id, _chatRoom.SitPoints[0].Id);
		}
		else if (_isHomeExit)
		{
			if (MilMo_Player.Instance.OkToEnterHub())
			{
				MilMo_Player.Instance.RequestEnterHub();
			}
		}
		else if (Item.Template.IsHomeDeliveryBox && MilMo_Player.InMyHome)
		{
			Singleton<GameNetwork>.Instance.RequestPickupHomeDeliveryBox(Item.Id);
		}
		else if (Item.Template.IsDoor)
		{
			Singleton<GameNetwork>.Instance.RequestUseDoor(Item.Id);
		}
		else
		{
			Singleton<GameNetwork>.Instance.RequestUseFurniture(Item.Id);
		}
	}

	public Vector3 GetPosition()
	{
		Vector3 position = base.GameObject.transform.position;
		if (Item.Template.IsDoor)
		{
			return position;
		}
		return position + GetCenter();
	}

	public Bounds GetBounds()
	{
		Renderer renderer = _visualRep?.Renderer;
		if (!(renderer == null))
		{
			return renderer.bounds;
		}
		return default(Bounds);
	}

	public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
	{
		return Quaternion.Euler(angles) * (point - pivot) + pivot;
	}

	public Vector3 GetCenter()
	{
		Transform transform = base.GameObject.transform;
		Vector3 position = transform.position;
		Vector3 localEulerAngles = transform.localEulerAngles;
		Vector3 center = GetBounds().center;
		Vector3 position2 = RotatePointAroundPivot(center, position, localEulerAngles);
		return base.GameObject.transform.InverseTransformPoint(position2);
	}

	public Vector3 GetMarkerOffset()
	{
		if (Item.Template.IsDoor)
		{
			return Vector3.zero;
		}
		Vector3 position = Vector3.up * GetBounds().extents.y;
		return base.GameObject.transform.InverseTransformPoint(position);
	}

	public string GetInteractionVerb()
	{
		if (_isHomeExit)
		{
			return MilMo_Localization.GetLocString("Interact_ExitHome")?.String;
		}
		if (Item.Template.IsDoor)
		{
			if (string.IsNullOrEmpty(Item.LeadsToRoomName))
			{
				return MilMo_Localization.GetLocString("Interact_Enter")?.String;
			}
			return MilMo_Localization.GetNotLocalizedLocString(Item.LeadsToRoomName)?.String;
		}
		if (Item.Template.IsHomeDeliveryBox && MilMo_Player.InMyHome)
		{
			return MilMo_Localization.GetLocString("Interact_PickUp")?.String;
		}
		return new MilMo_LocString("", removeTags: false).String;
	}

	public Interactable.InteractionType GetInteractionType()
	{
		return Interactable.InteractionType.PickDown;
	}
}
