using UnityEngine;

namespace UI.FX;

[CreateAssetMenu(menuName = "UI FX Presets/Color FX Preset")]
public class UIColorFXPresetSO : ScriptableObject
{
	public Color tintColor = Color.white;

	public float inTime;

	public float onTime;

	public float outTime = 1f;

	public LeanTweenType inEase = LeanTweenType.linear;

	public LeanTweenType outEase = LeanTweenType.linear;
}
