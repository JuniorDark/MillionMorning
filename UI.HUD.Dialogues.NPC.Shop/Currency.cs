using System;
using UnityEngine;
using UnityEngine.Events;

namespace UI.HUD.Dialogues.NPC.Shop;

[Serializable]
public abstract class Currency
{
	protected UnityAction<Currency> OnChange;

	public abstract string GetIdentifier();

	public abstract int GetAmount();

	public abstract Texture2D GetTexture2D();

	public abstract void LoadIconTexture();

	public abstract void RegisterOnChange(UnityAction<Currency> action);

	public abstract void UnregisterOnChange(UnityAction<Currency> action);
}
