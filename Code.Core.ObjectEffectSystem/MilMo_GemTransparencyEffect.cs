using Code.Core.ResourceSystem;
using Code.Core.Visual;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_GemTransparencyEffect : MilMo_ObjectEffect
{
	private float _time;

	private bool _isSetup;

	private Renderer _renderer;

	private Material _oldMaterial;

	private static readonly int MainTex = Shader.PropertyToID("_MainTex");

	private static readonly int Color = Shader.PropertyToID("_Color");

	private static readonly int AmbColor = Shader.PropertyToID("_AmbColor");

	private static readonly int VelvetColor = Shader.PropertyToID("_VelvetColor");

	private static readonly int VelvetChannel = Shader.PropertyToID("_VelvetChannel");

	private static readonly int Ramp = Shader.PropertyToID("_Ramp");

	private MilMo_GemTransparencyEffectTemplate Template => EffectTemplate as MilMo_GemTransparencyEffectTemplate;

	public override float Duration => Template.Duration;

	public MilMo_GemTransparencyEffect(GameObject gameObject, MilMo_GemTransparencyEffectTemplate template)
		: base(gameObject, template)
	{
		MilMo_VisualRepComponent componentInChildren = GameObject.GetComponentInChildren<MilMo_VisualRepComponent>();
		if (componentInChildren == null)
		{
			return;
		}
		componentInChildren.GetVisualRep()?.RegisterMaterialsDoneCallback(delegate
		{
			if (!_isSetup)
			{
				_time = 0f;
				Setup();
			}
		});
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
			return true;
		}
		_time += Time.deltaTime;
		if (_time > Template.Duration)
		{
			Destroy();
			return false;
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
		if (_renderer.material.HasProperty("_MainTex") && _oldMaterial.HasProperty("_MainTex"))
		{
			_renderer.material.SetTexture(MainTex, _oldMaterial.GetTexture(MainTex));
			_renderer.material.SetTextureOffset(MainTex, _oldMaterial.GetTextureOffset(MainTex));
			_renderer.material.SetTextureScale(MainTex, _oldMaterial.GetTextureScale(MainTex));
		}
		if (_renderer.material.HasProperty("_Color") && _oldMaterial.HasProperty("_Color"))
		{
			Color color = _oldMaterial.GetColor(Color);
			Color color2 = _oldMaterial.GetColor(AmbColor);
			color.r = Mathf.Max(color.r, color2.r);
			color.g = Mathf.Max(color.g, color2.g);
			color.b = Mathf.Max(color.b, color2.b);
			color.a = Template.Alpha;
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
		if (!(_renderer == null))
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
