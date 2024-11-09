using System;
using Core.State;
using Core.State.Basic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace UI.HUD.Dialogues.NPC.Shop;

public class CoinCurrency : Currency
{
	private Texture2D _texture2D;

	private readonly IntState _state;

	public CoinCurrency()
	{
		_state = GlobalStates.Instance.playerState.coins;
	}

	public override string GetIdentifier()
	{
		return "COIN";
	}

	public override int GetAmount()
	{
		if (!_state)
		{
			return 0;
		}
		return _state.Get();
	}

	public override Texture2D GetTexture2D()
	{
		return _texture2D;
	}

	public override void LoadIconTexture()
	{
		if (!(_texture2D != null))
		{
			_texture2D = Addressables.LoadAssetAsync<Texture2D>("Coins").WaitForCompletion();
		}
	}

	public override void RegisterOnChange(UnityAction<Currency> action)
	{
		if ((bool)_state)
		{
			OnChange = (UnityAction<Currency>)Delegate.Combine(OnChange, action);
			_state.OnChange += TriggerOnChange;
		}
	}

	public override void UnregisterOnChange(UnityAction<Currency> action)
	{
		if ((bool)_state)
		{
			OnChange = (UnityAction<Currency>)Delegate.Remove(OnChange, action);
			_state.OnChange -= TriggerOnChange;
		}
	}

	private void TriggerOnChange(int _)
	{
		OnChange?.Invoke(this);
	}
}
