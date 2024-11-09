using Code.Core.ResourceSystem;
using Code.Core.Utility;
using Code.Core.Visual;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_FadeEffect : MilMo_ObjectEffect
{
	private float _targetAlpha = 1f;

	private Material _oldMaterial;

	private Renderer _renderer;

	private bool _isSetup;

	private static readonly int MainTex = Shader.PropertyToID("_MainTex");

	private static readonly int Color = Shader.PropertyToID("_Color");

	private static readonly int AmbColor = Shader.PropertyToID("_AmbColor");

	private static readonly int VelvetColor = Shader.PropertyToID("_VelvetColor");

	private static readonly int VelvetChannel = Shader.PropertyToID("_VelvetChannel");

	private static readonly int Ramp = Shader.PropertyToID("_Ramp");

	public override float Duration
	{
		get
		{
			if (Template.Speed == 0f)
			{
				return 0f;
			}
			return Mathf.Abs(_targetAlpha / Template.Speed);
		}
	}

	private MilMo_FadeEffectTemplate Template => EffectTemplate as MilMo_FadeEffectTemplate;

	public MilMo_FadeEffect(GameObject gameObject, MilMo_FadeEffectTemplate template)
		: base(gameObject, template)
	{
	}

	public override bool Update()
	{
		if (Template.Speed == 0f || GameObject == null)
		{
			Destroy();
			return false;
		}
		if (!_isSetup)
		{
			Setup();
		}
		if (_renderer == null)
		{
			Destroy();
			return false;
		}
		Material material = _renderer.material;
		Color color = material.color;
		color.a += Template.Speed * Time.deltaTime;
		material.color = color;
		if ((Template.Speed > 0f && _renderer.material.color.a >= _targetAlpha) || (Template.Speed < 0f && _renderer.material.color.a <= _targetAlpha))
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
		MilMo_VisualRep milMo_VisualRep = componentInChildren.GetVisualRep()?.CurrentLod?.ParentVisualRep;
		if (milMo_VisualRep == null)
		{
			return;
		}
		_renderer = milMo_VisualRep.Renderer;
		if (_renderer == null || milMo_VisualRep.Materials.Count < 1)
		{
			return;
		}
		MilMo_Material milMo_Material = milMo_VisualRep.Materials[0];
		if (milMo_Material == null)
		{
			return;
		}
		_oldMaterial = new Material(_renderer.material);
		Shader shader = MilMo_ResourceManager.LoadShaderLocal("Shaders/Junebug/DiffuseTrans");
		if (shader == null)
		{
			Debug.LogWarning("Failed to load shader Shaders/Junebug/DiffuseTrans, will fall back to Shaders/Junebug/Diffuse");
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
			if (Template.Speed < 0f)
			{
				_targetAlpha = 0f;
			}
			else if (Template.Speed > 0f)
			{
				_targetAlpha = milMo_Material.MainColor.a;
				if (MilMo_Utility.Equals(color, milMo_Material.MainColor))
				{
					Material material = _renderer.material;
					material.color = new Color(material.color.r, material.color.g, material.color.b, 0f);
				}
			}
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
