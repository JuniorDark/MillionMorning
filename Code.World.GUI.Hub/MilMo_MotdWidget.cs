using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.World.Player;
using UnityEngine;

namespace Code.World.GUI.Hub;

public sealed class MilMo_MotdWidget : MilMo_ScrollView
{
	private sealed class CaptionWidget : MilMo_Widget
	{
		private readonly MilMo_SimpleLabel _caption;

		private readonly MilMo_SimpleLabel _date;

		public CaptionWidget(MilMo_UserInterface ui)
			: base(ui)
		{
			_caption = new MilMo_SimpleLabel(UI);
			_caption.SetFont(MilMo_GUI.Font.EborgSmall);
			_caption.UseParentAlpha = false;
			_caption.SetFontScale(1f);
			_caption.SetPosition(140f, 20f);
			_caption.SetScale(200f, 40f);
			_caption.SetAlignment(MilMo_GUI.Align.CenterCenter);
			_caption.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
			_date = new MilMo_SimpleLabel(UI);
			_date.SetFontScale(0.8f);
			_date.UseParentAlpha = false;
			_date.SetPosition(140f, 40f);
			_date.SetScale(120f, 20f);
			_date.SetAlignment(MilMo_GUI.Align.CenterCenter);
			_date.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
			AddChild(_caption);
			AddChild(_date);
			SetAlignment(MilMo_GUI.Align.TopCenter);
			SetScale(300f, 70f);
		}

		public void Set(string caption, string date)
		{
			_caption.SetTextNoLocalization(caption);
			_date.SetTextNoLocalization(date);
		}

		public override void Draw()
		{
			GUISkin skin = UnityEngine.GUI.skin;
			UnityEngine.GUI.skin = Skin;
			UnityEngine.GUI.color = CurrentColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
			Rect screenPosition = GetScreenPosition();
			screenPosition.x += 5f;
			screenPosition.y += 5f;
			screenPosition.width = 270f;
			screenPosition.height = 45f;
			UnityEngine.GUI.Box(screenPosition, "");
			UnityEngine.GUI.skin = skin;
			base.Draw();
		}
	}

	private float _lastTimeMotdRefresh = -1f;

	private readonly MilMo_Widget _txt;

	private readonly CaptionWidget _caption;

	private readonly Vector2 _scale;

	public MilMo_MotdWidget(MilMo_UserInterface ui, Vector2 scale)
		: base(ui)
	{
		_scale = scale;
		SetPosition(0f, 11f);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		_txt = new MilMo_Widget(UI);
		_txt.UseParentAlpha = false;
		_txt.SetFont(MilMo_GUI.Font.ArialRounded);
		_txt.SetWordWrap(w: true);
		_txt.SetScale(265f, 235f);
		_txt.SetPosition(15f, 55f);
		_txt.SetFontScale(1f);
		_txt.SetAlignment(MilMo_GUI.Align.TopLeft);
		_txt.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		_caption = new CaptionWidget(UI);
		_caption.SetPosition(155f, 5f);
		AddChild(_caption);
		AddChild(_txt);
	}

	public void RefreshMotd()
	{
		if (MilMo_NewsManager.Instance.HasNews)
		{
			_txt.SetTextNoLocalization(MilMo_NewsManager.Instance.TextBody);
			_caption.Set(MilMo_NewsManager.Instance.Headline, MilMo_NewsManager.Instance.Date);
		}
		_txt.SetScale(_scale.x - _txt.Pos.x, _scale.y - 50f);
		SetScale(_scale.x, _scale.y + 10f);
		ShowHorizontalBar(h: false);
		RefreshViewSize();
	}

	public override void Draw()
	{
		if (_lastTimeMotdRefresh == -1f || Time.time - _lastTimeMotdRefresh > 300f)
		{
			_lastTimeMotdRefresh = Time.time;
			RefreshMotd();
		}
		base.Draw();
	}
}
