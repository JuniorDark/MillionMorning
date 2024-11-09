using System;
using Code.Core.GUI.Core;
using UnityEngine;

namespace Code.Core.GUI.Widget;

public sealed class MilMo_ProgressBar : MilMo_Widget
{
	private readonly MilMo_Widget _mBackgroundRect;

	private readonly MilMo_Widget _mProgressRect;

	private readonly MilMo_Widget _mProgressShade;

	private readonly float _mWidth;

	private readonly float _mBorderWidth;

	private readonly float _mHeight;

	private readonly bool _mVertical;

	private float _mCurrentProgress;

	public bool UseBarShade
	{
		set
		{
			_mProgressShade.Enabled = value;
		}
	}

	public float CurrentProgress
	{
		get
		{
			return _mCurrentProgress;
		}
		set
		{
			_mCurrentProgress = Mathf.Clamp01(value);
			_mProgressRect.ScaleTo(GetTargetProgressRectScale());
			_mProgressShade.ScaleTo(GetTargetProgressRectScale());
		}
	}

	public float Pull
	{
		set
		{
			_mProgressRect.ScaleMover.Pull.x = value;
			_mProgressShade.ScaleMover.Pull.x = value;
		}
	}

	public float Drag
	{
		set
		{
			_mProgressRect.ScaleMover.Drag.x = value;
			_mProgressShade.ScaleMover.Drag.x = value;
		}
	}

	public MilMo_ProgressBar(MilMo_UserInterface ui, Vector2 position, float widthNoBorder, float heightNoBorder, float borderWidth, Color backgroundColor, Color progressColor, float startProgress, bool vertical = false)
		: base(ui)
	{
		float num = widthNoBorder + borderWidth * 2f;
		float num2 = heightNoBorder + borderWidth * 2f;
		_mCurrentProgress = startProgress;
		_mWidth = num;
		_mHeight = num2;
		_mBorderWidth = borderWidth;
		_mVertical = vertical;
		SetAlignment(MilMo_GUI.Align.BottomCenter);
		SetPosition(position);
		SetScale(num, num2);
		_mBackgroundRect = new MilMo_Widget(UI);
		_mBackgroundRect.SetTexture("Batch01/Textures/Core/White");
		_mBackgroundRect.SetAlignment(_mVertical ? MilMo_GUI.Align.BottomLeft : MilMo_GUI.Align.CenterLeft);
		_mBackgroundRect.SetPosition(0f, 0f);
		_mBackgroundRect.SetScale(num, num2);
		_mBackgroundRect.SetDefaultColor(backgroundColor);
		AddChild(_mBackgroundRect);
		_mProgressRect = new MilMo_Widget(UI);
		_mProgressRect.SetTexture("Batch01/Textures/Core/White");
		_mProgressRect.SetAlignment(_mVertical ? MilMo_GUI.Align.BottomLeft : MilMo_GUI.Align.CenterLeft);
		_mProgressRect.SetPosition(borderWidth, _mVertical ? (0f - borderWidth) : 0f);
		_mProgressRect.SetScale(GetTargetProgressRectScale());
		_mProgressRect.SetScaleDrag(0.5f, 0.5f);
		_mProgressRect.SetDefaultColor(progressColor);
		AddChild(_mProgressRect);
		_mProgressShade = new MilMo_Widget(UI);
		_mProgressShade.SetTexture("Batch01/Textures/HUD/HealthBarShade");
		_mProgressShade.SetAlignment(_mVertical ? MilMo_GUI.Align.BottomLeft : MilMo_GUI.Align.CenterLeft);
		_mProgressShade.SetPosition(borderWidth, _mVertical ? (0f - borderWidth) : 0f);
		_mProgressShade.SetScale(GetTargetProgressRectScale());
		_mProgressShade.SetScaleDrag(0.5f, 0.5f);
		_mProgressShade.Enabled = false;
		AddChild(_mProgressShade);
	}

	public void CurrentProgressNow(float value)
	{
		_mCurrentProgress = Mathf.Clamp01(value);
		_mProgressRect.SetScale(GetTargetProgressRectScale());
		_mProgressShade.SetScale(GetTargetProgressRectScale());
	}

	public void Remove()
	{
		UI.RemoveChild(this);
		UI.RemoveChild(_mBackgroundRect);
		UI.RemoveChild(_mProgressRect);
		UI.RemoveChild(_mProgressShade);
	}

	public void SetProgressColor(Color color)
	{
		float a = _mProgressRect.CurrentColor.a;
		_mProgressRect.SetDefaultColor(color);
		_mProgressRect.SetAlpha(a);
		_mProgressShade.SetAlpha(a);
	}

	private Vector2 GetTargetProgressRectScale()
	{
		Vector2 result = new Vector2((_mWidth - _mBorderWidth * 2f) * ((!_mVertical) ? _mCurrentProgress : 1f), (_mHeight - _mBorderWidth * 2f) * (_mVertical ? _mCurrentProgress : 1f));
		if (float.IsNaN(result.x) || float.IsNaN(result.y))
		{
			throw new Exception("NaN");
		}
		return result;
	}
}
