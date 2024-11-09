using Code.Core.Avatar;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace UI.Elements.Badges;

[CreateAssetMenu(fileName = "new badge", menuName = "Badge/New Badge")]
public class BadgeSO : ScriptableObject
{
	public enum BadgeType
	{
		Level,
		Member,
		MemberPlus,
		MemberPlusPlus,
		Role,
		Gm,
		Admin,
		GroupLeader
	}

	[SerializeField]
	private AssetReferenceSprite spriteRef;

	[SerializeField]
	private BadgeType type;

	[SerializeField]
	private LocalizedString tooltipLocString;

	private BadgeValidator _badgeValidator;

	private Sprite _sprite;

	private MilMo_Avatar _avatar;

	public void Init(MilMo_Avatar avatar)
	{
		Debug.LogWarning("BadgeSO Init");
		_avatar = avatar;
		if (!spriteRef.RuntimeKeyIsValid())
		{
			Debug.LogError("Badge Missing addressable sprite!");
			return;
		}
		_sprite = spriteRef.LoadAssetAsync<Sprite>().WaitForCompletion();
		_badgeValidator = new BadgeValidator(_avatar, type);
	}

	public Sprite GetSprite()
	{
		return _sprite;
	}

	public bool IsEarned(MilMo_Avatar avatar = null)
	{
		if (avatar != null)
		{
			_avatar = avatar;
		}
		if (_avatar == null)
		{
			return false;
		}
		if (_badgeValidator != null)
		{
			return _badgeValidator.Validate();
		}
		return false;
	}
}
