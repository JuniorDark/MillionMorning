using UnityEngine;

namespace UI.FX;

[CreateAssetMenu(menuName = "UI FX Presets/Sliding FX Preset")]
public class UISlidingFXPresetSO : ScriptableObject
{
	public Vector2 outPosition = Vector2.zero;

	public LeanTweenType slideInEase;

	public LeanTweenType slideOutEase;
}
