using System.Collections.Generic;
using System.Linq;
using Code.Core.Avatar;
using Code.Core.Avatar.Badges;
using Core.Utilities;
using UnityEngine;

namespace UI.Marker.Combat;

public class Badges : MonoBehaviour
{
	private MilMo_Avatar _avatar;

	private List<BaseBadge> _baseBadges = new List<BaseBadge>();

	[SerializeField]
	private List<BadgeTwo> badges;

	private void Start()
	{
		IHasAvatar componentInParent = base.gameObject.GetComponentInParent<IHasAvatar>();
		if (componentInParent == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find IHasAvatar");
			return;
		}
		_avatar = componentInParent.GetAvatar();
		if (_avatar == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find MilMo_Avatar");
			return;
		}
		_baseBadges = _avatar.GetAllBadges().ToList();
		if (_baseBadges.Count < 1)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find any badges");
		}
		else
		{
			LoadBadges();
		}
	}

	private void OnDestroy()
	{
		_baseBadges.Clear();
		foreach (BadgeTwo badge in badges)
		{
			UnityEngine.Object.Destroy(badge);
		}
	}

	public void Show(bool shouldShow)
	{
		if (base.gameObject.activeSelf != shouldShow)
		{
			base.gameObject.SetActive(shouldShow);
		}
	}

	private void LoadBadges()
	{
		foreach (BaseBadge baseBadge in _baseBadges)
		{
			BadgeTwo badgeTwo = Instantiator.Instantiate<BadgeTwo>("BadgeTwo", base.transform);
			if (!badgeTwo)
			{
				Debug.LogWarning("Unable to load Badge: " + baseBadge.GetIdentifier());
			}
			badgeTwo.Init(baseBadge);
			badges.Add(badgeTwo);
		}
	}
}
