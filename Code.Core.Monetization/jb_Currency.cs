using Code.Core.EventSystem;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Monetization;

public abstract class jb_Currency
{
	protected float m_ExchangeRate = 1f;

	protected string m_Name;

	protected string m_NameForButtons;

	protected string m_Id;

	protected string m_IconPath;

	protected string m_ChargeButtonText;

	protected int m_UserAccountBalance;

	protected string m_ChargeButtonTexture;

	protected string m_ChargeButtonTexturePressed;

	protected string m_ChargeButtonTextureMO;

	protected Vector2 m_ChargeButtonSize;

	protected MilMo_EventSystem.MilMo_Callback m_ChargeButtonCallback;

	public int UserAccountBalance
	{
		get
		{
			return m_UserAccountBalance;
		}
		set
		{
			m_UserAccountBalance = value;
		}
	}

	public abstract bool ShowAccountBalance { get; }

	public abstract bool ShowConfirmBuyDialog { get; }

	public abstract bool ExitFullscreenOnBuy { get; }

	public abstract bool ExitFullscreenOnCharge { get; }

	public float ExchangeRate
	{
		get
		{
			return m_ExchangeRate;
		}
		set
		{
			m_ExchangeRate = value;
			MilMo_EventSystem.Instance.PostEvent("currency_exchange_rate_changed", this);
		}
	}

	public MilMo_LocString Name => MilMo_Localization.GetLocString(m_Name);

	public MilMo_LocString NameForButtons => MilMo_Localization.GetLocString(m_NameForButtons);

	public MilMo_LocString ChargeButtonText => MilMo_Localization.GetLocString(m_ChargeButtonText);

	public string ChargeButtonTexture => MilMo_Localization.GetLocTexturePath(m_ChargeButtonTexture);

	public string ChargeButtonTexturePressed => MilMo_Localization.GetLocTexturePath(m_ChargeButtonTexturePressed);

	public string ChargeButtonTextureMO => MilMo_Localization.GetLocTexturePath(m_ChargeButtonTextureMO);

	public Vector2 ChargeButtonSize => m_ChargeButtonSize;

	public MilMo_EventSystem.MilMo_Callback ChargeButtonCallback => m_ChargeButtonCallback;

	public string Id => m_Id;

	public string IconPath => m_IconPath;

	protected jb_Currency(string id)
	{
		m_Id = id;
	}

	public virtual void RefreshAccountBalance()
	{
	}

	public virtual float ConvertValue(float value)
	{
		return Mathf.Floor(value * m_ExchangeRate);
	}
}
