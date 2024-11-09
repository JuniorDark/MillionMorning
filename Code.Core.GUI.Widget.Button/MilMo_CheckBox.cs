using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using UnityEngine;

namespace Code.Core.GUI.Widget.Button;

public sealed class MilMo_CheckBox : MilMo_Button
{
	private MilMo_Button _choiceButton;

	private bool _checked;

	private readonly ButtonFunc _onChecked;

	private bool _disabled;

	private bool _drawn;

	public bool Checked
	{
		get
		{
			return _checked;
		}
		set
		{
			if (_checked == value)
			{
				return;
			}
			_checked = value;
			if (_onChecked != null)
			{
				MilMo_EventSystem.NextFrame(delegate
				{
					_onChecked(this);
				});
			}
			_choiceButton.SetAllTextures(_checked ? "Batch01/Textures/CharBuilder/CheckboxTicked" : "Batch01/Textures/CharBuilder/CheckboxUnticked");
		}
	}

	public MilMo_CheckBox(MilMo_UserInterface ui, bool startChecked = false, ButtonFunc checkedChangedCallback = null)
		: base(ui)
	{
		Identifier = "CheckBox " + MilMo_UserInterface.GetRandomID();
		_checked = startChecked;
		_onChecked = checkedChangedCallback;
		_disabled = false;
		IgnoreGlobalFade = false;
		UseParentAlpha = true;
		AllowPointerFocus = false;
		CreateChoiceButton();
	}

	public void Disable(bool disable)
	{
		_disabled = disable;
	}

	public override void Draw()
	{
		if (!Enabled)
		{
			return;
		}
		Color currentColor = CurrentColor;
		if (Parent != null && UseParentAlpha)
		{
			currentColor.a *= Parent.CurrentColor.a;
		}
		UnityEngine.GUI.color = currentColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
		if (_disabled)
		{
			_choiceButton.AllowPointerFocus = false;
			_choiceButton.SetAlpha(0.5f);
		}
		DrawText();
		CheckPointerFocus();
		foreach (MilMo_Widget child in base.Children)
		{
			child.Draw();
		}
	}

	private void CreateChoiceButton()
	{
		_choiceButton = new MilMo_Button(UI);
		_choiceButton.SetAllTextures(Checked ? "Batch01/Textures/CharBuilder/CheckboxTicked" : "Batch01/Textures/CharBuilder/CheckboxUnticked");
		_choiceButton.SetScale(18f, 18f);
		_choiceButton.SetPosition(0f, 0f);
		_choiceButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		_choiceButton.AllowPointerFocus = true;
		_choiceButton.Function = delegate
		{
			Checked = !Checked;
		};
		AddChild(_choiceButton);
	}
}
