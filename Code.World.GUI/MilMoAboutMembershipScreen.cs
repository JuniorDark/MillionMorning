using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.World.CharacterShop;
using Code.World.Player;
using UnityEngine;

namespace Code.World.GUI;

public class MilMoAboutMembershipScreen : MilMo_Window
{
	private readonly MilMo_Button m_WhiteBackground;

	private readonly MilMo_Widget m_Image;

	private readonly MilMo_Button m_ShopButton;

	private readonly MilMo_Button m_ShopButtonTwo;

	private readonly MilMo_Button m_CloseButton;

	public MilMoAboutMembershipScreen()
		: base(MilMo_GlobalUI.GetSystemUI)
	{
		SetText(MilMo_LocString.Empty);
		AllowPointerFocus = true;
		MCaption.SetText(MilMo_LocString.Empty);
		m_WhiteBackground = new MilMo_Button(UI);
		m_WhiteBackground.SetAllTextures("Batch01/Textures/Core/White");
		m_WhiteBackground.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_WhiteBackground.Enabled = true;
		m_WhiteBackground.AllowPointerFocus = true;
		AddChild(m_WhiteBackground);
		m_Image = new MilMo_Widget(UI);
		m_Image.SetAlignment(MilMo_GUI.Align.CenterCenter);
		m_Image.SetTexture(MilMo_Localization.GetLocTexturePath("Batch01/Textures/Shop/MembershipScreen/AboutMembershipPage"));
		m_Image.Enabled = true;
		AddChild(m_Image);
		m_ShopButton = new MilMo_Button(UI);
		m_ShopButton.SetAlignment(MilMo_GUI.Align.TopRight);
		m_ShopButton.SetAllTextures(MilMo_Localization.GetLocTexturePath("Batch01/Textures/Shop/MembershipScreen/BecomeAMemberButton"));
		m_ShopButton.SetHoverTexture(MilMo_Localization.GetLocTexturePath("Batch01/Textures/Shop/MembershipScreen/BecomeAMemberButtonMO"));
		m_ShopButton.SetPressedTexture(MilMo_Localization.GetLocTexturePath("Batch01/Textures/Shop/MembershipScreen/BecomeAMemberButtonPressed"));
		m_ShopButton.AllowPointerFocus = true;
		m_ShopButton.Function = BuyMembershipCallback;
		m_Image.AddChild(m_ShopButton);
		m_ShopButtonTwo = new MilMo_Button(UI);
		m_ShopButtonTwo.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_ShopButtonTwo.SetAllTextures(MilMo_Localization.GetLocTexturePath("Batch01/Textures/Shop/MembershipScreen/BuyMembershipButton"));
		m_ShopButtonTwo.SetHoverTexture(MilMo_Localization.GetLocTexturePath("Batch01/Textures/Shop/MembershipScreen/BuyMembershipButtonMO"));
		m_ShopButtonTwo.SetPressedTexture(MilMo_Localization.GetLocTexturePath("Batch01/Textures/Shop/MembershipScreen/BuyMembershipButtonPressed"));
		m_ShopButtonTwo.AllowPointerFocus = true;
		m_ShopButtonTwo.Function = BuyMembershipCallback;
		m_Image.AddChild(m_ShopButtonTwo);
		m_CloseButton = new MilMo_Button(UI);
		m_CloseButton.SetAlignment(MilMo_GUI.Align.BottomLeft);
		m_CloseButton.SetAllTextures(MilMo_Localization.GetLocTexturePath("Batch01/Textures/Shop/MembershipScreen/SmallBackButton"));
		m_CloseButton.SetHoverTexture(MilMo_Localization.GetLocTexturePath("Batch01/Textures/Shop/MembershipScreen/SmallBackButtonMO"));
		m_CloseButton.SetPressedTexture(MilMo_Localization.GetLocTexturePath("Batch01/Textures/Shop/MembershipScreen/SmallBackButton"));
		m_CloseButton.AllowPointerFocus = true;
		m_CloseButton.Function = delegate
		{
			Close(null);
		};
		m_Image.AddChild(m_CloseButton);
		SetFadeSpeed(0.05f);
	}

	public void RefreshUI()
	{
		m_CloseButton.SetScale(120f, 30f);
		m_ShopButton.SetScale(120f, 30f);
		float num = 1f;
		if (Screen.height < 720)
		{
			num = (float)Screen.height / 720f;
		}
		float num2 = 1.163f;
		m_Image.SetScale(617f * num2 * num, 660f * num2 * num);
		m_ShopButton.SetScale(194f * num2 * num, 48f * num2 * num);
		m_ShopButton.SetPosition(m_Image.Scale.x - 59f * num, 120f * num);
		m_CloseButton.SetScale(85f * num2 * num, 47f * num2 * num);
		m_CloseButton.SetPosition(10f, m_Image.Scale.y - 12f * num);
		m_ShopButtonTwo.SetScale(252f * num2 * num, 66f * num2 * num);
		m_ShopButtonTwo.SetPosition(m_Image.Scale.x - 326f * num, m_Image.Scale.y - 154f * num);
		SpawnScale = new Vector2(Screen.width, Screen.height);
		TargetScale = SpawnScale;
		ExitScale = SpawnScale;
		SpawnPos = new Vector2((float)Screen.width * 0.5f, 0f);
		TargetPos = SpawnPos;
		m_WhiteBackground.SetScale(SpawnScale);
		m_Image.SetPosition(SpawnScale.x * 0.5f, SpawnScale.y * 0.5f);
		m_WhiteBackground.SetPosition(0f, 0f);
		SetScale(SpawnScale);
	}

	public override void Close(object obj)
	{
		SetAlpha(1f);
		AlphaTo(0f);
		m_ShopButtonTwo.Enabled = false;
		m_ShopButton.Enabled = false;
		m_CloseButton.Enabled = false;
		MilMo_EventSystem.At(0.7f, delegate
		{
			base.Close(obj);
		});
	}

	public override void Open()
	{
		RefreshUI();
		m_ShopButtonTwo.Enabled = true;
		m_ShopButton.Enabled = true;
		m_CloseButton.Enabled = true;
		base.Open();
		SetAlpha(0f);
		AlphaTo(1f);
		m_ShopButtonTwo.SetAlpha(0f);
		m_ShopButton.SetAlpha(0f);
		m_CloseButton.SetAlpha(0f);
		m_ShopButtonTwo.AlphaTo(1f);
		m_ShopButton.AlphaTo(1f);
		m_CloseButton.AlphaTo(1f);
	}

	public override void Draw()
	{
		if (UI.ScreenSizeDirty)
		{
			RefreshUI();
		}
		if (Enabled)
		{
			base.Draw();
			BringToFront();
		}
	}

	private void BuyMembershipCallback(object o)
	{
		if (!MilMo_Player.Instance.InShop)
		{
			if (!MilMo_Player.Instance.OkToEnterShop())
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
				return;
			}
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Select);
			MilMo_Player.Instance.OpenMembershipShop();
			Close(null);
		}
		else
		{
			MilMo_CharacterShop.BuySelectedItem();
			Close(null);
		}
	}
}
