using System.Collections.Generic;
using UnityEngine;

namespace Code.World.ImageEffects;

[ExecuteInEditMode]
public class MilMo_ImageEffectsHandler : MonoBehaviour
{
	public static class Effects
	{
		public const string WATER = "water";

		public const string BLOOM = "bloom";

		public const string ENHANCE = "enhance";
	}

	private Dictionary<string, List<MilMo_ImageEffectBase>> _imageEffects;

	private bool _initialized;

	public static MilMo_BloomEffect Bloom;

	public static MilMo_ColorCorrectionCurves ColorCorrectionCurves;

	private void Awake()
	{
		if (!_initialized)
		{
			if ((bool)base.gameObject.GetComponent<MilMo_ImageEffectsHandler>())
			{
				DestroyEffect("water");
				DestroyEffect("bloom");
				DestroyEffect("enhance");
			}
			_imageEffects = new Dictionary<string, List<MilMo_ImageEffectBase>>();
			AddColorTint("water", new Color(0f, 0f, 0.15f));
			AddBlur("water");
			AddBloom("bloom");
			AddColorCorrectionCurves("enhance");
			_initialized = true;
		}
	}

	private void AddImageEffect(string key, MilMo_ImageEffectBase effect)
	{
		if (_imageEffects.ContainsKey(key))
		{
			_imageEffects[key].Add(effect);
			return;
		}
		List<MilMo_ImageEffectBase> value = new List<MilMo_ImageEffectBase> { effect };
		_imageEffects.Add(key, value);
	}

	private void AddColorTint(string key, Color color, GameObject o = null)
	{
		if (!o)
		{
			o = base.gameObject;
		}
		MilMo_ColorTintEffect milMo_ColorTintEffect = o.AddComponent<MilMo_ColorTintEffect>();
		milMo_ColorTintEffect.shader = Shader.Find("ImageEffects/ColorTint");
		milMo_ColorTintEffect.colorTint = color;
		milMo_ColorTintEffect.enabled = false;
		AddImageEffect(key, milMo_ColorTintEffect);
	}

	private void AddColorCorrectionCurves(string key)
	{
		ColorCorrectionCurves = base.gameObject.AddComponent<MilMo_ColorCorrectionCurves>();
		ColorCorrectionCurves.shader = Shader.Find("ImageEffects/ColorCorrectionCurves");
		ColorCorrectionCurves.enabled = false;
		AddImageEffect(key, ColorCorrectionCurves);
	}

	private void AddBlur(string key)
	{
		MilMo_BlurEffect milMo_BlurEffect = base.gameObject.AddComponent<MilMo_BlurEffect>();
		milMo_BlurEffect.shader = Shader.Find("ImageEffects/Blur");
		milMo_BlurEffect.enabled = false;
		AddImageEffect(key, milMo_BlurEffect);
	}

	private void AddBloom(string key)
	{
		Bloom = base.gameObject.AddComponent<MilMo_BloomEffect>();
		Bloom.shader = Shader.Find("ImageEffects/Bloom");
		Bloom.enabled = false;
		AddImageEffect(key, Bloom);
	}

	public void EnableEffects(string key, bool enable)
	{
		if (_imageEffects != null && _imageEffects.ContainsKey(key))
		{
			_imageEffects[key].ForEach(delegate(MilMo_ImageEffectBase effect)
			{
				effect.enabled = enable;
			});
		}
	}

	public void DestroyEffect(string key)
	{
		if (_imageEffects != null && _imageEffects.ContainsKey(key))
		{
			_imageEffects[key].ForEach(Object.Destroy);
			_imageEffects.Remove(key);
		}
	}
}
