using System.Collections.Generic;
using Code.Core.Items;
using Code.Core.Network;
using Code.Core.Network.types;
using Code.Core.ObjectEffectSystem;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.Core.Visual.Effect;
using Code.World.Inventory;
using Code.World.Player;
using Core;
using Core.GameEvent;
using Core.Interaction;
using Localization;
using Player;
using UI;
using UI.Marker.Object;
using UnityEngine;

namespace Code.World.Level.LevelObject;

public class MilMo_LevelItem : MilMo_LevelObject, IHasInteraction, IHasHighlight
{
	public delegate void ItemDestroyedCallback(MilMo_LevelItem item);

	private const float PICKUP_REQUEST_COOLDOWN = 0.5f;

	private float _lastPickupRequest;

	private bool _pickedUp;

	private MilMo_Effect _pickup;

	private string _ownerId = "";

	private bool _havePlayedNotMineEffectOnce;

	private float _creationTime;

	private readonly List<MilMo_ObjectEffect> _unLootableEffects = new List<MilMo_ObjectEffect>();

	private readonly List<string> _unLootableEffectNames = new List<string>();

	private ItemDestroyedCallback _destroyedCallback;

	private ObjectMarker _marker;

	private int _layer;

	private MilMo_Player GetPlayer => MilMo_Player.Instance;

	private MilMo_Inventory Inventory => GetPlayer?.Inventory;

	private bool CanLoot
	{
		get
		{
			if (!(_ownerId == "") && !(_ownerId == GetPlayer.Id) && !Singleton<GroupManager>.Instance.InGroup(_ownerId))
			{
				return Time.time - _creationTime > 7f;
			}
			return true;
		}
	}

	private bool HaveItem
	{
		get
		{
			if (Inventory != null)
			{
				return Inventory.HaveItemTemplate(Item);
			}
			return false;
		}
	}

	public bool HaveUniqueItem
	{
		get
		{
			if (Item.Template.IsUnique)
			{
				return HaveItem;
			}
			return false;
		}
	}

	public bool HaveMaxAmount
	{
		get
		{
			if (HaveItem)
			{
				return Inventory.GetEntry(Item.Template.Identifier).Amount + 1 > 32767;
			}
			return false;
		}
	}

	public float SqrDistanceToPlayer { get; private set; }

	public MilMo_Item Item { get; private set; }

	public MilMo_ItemTemplate Template => Item?.Template;

	public MilMo_ObjectEffect InteractionEffect { get; set; }

	public MilMo_LevelItem(bool spawnEffect)
		: base("Content/Items/", spawnEffect)
	{
		SqrDistanceToPlayer = 10000f;
		base.DoneSpawning += InitializeMarker;
	}

	public override void Update()
	{
		base.Update();
		if (Paused)
		{
			return;
		}
		if (_pickup != null && !_pickup.Update())
		{
			_pickup = null;
			Unload();
			_destroyedCallback?.Invoke(this);
		}
		for (int num = _unLootableEffects.Count - 1; num >= 0; num--)
		{
			if (CanLoot)
			{
				_unLootableEffects[num].Destroy();
				_unLootableEffects.RemoveAt(num);
			}
			else if (!_unLootableEffects[num].Update())
			{
				_unLootableEffects.RemoveAt(num);
			}
		}
		if (!(base.GameObject == null))
		{
			SqrDistanceToPlayer = (base.GameObject.transform.position - GetPlayer.Avatar.Position).sqrMagnitude;
		}
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		if (Paused)
		{
			return;
		}
		foreach (MilMo_ObjectEffect unLootableEffect in _unLootableEffects)
		{
			unLootableEffect.FixedUpdate();
		}
	}

	public void PickedUp(ItemDestroyedCallback pickupEffectsDoneCallback)
	{
		_pickedUp = true;
		_destroyedCallback = pickupEffectsDoneCallback;
		if (!string.IsNullOrEmpty(Item.PickupParticle) && base.GameObject != null)
		{
			Renderer[] componentsInChildren = base.GameObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
			_pickup = MilMo_EffectContainer.GetEffect(Item.PickupParticle, base.GameObject);
		}
		if (_pickup == null)
		{
			Unload();
			_destroyedCallback?.Invoke(this);
		}
	}

	public override void Unload()
	{
		base.Unload();
		if (_marker != null)
		{
			_marker.Remove();
		}
		DestroyHighlightFX();
	}

	public int GetPrio()
	{
		return 4;
	}

	public void UseReaction()
	{
		if (Item != null && !Item.AutoPickup() && !(base.GameObject == null))
		{
			if (HaveUniqueItem)
			{
				GameEvent.ThinkEvent?.RaiseEvent(new LocalizedStringWithArgument("World_429").GetMessage());
				return;
			}
			if (HaveMaxAmount)
			{
				GameEvent.ThinkEvent?.RaiseEvent(new LocalizedStringWithArgument("World_432").GetMessage());
				return;
			}
			List<int> pickups = new List<int> { base.Id };
			Singleton<GameNetwork>.Instance.RequestPickup(GetPlayer.Avatar.Position, pickups);
			MilMo_World.Instance.PlayerController.Lock(5f, playMoveAnimationOnUnlock: false);
		}
	}

	public Vector3 GetPosition()
	{
		return base.GameObject.transform.position;
	}

	public Vector3 GetMarkerOffset()
	{
		Renderer renderer = base.VisualRep?.Renderer;
		if (renderer == null)
		{
			return new Vector3(0f, 0.5f, 0f);
		}
		Bounds bounds = renderer.bounds;
		Vector3 result = base.GameObject.transform.InverseTransformPoint(bounds.center);
		result.y += bounds.extents.y;
		return result;
	}

	public string GetInteractionVerb()
	{
		return MilMo_Localization.GetLocString("Interact_PickUp")?.String;
	}

	public Interactable.InteractionType GetInteractionType()
	{
		return Interactable.InteractionType.HighlightOnly;
	}

	public override void Read(Code.Core.Network.types.LevelObject levelObject, OnReadDone callback)
	{
		LevelItem levelItem = levelObject as LevelItem;
		if (levelItem == null)
		{
			Debug.LogWarning("Got non level item level object when creating level item");
			callback?.Invoke(success: false, null);
			return;
		}
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(levelItem.GetItem(), delegate(MilMo_Template template, bool timeOut)
		{
			if (timeOut || !(template is MilMo_ItemTemplate milMo_ItemTemplate))
			{
				callback?.Invoke(success: false, null);
			}
			else
			{
				Item = milMo_ItemTemplate.Instantiate(new Dictionary<string, string>());
				base.Read(levelObject, callback);
				VisualRepName = levelItem.GetVisualRep();
				_ownerId = levelItem.GetOwnerId();
				_creationTime = Time.time;
				if (Item is MilMo_Gem)
				{
					for (int i = 0; i < RemovalEffectNames.Count; i++)
					{
						if (RemovalEffectNames[i].Equals("ItemRemoveBlink"))
						{
							RemovalEffectNames[i] = "GemRemoveBlink";
							break;
						}
					}
				}
				FinishRead(null, timeOut: false);
			}
		});
	}

	protected override bool FinishLoad()
	{
		if (!base.FinishLoad())
		{
			return false;
		}
		MilMo_Level.GetWalkableHeight(base.GameObject.transform.position, out var normal);
		base.GameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal) * base.GameObject.transform.rotation;
		return true;
	}

	private void InitializeMarker()
	{
		if (!(_marker != null))
		{
			_marker = WorldSpaceManager.GetWorldSpaceObject<ObjectMarker>(ObjectMarker.AddressableAddress);
			bool silent = !CanLoot || (Item != null && Item.AutoPickup()) || HaveUniqueItem;
			_marker.Initialize(this, Item?.Template?.DisplayName, Template.PickupRadiusSquared, silent);
		}
	}

	public async void ReadGeneric(TemplateReference reference, OnReadDone callback)
	{
		ReadDoneCallback = callback;
		if (!(await Singleton<MilMo_TemplateContainer>.Instance.GetTemplateAsync(reference) is MilMo_ItemTemplate milMo_ItemTemplate))
		{
			ReadDoneCallback?.Invoke(success: false, null);
			return;
		}
		Item = milMo_ItemTemplate.Instantiate(new Dictionary<string, string>());
		if (milMo_ItemTemplate.GetHappyPickupTemplate() == null)
		{
			ReadDoneCallback?.Invoke(success: false, null);
			return;
		}
		VisualRepName = milMo_ItemTemplate.VisualRep;
		BasePath = "Content/Items/";
		FullPath = BasePath + VisualRepName;
		Debug.Log("Base path: " + BasePath + ", Visual rep: " + VisualRepName);
		FinishRead(null, timeOut: false);
	}

	public bool TestPickup(float sqrDistance)
	{
		if (_pickedUp || Time.time - _lastPickupRequest < 0.5f || !Item.MayPickUp())
		{
			return false;
		}
		if (!(sqrDistance < Template.PickupRadiusSquared))
		{
			return false;
		}
		_lastPickupRequest = Time.time;
		if (CanLoot || _unLootableEffects.Count > 0 || _havePlayedNotMineEffectOnce)
		{
			return true;
		}
		foreach (string unLootableEffectName in _unLootableEffectNames)
		{
			MilMo_ObjectEffect milMo_ObjectEffect = MilMo_ObjectEffectSystem.GetObjectEffect(base.GameObject, unLootableEffectName) ?? MilMo_ObjectEffectSystem.GetObjectEffect(base.GameObject, "CoinBouncerUnlootable");
			if (milMo_ObjectEffect != null)
			{
				_unLootableEffects.Add(milMo_ObjectEffect);
			}
			else
			{
				Debug.LogWarning("Trying to add a unlootable effect that is null.");
			}
			_havePlayedNotMineEffectOnce = true;
		}
		return true;
	}

	protected override void CreateObjectEffects()
	{
		base.CreateObjectEffects();
		if (HaveMaxAmount || HaveUniqueItem)
		{
			MilMo_ObjectEffect objectEffect = MilMo_ObjectEffectSystem.GetObjectEffect(base.GameObject, "UnlootableConstantTransparent");
			if (objectEffect != null)
			{
				ObjectEffects.Add(objectEffect);
			}
		}
	}

	protected override void CreateSpawnEffects()
	{
		if (!CanLoot)
		{
			SpawnEffectNames.Add((Item is MilMo_Gem) ? "UnlootableGem" : "UnlootableTransparent");
		}
		base.CreateSpawnEffects();
		foreach (MilMo_ObjectEffect spawnEffect in SpawnEffects)
		{
			if (spawnEffect is MilMo_MoverEffect)
			{
				_unLootableEffectNames.Add(spawnEffect.Name + "Unlootable");
				break;
			}
		}
		_unLootableEffectNames.Add("UnlootableSound");
	}

	public void ShowHighlight(bool shouldShow)
	{
		if (shouldShow)
		{
			CreateHighlightFX();
		}
		else
		{
			DestroyHighlightFX();
		}
	}

	private void DestroyHighlightFX()
	{
		if (InteractionEffect != null)
		{
			if (ObjectEffects.Contains(InteractionEffect))
			{
				ObjectEffects.Remove(InteractionEffect);
			}
			InteractionEffect.Destroy();
			InteractionEffect = null;
			base.VisualRep?.RestoreLayer();
		}
	}

	private void CreateHighlightFX()
	{
		if (InteractionEffect == null)
		{
			InteractionEffect = MilMo_ObjectEffectSystem.GetObjectEffect(base.GameObject, "InteractionEffect");
			if (InteractionEffect != null)
			{
				ObjectEffects.Add(InteractionEffect);
			}
			base.VisualRep?.SetTemporaryLayer(1);
		}
	}
}
