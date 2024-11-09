using Code.Core.ResourceSystem;
using Code.Core.Visual;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_ColorTintEffect : MilMo_ObjectEffect
{
	private float _time;

	private Material _oldMaterial;

	private bool _isSetup;

	private Renderer _renderer;

	private static readonly int MainTex = Shader.PropertyToID("_MainTex");

	private static readonly int Color = Shader.PropertyToID("_Color");

	private static readonly int AmbColor = Shader.PropertyToID("_AmbColor");

	private static readonly int VelvetColor = Shader.PropertyToID("_VelvetColor");

	private static readonly int VelvetChannel = Shader.PropertyToID("_VelvetChannel");

	private static readonly int Ramp = Shader.PropertyToID("_Ramp");

	private MilMo_ColorTintEffectTemplate Template => EffectTemplate as MilMo_ColorTintEffectTemplate;

	public override float Duration => float.MaxValue;

	public MilMo_ColorTintEffect(GameObject gameObject, MilMo_ColorTintEffectTemplate template)
		: base(gameObject, template)
	{
	}

	public override bool Update()
	{
		if (GameObject == null)
		{
			Destroy();
			return false;
		}
		if (!_isSetup)
		{
			Setup();
			if (!_isSetup)
			{
				Destroy();
				return false;
			}
		}
		if (!_renderer)
		{
			Destroy();
			return false;
		}
		return true;
	}

	private void Setup()
	{
		MilMo_VisualRepComponent componentInChildren = GameObject.GetComponentInChildren<MilMo_VisualRepComponent>();
		if (componentInChildren == null)
		{
			return;
		}
		MilMo_VisualRep visualRep = componentInChildren.GetVisualRep();
		if (visualRep == null)
		{
			return;
		}
		_renderer = visualRep.Renderer;
		if (_renderer == null)
		{
			return;
		}
		_oldMaterial = new Material(_renderer.material);
		Shader shader = MilMo_ResourceManager.LoadShaderLocal("Shaders/Junebug/DiffuseTrans");
		if (shader == null)
		{
			Debug.LogWarning("Failed to load shader Shaders/Junebug/DiffuseTrans, blink effect will not work");
			return;
		}
		_renderer.material = new Material(shader);
		if (_renderer.material.HasProperty(MainTex) && _oldMaterial.HasProperty(MainTex))
		{
			_renderer.material.SetTexture(MainTex, _oldMaterial.GetTexture(MainTex));
			_renderer.material.SetTextureOffset(MainTex, _oldMaterial.GetTextureOffset(MainTex));
			_renderer.material.SetTextureScale(MainTex, _oldMaterial.GetTextureScale(MainTex));
		}
		if (_renderer.material.HasProperty(Color) && _oldMaterial.HasProperty(Color))
		{
			Color color = _oldMaterial.GetColor(Color);
			if (_oldMaterial.HasProperty(AmbColor))
			{
				Color color2 = _oldMaterial.GetColor(AmbColor);
				color.r = Mathf.Max(color.r, color2.r);
				color.g = Mathf.Max(color.g, color2.g);
				color.b = Mathf.Max(color.b, color2.b);
			}
			_renderer.material.SetColor(Color, color);
		}
		if (_renderer.material.HasProperty(VelvetColor) && _oldMaterial.HasProperty(VelvetColor))
		{
			_renderer.material.SetColor(VelvetColor, _oldMaterial.GetColor(VelvetColor));
		}
		if (_renderer.material.HasProperty(VelvetChannel) && _oldMaterial.HasProperty(VelvetChannel))
		{
			_renderer.material.SetVector(VelvetChannel, _oldMaterial.GetVector(VelvetChannel));
		}
		if (_renderer.material.HasProperty(Ramp))
		{
			_renderer.material.SetTexture(Ramp, MilMo_Material.RampTexture);
		}
		_renderer.material.color = Template.Color;
		_isSetup = true;
	}

	private void ResetShader()
	{
		if (_renderer != null)
		{
			_renderer.material = _oldMaterial;
		}
	}

	public override void Destroy()
	{
		base.Destroy();
		ResetShader();
	}
}
