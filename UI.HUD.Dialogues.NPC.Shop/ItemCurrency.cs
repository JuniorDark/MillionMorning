using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core.Items;
using Code.Core.Template;
using Code.World.Inventory;
using Core;
using UI.Inventory;
using UnityEngine;
using UnityEngine.Events;

namespace UI.HUD.Dialogues.NPC.Shop;

public class ItemCurrency : Currency
{
	private Texture2D _texture2D;

	private readonly string _templateIdentifier;

	private readonly UI.Inventory.Inventory _inventory;

	public ItemCurrency(UI.Inventory.Inventory inventory, string templateIdentifier)
	{
		_templateIdentifier = templateIdentifier;
		_inventory = inventory;
	}

	public override string GetIdentifier()
	{
		return _templateIdentifier;
	}

	public override int GetAmount()
	{
		string templateIdentifier = _templateIdentifier;
		return (_inventory.GetEntryByTemplateIdentifier(templateIdentifier) as MilMo_InventoryEntry)?.GetAmount() ?? 0;
	}

	public override Texture2D GetTexture2D()
	{
		return _texture2D;
	}

	public override void RegisterOnChange(UnityAction<Currency> action)
	{
	}

	public override void UnregisterOnChange(UnityAction<Currency> action)
	{
	}

	public override void LoadIconTexture()
	{
		if (!(_texture2D != null))
		{
			LoadIconTextureAsync();
		}
	}

	private async Task LoadIconTextureAsync()
	{
		if (!(await Singleton<MilMo_TemplateContainer>.Instance.GetTemplateAsync(_templateIdentifier) is MilMo_ItemTemplate milMo_ItemTemplate))
		{
			Debug.LogWarning("Could not fetch item template for " + _templateIdentifier + ".");
		}
		else
		{
			_texture2D = await milMo_ItemTemplate.Instantiate(new Dictionary<string, string>()).AsyncGetIcon();
		}
	}
}
