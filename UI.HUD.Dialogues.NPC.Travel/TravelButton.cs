using Code.World.Player;
using Core.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.HUD.Dialogues.NPC.Travel;

public class TravelButton : DialogueButton
{
	[SerializeField]
	private Image requiredLevelTag;

	[SerializeField]
	private TMP_Text requiredLevelText;

	[SerializeField]
	private Image priceTag;

	[SerializeField]
	private TMP_Text priceText;

	[SerializeField]
	private UnityEvent onFlashPriceTag;

	[SerializeField]
	private UnityEvent onFlashRequiredLevelTag;

	[SerializeField]
	private Texture2D gemIcon;

	[SerializeField]
	private Texture2D ticketIcon;

	[SerializeField]
	private Texture2D premiumIcon;

	private TravelInfo _travelInfo;

	public override void Init(DialogueButtonInfo dialogueButtonInfo)
	{
		if (!(dialogueButtonInfo is TravelInfo travelInfo))
		{
			Debug.LogError(base.name + ": Got wrong type of dialogueButtonInfo");
			return;
		}
		_travelInfo = travelInfo;
		if ((bool)Fader)
		{
			Fader.FadeOutFast();
		}
		SetLabel(travelInfo.GetLabelText());
		if (travelInfo.ToLowLevel)
		{
			AddOnClick(FlashLevelTag);
		}
		else if (!travelInfo.EnoughGems || !travelInfo.EnoughTickets)
		{
			AddOnClick(FlashPriceTag);
		}
		else
		{
			AddOnClick(travelInfo.GetAction());
		}
		RefreshRequiredLevelTag();
		RefreshPriceTag();
	}

	protected override void Awake()
	{
		base.Awake();
		if (requiredLevelTag == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing requiredLevelTag");
		}
		else if (requiredLevelText == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing requiredLevelText");
		}
		else if (priceTag == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing priceTag");
		}
		else if (priceText == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing priceText");
		}
	}

	private void RefreshRequiredLevelTag()
	{
		EnableRequiredLevelTag(shouldEnable: false);
		if (_travelInfo.ToLowLevel)
		{
			SetRequiredLevel(_travelInfo.RequiredAvatarLevel);
			EnableRequiredLevelTag(shouldEnable: true);
		}
	}

	private void RefreshPriceTag()
	{
		EnablePriceTag(shouldEnable: false);
		if (_travelInfo != null && _travelInfo.HasLimits)
		{
			int price = 0;
			Texture2D priceIcon = null;
			if (_travelInfo.LocationIsMembersOnly)
			{
				priceIcon = premiumIcon;
				price = 0;
			}
			else if (_travelInfo.PriceInGems > 0)
			{
				priceIcon = gemIcon;
				price = _travelInfo.PriceInGems;
			}
			else if (_travelInfo.PriceInHelicopterTickets > 0)
			{
				price = _travelInfo.PriceInHelicopterTickets;
				priceIcon = ticketIcon;
			}
			SetPrice(price);
			SetPriceIcon(priceIcon);
			EnablePriceTag(shouldEnable: true);
		}
	}

	private void SetRequiredLevel(int level)
	{
		if (!(requiredLevelText == null))
		{
			requiredLevelText.text = level.ToString();
		}
	}

	private void SetPrice(int price)
	{
		if (!(priceText == null))
		{
			priceText.text = ((price > 0) ? price.ToString() : "");
		}
	}

	private void EnablePriceTag(bool shouldEnable)
	{
		if (!(priceTag == null))
		{
			priceTag.gameObject.SetActive(shouldEnable);
		}
	}

	private void EnableRequiredLevelTag(bool shouldEnable)
	{
		if (!(requiredLevelTag == null))
		{
			requiredLevelTag.gameObject.SetActive(shouldEnable);
		}
	}

	private void SetPriceIcon(Texture2D texture2D)
	{
		if (!(priceTag == null))
		{
			priceTag.enabled = false;
			if (!(texture2D == null))
			{
				Core.Utilities.UI.SetIcon(priceTag, texture2D);
			}
		}
	}

	private void FlashPriceTag()
	{
		onFlashPriceTag?.Invoke();
	}

	private void FlashLevelTag()
	{
		onFlashRequiredLevelTag?.Invoke();
	}
}
