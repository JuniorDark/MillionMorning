using System.Collections.Generic;
using Code.Core.Avatar;
using Code.Core.Global;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.World.Level;
using Code.World.Level.LevelInfo;
using Code.World.Player;
using Core;
using Player;
using UnityEngine;

namespace Code.World.GUI.PVP;

public class jb_DamageEffect : MilMo_Widget
{
	private readonly MilMo_Avatar m_Avatar;

	private readonly List<MilMo_Widget> m_DamageWidgets;

	public jb_DamageEffect(MilMo_UserInterface ui, MilMo_Avatar avatar)
		: base(ui)
	{
		Identifier = "DamageEffect";
		m_DamageWidgets = new List<MilMo_Widget>();
		Enabled = true;
		m_Avatar = avatar;
	}

	private MilMo_Widget CreateDamageWidget(float damage)
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopCenter);
		milMo_Widget.FadeToDefaultColor = false;
		milMo_Widget.SetFadeSpeed(0.1f);
		milMo_Widget.SetDefaultTextColor(Color.red);
		milMo_Widget.ScaleNow(250f, 30f);
		milMo_Widget.SetFont(MilMo_GUI.Font.EborgLarge);
		milMo_Widget.SetFontPreset(MilMo_GUI.FontPreset.Outline);
		milMo_Widget.SetPosition(Scale.x / 2f, 42f);
		milMo_Widget.SetTextColor(1f, 1f, 1f, 0f);
		milMo_Widget.AllowPointerFocus = false;
		milMo_Widget.Enabled = true;
		milMo_Widget.SetText(MilMo_Localization.GetNotLocalizedLocString(damage.ToString()));
		milMo_Widget.SetAlpha(1f);
		milMo_Widget.SetFadeSpeed(0.05f);
		AddChild(milMo_Widget);
		return milMo_Widget;
	}

	public void ShowDamage(float damage, float distance)
	{
		if (!(damage < 1f))
		{
			MilMo_Widget milMo_Widget = CreateDamageWidget(damage);
			float num = distance / 50f;
			milMo_Widget.SetFontScale(1f - num, 1f - num);
			m_DamageWidgets.Add(milMo_Widget);
		}
	}

	public override void Draw()
	{
		if (!Enabled)
		{
			return;
		}
		if (MilMo_Player.Instance == null || MilMo_Player.Instance.Avatar == null || m_Avatar == null || m_Avatar.Head == null)
		{
			Remove();
		}
		else
		{
			if ((MilMo_Player.Instance != null && MilMo_Player.Instance.Avatar != null && m_Avatar.Room != MilMo_Player.Instance.Avatar.Room) || (MilMo_Level.CurrentLevel != null && !MilMo_LevelInfo.IsPvp(MilMo_Level.CurrentLevel.VerboseName)))
			{
				return;
			}
			Vector3 vector = MilMo_Global.Camera.WorldToScreenPoint(m_Avatar.Head.position + new Vector3(0f, 0.1f, 0f));
			if (!(vector.z > 0f))
			{
				return;
			}
			GoToNow(vector.x / base.Res.x, ((float)MilMo_Global.Camera.pixelHeight - vector.y) / base.Res.y - 4f - (float)((m_Avatar.Id != MilMo_Player.Instance.Id && Singleton<GroupManager>.Instance.InGroup(m_Avatar.Id)) ? 15 : 7));
			m_DamageWidgets.RemoveAll(delegate(MilMo_Widget damageWidget)
			{
				damageWidget.SetYPos(damageWidget.Pos.y - 2f);
				if (damageWidget.Pos.y > -140f)
				{
					return false;
				}
				RemoveChild(damageWidget);
				return true;
			});
			base.Draw();
		}
	}

	public void Remove()
	{
		UI.RemoveChild(this);
	}
}
