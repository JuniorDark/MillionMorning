using System;
using System.Collections.Generic;
using Code.Core.BodyPack;
using Code.Core.HomePack;
using UnityEngine;

namespace Code.Core.Items.Home.HomeEquipment;

public abstract class MilMo_ColoredHomeEquipment : MilMo_HomeEquipment
{
	private Texture2D _baseIcon;

	protected Dictionary<string, int> ColorIndices { get; private set; }

	public MilMo_HomePackBase HomePackBase { get; private set; }

	protected string Name => HomePackBase.Name;

	protected MilMo_ColoredHomeEquipment(MilMo_ItemTemplate template, Dictionary<string, string> modifiers, MilMo_HomePackBase homePackBase)
		: base(template, modifiers)
	{
		ColorIndices = new Dictionary<string, int>();
		HomePackBase = homePackBase;
		foreach (KeyValuePair<string, string> modifier in modifiers)
		{
			if (modifier.Key.StartsWith("ColorGroup"))
			{
				string[] array = modifier.Key.Split(':');
				if (array.Length < 2 || !array[0].Equals("ColorGroup") || array[1].Length == 0)
				{
					Debug.LogWarning("Got invalid ColorGroup key " + modifier.Key);
					continue;
				}
				int value = int.Parse(modifier.Value);
				ColorIndices.Add(HomePackBase.Path + ":" + array[1], value);
			}
		}
		if (ColorIndices.Count != 0 || HomePackBase == null || HomePackBase.ColorGroups.Count <= 0)
		{
			return;
		}
		foreach (ColorGroup colorGroup in HomePackBase.ColorGroups)
		{
			if (colorGroup.ColorIndices.Count > 0)
			{
				string key = "ColorGroup:" + colorGroup.GroupName;
				string key2 = HomePackBase.Path + ":" + colorGroup.GroupName;
				int defaultColorIndex = HomePackBase.GetDefaultColorIndex(colorGroup);
				ColorIndices.Add(key2, defaultColorIndex);
				base.Modifiers.Add(key, defaultColorIndex.ToString());
			}
		}
		RecalculateIdentifier();
	}

	protected override void OnIconLoaded()
	{
		if (!(ItemIcon == null) && ColorIndices.Count != 0 && HomePackBase.ColorGroups.Count != 0)
		{
			if (_baseIcon == null)
			{
				_baseIcon = ItemIcon;
				ItemIcon = new Texture2D(128, 128, TextureFormat.ARGB32, mipChain: false);
			}
			MilMo_ItemIcon.GenerateColoredIcon(_baseIcon, ColorIndices, HomePackBase.Path, HomePackBase.ColorGroups, ItemIcon);
		}
	}

	public bool UpdateColorIndex(string key, int colorIndex)
	{
		if (ColorIndices.ContainsKey(key))
		{
			if (ColorIndices[key].Equals(colorIndex))
			{
				return false;
			}
			ColorIndices[key] = colorIndex;
		}
		else
		{
			ColorIndices.Add(key, colorIndex);
		}
		string key2 = "ColorGroup" + key.Substring(key.LastIndexOf(":", StringComparison.Ordinal));
		base.Modifiers[key2] = colorIndex.ToString();
		RecalculateIdentifier();
		OnIconLoaded();
		return true;
	}

	public int GetColorIndex(string key)
	{
		return ColorIndices[key];
	}

	public bool ContainsColorIndex(string key)
	{
		return ColorIndices.ContainsKey(key);
	}
}
