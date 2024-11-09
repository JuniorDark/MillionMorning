using UnityEngine;

namespace UI.FX;

[CreateAssetMenu(menuName = "UI FX Presets/Scale FX Preset")]
public class UIScaleFXPresetSO : ScriptableObject
{
	public Vector2 impulseScale = Vector2.one * 1.1f;

	public float inTime;

	public float onTime;

	public float outTime = 1f;

	public LeanTweenType inEase = LeanTweenType.linear;

	public LeanTweenType outEase = LeanTweenType.linear;
}
