using System;

namespace Code.Core.Avatar.Badges;

public abstract class BaseBadge
{
	protected readonly MilMo_Avatar Avatar;

	public event Action OnChange = delegate
	{
	};

	public BaseBadge(MilMo_Avatar avatar)
	{
		Avatar = avatar;
	}

	protected void TriggerOnChange()
	{
		this.OnChange?.Invoke();
	}

	public abstract void Destroy();

	public abstract string GetIconPath();

	public virtual string GetText()
	{
		return "";
	}

	public abstract string GetIdentifier();

	public virtual string GetTooltipText()
	{
		return "";
	}

	public abstract bool IsEarned();
}
