using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core.Items;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.World.Player;
using Core.State;
using UI.Elements.Slot;
using UI.Inventory;
using UnityEngine;

namespace UI.Tooltip.Data.GenerateTooltipData;

public class GenerateConverterTooltipData : GenerateTooltipData
{
	private readonly UI.Inventory.Inventory _inventory;

	public GenerateConverterTooltipData(IEntryItem item, Texture2D icon)
		: base(item, icon)
	{
		_inventory = UI.Inventory.Inventory.GetPlayerInventory();
	}

	public override async Task<TooltipData> CreateTooltip()
	{
		IEntryItem item = Item;
		if (!(item is MilMo_Converter converter))
		{
			return null;
		}
		if (converter.Template == null)
		{
			return null;
		}
		string title = converter.Template.DisplayName?.String;
		string description = converter.Template.Description?.String;
		ItemTooltipData reward = await GetRewardDataAsync(converter);
		List<ItemIngredientData> components = await GetConverterComponentsAsync(converter);
		if (components.Count <= 0)
		{
			Debug.LogWarning("Converter has no components");
			return null;
		}
		ItemRequiredToolData requiredTool = await GetRequiredToolAsync(converter);
		return new ConverterTooltipData(title, description, Icon, requiredTool, components.ToArray(), reward);
	}

	private async Task<ItemRequiredToolData> GetRequiredToolAsync(MilMo_Converter converter)
	{
		if (converter.Template.RequiredTool == null)
		{
			return null;
		}
		string identifier = converter.Template.RequiredTool.GetIdentifier();
		if (string.IsNullOrEmpty(identifier))
		{
			return null;
		}
		MilMo_Item milMo_Item = await MilMo_Item.AsyncGetItem(identifier);
		MilMo_LocString copy = MilMo_Localization.GetLocString("World_10261").GetCopy();
		copy.SetFormatArgs(milMo_Item.Template.DisplayName);
		return new ItemRequiredToolData(name: copy.String, icon: await milMo_Item.AsyncGetIcon());
	}

	private async Task<ItemTooltipData> GetRewardDataAsync(MilMo_Converter converter)
	{
		if (MilMo_Player.Instance == null || MilMo_Player.Instance.Avatar == null)
		{
			Debug.LogWarning("Player is null (MilMo_ConverterPopup)");
			return null;
		}
		TemplateReference templateReference = ((MilMo_Player.Instance.Avatar.Gender == 1) ? converter.Template.GirlReward.GetTemplate() : converter.Template.BoyReward.GetTemplate());
		if (templateReference == null)
		{
			Debug.LogWarning("Reward is null (MilMo_ConverterPopup)");
			return null;
		}
		MilMo_Item milMo_Item = await MilMo_Item.AsyncGetItem(templateReference.GetIdentifier());
		if (milMo_Item == null)
		{
			Debug.LogWarning("Unable to get reward item");
			return null;
		}
		string title = milMo_Item.Template.DisplayName?.String;
		string description = milMo_Item.Template.Description?.String;
		return new ItemTooltipData(title, description, await milMo_Item.AsyncGetIcon());
	}

	private async Task<List<ItemIngredientData>> GetConverterComponentsAsync(MilMo_Converter converter)
	{
		List<ItemIngredientData> ingredients = new List<ItemIngredientData> { GetGemsComponent(converter) };
		try
		{
			foreach (TemplateCountPair ingredient in converter.Template.Ingredients)
			{
				string templateIdentifier = ingredient.GetTemplate().GetIdentifier();
				if (string.IsNullOrEmpty(templateIdentifier))
				{
					Debug.LogWarning("Ingredient is null");
					continue;
				}
				MilMo_Item item = await MilMo_Item.AsyncGetItem(templateIdentifier);
				ISlotItemEntry entry = _inventory.GetEntryByTemplateIdentifier(templateIdentifier);
				Texture2D icon = await item.AsyncGetIcon();
				string name = item.Template.DisplayName?.String;
				int currentAmount = entry?.GetAmount() ?? 0;
				int count = ingredient.GetCount();
				ItemIngredientData item2 = new ItemIngredientData(icon, name, currentAmount, count);
				ingredients.Add(item2);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.StackTrace);
		}
		ingredients.RemoveAll((ItemIngredientData item) => item == null);
		return ingredients;
	}

	private ItemIngredientData GetGemsComponent(MilMo_Converter converter)
	{
		if (converter.Template.RequiredGems <= 0)
		{
			return null;
		}
		string @string = MilMo_Localization.GetLocString("World_5541").String;
		int currentAmount = GlobalStates.Instance.playerState.gems.Get();
		int requiredGems = converter.Template.RequiredGems;
		return new ItemIngredientData(null, @string, currentAmount, requiredGems);
	}
}
