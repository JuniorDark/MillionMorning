using UnityEngine;

namespace UI.HUD;

public abstract class HudElement : MonoBehaviour
{
	public abstract void SetHudVisibility(bool shouldShow);
}
