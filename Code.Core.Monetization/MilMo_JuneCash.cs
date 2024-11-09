using System;
using Code.World.GUI;
using Code.World.GUI.Steam;
using UnityEngine;

namespace Code.Core.Monetization;

public class MilMo_JuneCash : jb_Currency
{
	private static MilMo_JuneCash _instance;

	public static MilMo_JuneCash Instance => _instance ?? (_instance = new MilMo_JuneCash());

	public override bool ShowAccountBalance => true;

	public override bool ShowConfirmBuyDialog => true;

	public override bool ExitFullscreenOnBuy => false;

	public override bool ExitFullscreenOnCharge => true;

	private MilMo_JuneCash()
		: base("JCO")
	{
		if (_instance != null)
		{
			throw new InvalidOperationException("Each currency can only be created once!");
		}
		m_IconPath = "Batch01/Textures/Shop/JuneCash32";
		m_Name = "CharacterShop_244";
		m_NameForButtons = "CharacterShop_5585";
		m_ChargeButtonText = "CharacterShop_5584";
		m_ChargeButtonSize = new Vector2(128f, 64f);
		m_ChargeButtonTexture = "Batch01/Textures/Shop/ChargeButtonMilMoGame";
		m_ChargeButtonTextureMO = "Batch01/Textures/Shop/ChargeButtonMilMoGameMO";
		m_ChargeButtonTexturePressed = "Batch01/Textures/Shop/ChargeButtonMilMoGame";
		m_ChargeButtonCallback = delegate
		{
			SteamJuneCashDialog steamJuneCashDialog = new SteamJuneCashDialog(MilMo_GlobalUI.GetSystemUI);
			MilMo_GlobalUI.GetSystemUI.AddChild(steamJuneCashDialog);
			steamJuneCashDialog.Open();
		};
	}
}
