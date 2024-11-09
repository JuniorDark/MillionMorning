using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Network;
using Code.Core.Template;
using Code.World.Player;
using Core;
using Localization;
using UI.HUD.Dialogues;
using UI.HUD.Dialogues.Modal;
using UI.Sprites;

namespace Code.Core.Items;

public class MilMo_LockBox : MilMo_RandomBox, IUsable
{
	private Action _onUsed;

	private Action _onFail;

	public new MilMo_LockBoxTemplate Template => ((MilMo_Item)this).Template as MilMo_LockBoxTemplate;

	public MilMo_LockBox(MilMo_LockBoxTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public void RegisterOnUsed(Action onUsed)
	{
		_onUsed = (Action)Delegate.Combine(_onUsed, onUsed);
	}

	public void UnregisterOnUsed(Action onUsed)
	{
		_onUsed = (Action)Delegate.Remove(_onUsed, onUsed);
	}

	public void RegisterOnFailedToUse(Action onFail)
	{
		_onFail = (Action)Delegate.Combine(_onFail, onFail);
	}

	public void UnregisterOnFailedToUse(Action onFail)
	{
		_onFail = (Action)Delegate.Remove(_onFail, onFail);
	}

	public bool Use(int entryId)
	{
		if (MilMo_Player.Instance == null || MilMo_Player.Instance.IsExhausted)
		{
			return false;
		}
		string keyTemplateIdentifier = Template.KeyTemplateIdentifier;
		if (!MilMo_Player.Instance.Inventory.HaveItem(keyTemplateIdentifier))
		{
			IsMissingKey(keyTemplateIdentifier);
			return false;
		}
		Singleton<GameNetwork>.Instance.RequestOpenBox(entryId);
		return true;
	}

	private async void IsMissingKey(string requiredKey)
	{
		if (await Singleton<MilMo_TemplateContainer>.Instance.GetTemplateAsync(requiredKey) is MilMo_LockBoxKeyTemplate milMo_LockBoxKeyTemplate)
		{
			DialogueSpawner.SpawnGoToShopDialogue(new EnterShopModalMessageData(new LocalizedStringWithArgument("World_7992"), new LocalizedStringWithArgument("World_7993", milMo_LockBoxKeyTemplate.DisplayName.String), requiredKey, new AddressableSpriteLoader(milMo_LockBoxKeyTemplate.IconPath.Split("/").Last())));
		}
	}
}
