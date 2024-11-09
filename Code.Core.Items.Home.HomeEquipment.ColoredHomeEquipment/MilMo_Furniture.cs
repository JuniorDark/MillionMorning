using System;
using System.Collections.Generic;
using Code.Core.Global;
using Code.Core.HomePack;
using Code.Core.Items.Home.HomeEquipmentTemplate;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment;

public abstract class MilMo_Furniture : MilMo_ColoredHomeEquipment
{
	public delegate void FurnitureLoaded(GameObject gameObject);

	private const string STATE_INDEX_MODIFIER_KEY = "StateIndex";

	private const string TRANSITION_TARGET_MODIFIER_KEY = "TransitionTarget";

	public const short NO_STATE = -1;

	public MilMo_HomePack HomePack => (MilMo_HomePack)base.HomePackBase;

	public short CurrentState { get; set; }

	public virtual bool IsChatroom => false;

	public GameObject GameObject { get; private set; }

	public new MilMo_FurnitureTemplate Template => base.Template as MilMo_FurnitureTemplate;

	public bool IsRoomEntrance { get; set; }

	public MilMo_Furniture TransitionTarget { get; private set; }

	public override bool VisibleInStorage
	{
		get
		{
			if (!Template.IsDoor)
			{
				return base.VisibleInStorage;
			}
			if (IsRoomEntrance)
			{
				return false;
			}
			if (TransitionTarget == null)
			{
				return false;
			}
			if (TransitionTarget.IsRoomEntrance)
			{
				return false;
			}
			return base.Id <= TransitionTarget.Id;
		}
	}

	public AudioClip DoorEnterSound { get; private set; }

	public AudioClip DoorExitSound { get; private set; }

	public bool OtherSideOfDoorIsDifferent { get; private set; }

	public string LeadsToRoomName { get; set; }

	public abstract string RotateAnimation { get; }

	public long WantedTransitionTargetId
	{
		get
		{
			if (!base.Modifiers.ContainsKey("TransitionTarget"))
			{
				return -1L;
			}
			return long.Parse(base.Modifiers["TransitionTarget"]);
		}
	}

	public bool NeedsTransitionTarget
	{
		get
		{
			if (WantedTransitionTargetId != -1 && TransitionTarget == null)
			{
				return Template.IsDoor;
			}
			return false;
		}
	}

	protected MilMo_Furniture(MilMo_FurnitureTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers, MilMo_HomePack.GetHomePackByName(template.HomePack))
	{
		CurrentState = -1;
		if (modifiers.TryGetValue("StateIndex", out var value))
		{
			CurrentState = short.Parse(value);
		}
	}

	public async void AsyncLoadContent(FurnitureLoaded callback)
	{
		if (!string.IsNullOrEmpty(Template.DoorEnterSound))
		{
			DoorEnterSound = await MilMo_ResourceManager.Instance.LoadAudioAsync(Template.DoorEnterSound);
		}
		if (!string.IsNullOrEmpty(Template.DoorExitSound))
		{
			DoorExitSound = await MilMo_ResourceManager.Instance.LoadAudioAsync(Template.DoorExitSound);
		}
		if (HomePack != null)
		{
			GameObject = new GameObject(base.Name);
			HomePack.AsyncLoadContent(GameObject, delegate
			{
				callback(GameObject);
			});
		}
		else
		{
			callback(GameObject);
		}
	}

	public void AsyncApply()
	{
		HomePack.AsyncApply(GameObject, base.ColorIndices);
	}

	public void UnloadContent()
	{
		if (GameObject != null && HomePack != null)
		{
			HomePack.UnloadContent(GameObject);
			MilMo_Global.Destroy(GameObject);
		}
		if (!string.IsNullOrEmpty(Template.DoorEnterSound))
		{
			MilMo_ResourceManager.Instance.UnloadAsset(Template.DoorEnterSound);
			DoorEnterSound = null;
		}
		if (!string.IsNullOrEmpty(Template.DoorExitSound))
		{
			MilMo_ResourceManager.Instance.UnloadAsset(Template.DoorExitSound);
			DoorExitSound = null;
		}
	}

	public virtual bool ShouldFade()
	{
		return false;
	}

	public override void Read(Code.Core.Network.types.HomeEquipment equipmentData)
	{
		base.Read(equipmentData);
		Code.Core.Network.types.Furniture furniture = (Code.Core.Network.types.Furniture)equipmentData;
		IsRoomEntrance = furniture.GetIsRoomEntrance() != 0;
		OtherSideOfDoorIsDifferent = furniture.GetDoorIsDifferentOnOtherSide() != 0;
		LeadsToRoomName = furniture.GetLeadsToRoomName();
	}

	public void SetTransitionTarget(MilMo_Furniture target)
	{
		if (!Template.IsDoor)
		{
			throw new InvalidOperationException("Can only set transition target on a door");
		}
		if (TransitionTarget != null)
		{
			throw new InvalidOperationException("Can only set transition target once");
		}
		if (!base.Modifiers.ContainsKey("TransitionTarget"))
		{
			throw new InvalidOperationException("Trying to set transition target for a furniture that shouldn't have one.");
		}
		if (target.Id != long.Parse(base.Modifiers["TransitionTarget"]))
		{
			throw new ArgumentException("Trying to set transition target to furniture with id " + target.Id + " when id " + base.Modifiers["TransitionTarget"] + " was expected.");
		}
		TransitionTarget = target;
	}
}
