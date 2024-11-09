using System;
using System.Collections.Generic;
using Code.Core.Avatar;
using Code.Core.EventSystem;
using Code.Core.Network.types;
using Code.Core.Template;
using Core;
using UnityEngine;

namespace Code.Core.Items;

public class MilMo_WieldableFood : MilMo_Wieldable
{
	private const string IDLE_ANIMATION = "IdleWieldableFood";

	private float _lastEatTime;

	public sbyte UsesLeft { get; private set; }

	public float TimeSinceLastEat => Time.time - _lastEatTime;

	public new MilMo_WieldableFoodTemplate Template => ((MilMo_Item)this).Template as MilMo_WieldableFoodTemplate;

	public MilMo_WieldableFood(MilMo_WieldableFoodTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
		ReadUsesLeftFromModifiers();
		RunAnimation = "RunWieldableFood";
	}

	public override bool IsFood()
	{
		return true;
	}

	public override void Wield(MilMo_Avatar wielder, bool overrideIdleAnimation)
	{
		if (IsWielded)
		{
			return;
		}
		base.Wield(wielder, overrideIdleAnimation);
		if (overrideIdleAnimation && Wielder != null)
		{
			Wielder.StackAnimation("LandIdle", "IdleWieldableFood");
		}
		foreach (WieldableFoodOnUse value in Template.OnUse.Values)
		{
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(value.GetPlayerState(), delegate
			{
			});
		}
	}

	public override void Unwield()
	{
		if (IsWielded)
		{
			base.Unwield();
			if (Wielder != null)
			{
				Wielder.UnstackAnimation("LandIdle", "IdleWieldableFood");
			}
		}
	}

	protected override void ModifierChanged(string key)
	{
		base.ModifierChanged(key);
		if (key.Equals("UsesLeft"))
		{
			ReadUsesLeftFromModifiers();
		}
		MilMo_EventSystem.Instance.PostEvent("wieldable_food_uses_left_changed", this);
	}

	public string GetUseEmote(sbyte useNumber)
	{
		if (Template.UseEmotes.ContainsKey(useNumber))
		{
			return Template.UseEmotes[useNumber];
		}
		if (Template.UseEmotes.ContainsKey(0))
		{
			return Template.UseEmotes[0];
		}
		return "";
	}

	public void UpdateLastEatTime()
	{
		_lastEatTime = Time.time;
	}

	private void ReadUsesLeftFromModifiers()
	{
		try
		{
			if (base.Modifiers.ContainsKey("UsesLeft"))
			{
				UsesLeft = sbyte.Parse(base.Modifiers["UsesLeft"]);
			}
		}
		catch (FormatException ex)
		{
			Debug.LogWarning("Failed parsing string " + base.Modifiers["UsesLeft"] + " to sbyte when reading modifiers for wieldable food with template " + ((MilMo_Item)this).Template.Identifier + ".");
			Debug.LogWarning(ex.ToString());
		}
		catch (OverflowException ex2)
		{
			Debug.LogWarning("Failed parsing string " + base.Modifiers["UsesLeft"] + " to sbyte when reading modifiers for wieldable food with template " + ((MilMo_Item)this).Template.Identifier + ".");
			Debug.LogWarning(ex2.ToString());
		}
	}
}
