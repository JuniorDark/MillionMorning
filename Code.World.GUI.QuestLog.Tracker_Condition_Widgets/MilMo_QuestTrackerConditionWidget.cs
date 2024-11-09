using System;
using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.Items;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.World.Inventory;
using Code.World.Level.LevelInfo;
using Code.World.Level.LevelObject;
using Code.World.Player;
using Code.World.Quest;
using Code.World.Quest.Conditions;
using Core;
using Core.State;
using UnityEngine;

namespace Code.World.GUI.QuestLog.Tracker_Condition_Widgets;

public sealed class MilMo_QuestTrackerConditionWidget : MilMo_Widget
{
	private readonly MilMo_Widget _mCounter;

	private readonly MilMo_Widget _mChangingText;

	private int _mCurrentIndex;

	private List<MilMo_LocString> _mRotationStrings;

	private MilMo_TimerEvent _mTickEvent;

	private readonly List<MilMo_Widget> _mChangingIcons;

	private readonly List<string> _mChangingCounters;

	private bool _mRotate = true;

	private int _mCompletedItemIndex;

	public MilMo_QuestTrackerConditionWidget(MilMo_UserInterface ui, MilMo_QuestCondition condition)
		: base(ui)
	{
		if (condition == null)
		{
			throw new NullReferenceException("QuestTracker: Condition is null.");
		}
		SetScale(220f, 24f);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		SetFont(MilMo_GUI.Font.ArialRounded);
		SetFontPreset(MilMo_GUI.FontPreset.Outline);
		TextOutlineColor = Color.black;
		_mChangingText = new MilMo_Widget(UI);
		_mChangingText.SetScale(Scale);
		_mChangingText.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		_mChangingText.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mChangingText.SetFont(MilMo_GUI.Font.ArialRounded);
		_mChangingText.FadeToDefaultTextColor = false;
		_mChangingText.SetPosition(0f, 0f);
		_mChangingText.SetFontPreset(MilMo_GUI.FontPreset.Outline);
		_mChangingText.TextOutlineColor = Color.black;
		AddChild(_mChangingText);
		MilMo_Widget milMo_Widget = new MilMo_Widget(ui);
		milMo_Widget.SetScale(24f, 24f);
		milMo_Widget.SetTexture("Batch01/Textures/Quest/IconQuestCheckBox");
		milMo_Widget.SetAlignment(MilMo_GUI.Align.CenterCenter);
		milMo_Widget.SetPosition(-15f, 12f);
		AddChild(milMo_Widget);
		if (condition.Completed)
		{
			MilMo_Widget milMo_Widget2 = new MilMo_Widget(UI);
			milMo_Widget2.SetScale(17f, 17f);
			milMo_Widget2.SetTexture("Batch01/Textures/Homes/IconCheck");
			milMo_Widget2.SetAlignment(MilMo_GUI.Align.CenterCenter);
			milMo_Widget2.SetPosition(-15f, 10f);
			AddChild(milMo_Widget2);
		}
		_mCounter = new MilMo_Widget(ui);
		_mCounter.SetFont(MilMo_GUI.Font.EborgSmall);
		_mCounter.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mCounter.SetTextAlignment(MilMo_GUI.Align.CenterRight);
		_mCounter.FadeToDefaultTextColor = false;
		_mCounter.SetScale(Scale.x, Scale.y * 1.2f);
		_mCounter.SetPosition(0f, 0f);
		_mCounter.SetFontPreset(MilMo_GUI.FontPreset.Outline);
		_mCounter.TextOutlineColor = Color.black;
		AddChild(_mCounter);
		_mChangingIcons = new List<MilMo_Widget>();
		_mChangingCounters = new List<string>();
		if (condition is MilMo_ArrivesAt)
		{
			MilMo_ArrivesAt milMo_ArrivesAt = (MilMo_ArrivesAt)condition;
			MilMo_LocString copy = MilMo_Localization.GetLocString("QuestLog_9349").GetCopy();
			copy.SetFormatArgs(milMo_ArrivesAt.AreaDisplayName);
			_mChangingIcons.Add(CreateChangingWidget(new string[1] { MilMo_LevelInfo.GetLevelInfoData(milMo_ArrivesAt.FullLevelName).IconPath }, haveCounter: false, 0, useStandardPath: false));
			SetUp(copy);
		}
		else if (condition is MilMo_ArrivesAtAny)
		{
			MilMo_ArrivesAtAny milMo_ArrivesAtAny = (MilMo_ArrivesAtAny)condition;
			List<MilMo_LocString> list = new List<MilMo_LocString>();
			for (int i = 0; i < milMo_ArrivesAtAny.Areas.Count; i++)
			{
				MilMo_ArrivesAtAny.ArrivesAtAreaInfo arrivesAtAreaInfo = milMo_ArrivesAtAny.Areas[i];
				MilMo_LocString copy2 = MilMo_Localization.GetLocString("QuestLog_9349").GetCopy();
				copy2.SetFormatArgs(arrivesAtAreaInfo.AreaDisplayName);
				list.Add(copy2);
				string iconPath = MilMo_LevelInfo.GetLevelInfoData(arrivesAtAreaInfo.FullLevelName).IconPath;
				_mChangingIcons.Add(CreateChangingWidget(new string[1] { iconPath }, haveCounter: false, 0, useStandardPath: false));
			}
			MilMo_LocString copy3 = MilMo_Localization.GetLocString("QuestLog_9349").GetCopy();
			copy3.SetFormatArgs("");
			SetUp(copy3, list);
		}
		else if (condition is MilMo_CollectedAny)
		{
			List<MilMo_LocString> items = new List<MilMo_LocString>();
			MilMo_CollectedAny colAny = (MilMo_CollectedAny)condition;
			for (int j = 0; j < colAny.Items.Count; j++)
			{
				MilMo_CollectedAny.MilMo_CollectedInfo item1;
				MilMo_Item.AsyncGetItem((item1 = colAny.Items[j]).Item, delegate(MilMo_Item theItem)
				{
					MilMo_LocString copy10 = MilMo_Localization.GetLocString("QuestLog_9351").GetCopy();
					if (theItem.Template.DisplayName.String.Length > 14)
					{
						string @string = theItem.Template.DisplayName.String;
						@string = @string.Remove(14);
						@string += "...";
						copy10.SetFormatArgs(@string, "");
					}
					else
					{
						copy10.SetFormatArgs(theItem.Template.DisplayName, "");
					}
					MilMo_InventoryEntry entry2 = MilMo_Player.Instance.Inventory.GetEntry(item1.Item);
					int num = 0;
					if (entry2 != null)
					{
						num = entry2.Amount;
					}
					string counterText = num + "/" + item1.AmountToCollect;
					items.Add(copy10);
					MilMo_Widget widget = CreateChangingWidget();
					_mChangingIcons.Add(widget);
					_mChangingCounters.Add(counterText);
					if (num >= item1.AmountToCollect)
					{
						_mRotate = false;
						_mCompletedItemIndex = items.Count - 1;
					}
					theItem.AsyncGetIcon(delegate(Texture2D tex)
					{
						AddIconToWidget(widget, tex, haveCounter: true, counterText.Length);
					});
					if (_mChangingIcons.Count == colAny.Items.Count && items.Count == colAny.Items.Count)
					{
						MilMo_LocString copy11 = MilMo_Localization.GetLocString("QuestLog_9351").GetCopy();
						copy11.SetFormatArgs("", "");
						SetUp(copy11, items, num, item1.AmountToCollect);
					}
				});
			}
		}
		else if (condition is MilMo_CollectedGem)
		{
			MilMo_LocString copy4 = MilMo_Localization.GetLocString("QuestLog_9353").GetCopy();
			copy4.SetFormatArgs("");
			SetUp(copy4, ((MilMo_CollectedGem)condition).Amount, GlobalStates.Instance.playerState.gems.Get());
		}
		else if (condition is MilMo_KilledAny)
		{
			MilMo_KilledAny milMo_KilledAny = (MilMo_KilledAny)condition;
			MilMo_Killed milMo_Killed = new MilMo_Killed(new ConditionKilled(milMo_KilledAny.Kills[0].CreatureVisualRep, milMo_KilledAny.Kills[0].CreatureDisplayName, milMo_KilledAny.Kills[0].AmountToKill, milMo_KilledAny.Kills[0].AmountKilled, (sbyte)(milMo_KilledAny.Completed ? 1 : 0), (sbyte)(milMo_KilledAny.Active ? 1 : 0)));
			MilMo_LocString copy5 = MilMo_Localization.GetLocString("QuestLog_9354").GetCopy();
			copy5.SetFormatArgs(MilMo_Localization.GetLocString(milMo_Killed.CreatureDisplayName), "");
			string creatureVisualRep = milMo_Killed.CreatureVisualRep;
			string text = creatureVisualRep.Split('/')[^1];
			creatureVisualRep = "Batch01/Textures/Creatures/Icon" + text;
			string text2 = milMo_Killed.AmountKilled + "/" + milMo_Killed.AmountToKill;
			_mChangingIcons.Add(CreateChangingWidget(new string[1] { creatureVisualRep }, haveCounter: true, text2.Length, useStandardPath: true));
			SetUp(copy5, milMo_Killed.AmountKilled, milMo_Killed.AmountToKill);
		}
		else if (condition is MilMo_Collected)
		{
			MilMo_Collected con = (MilMo_Collected)condition;
			MilMo_Item.AsyncGetItem(con.Item, delegate(MilMo_Item item)
			{
				if (item == null)
				{
					Debug.LogWarning("QuestCondition_Collected, Item was null.");
				}
				else
				{
					MilMo_LocString loc = MilMo_Localization.GetLocString("QuestLog_9351").GetCopy();
					loc.SetFormatArgs(item.Template.DisplayName, "");
					MilMo_InventoryEntry entry = MilMo_Player.Instance.Inventory.GetEntry(con.Item);
					int amount = 0;
					if (entry != null)
					{
						amount = entry.Amount;
					}
					string counterText = amount + "/" + con.Amount;
					MilMo_Widget widget = CreateChangingWidget();
					_mChangingIcons.Add(widget);
					item.AsyncGetIcon(delegate(Texture2D tex)
					{
						AddIconToWidget(widget, tex, haveCounter: true, counterText.Length);
						SetUp(loc, amount, con.Amount);
					});
				}
			});
		}
		else if (condition is MilMo_Killed)
		{
			MilMo_LocString copy6 = MilMo_Localization.GetLocString("QuestLog_9354").GetCopy();
			copy6.SetFormatArgs(MilMo_Localization.GetLocString(((MilMo_Killed)condition).CreatureDisplayName), "");
			string text3 = ((MilMo_Killed)condition).AmountKilled + "/" + ((MilMo_Killed)condition).AmountToKill;
			string creatureVisualRep2 = ((MilMo_Killed)condition).CreatureVisualRep;
			string text4 = creatureVisualRep2.Split('/')[^1];
			creatureVisualRep2 = "Batch01/Textures/Creatures/Icon" + text4;
			_mChangingIcons.Add(CreateChangingWidget(new string[1] { creatureVisualRep2 }, haveCounter: true, text3.Length, useStandardPath: true));
			SetUp(copy6, ((MilMo_Killed)condition).AmountKilled, ((MilMo_Killed)condition).AmountToKill);
		}
		else if (condition is MilMo_TalkTo)
		{
			MilMo_LocString copy7 = MilMo_Localization.GetLocString("QuestLog_9355").GetCopy();
			copy7.SetFormatArgs(MilMo_Localization.GetLocString(((MilMo_TalkTo)condition).NPCDisplayName));
			IList<string> fullLevelNames = ((MilMo_TalkTo)condition).FullLevelNames;
			if (fullLevelNames == null || fullLevelNames.Count == 0)
			{
				Debug.LogWarning("Condition has no level name.");
				return;
			}
			MilMo_LevelInfoData levelInfoData = MilMo_LevelInfo.GetLevelInfoData(((MilMo_TalkTo)condition).FullLevelNames[0]);
			string text5 = "Content/Creatures/" + ((MilMo_TalkTo)condition).NPCVisualRep;
			string text6 = text5.Split('/')[^1];
			text5 = text5.Replace(text6, "");
			text5 = text5 + "Icon" + text6 + "0";
			_mChangingIcons.Add(CreateChangingWidget(new string[2] { levelInfoData.IconPath, text5 }, haveCounter: false, 0, useStandardPath: false));
			SetUp(copy7);
		}
		else if (condition is MilMo_TalkToAny)
		{
			MilMo_TalkToAny talkToAny = (MilMo_TalkToAny)condition;
			List<MilMo_LocString> npcs = new List<MilMo_LocString>();
			for (int k = 0; k < talkToAny.NpCs.Count; k++)
			{
				MilMo_TalkToAny.MilMo_TalkToInfo theNpc;
				MilMo_TalkToAny.MilMo_TalkToInfo milMo_TalkToInfo = (theNpc = talkToAny.NpCs[k]);
				Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(milMo_TalkToInfo.NPCTemplateIdentifier, delegate(MilMo_Template template, bool timeout)
				{
					if (!(template is MilMo_NpcTemplate milMo_NpcTemplate))
					{
						Debug.LogWarning("Npc template is null.");
					}
					else
					{
						MilMo_LocString copy8 = MilMo_Localization.GetLocString("QuestLog_9356").GetCopy();
						copy8.SetFormatArgs(MilMo_Localization.GetLocString(milMo_NpcTemplate.Name));
						npcs.Add(copy8);
						string text7 = "Content/Creatures/" + milMo_NpcTemplate.VisualRep;
						string text8 = text7.Split('/')[^1];
						text7 = text7.Replace(text8, "");
						text7 = text7 + "Icon" + text8 + "0";
						_mChangingIcons.Add(CreateChangingWidget(new string[2]
						{
							text7,
							MilMo_LevelInfo.GetLevelInfoData(theNpc.FullLevelNames[0]).IconPath
						}, haveCounter: false, 0, useStandardPath: false));
						if (npcs.Count == talkToAny.NpCs.Count)
						{
							MilMo_LocString copy9 = MilMo_Localization.GetLocString("QuestLog_9356").GetCopy();
							copy9.SetFormatArgs("");
							SetUp(copy9, npcs);
						}
					}
				});
			}
		}
		else
		{
			if (!(condition is MilMo_Wear))
			{
				return;
			}
			MilMo_Item.AsyncGetItem(((MilMo_Wear)condition).Item, delegate(MilMo_Item item)
			{
				if (item == null)
				{
					Debug.LogWarning("QuestCondition_Wear, Item was null.");
				}
				else
				{
					MilMo_LocString loc = MilMo_Localization.GetLocString("QuestLog_9503");
					loc.SetFormatArgs(item.Template.DisplayName);
					MilMo_Widget widget = CreateChangingWidget();
					_mChangingIcons.Add(widget);
					item.AsyncGetIcon(delegate(Texture2D tex)
					{
						AddIconToWidget(widget, tex, haveCounter: false, 0);
						SetUp(loc);
					});
				}
			});
		}
	}

	private MilMo_Widget CreateChangingWidget(string[] textures, bool haveCounter, int counterLength, bool useStandardPath)
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetScale(220f, 24f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.FadeToDefaultColor = false;
		milMo_Widget.SetPosition(-1f, 0f);
		for (int i = 0; i < textures.Length; i++)
		{
			MilMo_Widget milMo_Widget2 = new MilMo_Widget(UI);
			milMo_Widget2.SetScale(Scale.y, Scale.y);
			milMo_Widget2.SetTexture(textures[i], useStandardPath);
			milMo_Widget2.SetAlignment(MilMo_GUI.Align.CenterRight);
			float num = 0f;
			if (haveCounter)
			{
				num = counterLength * 9;
			}
			milMo_Widget2.SetPosition(milMo_Widget.Scale.x - num - milMo_Widget.Scale.y * (float)i, milMo_Widget.Scale.y * 0.5f);
			milMo_Widget.AddChild(milMo_Widget2);
		}
		return milMo_Widget;
	}

	private MilMo_Widget CreateChangingWidget()
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetScale(220f, 24f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.FadeToDefaultColor = false;
		milMo_Widget.SetPosition(-1f, 0f);
		return milMo_Widget;
	}

	private void AddIconToWidget(MilMo_Widget widget, Texture2D texture, bool haveCounter, int counterLength)
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetScale(Scale.y, Scale.y);
		milMo_Widget.SetTexture(texture);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.CenterRight);
		float num = 0f;
		if (haveCounter)
		{
			num = counterLength * 9;
		}
		milMo_Widget.SetPosition(widget.Scale.x - num, widget.Scale.y * 0.5f);
		widget.AddChild(milMo_Widget);
	}

	private void SetUp(MilMo_LocString text)
	{
		SetText(text);
		if (_mChangingIcons.Count != 0)
		{
			AddChild(_mChangingIcons[0]);
		}
	}

	private void SetUp(MilMo_LocString baseString, List<MilMo_LocString> rotation)
	{
		SetText(baseString);
		_mRotationStrings = rotation;
		_mChangingText.SetText(_mRotationStrings[0]);
		_mTickEvent = MilMo_EventSystem.At(0.5f, Tick);
		if (_mChangingIcons.Count != 0)
		{
			AddChild(_mChangingIcons[0]);
		}
	}

	private void SetUp(MilMo_LocString baseString, List<MilMo_LocString> rotation, int current, int target)
	{
		if (current > target)
		{
			current = target;
		}
		SetText(baseString);
		_mCounter.SetTextNoLocalization(current + "/" + target);
		_mRotationStrings = rotation;
		_mChangingText.SetText(_mRotationStrings[0]);
		if (_mChangingIcons.Count != 0)
		{
			AddChild(_mChangingIcons[0]);
		}
		_mTickEvent = MilMo_EventSystem.At(0.5f, Tick);
	}

	private void SetUp(MilMo_LocString text, int current, int target)
	{
		if (current > target)
		{
			current = target;
		}
		SetText(text);
		_mCounter.SetTextNoLocalization(current + "/" + target);
		if (_mChangingIcons.Count != 0)
		{
			AddChild(_mChangingIcons[0]);
		}
	}

	internal void Destroy()
	{
		if (_mTickEvent == null)
		{
			return;
		}
		MilMo_EventSystem.RemoveTimerEvent(_mTickEvent);
		_mTickEvent = null;
		foreach (MilMo_Widget mChangingIcon in _mChangingIcons)
		{
			RemoveChild(mChangingIcon);
		}
	}

	private void Tick()
	{
		int previousIndex = _mCurrentIndex;
		if (!_mRotate)
		{
			_mCurrentIndex = _mCompletedItemIndex;
			if (previousIndex != _mCurrentIndex)
			{
				_mChangingText.SetText(_mRotationStrings[_mCurrentIndex]);
				_mChangingText.SetTextColor(1f, 1f, 1f, 1f);
				if (_mChangingIcons.Count != 0)
				{
					RemoveChild(_mChangingIcons[previousIndex]);
					AddChild(_mChangingIcons[_mCurrentIndex]);
					_mChangingIcons[_mCurrentIndex].SetAlpha(1f);
				}
				if (_mChangingCounters.Count != 0)
				{
					_mCounter.SetTextColor(1f, 1f, 1f, 1f);
					_mCounter.SetTextNoLocalization(_mChangingCounters[_mCurrentIndex]);
				}
			}
			return;
		}
		_mCurrentIndex++;
		if (_mCurrentIndex == _mRotationStrings.Count)
		{
			_mCurrentIndex = 0;
		}
		if (_mChangingCounters.Count != 0)
		{
			_mCounter.TextColorTo(1f, 1f, 1f, 0f);
		}
		_mChangingText.TextColorTo(1f, 1f, 1f, 0f);
		if (_mChangingIcons.Count != 0)
		{
			_mChangingIcons[previousIndex].AlphaTo(0f);
		}
		_mTickEvent = MilMo_EventSystem.At(1f, delegate
		{
			_mChangingText.SetText(_mRotationStrings[_mCurrentIndex]);
			if (_mChangingIcons.Count != 0)
			{
				RemoveChild(_mChangingIcons[previousIndex]);
				_mChangingIcons[_mCurrentIndex].SetAlpha(0f);
				AddChild(_mChangingIcons[_mCurrentIndex]);
				_mChangingIcons[_mCurrentIndex].AlphaTo(1f);
			}
			_mChangingText.TextColorTo(1f, 1f, 1f, 1f);
			if (_mChangingCounters.Count != 0)
			{
				_mCounter.TextColorTo(1f, 1f, 1f, 1f);
				_mCounter.SetTextNoLocalization(_mChangingCounters[_mCurrentIndex]);
			}
			_mTickEvent = MilMo_EventSystem.At(2.7f, Tick);
		});
	}
}
