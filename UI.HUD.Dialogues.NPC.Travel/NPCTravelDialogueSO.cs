using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.World.GUI.LoadingScreen;
using Code.World.Level;
using Code.World.Player;
using Core;
using Core.GameEvent;
using Localization;
using Player;
using UI.HUD.Dialogues.Modal;
using UI.Sprites;
using UnityEngine;

namespace UI.HUD.Dialogues.NPC.Travel;

public class NPCTravelDialogueSO : NPCDialogueSO
{
	private List<TravelInfo> _travelLocations;

	public override string GetAddressableKey()
	{
		return "NPCTravelDialogue";
	}

	public override void Init(NPCMessageData npcMessageData)
	{
		if (!(npcMessageData is NPCTravelMessageData nPCTravelMessageData))
		{
			return;
		}
		base.Init(npcMessageData);
		_travelLocations = nPCTravelMessageData.GetTravelLocations();
		foreach (TravelInfo travelLocation in _travelLocations)
		{
			travelLocation.SetAction(delegate
			{
				ConfirmTravel(travelLocation);
			});
		}
	}

	private void ConfirmTravel(TravelInfo travelInfo)
	{
		if (!MayTravel(travelInfo))
		{
			Fail();
		}
		else if (!MilMo_Player.Instance.OkToTeleport())
		{
			Fail();
		}
		else
		{
			Success(travelInfo);
		}
	}

	private void NeedMemberShip()
	{
		DialogueSpawner.SpawnGoToShopDialogue(new EnterShopModalMessageData(new LocalizedStringWithArgument("World_467"), new LocalizedStringWithArgument("World_7629"), "Shop:Batch01.Subscriptions.SubscriptionSixMonths", new AddressableSpriteLoader("IconPremium")));
	}

	private void NeedGems()
	{
		GameEvent.GemsNotEnoughEvent.RaiseEvent();
	}

	private void NeedTickets()
	{
		DialogueSpawner.SpawnGoToShopDialogue(new EnterShopModalMessageData(new LocalizedStringWithArgument("World_467"), new LocalizedStringWithArgument("World_7629"), "Shop:Batch01.Subscriptions.SubscriptionSixMonths", new AddressableSpriteLoader("IconPremium")));
	}

	private void ToLowLevel()
	{
		GameEvent.PlayerTooLowLevelEvent?.RaiseEvent();
	}

	private bool MayTravel(TravelInfo travelInfo)
	{
		if (travelInfo.NeedMembership)
		{
			NeedMemberShip();
			return false;
		}
		if (!travelInfo.EnoughGems)
		{
			NeedGems();
			return false;
		}
		if (!travelInfo.EnoughTickets)
		{
			NeedTickets();
			return false;
		}
		if (travelInfo.ToLowLevel)
		{
			ToLowLevel();
			return false;
		}
		return true;
	}

	private async void Success(TravelInfo travelInfo)
	{
		MilMo_Player.Instance.RequestLevelChange(GetNPCId(), travelInfo.LevelIndex);
		DialogueWindow.Close();
		if (Singleton<GroupManager>.Instance.PlayerIsInGroup)
		{
			return;
		}
		MilMo_LoadingScreen.Instance.LevelLoadFade(12f, travelInfo.ArriveSound);
		await Task.Delay(500);
		if (!string.IsNullOrEmpty(travelInfo.TravelSound))
		{
			AudioClip audioClip = await MilMo_ResourceManager.Instance.LoadAudioAsync(travelInfo.TravelSound);
			if (audioClip != null)
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(audioClip);
			}
		}
		MilMo_LocString locString = MilMo_Localization.GetLocString("Generic_56");
		MilMo_LocString locString2 = MilMo_Localization.GetLocString(travelInfo.DisplayName);
		locString.SetFormatArgs(locString2.String);
		MilMo_LoadingScreen.Instance.SetLoadingText(locString);
		MilMo_LevelData.LoadAndSetLevelIcon(travelInfo.World, travelInfo.Level);
	}

	private async void Fail()
	{
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
		await Task.Delay(300);
		DialogueWindow.Close();
	}

	public List<TravelInfo> GetTravelInfo()
	{
		return _travelLocations;
	}
}
