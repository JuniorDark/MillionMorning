using System;
using System.Linq;
using System.Threading.Tasks;
using Code.Core.Emote;
using UnityEngine;

namespace Code.Core.Avatar.HappyPickup;

public abstract class HappyPickup
{
	protected readonly MilMo_Avatar Avatar;

	protected readonly HappyPickupItem Item;

	protected HappyPickupTemplate HappyPickupTemplate => Item?.HappyPickupTemplate;

	protected HappyPickup(MilMo_Avatar avatar, HappyPickupItem item)
	{
		Avatar = avatar;
		Item = item;
	}

	private double GetAnimationLength(string animationName)
	{
		Animation component = Avatar.GameObject.GetComponent<Animation>();
		return (component != null && component[animationName] != null) ? component[animationName].length : 0f;
	}

	private Transform GetAttachNode(string nodeName)
	{
		return Avatar.GameObject.GetComponentsInChildren<Transform>().FirstOrDefault((Transform node) => node.name == nodeName);
	}

	public async void BeHappy(Action stopBeingHappy = null)
	{
		if (HappyPickupTemplate == null)
		{
			stopBeingHappy?.Invoke();
			return;
		}
		int num = (int)(HappyPickupTemplate.ItemAppearanceTime * 1000f);
		int phaseTwoTime = (int)(HappyPickupTemplate.Duration * 1000f);
		if (PhaseOne() && num > 0)
		{
			await Task.Delay(num);
		}
		if (PhaseTwo() && phaseTwoTime > 0)
		{
			await Task.Delay(phaseTwoTime);
		}
		Finish();
		await Task.Delay(100);
		stopBeingHappy?.Invoke();
	}

	protected virtual bool PhaseOne()
	{
		if (Avatar == null || Avatar.IsDestroyed || !Avatar.GameObject)
		{
			return false;
		}
		Avatar.InHappyPickup = true;
		Avatar.PickUpItem = Item;
		if (Item?.GameObject != null)
		{
			Item.GameObject.SetActive(value: false);
		}
		if (Avatar.WieldedItem != null)
		{
			Avatar.UnequipLocal(Avatar.WieldedItem);
			Avatar.AsyncApply();
		}
		if (!string.IsNullOrEmpty(HappyPickupTemplate.Animation))
		{
			double animationLength = GetAnimationLength(HappyPickupTemplate.Animation);
			if ((double)HappyPickupTemplate.Duration > animationLength)
			{
				Avatar.PlayAnimation(HappyPickupTemplate.Animation + "Idle");
			}
			Avatar.PlayAnimation(HappyPickupTemplate.Animation);
		}
		if (!string.IsNullOrEmpty(HappyPickupTemplate.Emote))
		{
			Avatar.PlayEmoteLocal(MilMo_EmoteSystem.GetEmoteByName(HappyPickupTemplate.Emote), HappyPickupTemplate.Duration);
		}
		return true;
	}

	protected virtual bool PhaseTwo()
	{
		if (Avatar == null || Avatar.IsDestroyed || !Avatar.GameObject)
		{
			return false;
		}
		if (Item?.GameObject == null)
		{
			return false;
		}
		Item.GameObject.SetActive(value: true);
		Transform attachNode = GetAttachNode(HappyPickupTemplate.ItemNode);
		if ((bool)attachNode)
		{
			Transform transform = Item.GameObject.transform;
			transform.parent = attachNode;
			transform.localPosition = HappyPickupTemplate.ItemOffset;
			transform.localEulerAngles = Vector3.zero;
		}
		else
		{
			Debug.LogWarning("Failed to find node " + HappyPickupTemplate.ItemNode + " for happy pickup type " + HappyPickupTemplate.Path);
		}
		return true;
	}

	protected virtual void Finish()
	{
		if (Avatar != null && !Avatar.IsDestroyed && (bool)Avatar.GameObject)
		{
			Avatar.PickUpItem = null;
			Item?.Destroy();
			if (Avatar.WieldedItem != null)
			{
				Avatar.EquipLocal(Avatar.WieldedItem);
				Avatar.AsyncApply();
			}
			Avatar.PlayAnimation(Avatar.IdleAnimation);
			Avatar.InHappyPickup = false;
		}
	}
}
