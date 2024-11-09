using Code.Core.ResourceSystem;
using Code.Core.Visual;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_BlinkEffect : MilMo_ObjectEffect
{
	private int _direction = -1;

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

	private MilMo_BlinkEffectTemplate Template => EffectTemplate as MilMo_BlinkEffectTemplate;

	public override float Duration => Template.Duration;

	public MilMo_BlinkEffect(GameObject gameObject, MilMo_BlinkEffectTemplate template)
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
		_time += Time.deltaTime;
		if (_time > Template.Duration)
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
		if (_renderer == null)
		{
			Destroy();
			return false;
		}
		Material material = _renderer.material;
		Color color = material.color;
		color.a += (float)_direction * Template.Speed * Time.deltaTime;
		material.color = color;
		if (color.a >= 1f || color.a <= 0f)
		{
			_direction *= -1;
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
		_renderer = visualRep.CurrentLod.ParentVisualRep.Renderer;
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
		if (_renderer.material.HasProperty("_MainTex") && _oldMaterial.HasProperty("_MainTex"))
		{
			_renderer.material.SetTexture(MainTex, _oldMaterial.GetTexture(MainTex));
			_renderer.material.SetTextureOffset(MainTex, _oldMaterial.GetTextureOffset(MainTex));
			_renderer.material.SetTextureScale(MainTex, _oldMaterial.GetTextureScale(MainTex));
		}
		if (_renderer.material.HasProperty("_Color") && _oldMaterial.HasProperty("_Color"))
		{
			Color color = _oldMaterial.GetColor(Color);
			if (_oldMaterial.HasProperty("_AmbColor"))
			{
				Color color2 = _oldMaterial.GetColor(AmbColor);
				color.r = Mathf.Max(color.r, color2.r);
				color.g = Mathf.Max(color.g, color2.g);
				color.b = Mathf.Max(color.b, color2.b);
			}
			_renderer.material.SetColor(Color, color);
		}
		if (_renderer.material.HasProperty("_VelvetColor") && _oldMaterial.HasProperty("_VelvetColor"))
		{
			_renderer.material.SetColor(VelvetColor, _oldMaterial.GetColor(VelvetColor));
		}
		if (_renderer.material.HasProperty("_VelvetChannel") && _oldMaterial.HasProperty("_VelvetChannel"))
		{
			_renderer.material.SetVector(VelvetChannel, _oldMaterial.GetVector(VelvetChannel));
		}
		if (_renderer.material.HasProperty("_Ramp"))
		{
			_renderer.material.SetTexture(Ramp, MilMo_Material.RampTexture);
		}
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
