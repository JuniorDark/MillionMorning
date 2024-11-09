using System;
using System.Collections.Generic;
using Core.Utilities;

namespace Code.Core.Avatar.Badges;

public class BadgeManager
{
	private readonly MilMo_Avatar _avatar;

	private readonly List<BaseBadge> _badges;

	public BadgeManager(MilMo_Avatar avatar)
	{
		_avatar = avatar;
		_badges = new List<BaseBadge>();
		InitAllBadgeTypes();
	}

	public void Destroy()
	{
		foreach (BaseBadge badge in _badges)
		{
			badge.Destroy();
		}
		_badges.Clear();
	}

	private void InitAllBadgeTypes()
	{
		Type[] allDerivedTypes = AppDomain.CurrentDomain.GetAllDerivedTypes<BaseBadge>();
		for (int i = 0; i < allDerivedTypes.Length; i++)
		{
			BaseBadge item = (BaseBadge)Activator.CreateInstance(allDerivedTypes[i], _avatar);
			_badges.Add(item);
		}
	}

	public IEnumerable<BaseBadge> GetAll()
	{
		return _badges;
	}
}
