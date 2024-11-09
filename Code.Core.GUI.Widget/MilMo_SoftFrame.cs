using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.GUI.Widget;

public sealed class MilMo_SoftFrame : MilMo_Widget
{
	private MilMo_Texture _mLeftTexture;

	private MilMo_Texture _mRightTexture;

	private float _mFrameWidth = 16f;

	public MilMo_SoftFrame(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "SoftFrame " + MilMo_UserInterface.GetRandomID();
	}

	public override void Draw()
	{
		if (!IsEnabled())
		{
			return;
		}
		if (FadeToDefaultTextColor)
		{
			TextColorTo(DefaultTextColor);
		}
		Rect screenPosition = GetScreenPosition();
		UnityEngine.GUI.color = CurrentColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
		if (CurrentTexture != null && CurrentTexture.Texture != null)
		{
			UnityEngine.GUI.DrawTexture(screenPosition, CurrentTexture.Texture, ScaleMode.StretchToFill, alphaBlend: true, 0f);
		}
		float width = screenPosition.width;
		screenPosition.width = _mFrameWidth;
		screenPosition.x -= _mFrameWidth;
		if (_mLeftTexture != null && _mLeftTexture.Texture != null)
		{
			UnityEngine.GUI.DrawTexture(screenPosition, _mLeftTexture.Texture, ScaleMode.StretchToFill, alphaBlend: true, 0f);
		}
		screenPosition.x += _mFrameWidth + width;
		if (_mRightTexture != null && _mRightTexture.Texture != null)
		{
			UnityEngine.GUI.DrawTexture(screenPosition, _mRightTexture.Texture, ScaleMode.StretchToFill, alphaBlend: true, 0f);
		}
		DrawText();
		CheckPointerFocus();
		foreach (MilMo_Widget child in base.Children)
		{
			child.Draw();
		}
	}

	public void SetFrameWidth(float x)
	{
		x *= base.Res.x;
		_mFrameWidth = x;
	}

	public void SetLeftTexture(string filename)
	{
		_mLeftTexture = new MilMo_Texture("Content/GUI/" + filename);
		_mLeftTexture.AsyncLoad();
	}

	public void SetRightTexture(string filename)
	{
		_mRightTexture = new MilMo_Texture("Content/GUI/" + filename);
		_mRightTexture.AsyncLoad();
	}
}
