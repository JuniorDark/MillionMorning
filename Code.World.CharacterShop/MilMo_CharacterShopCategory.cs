using System.Collections.Generic;
using Code.Core.GUI;
using Code.Core.GUI.Widget;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.CharacterShop;

public sealed class MilMo_CharacterShopCategory
{
	public int Number;

	public MilMo_Button Button;

	public MilMo_Widget ButtonReflect;

	public int CurrentSubCategory;

	public float ScrollTarget;

	public Color CaptionColor;

	public readonly List<MilMo_Button> TabList = new List<MilMo_Button>();

	public readonly List<MilMo_ScrollView> ScrollViewList = new List<MilMo_ScrollView>();

	public string IdentifierName { get; private set; }

	public MilMo_LocString DisplayName { get; private set; }

	public MilMo_CharacterShopCategory(int categoryIndex, string identifierName, MilMo_LocString displayName)
	{
		IdentifierName = identifierName;
		DisplayName = displayName;
		CaptionColor = new Color(1f, 1f, 1f, 1f);
	}
}
