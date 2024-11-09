using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI.Tutorial;

[CreateAssetMenu(menuName = "UI FX Presets/ArrowPreset")]
public class ArrowPresetSO : ScriptableObject
{
	public AssetReference arrowPrefab;

	public float animationDistance = 100f;

	public float animationDuration = 1f;

	public LeanTweenType leanTweenType = LeanTweenType.easeInOutSine;

	public event Action OnChange;

	private void OnValidate()
	{
		this.OnChange?.Invoke();
	}
}
