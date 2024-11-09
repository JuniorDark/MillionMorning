using Code.Core.GUI;
using Code.Core.GUI.Core;
using UnityEngine;

namespace Code.World.GUI;

public sealed class MilMo_PlayerCountTag : MilMo_Widget
{
	private static readonly Vector2 DotScale = new Vector2(40f, 40f);

	private static readonly Vector2 NumberOffset = new Vector2(0f, -2f);

	private readonly MilMo_Widget _number;

	private int _playerCount;

	public int PlayerCount
	{
		set
		{
			_playerCount = value;
			if (_playerCount > 0)
			{
				_number.SetTextNoLocalization(_playerCount.ToString());
				SetTexture("Batch01/Textures/WorldMap/SplineDot");
			}
			else
			{
				_number.SetTextNoLocalization("");
				SetTextureInvisible();
			}
		}
	}

	public MilMo_PlayerCountTag(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "PlayerCountTag";
		SetAlignment(MilMo_GUI.Align.BottomLeft);
		FadeToDefaultColor = false;
		SetFadeSpeed(0.05f);
		ScaleNow(DotScale);
		SetTexture("Batch01/Textures/WorldMap/SplineDot");
		SetColor(0f, 0f, 0f, 0.7f);
		AllowPointerFocus = false;
		_number = new MilMo_Widget(ui);
		_number.SetAlignment(MilMo_GUI.Align.TopLeft);
		_number.SetPosition(NumberOffset);
		_number.UseParentAlpha = false;
		_number.FadeToDefaultColor = false;
		_number.FadeToDefaultTextColor = false;
		_number.SetFadeSpeed(0.05f);
		_number.SetDefaultTextColor(new Color(1f, 1f, 1f, 1f));
		_number.ScaleNow(DotScale);
		_number.SetTextureInvisible();
		_number.SetFont(MilMo_GUI.Font.EborgSmall);
		_number.SetTextDropShadowPos(2f, 2f);
		_number.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_number.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		_number.AllowPointerFocus = false;
		AddChild(_number);
	}

	public void FadeIn()
	{
		SetFadeSpeed(0.01f);
		_number.SetFadeSpeed(0.01f);
		AlphaTo(0.6f);
		_number.AlphaTo(1f);
	}

	public void FadeOut()
	{
		SetFadeSpeed(0.07f);
		_number.SetFadeSpeed(0.07f);
		AlphaTo(0f);
		_number.AlphaTo(0f);
	}

	public void SetVisible()
	{
		SetAlpha(0.6f);
		_number.SetAlpha(1f);
	}

	public void SetInvisible()
	{
		SetAlpha(0f);
		_number.SetAlpha(0f);
	}

	public void Remove()
	{
		RemoveChild(_number);
		UI.RemoveChild(this);
	}

	public void RefreshPositionAndScale()
	{
		SetScale(DotScale);
		_number.SetScale(DotScale);
		_number.SetPosition(NumberOffset);
	}
}
