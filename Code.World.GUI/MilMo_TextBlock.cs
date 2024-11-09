using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI;

public sealed class MilMo_TextBlock : MilMo_Widget
{
	private readonly MilMo_SimpleLabel _mTextWidget;

	private readonly GUISkin _mSkin;

	public MilMo_SimpleLabel TextWidget => _mTextWidget;

	public MilMo_TextBlock(MilMo_UserInterface ui, MilMo_LocString str, Vector2 scale, bool fixedHeight = false)
		: base(ui)
	{
		UI = ui;
		Identifier = "TextBlock " + MilMo_UserInterface.GetRandomID();
		base.FixedRes = true;
		_mSkin = UI.Skins[0];
		if (_mSkin == null)
		{
			Debug.LogWarning("Skin is null in MilMo_TextBlock");
		}
		_mSkin.label.wordWrap = true;
		ScaleNow(scale.x, scale.y);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetTexture("Batch01/Textures/Core/Invisible");
		AllowPointerFocus = false;
		_mTextWidget = new MilMo_SimpleLabel(UI);
		_mTextWidget.SetText(str);
		_mTextWidget.FixedRes = true;
		float y = (fixedHeight ? scale.y : GetTextHeight(_mTextWidget.Text.String, scale.x));
		_mTextWidget.SetScale(scale.x, y);
		_mTextWidget.SetPosition(0f, 0f);
		_mTextWidget.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		_mTextWidget.SetWordWrap(w: true);
		_mTextWidget.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mTextWidget.SetTexture("Batch01/Textures/Core/Black");
		_mTextWidget.AllowPointerFocus = false;
		AddChild(_mTextWidget);
		ScaleNow(scale.x, _mTextWidget.ScaleMover.Target.y / base.Res.y);
		UI.SetNext(PosMover.Target.x + ScaleMover.Target.x, PosMover.Target.y + ScaleMover.Target.y);
	}

	private float GetTextHeight(string msg, float width)
	{
		return _mSkin.label.CalcHeight(new GUIContent(msg), width) + 10f;
	}
}
