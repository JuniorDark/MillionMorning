using System;
using Code.Core.EventSystem;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Core;

namespace Code.Core.Monetization;

public class MilMo_Monetization
{
	public static MilMo_Monetization Instance { get; private set; }

	public jb_Currency Currency { get; private set; }

	private MilMo_Monetization()
	{
		MilMo_EventSystem.Listen("got_currency_exchange_rate", GotExchangeRate);
	}

	static MilMo_Monetization()
	{
		Instance = new MilMo_Monetization();
	}

	public void RequestExchangeRate()
	{
		Singleton<GameNetwork>.Instance.RequestCurrencyInfo(Currency.Id);
	}

	public void Initialize(jb_Currency currency)
	{
		if (Currency != null)
		{
			throw new InvalidOperationException("Monetization framework can only be initialized once!");
		}
		SetCurrency(currency);
	}

	private void SetCurrency(jb_Currency currency)
	{
		Currency = currency;
		MilMo_EventSystem.Instance.PostEvent("currency_changed", Currency);
	}

	public MilMo_Price GetPrice(int basePrice)
	{
		if (Currency == null)
		{
			throw new InvalidOperationException("Trying to get a price from the monetization framework before it is initialized.");
		}
		return new MilMo_Price(basePrice, Currency);
	}

	public void SetExchangeRate(string currencyId, float exchangeRate)
	{
		if (Currency == null)
		{
			throw new InvalidOperationException("Trying to set currency exchange rate before the monetization framework is initialized.");
		}
		if (!(Currency.Id != currencyId))
		{
			Currency.ExchangeRate = exchangeRate;
		}
	}

	private void GotExchangeRate(object msgAsObj)
	{
		if (msgAsObj is ServerCurrencyInfo serverCurrencyInfo)
		{
			SetExchangeRate(serverCurrencyInfo.getCurrency().GetId(), serverCurrencyInfo.getCurrency().GetExchangeRate());
		}
	}
}
