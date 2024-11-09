using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.GUI.Widget.Button;

public sealed class MilMo_ImBubble : MilMo_Button
{
	private const float M_JUMP_RATE = 2f;

	private float _mLastJump = Time.time + MilMo_Utility.Random();

	public Vector2 Size = new Vector2(26f, 26f);

	private bool _mAllowJump = true;

	private bool IsVisible { get; set; }

	public MilMo_ImBubble(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "IMBubble" + MilMo_UserInterface.GetRandomID();
		SetAllTextures("Batch01/Textures/FriendList/IMBubbleLeft");
		SetAlignment(MilMo_GUI.Align.BottomCenter);
		SetScale(0f, 0f);
		SetScalePull(0.05f, 0.05f);
		SetScaleDrag(0.75f, 0.75f);
		SetPosPull(0.05f, 0.05f);
		SetPosDrag(0.8f, 0.8f);
		SetFadeSpeed(0.05f);
		SetFont(MilMo_GUI.Font.ArialRounded);
		SetText(MilMo_Localization.GetNotLocalizedLocString(".."));
		SetDefaultTextColor(0.27f, 0.54f, 0.69f, 0.65f);
		SetHoverTextColor(0.27f, 0.54f, 0.69f, 0.65f);
		SetTextOffset(0f, -2f);
		IsVisible = false;
		AllowPointerFocus = false;
		MilMo_Button milMo_Button = new MilMo_Button(ui);
		milMo_Button.SetScale(15f, 15f);
		milMo_Button.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Button.SetPosition(10f, 10f);
		milMo_Button.AllowPointerFocus = true;
		AddChild(milMo_Button);
		milMo_Button.Function = delegate
		{
			Function(null);
		};
		Hide();
	}

	private void Jump()
	{
		if (_mAllowJump)
		{
			_mAllowJump = false;
			MilMo_EventSystem.At(1f, delegate
			{
				_mAllowJump = true;
			});
			Impulse(0f, -1.5f);
			ScaleImpulse(-1f, 1f);
			if (Time.time > _mLastJump + 2f + 1f)
			{
				_mLastJump = Time.time + MilMo_Utility.Random();
			}
			else
			{
				_mLastJump = Time.time;
			}
		}
	}

	public void Show(bool impulse)
	{
		IsVisible = true;
		if (impulse)
		{
			ScaleNow(0f, 0f);
			ScaleTo(Size);
			SetAngle(0f - 25f * MilMo_Utility.Random() - 12.5f);
			Angle(0f);
			_mLastJump += 2.5f;
		}
		else
		{
			ScaleTo(Size);
		}
		_mLastJump += MilMo_Utility.Random() * 0.5f;
	}

	public void Hide()
	{
		IsVisible = false;
		ScaleTo(0f, 0f);
	}

	public override void Step()
	{
		if (IsVisible && Time.time > _mLastJump + 2f)
		{
			Jump();
		}
		base.Step();
	}

	public override void Draw()
	{
		if (Enabled && !(Scale.x < 5f) && !(Scale.y < 5f))
		{
			base.Draw();
		}
	}
}
