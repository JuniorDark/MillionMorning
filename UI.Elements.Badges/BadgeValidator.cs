using System;
using Code.Core.Avatar;
using Code.World.Player;
using Core;
using Player;
using UnityEngine;

namespace UI.Elements.Badges;

[Serializable]
public class BadgeValidator
{
	protected MilMo_Avatar Avatar;

	private BadgeSO.BadgeType _type;

	public BadgeValidator(MilMo_Avatar avatar, BadgeSO.BadgeType type)
	{
		Avatar = avatar;
		_type = type;
	}

	private MilMo_Player GetPlayer()
	{
		return MilMo_Player.Instance;
	}

	public virtual bool Validate()
	{
		switch (_type)
		{
		case BadgeSO.BadgeType.Level:
			return true;
		case BadgeSO.BadgeType.MemberPlusPlus:
			return Avatar.MembershipDaysLeft > 30000;
		case BadgeSO.BadgeType.MemberPlus:
			return Avatar.MembershipDaysLeft > 93;
		case BadgeSO.BadgeType.Member:
			return Avatar.MembershipDaysLeft > 31;
		case BadgeSO.BadgeType.Gm:
		{
			sbyte role = Avatar.Role;
			return role == 1 || role == 2;
		}
		case BadgeSO.BadgeType.Admin:
		{
			sbyte role = Avatar.Role;
			return role == 3 || role == 4;
		}
		case BadgeSO.BadgeType.GroupLeader:
			if (Avatar.Player.IsLocalPlayer && Singleton<GroupManager>.Instance.InGroup(Avatar.Id))
			{
				return Singleton<GroupManager>.Instance.IsGroupLeader(Avatar.Id);
			}
			return false;
		default:
			Debug.LogError("Invalid BadgeType");
			throw new ArgumentOutOfRangeException();
		case BadgeSO.BadgeType.Role:
			return false;
		}
	}
}
