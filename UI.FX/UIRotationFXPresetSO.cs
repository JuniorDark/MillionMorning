using UnityEngine;

namespace UI.FX;

[CreateAssetMenu(menuName = "UI FX Presets/Rotation FX Preset")]
public class UIRotationFXPresetSO : ScriptableObject
{
	public float rotateFrom;

	public float rotateTo;

	public float inTime;

	public float onTime;

	public float outTime = 1f;

	public LeanTweenType inEase = LeanTweenType.linear;

	public LeanTweenType outEase = LeanTweenType.linear;
}
