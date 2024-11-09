using System;
using System.Collections.Generic;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.Items;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.Core.Utility;
using Code.World.GUI.Hub;
using Code.World.Player;
using Core;
using UnityEngine;

namespace Code.World.GUI;

public sealed class MilMoDeliveryBoxTimer : MilMo_Button
{
	private const float M_SIZE = 104f;

	private readonly Vector2 _mPos = new Vector2(55f, 102f);

	private readonly MilMo_Widget _mPane;

	private readonly MilMo_Widget _mClock;

	private readonly MilMo_ProgressBar _mProgressBar;

	private bool _mIsOpen;

	public MilMoDeliveryBoxTimer(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "DeliveryBoxTimer";
		base.FixedRes = true;
		SetAlignment(MilMo_GUI.Align.BottomCenter);
		base.Tooltip = new MilMo_Tooltip(MilMo_Localization.GetLocString("Homes_9340"));
		base.Tooltip.SetYScale(50f);
		SetAllTextures("Batch01/Textures/Core/Invisible");
		SetScale(104f, 104f);
		SetScalePull(0.07f, 0.07f);
		SetScaleDrag(0.6f, 0.6f);
		SetPosPull(0f, 0.02f);
		SetPosDrag(0f, 0.9f);
		PosMover.MinVel = new Vector2(0.0001845f, 0.0001845f);
		SetFadeSpeed(0.02f);
		SetFadeInSpeed(0.02f);
		SetFadeOutSpeed(0.02f);
		ScaleMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Sinus);
		ScaleMover.SinRate = new Vector2(0f, 2f);
		ScaleMover.SinAmp = new Vector2(0f, 1f);
		SetDefaultColor(1f, 1f, 1f, 1f);
		Vector2 vector = new Vector2(20f, 10f);
		_mPane = new MilMo_Widget(UI);
		_mPane.SetTexture("Batch01/Textures/NPCDialog/NPCDialogNameTag");
		_mPane.SetPosition(134f + vector.x, 40f + vector.y);
		_mPane.UseParentAlpha = false;
		_mPane.SetScale(85f, 36f);
		_mPane.SetDefaultColor(0f, 0f, 0f, 0.35f);
		_mPane.SetFadeSpeed(0.02f);
		UI.AddChild(_mPane);
		_mClock = new MilMo_Widget(UI);
		_mClock.SetTextNoLocalization("15:30");
		_mClock.SetDefaultTextColor(1f, 1f, 1f, 1f);
		_mClock.SetFont(MilMo_GUI.Font.EborgSmall);
		_mClock.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		_mClock.TextOutlineColor = new Color(0f, 0f, 0f, 1f);
		_mClock.SetFontScale(0.9f);
		_mClock.SetPosition(132f + vector.x, 46f + vector.y);
		_mClock.SetDefaultTextColor(1f, 1f, 1f, 1f);
		_mClock.SetAlignment(MilMo_GUI.Align.BottomCenter);
		_mClock.SetTextAlignment(MilMo_GUI.Align.BottomCenter);
		_mClock.SetScale(150f, 50f);
		_mClock.UseParentAlpha = false;
		_mClock.FadeToDefaultColor = false;
		_mClock.SetFadeSpeed(0.1f);
		UI.AddChild(_mClock);
		_mProgressBar = new MilMo_ProgressBar(UI, new Vector2(132f + vector.x, 48f + vector.y), 48f, 7f, 1f, new Color(0f, 0f, 0f, 1f), new Color(1f, 1f, 1f, 0.75f), 0f);
		_mProgressBar.SetAlignment(MilMo_GUI.Align.TopCenter);
		_mProgressBar.Pull = 0.07f;
		_mProgressBar.Drag = 0.65f;
		_mProgressBar.FadeToDefaultColor = false;
		_mProgressBar.SetFadeSpeed(0.1f);
		_mProgressBar.CurrentProgress = 0.5f;
		UI.AddChild(_mProgressBar);
		SetAlpha(0f);
		_mPane.SetDefaultColor(0f, 0f, 0f, 0f);
		_mClock.SetAlpha(0f);
		_mProgressBar.SetAlpha(0f);
	}

	public override void Step()
	{
		if (MilMo_Hub.Instance.StartScreenInfo != null && MilMo_Hub.Instance.StartScreenInfo.getHomeDeliveryBox() == null)
		{
			int minutes = (MilMo_Player.Instance.NextHomeDeliveryBoxSpawnTime - DateTime.Now).Minutes;
			int seconds = (MilMo_Player.Instance.NextHomeDeliveryBoxSpawnTime - DateTime.Now).Seconds;
			if (minutes > -1 && seconds > -1 && Open())
			{
				string text = minutes.ToString();
				string text2 = seconds.ToString();
				if (minutes < 10)
				{
					text = "0" + text;
				}
				if (seconds < 10)
				{
					text2 = "0" + text2;
				}
				_mClock.SetTextNoLocalization(text + ":" + text2);
				float num = Mathf.Abs(minutes) * 60 + Mathf.Abs(seconds);
				float num2 = 1f - num / 1800f;
				_mProgressBar.CurrentProgress = num2;
				if (num2 >= 1f)
				{
					Close();
				}
			}
			else
			{
				Close();
			}
		}
		else
		{
			Close();
		}
		base.Step();
	}

	public bool Open()
	{
		if (MilMo_Player.Instance.NextHomeDeliveryBox == null)
		{
			return false;
		}
		if (_mIsOpen)
		{
			return true;
		}
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(MilMo_Player.Instance.NextHomeDeliveryBox, delegate(MilMo_Template template, bool timeout)
		{
			if (template is MilMo_ItemTemplate milMo_ItemTemplate)
			{
				milMo_ItemTemplate.Instantiate(new Dictionary<string, string>()).AsyncGetIcon(delegate(Texture2D icon)
				{
					if (icon != null)
					{
						SetAllTextures(icon);
					}
				});
			}
		});
		AlphaTo(1f);
		_mPane.SetDefaultColor(0f, 0f, 0f, 0.35f);
		_mClock.AlphaTo(1f);
		_mProgressBar.AlphaTo(1f);
		SetPosition(_mPos);
		UI.BringToFront(this);
		_mIsOpen = true;
		return true;
	}

	private void Close()
	{
		_mClock.SetTextNoLocalization("00:00");
		AlphaTo(0f);
		_mPane.SetDefaultColor(0f, 0f, 0f, 0f);
		_mClock.AlphaTo(0f);
		_mProgressBar.AlphaTo(0f);
		if (base.Tooltip != null && base.Tooltip.Enabled)
		{
			base.Tooltip.Close();
		}
		_mIsOpen = false;
	}
}
