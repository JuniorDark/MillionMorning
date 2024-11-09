using System;
using System.Collections.Generic;
using Code.Core.BodyPack;
using Code.Core.EventSystem;
using Code.World.Player;
using UnityEngine;

namespace Code.Core.Items;

public class MilMo_Wearable : MilMo_Item, IUsable
{
	private Texture2D _baseIcon;

	private Action _onUsed;

	private Action _onFail;

	public MilMo_BodyPack BodyPack { get; protected set; }

	public IDictionary<string, int> ColorIndices { get; protected set; }

	public new MilMo_WearableTemplate Template => base.Template as MilMo_WearableTemplate;

	public override ItemGender UseableGender => BodyPack.Gender switch
	{
		MilMo_BodyPackGender.Boy => ItemGender.Boy, 
		MilMo_BodyPackGender.Girl => ItemGender.Girl, 
		_ => ItemGender.Both, 
	};

	public MilMo_Wearable(MilMo_WearableTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
		ColorIndices = new Dictionary<string, int>();
		BodyPack = MilMo_BodyPackSystem.GetBodyPackByName(template.BodyPackName);
		if (BodyPack == null)
		{
			Debug.LogWarning("Failed to get body pack " + template.BodyPackName + " when creating wearable item " + template.Identifier);
			return;
		}
		foreach (KeyValuePair<string, string> modifier in modifiers)
		{
			if (modifier.Key.StartsWith("ColorGroup"))
			{
				string[] array = modifier.Key.Split(':');
				if (array.Length < 2 || !array[0].Equals("ColorGroup") || array[1].Length == 0)
				{
					Debug.LogWarning("Got invalid ColorGroup key " + modifier.Key + " in wearable " + template.BodyPackName);
					continue;
				}
				int value = int.Parse(modifier.Value);
				ColorIndices.Add(BodyPack.Path + ":" + array[1], value);
			}
		}
		if (ColorIndices.Count != 0 || BodyPack.ColorGroups.Count <= 0)
		{
			return;
		}
		foreach (ColorGroup colorGroup in BodyPack.ColorGroups)
		{
			if (colorGroup.ColorIndices.Count > 0)
			{
				string key = "ColorGroup:" + colorGroup.GroupName;
				string key2 = BodyPack.Path + ":" + colorGroup.GroupName;
				int defaultColorIndex = BodyPack.GetDefaultColorIndex(colorGroup);
				ColorIndices.Add(key2, defaultColorIndex);
				base.Modifiers.Add(key, defaultColorIndex.ToString());
			}
		}
		RecalculateIdentifier();
	}

	public string GetColorGroup(int index = 0)
	{
		return BodyPack.Path + ":" + BodyPack.ColorGroups[index].GroupName;
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

	public virtual bool Use(int entryId)
	{
		if (MilMo_Player.Instance == null || MilMo_Player.Instance.IsExhausted)
		{
			return false;
		}
		MilMo_EventSystem.Instance.PostEvent("toggle_wearable", entryId);
		return true;
	}

	public override void UnloadIcon()
	{
	}

	protected override void OnIconLoaded()
	{
		if (!(ItemIcon == null) && ColorIndices.Count != 0 && BodyPack.ColorGroups.Count != 0)
		{
			if (_baseIcon == null)
			{
				_baseIcon = ItemIcon;
				ItemIcon = new Texture2D(128, 128, TextureFormat.ARGB32, mipChain: false);
			}
			MilMo_ItemIcon.GenerateColoredIcon(_baseIcon, ColorIndices, BodyPack.Path, BodyPack.ColorGroups, ItemIcon);
		}
	}

	public override bool IsWieldable()
	{
		return false;
	}

	public override bool IsWearable()
	{
		return true;
	}

	public override bool IsUseableByGender(bool isBoy)
	{
		if (BodyPack == null)
		{
			return false;
		}
		if (BodyPack.Gender == MilMo_BodyPackGender.Boy && !isBoy)
		{
			return false;
		}
		if (BodyPack.Gender == MilMo_BodyPackGender.Girl && isBoy)
		{
			return false;
		}
		return true;
	}

	public void UpdateColorIndex(string colorGroup, int index)
	{
		if (ColorIndices.ContainsKey(colorGroup))
		{
			ColorIndices[colorGroup] = index;
			string key = "ColorGroup" + colorGroup.Substring(colorGroup.LastIndexOf(":", StringComparison.Ordinal));
			base.Modifiers[key] = index.ToString();
			RecalculateIdentifier();
			OnIconLoaded();
		}
	}

	public int GetColor(string colorGroup)
	{
		return ColorIndices[colorGroup];
	}
}
