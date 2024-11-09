using UnityEngine;

namespace UI.FX;

[CreateAssetMenu(menuName = "UI FX Presets/SizeBounce FX Preset")]
public class UISizeBounceFXPresetSO : ScriptableObject
{
	public Vector2 toSize;

	public float time;
}
