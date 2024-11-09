using Code.Core.ResourceSystem;
using Core.GameEvent;
using UI.HUD.Dialogues.ClassChoice.Abilities;
using UI.Tooltip.Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.Dialogues.ClassChoice;

public class RewardAbility : MonoBehaviour
{
	[SerializeField]
	private Image icon;

	[SerializeField]
	private ClassAbility ability;

	public void Init(ClassAbility newAbility)
	{
		ability = newAbility;
		icon.sprite = newAbility.GetIcon();
	}

	private void OnValidate()
	{
		icon.sprite = ability.GetIcon();
	}

	public void ShowTooltip()
	{
		if (!(ability == null))
		{
			string @string = MilMo_Localization.GetLocString(ability.GetClassName()).String;
			string string2 = MilMo_Localization.GetLocString(ability.GetDescription()).String;
			ItemTooltipData args = new ItemTooltipData(@string, string2, ability.GetIcon());
			GameEvent.ShowTooltipEvent?.RaiseEvent(args);
		}
	}

	public void HideTooltip()
	{
		GameEvent.HideTooltipEvent?.RaiseEvent();
	}
}
