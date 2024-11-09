using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.World.GUI;

public sealed class MilMo_TutorialImage : MilMo_Widget
{
	private Vector2 _spawnPosition;

	private Vector2 _targetPosition;

	private Vector2 _targetScale = new Vector2(128f, 128f);

	private bool _isActive;

	private bool _hasOpened;

	private readonly Vector2 _offset;

	public MilMo_TutorialImage(MilMo_UserInterface ui, string icon, Vector2 offset)
		: base(ui)
	{
		Identifier = "TutorialImage" + MilMo_Utility.RandomID();
		_offset = offset;
		SetAlignment(MilMo_GUI.Align.CenterCenter);
		SetPosPull(0.1f, 0.1f);
		SetPosDrag(0.2f, 0.2f);
		SetScale(_targetScale);
		SetScalePull(0.1f, 0.1f);
		SetScaleDrag(0.2f, 0.2f);
		base.FixedRes = true;
		AllowPointerFocus = false;
		SetFadeSpeed(0.05f);
		FadeToDefaultColor = false;
		SetEnabled(e: false);
		if (!string.IsNullOrEmpty(icon))
		{
			LoadAndSetIconAsync(icon);
		}
	}

	private async void LoadAndSetIconAsync(string path)
	{
		Texture2D texture2D = await MilMo_ResourceManager.Instance.LoadTextureAsync(path);
		if (!(texture2D == null))
		{
			SetTexture(new MilMo_Texture(texture2D));
			Open();
		}
	}

	private void Open()
	{
		if (_isActive)
		{
			return;
		}
		_isActive = true;
		if (base.Texture == null || base.Texture.Texture == null)
		{
			return;
		}
		SetPosDrag(0.4f, 0.4f);
		SetScaleDrag(0.4f, 0.4f);
		_spawnPosition = new Vector2(60f, 0f);
		_targetPosition = new Vector2(190f + _offset.x, -30f + _offset.y);
		SetPosition(_spawnPosition);
		GoTo(_targetPosition);
		_targetScale = new Vector2(base.Texture.Texture.width, base.Texture.Texture.height);
		if (_targetScale.x > 100f || _targetScale.y > 100f)
		{
			if (_targetScale.x <= _targetScale.y)
			{
				float num = 100f / _targetScale.x;
				_targetScale.x *= num;
				_targetScale.y *= num;
			}
			else if (_targetScale.x > _targetScale.y)
			{
				float num2 = 100f / _targetScale.y;
				_targetScale.x *= num2;
				_targetScale.y *= num2;
			}
		}
		SetScale(0f, 0f);
		ScaleTo(_targetScale);
		SetAlpha(0f);
		AlphaTo(1f);
		UI.BringToFront(this);
		SetEnabled(e: true);
	}

	public void Close()
	{
		SetPosDrag(0.2f, 0.2f);
		SetScaleDrag(0.2f, 0.2f);
		_isActive = false;
		AlphaTo(0f);
		Angle(300f);
		ScaleTo(0f, 0f);
		GoTo(-200f, 50f);
	}
}
