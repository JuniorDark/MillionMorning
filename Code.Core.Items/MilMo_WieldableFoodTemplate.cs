using System.Collections.Generic;
using Code.Core.Network.types;
using UnityEngine;

namespace Code.Core.Items;

public class MilMo_WieldableFoodTemplate : MilMo_WieldableTemplate
{
	public Dictionary<sbyte, string> UseEmotes { get; protected set; }

	public Dictionary<sbyte, WieldableFoodOnUse> OnUse { get; protected set; }

	public sbyte Uses { get; protected set; }

	public Color ProgressBarColor { get; protected set; }

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		WieldableFoodTemplate wieldableFoodTemplate = t as WieldableFoodTemplate;
		base.LoadFromNetwork((Code.Core.Network.types.Template)wieldableFoodTemplate);
		if (wieldableFoodTemplate == null)
		{
			return false;
		}
		base.WieldAnimations = new List<string>();
		foreach (WieldableFoodUseEmote useEmote in wieldableFoodTemplate.GetUseEmotes())
		{
			if (!UseEmotes.ContainsKey(useEmote.GetUseNumber()))
			{
				UseEmotes.Add(useEmote.GetUseNumber(), useEmote.GetEmote());
			}
			else
			{
				UseEmotes[useEmote.GetUseNumber()] = useEmote.GetEmote();
			}
		}
		foreach (WieldableFoodOnUse item in wieldableFoodTemplate.GetOnUse())
		{
			if (!OnUse.ContainsKey(item.GetUseNumber()))
			{
				OnUse.Add(item.GetUseNumber(), item);
			}
			else
			{
				OnUse[item.GetUseNumber()] = item;
			}
		}
		Uses = wieldableFoodTemplate.GetUses();
		color progressBarColor = wieldableFoodTemplate.GetProgressBarColor();
		ProgressBarColor = new Color(progressBarColor.getR(), progressBarColor.getG(), progressBarColor.getB(), progressBarColor.getA());
		return true;
	}

	protected MilMo_WieldableFoodTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "WieldableFood")
	{
		OnUse = new Dictionary<sbyte, WieldableFoodOnUse>();
		UseEmotes = new Dictionary<sbyte, string>();
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_WieldableFood(this, modifiers);
	}

	public new static MilMo_WieldableFoodTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_WieldableFoodTemplate(category, path, filePath);
	}
}
