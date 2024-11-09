using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Widget;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.CharacterShop;

public static class MilMo_ShopAssetManager
{
	private class LoadingItem
	{
		public readonly MilMo_ItemCard itemCard;

		public LoadingItem(MilMo_ItemCard itemCard)
		{
			this.itemCard = itemCard;
		}
	}

	private const int maxNumberOfLoadedItems = 20;

	private static readonly Stack<LoadingItem> m_QueuedItems = new Stack<LoadingItem>();

	private static readonly Queue<LoadingItem> m_LoadedItems = new Queue<LoadingItem>();

	public static void CategoryChanged(IList<MilMo_CharacterShopCategory> mainCategories, MilMo_CharacterShopCategory currentMainCategory)
	{
		bool num = m_QueuedItems.Count == 0;
		LoadMainCategory(currentMainCategory);
		if (num)
		{
			NextItem();
		}
	}

	private static void LoadMainCategory(MilMo_CharacterShopCategory mainCategory)
	{
		IList<MilMo_ScrollView> scrollViewList = mainCategory.ScrollViewList;
		if (scrollViewList.Count != 0)
		{
			LoadSubCategory(scrollViewList[mainCategory.CurrentSubCategory]);
		}
	}

	private static void LoadSubCategory(MilMo_Widget subCategory)
	{
		for (int num = subCategory.Children.Count - 1; num >= 0; num--)
		{
			if (subCategory.Children[num] is MilMo_ItemCard { HaveIcon: false } milMo_ItemCard)
			{
				m_QueuedItems.Push(new LoadingItem(milMo_ItemCard));
			}
		}
	}

	private static void NextItem()
	{
		if (m_QueuedItems.Count != 0)
		{
			LoadingItem item = m_QueuedItems.Pop();
			item.itemCard.ShopItem.AsyncGetIcon(MilMo_ResourceManager.Priority.High, delegate(Texture2D icon)
			{
				item.itemCard.SetIcon(icon);
				_ = m_LoadedItems.Count;
				_ = 20;
				MilMo_EventSystem.At(0.1f, NextItem);
			});
		}
	}
}
