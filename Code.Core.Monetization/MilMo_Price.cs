using System;
using Code.Core.EventSystem;
using UnityEngine;

namespace Code.Core.Monetization;

public class MilMo_Price
{
	private const float MEMBER_PRICE_MULTIPLIER = 0.8f;

	private readonly int _basePrice;

	private bool _isMemberPrice;

	private jb_Currency _currency;

	private readonly MilMo_GenericReaction _currencyChangedReaction;

	private readonly MilMo_GenericReaction _exchangeRateChangedReaction;

	public int Price { get; private set; }

	public int BasePriceMemberModified
	{
		get
		{
			float num = (_isMemberPrice ? 0.8f : 1f);
			return Mathf.FloorToInt((float)_basePrice * num);
		}
	}

	public bool IsMemberPrice
	{
		set
		{
			if (value != _isMemberPrice)
			{
				_isMemberPrice = value;
				SetCurrencyPrice();
			}
		}
	}

	public MilMo_Price(int basePrice, jb_Currency currency)
	{
		_basePrice = basePrice;
		_currency = currency;
		SetCurrencyPrice();
		_currencyChangedReaction = MilMo_EventSystem.Listen("currency_changed", CurrencyChanged);
		_currencyChangedReaction.Repeating = true;
		_exchangeRateChangedReaction = MilMo_EventSystem.Listen("currency_exchange_rate_changed", ExchangeRateChanged);
		_exchangeRateChangedReaction.Repeating = true;
	}

	public void Destroy()
	{
		MilMo_EventSystem.RemoveReaction(_currencyChangedReaction);
		MilMo_EventSystem.RemoveReaction(_exchangeRateChangedReaction);
	}

	private void CurrencyChanged(object newCurrencyAsObj)
	{
		_currency = newCurrencyAsObj as jb_Currency;
		SetCurrencyPrice();
	}

	private void ExchangeRateChanged(object changedCurrencyAsObj)
	{
		if (changedCurrencyAsObj is jb_Currency jb_Currency2 && jb_Currency2 == _currency)
		{
			SetCurrencyPrice();
		}
	}

	private void SetCurrencyPrice()
	{
		float num = (_isMemberPrice ? 0.8f : 1f);
		Price = Math.Max(1, Mathf.FloorToInt((float)_basePrice * num));
	}
}
