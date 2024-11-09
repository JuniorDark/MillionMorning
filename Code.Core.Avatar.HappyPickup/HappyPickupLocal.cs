using System;
using Code.Core.Camera;
using Code.World;
using Code.World.Player;
using Core.GameEvent;
using UnityEngine;

namespace Code.Core.Avatar.HappyPickup;

public class HappyPickupLocal : HappyPickup
{
	private readonly MilMo_PlayerControllerBase _playerController;

	private readonly MilMo_Camera _camera;

	private MilMo_World WorldInstance => MilMo_World.Instance;

	public HappyPickupLocal(MilMo_Avatar avatar, HappyPickupItem item)
		: base(avatar, item)
	{
		if (!(WorldInstance == null))
		{
			_playerController = WorldInstance.PlayerController;
			_camera = WorldInstance.Camera;
		}
	}

	protected override bool PhaseOne()
	{
		_playerController?.Lock(0f, playMoveAnimationOnUnlock: true);
		if (_camera != null)
		{
			_camera.HappyPickupInit();
		}
		if (!base.PhaseOne())
		{
			return false;
		}
		Quaternion targetRotation = Quaternion.LookRotation((MilMo_CameraController.CameraTransform.position - Avatar.Position).normalized, Vector3.up);
		targetRotation.eulerAngles = new Vector3(0f, targetRotation.eulerAngles.y, 0f);
		MilMo_PlayerControllerBase.TargetRotation = targetRotation;
		if (base.HappyPickupTemplate.StartSound != null)
		{
			Avatar.PlaySoundEffect(base.HappyPickupTemplate.StartSound);
		}
		if (Item?.ThinkBubbleText != null && !Item.ThinkBubbleText.IsEmpty)
		{
			GameEvent.ThinkEvent?.RaiseEvent(Item.ThinkBubbleText.Identifier);
		}
		return true;
	}

	protected override bool PhaseTwo()
	{
		if (!base.PhaseTwo())
		{
			return false;
		}
		Transform[] componentsInChildren = Avatar.PickUpItem.GameObject.GetComponentsInChildren<Transform>(includeInactive: true);
		foreach (Transform transform in componentsInChildren)
		{
			if (!transform.name.StartsWith("Lod", StringComparison.InvariantCultureIgnoreCase))
			{
				transform.gameObject.SetActive(value: true);
			}
		}
		if (base.HappyPickupTemplate.ShowItemSound != null)
		{
			Avatar.PlaySoundEffect(base.HappyPickupTemplate.ShowItemSound);
		}
		return true;
	}

	protected override void Finish()
	{
		base.Finish();
		_playerController?.Unlock();
		if (_camera != null)
		{
			_camera.HappyPickupEnd();
		}
	}
}
