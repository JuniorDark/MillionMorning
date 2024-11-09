using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.World.Quest;
using UnityEngine;

namespace Code.World.GUI.QuestLog;

public sealed class MilMo_QuestEntryWidget : MilMo_Button
{
	private readonly MilMo_Quest _mQuest;

	private readonly ButtonFunc _mSelectCallback;

	internal readonly MilMo_Widget MTxt;

	internal bool MIsSelected;

	public MilMo_Quest Quest => _mQuest;

	public MilMo_QuestEntryWidget(MilMo_UserInterface ui, MilMo_Quest quest, ButtonFunc selectCallback)
		: base(ui)
	{
		_mQuest = quest;
		MTxt = new MilMo_Widget(ui);
		MTxt.SetPosition(5f, 0f);
		MTxt.SetScale(175f, 24f);
		MTxt.SetAlignment(MilMo_GUI.Align.TopLeft);
		MTxt.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		MTxt.SetFont(MilMo_GUI.Font.EborgSmall);
		MTxt.SetFontScale(0.8f);
		MTxt.AllowPointerFocus = false;
		MTxt.FadeToDefaultTextColor = false;
		if (quest.Name.String.Length > 20)
		{
			string text = quest.Name.String.Remove(20);
			text += "...";
			MTxt.SetTextNoLocalization(text);
		}
		else
		{
			MTxt.SetText(quest.Name);
		}
		AddChild(MTxt);
		SetScale(175f, 24f);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		_mSelectCallback = selectCallback;
		Function = ClickFunction;
		PointerHoverFunction = delegate
		{
			if (!MIsSelected)
			{
				MTxt.TextColorTo(Color.gray);
			}
		};
		PointerLeaveFunction = delegate
		{
			if (!MIsSelected)
			{
				MTxt.TextColorTo(Color.white);
			}
		};
	}

	public void SetNoneSelected()
	{
		MTxt.SetDefaultTextColor(Color.white);
		MIsSelected = false;
	}

	private void ClickFunction(object o)
	{
		if (!MIsSelected)
		{
			MIsSelected = true;
			MTxt.SetDefaultTextColor(Color.green);
			_mSelectCallback(_mQuest);
		}
	}
}
